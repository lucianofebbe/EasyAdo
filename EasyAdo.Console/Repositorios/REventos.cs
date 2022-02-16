using EasyAdo.Console.Modelos;
using EasyAdo.SqlServer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace EasyAdo.Console.Repositorios
{
    public class REventos : Context<Eventos>
    {
        public REventos()
        {
            connectionString =
                "Data Source=localhost;Initial Catalog=dbProEventos;Integrated Security=True";

            //Opcional
            timeOut = 180;
        }

        public List<Eventos> GetByActivedAutoConverter(bool actived)
        {
            List<SqlParameter> parameters = new();
            parameters.Add(new SqlParameter() { ParameterName = "@actived", Value = actived });

            string query = @"SELECT TOP (1000) Id
                              ,Local
                              ,DataEvento
                              ,Tema
                              ,QtdPessoas
                              ,ImagemUrl
                              ,Telefone
                              ,InsertDate
                              ,UpdateDate
                              ,Deleted
                              ,Active
                          FROM Eventos WHERE Active = @actived";

            Execute(query,
                parameters,
                SqlServer.EnumExecuteType.ExecuteType.ReaderQuery, //LEITURA ATRAVÉS DE QUERY
                false);

            //Somente funcional caso o Modelo for identico a tabela na base de dados.
            //Caso não, utilize como referencia o método GetByActivedManualConverter nessa classe
            var result = ConverterDataTableToList<Eventos>(dataTable);

            return result;
        }

        public List<Eventos> GetByActivedManualConverter(bool actived)
        {
            List<Eventos> result = new();
            List<SqlParameter> parameters = new();
            parameters.Add(new SqlParameter() { ParameterName = "@actived", Value = actived });

            string query = @"SELECT TOP (1000) Id
                              ,Local
                              ,DataEvento
                              ,Tema
                              ,QtdPessoas
                              ,ImagemUrl
                              ,Telefone
                              ,InsertDate
                              ,UpdateDate
                              ,Deleted
                              ,Active
                          FROM Eventos WHERE Active = @actived";

            Execute(query,
                parameters,
                SqlServer.EnumExecuteType.ExecuteType.ReaderQuery, //LEITURA ATRAVÉS DE QUERY
                false);

            foreach (DataRow item in dataTable.Rows)
            {
                result.Add(new Eventos()
                {
                    Id = int.Parse(item["Id"].ToString()),
                    Active = bool.Parse(item["Active"].ToString()),
                    DataEvento = DateTime.Parse(item["DataEvento"].ToString()),
                    Deleted = bool.Parse(item["Deleted"].ToString()),
                    ImagemUrl = item["ImagemUrl"].ToString(),
                    InsertDate = DateTime.Parse(item["InsertDate"].ToString()),
                    Local = item["Local"].ToString(),
                    QtdPessoas = int.Parse(item["QtdPessoas"].ToString()),
                    Telefone = item["Telefone"].ToString(),
                    Tema = item["Tema"].ToString(),
                    UpdateDate = DateTime.Parse(item["UpdateDate"].ToString())
                });
            }

            return result;
        }

        public List<Eventos> GetByProcActived(bool actived)
        {
            List<Eventos> result = new();
            List<SqlParameter> parameters = new();

            Execute("ProcGetByActived",
                parameters,
                SqlServer.EnumExecuteType.ExecuteType.ReaderProcedure, //LEITURA ATRAVÉS DE PROCEDURE
                false);

            foreach (DataRow item in dataTable.Rows)
            {
                result.Add(new Eventos()
                {
                    Id = int.Parse(item["Id"].ToString()),
                    Active = bool.Parse(item["Active"].ToString()),
                    DataEvento = DateTime.Parse(item["DataEvento"].ToString()),
                    Deleted = bool.Parse(item["Deleted"].ToString()),
                    ImagemUrl = item["ImagemUrl"].ToString(),
                    InsertDate = DateTime.Parse(item["InsertDate"].ToString()),
                    Local = item["Local"].ToString(),
                    QtdPessoas = int.Parse(item["QtdPessoas"].ToString()),
                    Telefone = item["Telefone"].ToString(),
                    Tema = item["Tema"].ToString(),
                    UpdateDate = DateTime.Parse(item["UpdateDate"].ToString())
                });
            }

            return result;
        }

        public int ManualInsertNonQuery(Eventos item)
        {
            List<SqlParameter> parameters = new();
            parameters.Add(new SqlParameter() { ParameterName = "@Local", Value = item.Local });
            parameters.Add(new SqlParameter() { ParameterName = "@DataEvento", Value = item.DataEvento });
            parameters.Add(new SqlParameter() { ParameterName = "@Tema", Value = item.Tema });
            parameters.Add(new SqlParameter() { ParameterName = "@QtdPessoas", Value = item.QtdPessoas });
            parameters.Add(new SqlParameter() { ParameterName = "@ImagemUrl", Value = item.ImagemUrl });
            parameters.Add(new SqlParameter() { ParameterName = "@Telefone", Value = item.Telefone });
            parameters.Add(new SqlParameter() { ParameterName = "@InsertDate", Value = item.InsertDate });
            parameters.Add(new SqlParameter() { ParameterName = "@UpdateDate", Value = item.UpdateDate });
            parameters.Add(new SqlParameter() { ParameterName = "@Deleted", Value = item.Deleted });
            parameters.Add(new SqlParameter() { ParameterName = "@Active", Value = item.Active });

            string query = @"INSERT INTO Eventos
                               (Local
                               ,DataEvento
                               ,Tema
                               ,QtdPessoas
                               ,ImagemUrl
                               ,Telefone
                               ,InsertDate
                               ,UpdateDate
                               ,Deleted
                               ,Active)
                         VALUES
                               (@Local
                               ,@DataEvento
                               ,@Tema
                               ,@QtdPessoas
                               ,@ImagemUrl
                               ,@Telefone
                               ,@InsertDate
                               ,@UpdateDate
                               ,@Deleted
                               ,@Active)";

            return Execute(query,
                parameters,
                SqlServer.EnumExecuteType.ExecuteType.NonQuery, //INSERT ATRAVÉS DE QUERY
                false);
        }

        public int ManualInsertNonProcedure(Eventos item)
        {
            List<SqlParameter> parameters = new();
            parameters.Add(new SqlParameter() { ParameterName = "@Local", Value = item.Local });
            parameters.Add(new SqlParameter() { ParameterName = "@DataEvento", Value = item.DataEvento });
            parameters.Add(new SqlParameter() { ParameterName = "@Tema", Value = item.Tema });
            parameters.Add(new SqlParameter() { ParameterName = "@QtdPessoas", Value = item.QtdPessoas });
            parameters.Add(new SqlParameter() { ParameterName = "@ImagemUrl", Value = item.ImagemUrl });
            parameters.Add(new SqlParameter() { ParameterName = "@Telefone", Value = item.Telefone });
            parameters.Add(new SqlParameter() { ParameterName = "@InsertDate", Value = item.InsertDate });
            parameters.Add(new SqlParameter() { ParameterName = "@UpdateDate", Value = item.UpdateDate });
            parameters.Add(new SqlParameter() { ParameterName = "@Deleted", Value = item.Deleted });
            parameters.Add(new SqlParameter() { ParameterName = "@Active", Value = item.Active });

            return Execute("ProcInsert",
                parameters,
                SqlServer.EnumExecuteType.ExecuteType.NonProcedure, //INSERT ATRAVÉS DE PROCEDURE
                false);
        }
    }
}

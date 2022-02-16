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

            Execute(query, parameters, SqlServer.EnumExecuteType.ExecuteType.ReaderQuery, false);
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

            Execute(query, parameters, SqlServer.EnumExecuteType.ExecuteType.ReaderQuery, false);

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
    }
}

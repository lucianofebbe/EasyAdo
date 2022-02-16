# EasyAdoSqlServer
É um gerenciador de conexão para banco de dados SqlServer.
Com ele não é mais preciso ficar gerenciado o estado da conexão, sendo somente necessário passar a query ou o nome da procedure que deseja executar, e caso o modelo que é passado pelo parâmetro do construtor for idêntico ao da tabela em questão da requisição, o mesmo possui um método de conversão automático para List<T> caso não for, poderá percorrer manualmente o DataTable de retorno e popular manualmente

Segue exemplo a baixo:

- Modelo
```sh
using System;
namespace EasyAdo.Console.Modelos
{
    public class Eventos
    {
        public int Id { get; set; }
        public string Local { get; set; }
        public DateTime DataEvento { get; set; }
        public string Tema { get; set; }
        public int QtdPessoas { get; set; }
        public string ImagemUrl { get; set; }
        public string Telefone { get; set; }
        public DateTime InsertDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public bool Deleted { get; set; }
        public bool Active { get; set; }
    }
}
```
- Repositorio
```sh
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
```
- Negocio
```sh
using EasyAdo.Console.Modelos;
using EasyAdo.Console.Repositorios;
using System;
using System.Collections.Generic;
namespace EasyAdo.Console.Negocios
{
    public class NEventos
    {
        public List<Eventos> GetByActivedAutoConverter(bool actived)
        {
            using(REventos rEventos = new REventos())
            {
                var result = rEventos.GetByActivedAutoConverter(actived);
                return result;
            }
        }

        public List<Eventos> GetByActivedManualConverter(bool actived)
        {
            using (REventos rEventos = new REventos())
            {
                var result = rEventos.GetByActivedAutoConverter(actived);
                return result;
            }
        }

        /// <summary>
        /// Caso não queira utilizar os métodos padrão do contexto e ou
        /// o modelo nao corresponda a tabela em especifico
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public int ManualInsert(Eventos item)
        {
            using (REventos rEventos = new REventos())
            {
                var result = rEventos.ManualInsert(item);
                return result;
            }
        }


        /// <summary>
        /// Métodos Default do Context.
        /// Somente utilizar, caso o model que foi passado no parametro da classe
        /// seja identico a sua tabela correspondente na base de dados
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public List<Eventos> AutoGetAll(Eventos item)
        {
            using (REventos rEventos = new REventos())
            {
                var result = rEventos.GetAll();
                return result;
            }
        }

        public int AutoInsert(Eventos item)
        {
            using (REventos rEventos = new REventos())
            {
                var result = rEventos.Insert(item);
                return result;
            }
        }

        public bool AutoUpdate(Eventos item)
        {
            using (REventos rEventos = new REventos())
            {
                var result = rEventos.Update(item, new Tuple<string, string>("Id", "2"));
                return result;
            }
        }

        public bool AutoDelete(Eventos item)
        {
            using (REventos rEventos = new REventos())
            {
                var result = rEventos.Delete(item, new Tuple<string, string>("Id", "1"));
                return result;
            }
        }
    }
}
```
- Negocio
```sh
using EasyAdo.Console.Negocios;
namespace EasyAdo.Console
{
    public class Program
    {
        static void Main(string[] args)
        {
            var resultGetByActivedAutoConverter =
                new NEventos().GetByActivedAutoConverter(false);
            
            var resultGetByActivedManualConverter =
                new NEventos().GetByActivedManualConverter(false);
        }
    }
}
```
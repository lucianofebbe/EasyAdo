using AdoNetCore.AseClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using static EasyAdo.SapAse.EnumExecuteType;

namespace EasyAdo.SapAse
{
    public class Context<Model> : IDisposable
    {
        protected string connectionString = "";
        protected int timeOut = 180;
        /// <summary>
        /// Recebe os parametros da execução.
        /// </summary>
        protected AseCommand command;
        /// <summary>
        /// Contém o retorno dos dados da consulta.
        /// </summary>
        protected DataTable dataTable;

        private StringBuilder strBuilder;
        private AseConnection connection;
        public Context()
        {
            if (string.IsNullOrEmpty(connectionString))
                throw new Exception("Especifique uma ConnectionString");
            else
            {
                dataTable = new DataTable();
                connection = new AseConnection();
                command = new AseCommand
                {
                    Connection = connection,
                    CommandTimeout = timeOut == 0 ? 180 : timeOut
                };
                OpenConnection(false);
            }
        }

        /// <summary>
        /// Abre conexao e inicia processo de execução contra a base de dados.
        /// Caso a execução seja para retornar dados, Utilize o comando READER para a leitura
        /// Exemplo:
        /// if (Execute(sql, ExecuteType.ReaderQuery, out feedBack) > 0)
        /// {
        /// while (Reader.Read()){...}
        /// }
        /// </summary>
        /// <param name="commandText">Insira o comando SQL ou o nome da procedure que deseja executar</param>
        /// <param name="executeType">Que tipo de execução contra o banco de dados quer que ocorra. (ReaderQuery), (ReaderProcedure), (NonQuery), (NonProcedure)</param>
        /// <param name="feedBack">Caso o retorno da execução seja igual a 0, será mostrado o motivo do mesmo nesse campo</param>
        /// <param name="transaction">Para força que a execução seja feita com Transaction</param>
        /// <returns></returns>
        protected int Execute(string commandText, List<AseParameter> parameters, ExecuteType executeType, bool transaction = false)
        {
            try
            {
                ConnectionStringExist();
                OpenConnection();
                int feedExecute = 0;
                if (!String.IsNullOrEmpty(commandText))
                {
                    if (connection != null)
                    {
                        command.CommandText = commandText;

                        if (parameters != null && parameters.Count > 0)
                            command.Parameters.AddRange(parameters.ToArray());

                        switch (executeType)
                        {
                            case ExecuteType.ReaderQuery:
                                command.CommandType = System.Data.CommandType.Text;
                                if (transaction) command.Transaction = connection.BeginTransaction();
                                dataTable.Load(command.ExecuteReader());
                                feedExecute = dataTable.Rows.Count;
                                break;
                            case ExecuteType.ReaderProcedure:
                                command.CommandType = System.Data.CommandType.StoredProcedure;
                                if (transaction) command.Transaction = connection.BeginTransaction();
                                dataTable.Load(command.ExecuteReader());
                                feedExecute = dataTable.Rows.Count;
                                break;
                            case ExecuteType.NonQuery:
                                command.CommandType = System.Data.CommandType.Text;
                                if (transaction) command.Transaction = connection.BeginTransaction();
                                feedExecute = command.ExecuteNonQuery();
                                break;
                            case ExecuteType.NonProcedure:
                                command.CommandType = System.Data.CommandType.StoredProcedure;
                                if (transaction) command.Transaction = connection.BeginTransaction();
                                feedExecute = command.ExecuteNonQuery();
                                break;
                        }
                        return feedExecute;
                    }
                    else
                        throw new Exception("Não foi possível abrir conexão.");
                }
                else
                    throw new Exception("Query vazia.");
            }
            catch (Exception ex) { throw new Exception("Context Execute", ex); }
            finally { CloseConnection(); }
        }

        public virtual int Insert(Model entidade)
        {
            try
            {
                ConnectionStringExist();
                OpenConnection();
                int idReturn = 0;
                if (entidade != null)
                {
                    var tipo = entidade.GetType();
                    strBuilder = new StringBuilder();
                    strBuilder.Append("INSERT INTO ");
                    strBuilder.Append(tipo.Name + " (");

                    foreach (var prop in tipo.GetProperties())
                    {
                        if (prop.GetValue(entidade) != null &&
                            !String.IsNullOrEmpty(prop.GetValue(entidade).ToString()) &&
                            prop.Name != "Id")
                        {
                            strBuilder.Append(prop.Name + ",");
                            command.Parameters.AddWithValue("@" + prop.Name, prop.GetValue(entidade));
                        }
                    }
                    strBuilder.Remove(strBuilder.Length - 1, 1);
                    strBuilder.Append(") VALUES (");
                    if (command.Parameters.Count > 0)
                    {
                        foreach (var par in command.Parameters)
                            strBuilder.Append(par.ToString() + ",");
                        strBuilder.Remove(strBuilder.Length - 1, 1);
                        strBuilder.Append("); SELECT SCOPE_IDENTITY()");
                        try
                        {
                            command.CommandText = strBuilder.ToString();
                            command.Connection = connection;
                            command.CommandType = CommandType.Text;
                            idReturn = int.Parse(command.ExecuteScalar().ToString());
                            return idReturn;
                        }
                        catch (Exception ex) { throw new Exception("Context Insert", ex); }
                    }
                    else
                        return idReturn;
                }
                else
                    return idReturn;
            }
            catch (Exception ex) { throw new Exception("Context Insert", ex); }
            finally { CloseConnection(); }
        }

        /// <summary>
        /// Parametro Model, Objeto que será utilizado para atualizar a base de dados
        /// Tuple: Key do tuple é utilizado na condição Where para determinar a coluna que será utilizada
        /// como regra.
        /// Value: Value do tuple é a condição que a coluna tem que ter para efetuar a atualização do registro.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="KeyValue"></param>
        /// <returns></returns>
        public virtual bool Update(Model model, Tuple<string, string> KeyValue)
        {
            try
            {
                ConnectionStringExist();
                OpenConnection();
                int idReturn = 0;
                if (model != null)
                {
                    var type = model.GetType();
                    strBuilder = new StringBuilder();
                    strBuilder.Append("UPDATE ");
                    strBuilder.Append(type.Name + " SET ");

                    foreach (var prop in type.GetProperties())
                    {
                        if (prop.GetValue(model) != null &&
                            !String.IsNullOrEmpty(prop.GetValue(model).ToString()) &&
                            prop.Name != "Id")
                        {
                            strBuilder.Append(prop.Name + "=@" + prop.Name + ",");
                            command.Parameters.AddWithValue("@" + prop.Name, prop.GetValue(model));
                        }
                    }
                    strBuilder.Remove(strBuilder.Length - 1, 1);
                    if (command.Parameters.Count > 0)
                    {
                        try
                        {
                            strBuilder.Append(" WHERE " + KeyValue.Item1 + " = " + KeyValue.Item2);
                            command.CommandText = strBuilder.ToString();
                            command.Connection = connection;
                            command.CommandType = CommandType.Text;
                            idReturn = command.ExecuteNonQuery();
                            return (idReturn > 0);
                        }
                        catch (Exception ex)
                        { throw new Exception("Context Update", ex); }
                    }
                    else
                        return false;
                }
                else
                    return false;
            }
            catch (Exception ex)
            { throw new Exception("Context Update", ex); }
            finally { CloseConnection(); }
        }

        /// <summary>
        /// Parametro Model, Objeto que será utilizado como referencia a tabela da base de dados
        /// Tuple: Key do tuple é utilizado na condição Where para determinar a coluna que será utilizada
        /// como regra.
        /// Value: Value do tuple é a condição que a coluna tem que ter para efetuar a exclusão do registro.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="KeyValue"></param>
        /// <returns></returns>
        public virtual bool Delete(Model model, Tuple<string, string> KeyValue)
        {
            try
            {
                ConnectionStringExist();
                OpenConnection();
                int idReturn = 0;
                if (model != null)
                {
                    var type = model.GetType();
                    strBuilder = new StringBuilder();
                    strBuilder.Append("DELETE FROM ");
                    strBuilder.Append(type.Name);
                    try
                    {
                        strBuilder.Append(" WHERE " + KeyValue.Item1 + " = " + KeyValue.Item2);
                        command.CommandText = strBuilder.ToString();
                        command.Connection = connection;
                        command.CommandType = CommandType.Text;
                        idReturn = command.ExecuteNonQuery();
                        return (idReturn > 0);
                    }
                    catch (Exception ex)
                    { throw new Exception("Context Delete", ex); }
                }
                else
                    return false;
            }
            catch (Exception ex)
            { throw new Exception("Context Update", ex); }
            finally { CloseConnection(); }
        }

        public virtual List<Model> FindAll()
        {
            try
            {
                ConnectionStringExist();
                OpenConnection();
                var model = Activator.CreateInstance<Model>();
                if (model != null)
                {
                    var type = model.GetType();
                    strBuilder = new StringBuilder();
                    strBuilder.Append("SELECT ");

                    foreach (var prop in type.GetProperties())
                        strBuilder.Append(prop.Name + ",");

                    strBuilder.Remove(strBuilder.Length - 1, 1);
                    strBuilder.Append(" FROM " + type.Name);

                    command.CommandText = strBuilder.ToString();
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                            dataTable.Load(reader);
                    }
                    return ConverterDataTableToList<Model>(dataTable);
                }
                else
                    return null;
            }
            catch (Exception ex)
            { throw new Exception("Context FindAll", ex); }
            finally { CloseConnection(); }
        }

        public List<T> ConverterDataTableToList<T>(DataTable dataTable)
        {
            try
            {
                List<T> data = new();
                foreach (DataRow row in dataTable.Rows)
                {
                    T item = GetItem<T>(row);
                    data.Add(item);
                }
                return data;
            }
            catch (Exception ex)
            { throw new Exception("Context ConverterDataTableToList", ex); }
        }

        private static T GetItem<T>(DataRow dataRow)
        {
            try
            {
                Type tipoObjeto = typeof(T);
                T objeto = Activator.CreateInstance<T>();
                foreach (DataColumn coluna in dataRow.Table.Columns)
                {
                    PropertyInfo propInfoColunaObjeto = tipoObjeto.GetProperties().FirstOrDefault(o => o.Name == coluna.ColumnName);
                    Type propColunaObjeto = Nullable.GetUnderlyingType(propInfoColunaObjeto.PropertyType) ?? propInfoColunaObjeto.PropertyType;

                    #region validacao de tipos
                    object valor = null;
                    if (DBNull.Value.Equals(dataRow[coluna.ColumnName]))
                    {
                        switch (Type.GetTypeCode(propColunaObjeto))
                        {
                            case TypeCode.Boolean:
                                valor = null;
                                break;
                            case TypeCode.Byte:
                                valor = 0;
                                break;
                            case TypeCode.Char:
                                valor = "";
                                break;
                            case TypeCode.DateTime:
                                valor = null;
                                break;
                            case TypeCode.DBNull:
                                valor = null;
                                break;
                            case TypeCode.Decimal:
                                valor = 0;
                                break;
                            case TypeCode.Double:
                                valor = 0;
                                break;
                            case TypeCode.Empty:
                                valor = string.Empty;
                                break;
                            case TypeCode.Int16:
                                valor = 0;
                                break;
                            case TypeCode.Int32:
                                valor = 0;
                                break;
                            case TypeCode.Int64:
                                valor = 0;
                                break;
                            case TypeCode.Object:
                                valor = null;
                                break;
                            case TypeCode.SByte:
                                valor = 0;
                                break;
                            case TypeCode.Single:
                                valor = 0;
                                break;
                            case TypeCode.String:
                                valor = string.Empty;
                                break;
                            case TypeCode.UInt16:
                                valor = 0;
                                break;
                            case TypeCode.UInt32:
                                valor = 0;
                                break;
                            case TypeCode.UInt64:
                                valor = 0;
                                break;
                        }
                    }
                    else
                        valor = Convert.ChangeType(dataRow[coluna.ColumnName], propColunaObjeto);
                    #endregion

                    propInfoColunaObjeto.SetValue(objeto, valor, null);
                }
                return objeto;
            }
            catch (Exception ex)
            { throw new Exception("Context GetItem", ex); }
        }

        private void OpenConnection(bool openConnection = true)
        {
            try
            {
                if (dataTable != null)
                {
                    dataTable.Dispose();
                    dataTable.Clear();
                }

                connection.ConnectionString = connectionString;

                if (openConnection)
                    connection.Open();
            }
            catch (Exception ex)
            { throw new Exception("Context Open", ex); }
        }

        private void CloseConnection()
        {
            try
            {
                if (connection != null)
                {
                    connection.Close();
                    connection.Dispose();
                }

                if (command != null)
                {
                    command.Dispose();
                }
            }
            catch (Exception ex)
            { throw new Exception("Context CloseConnection", ex); }
        }

        private void ConnectionStringExist()
        {
            if (string.IsNullOrEmpty(connectionString))
                throw new Exception("Especifique uma ConnectionString");
        }

        public void Dispose()
        {
            CloseConnection();
            GC.SuppressFinalize(this);
        }
    }

    public class EnumExecuteType
    {
        public enum ExecuteType
        {
            ReaderQuery,
            ReaderProcedure,
            NonQuery,
            NonProcedure
        }
    }
}

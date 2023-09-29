using Dapper;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace DomainModel.Concrete
{
    public class BaseRepository
    {
        public string connectionString = "";
        IDbConnection conn_transaction = null;
        public IDbTransaction transaction = null;

        protected T QueryFirstOrDefault<T>(string sql, object parameters = null)
        {
            if (conn_transaction != null && transaction != null)
            {
                return conn_transaction.QueryFirstOrDefault<T>(sql, parameters, transaction);

            }
            else
            {
                using (var connection = CreateConnection())
                {
                    return connection.QueryFirstOrDefault<T>(sql, parameters);
                }
            }

        }

        protected List<T> Query<T>(string sql, object parameters = null)
        {
            if (conn_transaction != null && transaction != null)
            {
                return conn_transaction.Query<T>(sql, parameters, transaction).ToList();
            }
            else
            {
                using (var connection = CreateConnection())
                {
                    return connection.Query<T>(sql, parameters).ToList();
                }
            }
        }

        protected int Execute(string sql, object parameters = null)
        {
            if (conn_transaction != null && transaction != null)
            {
                return conn_transaction.Execute(sql, parameters, transaction: transaction);
            }
            else
            {
                using (var connection = CreateConnection())
                {
                    return connection.Execute(sql, parameters);
                }
            }

        }

        protected int ExecuteScalar(string sql, object parameters = null)
        {
            int return_val = 0;
            if (conn_transaction != null && transaction != null)
            {
                Object val = conn_transaction.ExecuteScalar(sql, parameters, transaction: transaction);
                if (val != null)
                {
                    return_val = (int)val;
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                using (var connection = CreateConnection())
                {
                    connection.Open();
                    using (var transaction = connection.BeginTransaction())
                    {
                        Object val = connection.ExecuteScalar(sql, parameters, transaction: transaction);
                        if (val != null)
                        {
                            return_val = (int)val;
                        }
                        else
                        {
                            return 0;
                        }

                        transaction.Commit();
                    }
                }
            }

            return return_val;
        }

        protected T ExecuteScalar<T>(string sql, object parameters = null)
        {
            if (conn_transaction != null && transaction != null)
            {
                var return_val = conn_transaction.ExecuteScalar<T>(sql, parameters, transaction: transaction);
                return return_val;
            }
            else
            {
                using (var connection = CreateConnection())
                {
                    var return_val = connection.ExecuteScalar<T>(sql, parameters);
                    return return_val;
                }
            }
        }


        private IDbConnection CreateConnection()
        {
            // Postgres local
            // var connection = new NpgsqlConnection("Server=127.0.0.1;Port=5432;Database=ProjectTemplateDB;User Id=postgres;Password=t66ZsZbBeT0F1g3VZrh1;");
            var connection = new NpgsqlConnection("Server=localhost; Port=5433; Database=ProjectTemplateDB; User Id=postgres;Password=root;");

            return connection;
        }
    }
}

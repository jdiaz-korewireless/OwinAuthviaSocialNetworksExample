using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace AuthDomain.Dal
{
    class BaseDal : IBaseDal
    {
        public void Execute(IsolationLevel isolationLevel, Action<SqlTransaction> action)
        {
            using (var connection = CreateSqlConnection())
            {
                connection.Open();

                using (SqlTransaction tran = connection.BeginTransaction(isolationLevel))
                {
                    action(tran);
                    tran.Commit();
                }
            }
        }

        public TResult Execute<TResult>(IsolationLevel isolationLevel, Func<SqlTransaction, TResult> function)
        {
            using (var connection = CreateSqlConnection())
            {
                connection.Open();

                using (SqlTransaction tran = connection.BeginTransaction(isolationLevel))
                {
                    var result = function(tran);
                    tran.Commit();

                    return result;
                }
            }
        }

        private static SqlConnection CreateSqlConnection()
        {
            return new SqlConnection(ConfigurationManager.ConnectionStrings["AuthDB"].ConnectionString);
        }
    }
}

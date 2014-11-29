using System;
using System.Data;
using System.Data.SqlClient;

namespace AuthDomain.Dal
{
    public interface IBaseDal
    {
        void Execute(IsolationLevel isolationLevel, Action<SqlTransaction> action);

        TResult Execute<TResult>(IsolationLevel isolationLevel, Func<SqlTransaction, TResult> function);
    }
}

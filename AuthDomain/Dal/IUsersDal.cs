using AuthDomain.Models.Account;
using System;
using System.Data;
using System.Data.SqlClient;

namespace AuthDomain.Dal
{
    public interface IUsersDal
    {
        TResult Execute<TResult>(IsolationLevel isolationLevel, Func<SqlTransaction, TResult> function);

        UserDb GetUser(SqlTransaction transaction, string email);

        UserDb GetUser(SqlTransaction transaction, ExternalLoginProvider loginProvider, string providerKey);

        UserDb CreateUser(SqlTransaction transaction, UserRegistration userRegistration);

        void CreateUserExtLoginInfo(SqlTransaction transaction, int userId, ExternalLoginInfo externalLoginInfo);

        void CreateUserAvatar(SqlTransaction transaction, int userId, byte[] avatar);

        void UpdateUserAvatar(SqlTransaction transaction, int userId, byte[] avatar);
    }
}
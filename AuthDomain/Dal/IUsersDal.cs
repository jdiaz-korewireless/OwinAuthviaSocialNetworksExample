using AuthDomain.Models.Account;
using System;
using System.Data;
using System.Data.SqlClient;

namespace AuthDomain.Dal
{
    public interface IUsersDal
    {
        void Execute(IsolationLevel isolationLevel, Action<SqlTransaction> action);

        TResult Execute<TResult>(IsolationLevel isolationLevel, Func<SqlTransaction, TResult> function);

        UserDb GetUser(SqlTransaction transaction, int userId);

        UserDb GetUser(SqlTransaction transaction, string email);

        UserDb GetUser(SqlTransaction transaction, ExternalLoginProvider loginProvider, string providerKey);

        byte[] GetAvatar(SqlTransaction transaction, int userId);

        UserDb CreateUser(SqlTransaction transaction, UserRegistration userRegistration);

        void CreateUserExtLoginInfo(SqlTransaction transaction, int userId, ExternalLoginInfo externalLoginInfo);

        void CreateUserAvatar(SqlTransaction transaction, int userId, byte[] avatar);

        UserDb UpdateUser(SqlTransaction transaction, UserDb user);

        void UpdateUserAvatar(SqlTransaction transaction, int userId, byte[] avatar);

        void DeleteUserWithDependencies(SqlTransaction transaction, int userId);
    }
}
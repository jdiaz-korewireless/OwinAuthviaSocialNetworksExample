using AuthDomain.Models.Account;
using AuthDomain.Resources;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace AuthDomain.Dal
{
    class UsersDal : IUsersDal
    {
        public UserDb GetUser(SqlTransaction transaction, string email)
        {
            using (var cmd = new SqlCommand("[dbo].[spGetUserByEmail]", transaction.Connection, transaction))
            {
                cmd.Parameters.AddWithValue("email", email);
                cmd.CommandType = CommandType.StoredProcedure;

                using (var sqlDataReader = cmd.ExecuteReader())
                {
                    var users = MapUsersFromDb(sqlDataReader);

                    if (users.Count > 1)
                        throw new InvalidOperationException(string.Format(Exceptions.MoreThanOneUserFoundByEmail, email));

                    return users.SingleOrDefault();
                }
            }
        }

        public UserDb GetUser(SqlTransaction transaction, ExternalLoginProvider loginProvider, string providerKey)
        {  
            using (var cmd = new SqlCommand("[dbo].[spGetExternalUser]", transaction.Connection, transaction))
            {
                cmd.Parameters.AddWithValue("providerId", loginProvider);
                cmd.Parameters.AddWithValue("providerKey", providerKey);
                cmd.CommandType = CommandType.StoredProcedure;

                using (var sqlDataReader = cmd.ExecuteReader())
                {
                    var users = MapUsersFromDb(sqlDataReader);

                    if (users.Count > 1)
                        throw new InvalidOperationException(string.Format(Exceptions.MoreThanOneUserFoundByExtProvider, loginProvider, providerKey));

                    return users.SingleOrDefault();
                }
            }
        }

        public UserDb CreateUser(SqlTransaction transaction, UserRegistration userRegistration)
        {
            using (var cmd = new SqlCommand("[dbo].[spCreateUser]", transaction.Connection, transaction))
            {
                var createdDate = DateTime.Now;
                var verifyEmailCode = Guid.NewGuid();

                cmd.Parameters.AddWithValue("email", userRegistration.Email);
                cmd.Parameters.AddWithValue("password", NullableValue(userRegistration.Password));
                cmd.Parameters.AddWithValue("fullName", NullableValue(userRegistration.FullName));
                cmd.Parameters.AddWithValue("createdDate", createdDate);
                cmd.Parameters.AddWithValue("updatedDate", createdDate);
                cmd.Parameters.AddWithValue("verifyEmailCode", verifyEmailCode);
                cmd.CommandType = CommandType.StoredProcedure;

                int userId = Convert.ToInt32(cmd.ExecuteScalar());

                return new UserDb()
                {
                    Id = userId,
                    Email = userRegistration.Email,
                    Password = userRegistration.Password,
                    FullName = userRegistration.FullName,
                    CreatedDate = createdDate,
                    TimeStamp = createdDate,
                    VerifyEmailCode = verifyEmailCode,
                    IsVerified = false,
                    AvatarUrl = GetAvatarUrl(userId)
                };
            }
        }

        public void CreateUserExtLoginInfo(SqlTransaction transaction, int userId, ExternalLoginInfo externalLoginInfo)
        {
            using (var cmd = new SqlCommand("[dbo].[spCreateUserExtLogin]", transaction.Connection, transaction))
            {                
                cmd.Parameters.AddWithValue("userId", userId);
                cmd.Parameters.AddWithValue("providerId", externalLoginInfo.ProviderType);
                cmd.Parameters.AddWithValue("providerKey", externalLoginInfo.ProviderKey);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.ExecuteNonQuery();
            }
        }

        public void CreateUserAvatar(SqlTransaction transaction, int userId, byte[] avatar)
        {
            throw new NotImplementedException();
        }

        public void UpdateUserAvatar(SqlTransaction transaction, int userId, byte[] avatar)
        {
            throw new NotImplementedException();
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

        private static IList<UserDb> MapUsersFromDb(SqlDataReader sqlDataReader)
        {
            IList<UserDb> result = new List<UserDb>();

            while (sqlDataReader.Read())
            {
                int userId = (int)sqlDataReader["Id"];
                Guid? verifyEmailCode = (Guid?)sqlDataReader["VerifyEmailCode"];

                var user = new UserDb()
                {
                    Id = userId,
                    Email = (string)sqlDataReader["Email"],
                    Password = (string)sqlDataReader["Password"],
                    FullName = (string)sqlDataReader["FullName"],
                    CreatedDate = (DateTime)sqlDataReader["CreatedDate"],
                    TimeStamp = (DateTime)sqlDataReader["UpdatedDate"],
                    VerifyEmailCode = verifyEmailCode,
                    IsVerified = !verifyEmailCode.HasValue,
                    AvatarUrl = GetAvatarUrl(userId)
                };

                result.Add(user);
            }

            return result;
        }

        private static string GetAvatarUrl(int userId)
        {
            return string.Format("/avatar/{0}?anticache={1}", userId, Environment.TickCount);
        }

        private static object NullableValue(string value)
        {
            if (string.IsNullOrEmpty(value))
                return DBNull.Value;
            else
                return value.Trim();
        }
    }
}

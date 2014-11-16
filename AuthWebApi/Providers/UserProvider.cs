using AuthDomain.Logic;
using AuthDomain.Models.Account;
using AuthWebApi.Models.Account;
using AuthWebApi.Utils;
using Microsoft.AspNet.Identity;
using System;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;

namespace AuthWebApi.Providers
{
    public class UserProvider
    {
        public UsersManager UsersManager { get; set; }

        public UserProvider()
        {
            this.UsersManager = new UsersManager();
        }

        public bool IsRegisteredUserUpdated(ClaimsIdentity claimsIdentity)
        {
            var userFromToken = this.TryGetRegisteredUserFromIdentity(claimsIdentity);
            if (userFromToken == null)
                return false;//user is not registered, skip the check

            var userFromDB = this.FindAsync(claimsIdentity).Result;
            return userFromToken.TimeStamp != userFromDB.TimeStamp;
        }

        /// <summary>
        /// Extracts user info from identity
        /// </summary>
        /// <param name="identity"></param>
        /// <returns></returns>
        public User TryGetRegisteredUserFromIdentity(ClaimsIdentity claimsIdentity)
        {
            if (claimsIdentity == null || !claimsIdentity.IsAuthenticated)
                return null;

            //if issued by external => user is not registered, return null
            Claim userIdClaim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim.Issuer != ClaimsIdentity.DefaultIssuer)
                return null;

            return OwinHelper.CreateUser(claimsIdentity);
        }

        public Task<User> FindAsync(IIdentity identity)
        {
            int userId = Int32.Parse(identity.GetUserId());
            return this.UsersManager.GetUserAsync(userId);
        }

        public Task<User> FindAsync(string login, string password)
        {
            throw new NotImplementedException();

            //return this.UsersManager.GetUserAsync(login, password);
        }

        public Task<User> FindAsync(ExternalLoginProvider loginProvider, string providerKey)
        {
            return this.UsersManager.GetUserAsync(loginProvider, providerKey);
        }

        public async Task<User> CreateExternalAsync(ExternalLoginModel externalInfo)
        {
            var userRegistration = new UserRegistration()
            {
                Email = externalInfo.Email,
                FullName = externalInfo.FullName,
                Avatar = await GetExternalAvatarAsync(externalInfo),
                ExternalLoginInfo = new ExternalLoginInfo
                {
                    ProviderType = externalInfo.Provider,
                    ProviderKey = externalInfo.ProviderKey
                }
            };

            return await this.UsersManager.CreateUserAsync(userRegistration);
        }

        public Task<byte[]> GetAvatarAsync(int userId)
        {
            return this.UsersManager.GetAvatarAsync(userId);
        }

        public Task<UserVerification> VerifyAsync(IIdentity identity)
        {
            int userId = Int32.Parse(identity.GetUserId());
            return this.UsersManager.VerifyAsync(userId);
        }

        public Task DeleteUserWithDependenciesAsync(User user)
        {
            return this.UsersManager.DeleteUserWithDependenciesAsync(user.Id);
        }

        private static Task<byte[]> GetExternalAvatarAsync(ExternalLoginModel externalInfo)
        {
            if (string.IsNullOrEmpty(externalInfo.AvatarUrl))
                return null;

            var client = HttpHelper.CreateHttpClient();
            return client.GetByteArrayAsync(externalInfo.AvatarUrl);
        }

        public static UserViewModel MapUserToViewModel(User user, ExternalLoginModel externalLogin)
        {
            return new UserViewModel
            {
                Email = user.Email,
                FullName = user.FullName,
                IsVerified = user.IsVerified,
                AvatarUrl = GetAvatarUrl(user),
                IsRegistered = true,
                LoginProvider = externalLogin.Provider.ToString()
            };
        }

        public static string GetAvatarUrl(User user)
        {
            return string.Format("api/account/avatar/{0}?anticache={1}", user.Id, Environment.TickCount);
        }
    }
}
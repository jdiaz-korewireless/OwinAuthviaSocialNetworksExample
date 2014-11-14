using AuthDomain.Logic;
using AuthDomain.Models.Account;
using AuthWebApi.Models.Account;
using AuthWebApi.Providers.ClaimsMappingStrategies;
using AuthWebApi.Utils;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;

namespace AuthWebApi.Providers
{
    public class UserProvider
    {
        public static string ClaimTypeAvatarUrl = "avatarUrl";

        public UsersManager UsersManager { get; set; }

        public UserProvider()
        {
            this.UsersManager = new UsersManager();
        }

        public ClaimsIdentity CreateIdentity(ClaimsMapper claimsMapper, string authenticationType)
        {
            IList<Claim> claims = new List<Claim>();

            claims.Add(new Claim(ClaimTypes.NameIdentifier, claimsMapper.Id, null, claimsMapper.Issuer, claimsMapper.OriginalIssuer));
            claims.Add(new Claim(ClaimTypes.Email, claimsMapper.Email, null, claimsMapper.Issuer, claimsMapper.OriginalIssuer));
            claims.Add(new Claim(ClaimTypes.GivenName, claimsMapper.FullName, null, claimsMapper.Issuer, claimsMapper.OriginalIssuer));
            claims.Add(new Claim(ClaimTypes.Sid, claimsMapper.Sid, null, claimsMapper.Issuer, claimsMapper.OriginalIssuer));
            claims.Add(new Claim(ClaimTypes.Version, claimsMapper.Version, null, claimsMapper.Issuer, claimsMapper.OriginalIssuer));
            claims.Add(new Claim(ClaimTypeAvatarUrl, claimsMapper.AvatarUrl, null, claimsMapper.Issuer, claimsMapper.OriginalIssuer));

            return new ClaimsIdentity(claims, authenticationType);
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

            return new User
            {
                Id = Int32.Parse(userIdClaim.Value),
                Email = claimsIdentity.FindFirstValue(ClaimTypes.Email),
                FullName = claimsIdentity.FindFirstValue(ClaimTypes.GivenName),
                AvatarUrl = claimsIdentity.FindFirstValue(ClaimTypeAvatarUrl),
                TimeStamp = ClaimsMapper.GetTimeStamp(claimsIdentity.FindFirstValue(ClaimTypes.Version))
            };
        }

        public Task<User> FindAsync(IIdentity identity)
        {
            int userId = Int32.Parse(identity.GetUserId());
            throw new NotImplementedException();

            //return this.UsersManager.GetUserAsync(userId);
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
            var user = new UserRegistration()
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

            return await this.UsersManager.CreateUserAsync(user);
        }

        private Task<byte[]> GetExternalAvatarAsync(ExternalLoginModel externalInfo)
        {
            if (string.IsNullOrEmpty(externalInfo.AvatarUrl))
                return null;

            var client = HttpHelper.CreateHttpClient();
            return client.GetByteArrayAsync(externalInfo.AvatarUrl);
        }
    }
}
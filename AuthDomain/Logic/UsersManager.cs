using AuthDomain.Models.Account;
using System.Threading.Tasks;

namespace AuthDomain.Logic
{
    public class UsersManager
    {
        public Task<User> GetUserAsync(ExternalLoginProvider loginProvider, string providerKey)
        {
            return Task<User>.Factory.StartNew(() =>
            {
                return null;
            });
        }
    }
}

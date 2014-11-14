using System.Collections.Generic;

namespace AuthDomain.Models.Account
{
    public class UserRegistration
    {        
        public string Email { get; set; }

        public string FullName { get; set; }
        
        public string Password { get; set; }

        public byte[] Avatar { get; set; }

        public ExternalLoginInfo ExternalLoginInfo { get; set; }
    }
}

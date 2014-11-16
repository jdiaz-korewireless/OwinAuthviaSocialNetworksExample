using System;

namespace AuthDomain.Models.Account
{
    public class UserVerification
    {
        public User User { get; set; }

        public Guid VerifyEmailCode { get; set; }
    }
}

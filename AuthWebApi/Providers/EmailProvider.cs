using AuthDomain.Models.Account;
using AuthWebApi.Resources;
using Mvc.Mailer;
using System;
using System.Threading.Tasks;

namespace AuthWebApi.Providers
{
    public class EmailProvider : MailerBase
    {
        public EmailProvider()
        {
            base.MasterName = "_MailLayout";
        }

        public void SendConfirmationCodeAsync(UserVerification userVerification)
        {
            if (userVerification == null)
                throw new ArgumentNullException("userVerification");

            ViewData.Model = userVerification;
            var mailMessage = Populate(x =>
            {
                x.Subject = Emails.RegistrationConfirmation;
                x.ViewName = "RegistrationConfirmation";
                x.To.Add(userVerification.User.Email);
            });

            Task.Factory.StartNew(() => mailMessage.Send());
        }

        public void SendDeleteConfirmationAsync(User user)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            ViewData.Model = user;
            var mailMessage = Populate(x =>
            {
                x.Subject = Emails.DeleteConfirmation;
                x.ViewName = "DeleteConfirmation";
                x.To.Add(user.Email);
            });

            Task.Factory.StartNew(() => mailMessage.Send());
        }
    }
}
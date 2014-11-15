namespace AuthDomain.Models.Account
{
    public class ExternalLoginInfo
    {
        public ExternalLoginProvider ProviderType { get; set; }

        public string ProviderKey { get; set; }
    }
}

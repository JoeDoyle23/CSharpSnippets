namespace WebApi.HmacAuth
{
    public class Configuration
    {
        public const string ApplicationIdHeader = "X-ApiAuth-ApplicationId";
        public const string UserHeader = "X-ApiAuth-User";
        public const string AuthenticationScheme = "ApiAuth";
        public const int ValidityPeriodInMinutes = 5;
    }
}
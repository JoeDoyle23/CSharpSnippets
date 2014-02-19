namespace WebApi.HmacAuth.Interfaces
{
    public interface ISecretRepository
    {
        string GetToken(string applicationId, string user);
    }
}
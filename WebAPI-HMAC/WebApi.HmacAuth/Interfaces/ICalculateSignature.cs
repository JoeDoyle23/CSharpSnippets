namespace WebApi.HmacAuth.Interfaces
{
    public interface ICalculateSignature
    {
        string Signature(string secret, string value);
    }
}
using System.Net.Http;

namespace WebApi.HmacAuth.Interfaces
{
    public interface IHashHelper
    {
        byte[] ComputeHash(HttpContent httpContent);
    }
}
using System.Net.Http;

namespace WebApi.HmacAuth.Interfaces
{
    public interface IBuildMessageRepresentation
    {
        string BuildRequestRepresentation(HttpRequestMessage requestMessage);
    }
}
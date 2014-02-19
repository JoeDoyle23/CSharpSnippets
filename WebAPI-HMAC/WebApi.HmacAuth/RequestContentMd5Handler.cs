using System.Net.Http;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace WebApi.HmacAuth
{
    public class RequestContentMd5Handler : DelegatingHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request.Content == null)
            {
                return base.SendAsync(request, cancellationToken);
            }

            var content = request.Content.ReadAsByteArrayAsync().Result;
            var md5 = MD5.Create();
            var hash = md5.ComputeHash(content);
            request.Content.Headers.ContentMD5 = hash;
            return base.SendAsync(request, cancellationToken);
        }
    }
}
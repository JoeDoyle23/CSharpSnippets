using System.Net.Http;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace WebApi.HmacAuth
{
    public class ResponseContentMd5Handler : DelegatingHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var response = base.SendAsync(request, cancellationToken).Result;
            if (response.Content != null)
            {
                var content = response.Content.ReadAsByteArrayAsync().Result;
                var md5 = MD5.Create();
                var hash = md5.ComputeHash(content);
                response.Content.Headers.ContentMD5 = hash;
            }

            return Task<HttpResponseMessage>.Factory.StartNew(() => response, cancellationToken);
        }
    }
}
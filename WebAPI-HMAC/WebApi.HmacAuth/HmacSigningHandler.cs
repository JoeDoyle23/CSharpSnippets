using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using WebApi.HmacAuth.Interfaces;

namespace WebApi.HmacAuth
{
    public class HmacSigningHandler : HttpClientHandler
    {
        private readonly IBuildMessageRepresentation _representationBuilder;
        private readonly ICalculateSignature _signatureCalculator;

        public Guid Token { get; set; }
        public string ApplicationId { get; set; }
        public string User { get; set; }

        public HmacSigningHandler(IBuildMessageRepresentation representationBuilder, ICalculateSignature signatureCalculator)
        {
            _representationBuilder = representationBuilder;
            _signatureCalculator = signatureCalculator;
        }

        protected  override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (!request.Headers.Contains(Configuration.ApplicationIdHeader))
            {
                request.Headers.Add(Configuration.ApplicationIdHeader, ApplicationId);
                request.Headers.Add(Configuration.UserHeader, User);
            }

            request.Headers.Date = new DateTimeOffset(DateTime.Now,DateTime.Now-DateTime.UtcNow);
            var representation = _representationBuilder.BuildRequestRepresentation(request);
            var signature = _signatureCalculator.Signature(Token.ToString(), representation);

            var header = new AuthenticationHeaderValue(Configuration.AuthenticationScheme, signature);

            request.Headers.Authorization = header;
            return base.SendAsync(request, cancellationToken);
        }
    }
}
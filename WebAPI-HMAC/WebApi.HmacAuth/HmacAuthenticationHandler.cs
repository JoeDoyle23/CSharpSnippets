using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using WebApi.HmacAuth.Interfaces;

namespace WebApi.HmacAuth
{
    public class HmacAuthenticationHandler : DelegatingHandler
    {
        private const string UNAUTHORIZED_MESSAGE = "Unauthorized request";

        private readonly ISecretRepository _secretRepository;
        private readonly IBuildMessageRepresentation _representationBuilder;
        private readonly ICalculateSignature _signatureCalculator;
        private readonly IHashHelper _hashHelper;

        public HmacAuthenticationHandler(ISecretRepository secretRepository,
            IBuildMessageRepresentation representationBuilder,
            ICalculateSignature signatureCalculator,
            IHashHelper hashHelper)
        {
            _secretRepository = secretRepository;
            _representationBuilder = representationBuilder;
            _signatureCalculator = signatureCalculator;
            _hashHelper = hashHelper;
        }

        protected bool IsAuthenticated(HttpRequestMessage requestMessage)
        {
            if (!AreHeadersValid(requestMessage))
            {
                return false;
            }

            var user = requestMessage.Headers.GetValues(Configuration.UserHeader).First();
            var applicationId = requestMessage.Headers.GetValues(Configuration.ApplicationIdHeader).First();
            
            var secret = _secretRepository.GetToken(applicationId, user);
            if (secret == null)
            {
                return false;
            }

            var representation = _representationBuilder.BuildRequestRepresentation(requestMessage);
            if (representation == null)
            {
                return false;
            }

            if (requestMessage.Content.Headers.ContentMD5 != null 
                && !IsMd5Valid(requestMessage))
            {
                return false;
            }

            var signature = _signatureCalculator.Signature(secret, representation);

            return requestMessage.Headers.Authorization.Parameter == signature;

        }

        private bool AreHeadersValid(HttpRequestMessage requestMessage)
        {
            return requestMessage.Headers.Contains(Configuration.ApplicationIdHeader) &&
                   requestMessage.Headers.Contains(Configuration.UserHeader) &&
                   IsDateValid(requestMessage) &&
                   IsAuthorizationValid(requestMessage);
        }

        private bool IsMd5Valid(HttpRequestMessage requestMessage)
        {
            var hashHeader = requestMessage.Content.Headers.ContentMD5;
            if (requestMessage.Content == null)
            {
                return hashHeader == null || hashHeader.Length == 0;
            }
            var hash = _hashHelper.ComputeHash(requestMessage.Content);
            return hash.SequenceEqual(hashHeader);
        }

        private bool IsDateValid(HttpRequestMessage requestMessage)
        {
            if (!requestMessage.Headers.Date.HasValue)
                return false;

            var utcNow = DateTime.UtcNow;
            var date = requestMessage.Headers.Date.Value.UtcDateTime;
            
            return date < utcNow.AddMinutes(Configuration.ValidityPeriodInMinutes) && 
                   date > utcNow.AddMinutes(-Configuration.ValidityPeriodInMinutes);
        }

        private bool IsAuthorizationValid(HttpRequestMessage requestMessage)
        {
            return requestMessage.Headers.Authorization != null && requestMessage.Headers.Authorization.Scheme == Configuration.AuthenticationScheme;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
        {
            var isAuthenticated = IsAuthenticated(request);

            if (!isAuthenticated)
            {
                var response =request.CreateErrorResponse(HttpStatusCode.Unauthorized, UNAUTHORIZED_MESSAGE);
                response.Headers.WwwAuthenticate.Add(new AuthenticationHeaderValue(Configuration.AuthenticationScheme));
                return Task<HttpResponseMessage>.Factory.StartNew(() => response, cancellationToken);
            }

            return base.SendAsync(request, cancellationToken);
        }
    }
}
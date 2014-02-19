using System;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using Pinnacle.WebApi.HmacAuth;
using WebApi.HmacAuth.Interfaces;

namespace WebApi.HmacAuth
{
    public class CanonicalRepresentationBuilder : IBuildMessageRepresentation
    {
        /// <summary>
        /// Builds message representation as follows:
        /// HTTP METHOD\n +
        /// Content-MD5\n +  
        /// Timestamp\n +
        /// Username\n +
        /// Request URI
        /// </summary>
        /// <returns></returns>
        public string BuildRequestRepresentation(HttpRequestMessage requestMessage)
        {
            var valid = IsRequestValid(requestMessage);

            if (!valid || !requestMessage.Headers.Date.HasValue)
            {
                return null;
            }

            var date = requestMessage.Headers.Date.Value.UtcDateTime;
            var md5 = GetMd5Header(requestMessage);

            var httpMethod = requestMessage.Method.Method;
            var user = requestMessage.Headers.GetValues(Configuration.UserHeader).FirstOrDefault();
            var applicationId = requestMessage.Headers.GetValues(Configuration.ApplicationIdHeader).FirstOrDefault();
            
            if (string.IsNullOrWhiteSpace(applicationId) || string.IsNullOrWhiteSpace(user))
            {
                return null;
            }
            
            var uri = requestMessage.RequestUri.AbsolutePath.ToLower();
            // you may need to add more headers if thats required for security reasons
            var representation = String.Join("\n", httpMethod, md5, date.ToString(CultureInfo.InvariantCulture), applicationId, user, uri);
            
            return representation;
        }

        private string GetMd5Header(HttpRequestMessage requestMessage)
        {
            return requestMessage.Content == null || 
                   requestMessage.Content.Headers.ContentMD5 == null ?  "" 
                       : Convert.ToBase64String(requestMessage.Content.Headers.ContentMD5);
        }

        private bool IsRequestValid(HttpRequestMessage requestMessage)
        {
            //for simplicity I am omitting headers check (all required headers should be present)
            return true;
        }
    }
}
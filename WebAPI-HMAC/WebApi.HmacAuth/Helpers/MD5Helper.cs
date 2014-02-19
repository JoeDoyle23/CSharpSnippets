using System.Net.Http;
using System.Security.Cryptography;
using WebApi.HmacAuth.Interfaces;

namespace WebApi.HmacAuth.Helpers
{
    public class Md5Helper : IHashHelper
    {
        public byte[] ComputeHash(HttpContent httpContent)
        {
            using (var md5 = MD5.Create())
            {
                var content = httpContent.ReadAsByteArrayAsync().Result;
                var hash = md5.ComputeHash(content);
                return hash;
            }
        }
    }
}
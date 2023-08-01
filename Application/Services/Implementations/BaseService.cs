using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using UAParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Implementations
{
    public class BaseService
    {
        private readonly IHttpContextAccessor accessor;
        public BaseService(IHttpContextAccessor accessor)
        {
            this.accessor = accessor;
        }

        public string IpAddress => accessor?.HttpContext?.Connection.RemoteIpAddress.ToString();
        public string Device => accessor.HttpContext != null ? GetDevice(accessor.HttpContext.Request.Headers["User-Agent"]) : null;
        public List<Claim> Claims => accessor.HttpContext.User?.Claims?.ToList();
        public string UserId => Claims.FirstOrDefault(x => x.Type == "UserId")?.Value;
        public string Email => Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;
        public string CompanyId => Claims.FirstOrDefault(x => x.Type == "CompanyId")?.Value;
        //public string EnterpriseName => Claims.FirstOrDefault(x => x.Type == "EnterpriseName")?.Value;
        public string LastName => Claims.FirstOrDefault(x => x.Type == "LastName")?.Value;
        public string FirstName => Claims.FirstOrDefault(x => x.Type == "FirstName")?.Value;

        private static string GetDevice(StringValues userAgent)
        {
            string uaString = Convert.ToString(userAgent[0]);
            var uaParser = Parser.GetDefault();
            ClientInfo c = uaParser.Parse(uaString);
            return c.UA.ToString();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.InfraInterfaces
{
    public interface IHttpConnection
    {
        Task<T> DevAPIRequest<T>(HttpRequestMessage request, HttpContent content, string client) where T : new();
        Task<T> WebRequest<T>(string url, object request, string requestType, Dictionary<string, string> headers = null, string authUserName = null, string authPword = null) where T : new();
    }
}

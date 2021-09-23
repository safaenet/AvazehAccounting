using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace AvazehWpfApiClient.Logics
{
    public class ApiProcessor : IApiProcessor
    {
        public ApiProcessor()
        {
            ApiClient = new();
            ApiClient.BaseAddress = new Uri("http://localhost:44342/");
        }
        public HttpClient ApiClient { get; set; }
    }
}
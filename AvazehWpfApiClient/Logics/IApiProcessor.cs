using System.Net.Http;

namespace AvazehWpfApiClient.Logics
{
    public interface IApiProcessor
    {
        HttpClient ApiClient { get; set; }
    }
}
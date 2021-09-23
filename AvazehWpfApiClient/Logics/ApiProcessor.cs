using AvazehWpfApiClient.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
            ApiClient.BaseAddress = new Uri("http://localhost:44342/api/");
        }
        private HttpClient ApiClient { get; set; }

        public async Task<T> GetCollectionAsync<T>(string Key, int Page = 1, string SearchText = "") where T : class
        {
            var Url = $"{Key}?Page={Page}&SearchText={SearchText}";
            var response = await ApiClient.GetAsync(Url);
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadAsAsync<T>();
            return null;
        }

        public async Task<T> GetItemAsync<T>(string Key, int Id) where T : class
        {
            var Url = $"{Key}/{Id}";
            var response = await ApiClient.GetAsync(Url);
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadAsAsync<T>();
            return null;
        }

        public async Task<bool> CreateItemAsync<T>(string Key, T model)
        {
            var Url = $"{Key}";
            var response = await ApiClient.PostAsJsonAsync(Url, model);
            if (response.IsSuccessStatusCode)
                return true;
            return false;
        }

        public async Task<bool> UpdateItem<T>(string Key, int Id, T model)
        {
            var Url = $"{Key}/{Id}";
            var response = await ApiClient.PutAsJsonAsync(Url, model);
            if (response.IsSuccessStatusCode)
                return true;
            return false;
        }

        public async Task<bool> DeleteItem<T>(string Key, int Id)
        {
            var Url = $"{Key}";
            var response = await ApiClient.DeleteAsync(Url);
            if (response.IsSuccessStatusCode)
                return true;
            return false;
        }
    }
}
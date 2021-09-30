using AvazehApiClient.DataAccess.Interfaces;
using SharedLibrary.Enums;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace AvazehApiClient.DataAccess
{
    public class ApiProcessor : IApiProcessor
    {
        public ApiProcessor()
        {
            ApiClient = new();
            ApiClient.BaseAddress = new Uri(SettingsDataAccess.AppConfiguration().GetSection("BaseUrl").Value);
        }
        private HttpClient ApiClient { get; set; }

        public async Task<T> GetCollectionAsync<T>(string Key, string OrderBy, OrderType orderType, int Page = 1, string SearchText = "", int PageSize = 50, bool ForceLoad = false) where T : class
        {
            var Url = $"{Key}?OrderBy={OrderBy}&OrderType={orderType}&Page={Page}&SearchText={SearchText}&PageSize={PageSize}&ForceLoad={ForceLoad}";
            var response = await ApiClient.GetAsync(Url);
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadAsAsync<T>();
            return null;
        }

        public async Task<T> GetInvoiceCollectionAsync<T>(string Key, string OrderBy, OrderType orderType, int Page = 1, string SearchText = "", InvoiceLifeStatus? LifeStatus = null, InvoiceFinancialStatus? FinStatus = null, int PageSize = 50, bool ForceLoad = false) where T : class
        {
            var Url = $"{Key}?OrderBy={OrderBy}&OrderType={orderType}&Page={Page}&SearchText={SearchText}&LifeStatus={LifeStatus}&FinStatus={FinStatus}&PageSize={PageSize}&ForceLoad={ForceLoad}";
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

        public async Task<U> CreateItemAsync<T, U>(string Key, T model) where U : class
        {
            var Url = $"{Key}";
            var response = await ApiClient.PostAsJsonAsync(Url, model);
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadAsAsync<U>();
            return null;
        }

        public async Task<U> UpdateItemAsync<T, U>(string Key, int Id, T model) where U : class
        {
            var Url = $"{Key}/{Id}";
            var response = await ApiClient.PutAsJsonAsync(Url, model);
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadAsAsync<U>();
            return null;
        }

        public async Task<bool> DeleteItemAsync(string Key, int Id)
        {
            var Url = $"{Key}?Id={Id}";
            var response = await ApiClient.DeleteAsync(Url);
            if (response.IsSuccessStatusCode)
                return true;
            return false;
        }
    }
}
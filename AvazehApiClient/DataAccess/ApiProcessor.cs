using AvazehApiClient.DataAccess.Interfaces;
using SharedLibrary.DalModels;
using SharedLibrary.DtoModels;
using SharedLibrary.Enums;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace AvazehApiClient.DataAccess
{
    public class ApiProcessor : IApiProcessor
    {
        public ApiProcessor()
        {
            ApiClient = new();
            ApiClient.BaseAddress = new Uri(SettingsDataAccess.AppConfiguration().GetSection("BaseUrl").Value);
            ApiClient.DefaultRequestHeaders.Accept.Clear();
            ApiClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
        private HttpClient ApiClient { get; set; }
        private string token;
        public string Token
        {
            get => token;
            set
            {
                token = value;
                ApiClient.DefaultRequestHeaders.Clear();
                ApiClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
            }
        }

        public async Task<T> GetCollectionAsync<T>(string Key, string OrderBy, OrderType orderType, int Page = 1, string SearchText = "", int PageSize = 50, bool ForceLoad = false) where T : class
        {
            var Url = $"{Key}?OrderBy={OrderBy}&OrderType={orderType}&Page={Page}&SearchText={SearchText}&PageSize={PageSize}&ForceLoad={ForceLoad}";
            var response = await ApiClient.GetAsync(Url);
            return response.IsSuccessStatusCode ? await response.Content.ReadAsAsync<T>() : null;
        }

        public async Task<T> GetCollectionAsync<T>(string Key, string SearchText) where T : class //Mostly used for Load Product items in invoice details page.
        {
            var Url = $"{Key}?SearchText={SearchText}";
            var response = await ApiClient.GetAsync(Url);
            return response.IsSuccessStatusCode ? await response.Content.ReadAsAsync<T>() : null;
        }

        public async Task<T> GetCollectionAsync<T>(string Key, int Id) where T : class
        {
            var Url = $"{Key}/{Id}";
            var response = await ApiClient.GetAsync(Url);
            return response.IsSuccessStatusCode ? await response.Content.ReadAsAsync<T>() : null;
        }

        public async Task<T> GetCollectionAsync<T>(string Key, int Id1, int Id2, int Id3) where T : class //Mostly used for Load Product recent prices in invoice details page.
        {
            var Url = $"{Key}/{Id1}/{Id2}/{Id3}";
            var response = await ApiClient.GetAsync(Url);
            return response.IsSuccessStatusCode ? await response.Content.ReadAsAsync<T>() : null;
        }

        public async Task<bool> GetBooleanAsync(string Key)
        {
            var Url = $"{Key}";
            var response = await ApiClient.GetAsync(Url);
            return await response.Content.ReadAsAsync<bool>();
        }

        public async Task<T> GetInvoiceCollectionAsync<T>(string Key, string OrderBy, OrderType orderType, int Page = 1, string SearchText = "", InvoiceLifeStatus? LifeStatus = null, InvoiceFinancialStatus? FinStatus = null, int PageSize = 50, bool ForceLoad = false) where T : class
        {
            var Url = $"{Key}?OrderBy={OrderBy}&OrderType={orderType}&Page={Page}&SearchText={SearchText}&LifeStatus={LifeStatus}&FinStatus={FinStatus}&PageSize={PageSize}&ForceLoad={ForceLoad}";
            var response = await ApiClient.GetAsync(Url);
            return response.IsSuccessStatusCode ? await response.Content.ReadAsAsync<T>() : null;
        }

        public async Task<T> GetTransactionCollectionAsync<T>(string Key, string OrderBy, OrderType orderType, int Page = 1, string SearchText = "", TransactionFinancialStatus? FinStatus = null, int PageSize = 50, bool ForceLoad = false) where T : class
        {
            var Url = $"{Key}?OrderBy={OrderBy}&OrderType={orderType}&Page={Page}&SearchText={SearchText}&FinStatus={FinStatus}&PageSize={PageSize}&ForceLoad={ForceLoad}";
            var response = await ApiClient.GetAsync(Url);
            return response.IsSuccessStatusCode ? await response.Content.ReadAsAsync<T>() : null;
        }

        public async Task<ItemsCollection_DTO<ChequeModel>> GetChequeCollectionAsync(string Key, string OrderBy, OrderType orderType, ChequeListQueryStatus? listQueryStatus = ChequeListQueryStatus.FromNowOn, int Page = 1, string SearchText = "", int PageSize = 50, bool ForceLoad = false)
        {
            var Url = $"{Key}?OrderBy={OrderBy}&OrderType={orderType}&listQueryStatus={listQueryStatus}&Page={Page}&SearchText={SearchText}&PageSize={PageSize}&ForceLoad={ForceLoad}";
            var response = await ApiClient.GetAsync(Url);
            return response.IsSuccessStatusCode ? await response.Content.ReadAsAsync<ItemsCollection_DTO<ChequeModel>>() : null;
        }

        public async Task<T> GetItemAsync<T>(string Key, string Id) where T : class
        {
            var Url = $"{Key}/{Id}";
            var response = await ApiClient.GetAsync(Url);
            return response.IsSuccessStatusCode ? await response.Content.ReadAsAsync<T>() : null;
        }

        public async Task<U> CreateItemAsync<T, U>(string Key, T model) where U : class
        {
            var Url = $"{Key}";
            var response = await ApiClient.PostAsJsonAsync(Url, model);
            return response.IsSuccessStatusCode ? await response.Content.ReadAsAsync<U>() : null;
        }

        public async Task<U> UpdateItemAsync<T, U>(string Key, int Id, T model) where U : class
        {
            var Url = $"{Key}/{Id}";
            var response = await ApiClient.PutAsJsonAsync(Url, model);
            return response.IsSuccessStatusCode ? await response.Content.ReadAsAsync<U>() : null;
        }

        public async Task<bool> DeleteItemAsync(string Key, int Id)
        {
            var Url = $"{Key}?Id={Id}";
            var response = await ApiClient.DeleteAsync(Url);
            return response.IsSuccessStatusCode;
        }

        public async Task<T?> GetValueOrNullAsync<T>(string Key, int Id1, int Id2) where T : struct
        {
            var Url = $"{Key}/{Id1}/{Id2}";
            var response = await ApiClient.GetAsync(Url);
            return response.IsSuccessStatusCode ? await response.Content.ReadAsAsync<T>() : null;
        }

        public async Task<double> GetValueOrZeroAsync<T>(string Key, int Id1, int Id2)
        {
            var Url = $"{Key}/{Id1}/{Id2}";
            var response = await ApiClient.GetAsync(Url);
            return response.IsSuccessStatusCode ? await response.Content.ReadAsAsync<double>() : 0;
        }
    }
}
using AvazehApiClient.DataAccess.Interfaces;
using Serilog;
using SharedLibrary.DalModels;
using SharedLibrary.DtoModels;
using SharedLibrary.Enums;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace AvazehApiClient.DataAccess;

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
            if (!string.IsNullOrEmpty(token))
            {
                ApiClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
            }
        }
    }

    public async Task<bool> TestDBConnectionAsync()
    {
        try
        {
            var Url = $"Auth/TestConnection";
            //ApiClient.Timeout
            try
            {
                var response = await ApiClient.GetAsync(Url);
                if (response.IsSuccessStatusCode && (await response.Content.ReadAsAsync<bool>())) return true;
            }
            catch
            {
                return false;
            }
            return false;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error in ApiProcessor");
        }
        return false;
    }

    public bool IsInRole(string role)
    {
        try
        {
            if (string.IsNullOrEmpty(Token)) return false;
            var handler = new JwtSecurityTokenHandler();
            var jwtSecurityToken = handler.ReadJwtToken(Token);
            var result = jwtSecurityToken.Claims.Where(claim => claim.Type == System.Security.Claims.ClaimTypes.Role && claim.Value == role).Any();
            return result;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error in ApiProcessor");
        }
        return false;
    }

    public string GetRoleValue(string roleType)
    {
        try
        {
            if (string.IsNullOrEmpty(Token)) return null;
            var handler = new JwtSecurityTokenHandler();
            var jwtSecurityToken = handler.ReadJwtToken(Token);
            var result = jwtSecurityToken.Claims.Where(claim => claim.Type == roleType).FirstOrDefault().Value;
            return result;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error in ApiProcessor");
        }
        return null;
    }

    public async Task<T> GetCollectionAsync<T>(string Key, string OrderBy, OrderType orderType, int Page = 1, string SearchText = "", int PageSize = 50, bool ForceLoad = false) where T : class
    {
        try
        {
            var Url = $"{Key}?OrderBy={OrderBy}&OrderType={orderType}&Page={Page}&SearchText={SearchText}&PageSize={PageSize}&ForceLoad={ForceLoad}";
            var response = await ApiClient.GetAsync(Url);
            return response.IsSuccessStatusCode ? await response.Content.ReadAsAsync<T>() : null;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error in ApiProcessor");
        }
        return default;
    }

    public async Task<T> GetCollectionAsync<T>(string Key, string SearchText) where T : class //Mostly used for Load Product items in invoice details page.
    {
        try
        {
            var Url = $"{Key}?SearchText={SearchText}";
            var response = await ApiClient.GetAsync(Url);
            return response.IsSuccessStatusCode ? await response.Content.ReadAsAsync<T>() : null;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error in ApiProcessor");
        }
        return default;
    }

    public async Task<T> GetCollectionAsync<T>(string Key, int Id) where T : class
    {
        try
        {
            var Url = $"{Key}/{Id}";
            var response = await ApiClient.GetAsync(Url);
            return response.IsSuccessStatusCode ? await response.Content.ReadAsAsync<T>() : null;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error in ApiProcessor");
        }
        return default;
    }

    public async Task<T> GetCollectionAsync<T>(string Key, int Id1, int Id2, int Id3) where T : class //Mostly used for Load Product recent prices in invoice details page.
    {
        try
        {
            var Url = $"{Key}/{Id1}/{Id2}/{Id3}";
            var response = await ApiClient.GetAsync(Url);
            return response.IsSuccessStatusCode ? await response.Content.ReadAsAsync<T>() : null;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error in ApiProcessor");
        }
        return default;
    }

    public async Task<bool> GetBooleanAsync(string Key)
    {
        try
        {
            var Url = $"{Key}";
            var response = await ApiClient.GetAsync(Url);
            return await response.Content.ReadAsAsync<bool>();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error in ApiProcessor");
        }
        return false;
    }

    public async Task<T> GetInvoiceCollectionAsync<T>(string Key, int FetcheSize = 50, int InvoiceId = -1, int CustomerId = -1, string InvoiceDate = "%", string SearchValue = "%", InvoiceLifeStatus? LifeStatus = InvoiceLifeStatus.Active, InvoiceFinancialStatus? FinStatus = InvoiceFinancialStatus.Outstanding, SqlQuerySearchMode SearchMode = SqlQuerySearchMode.Backward, OrderType orderType = OrderType.DESC, int StartId = -1) where T : class
    {
        try
        {
            var Url = $"{Key}?FetcheSize={FetcheSize}&InvoiceId={InvoiceId}&CustomerId={CustomerId}&InvoiceDate={InvoiceDate}&SearchValue={SearchValue}&LifeStatus={LifeStatus}&FinStatus={FinStatus}&SearchMode={SearchMode}&orderType={orderType}&StartId={StartId}";
            var response = await ApiClient.GetAsync(Url);
            return response.IsSuccessStatusCode ? await response.Content.ReadAsAsync<T>() : null;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error in ApiProcessor");
        }
        return default;
    }

    public async Task<T> GetTransactionCollectionAsync<T>(string Key, string OrderBy, OrderType orderType, int Page = 1, string SearchText = "", TransactionFinancialStatus? FinStatus = null, int PageSize = 50, bool ForceLoad = false) where T : class
    {
        try
        {
            var Url = $"{Key}?OrderBy={OrderBy}&OrderType={orderType}&Page={Page}&SearchText={SearchText}&FinStatus={FinStatus}&PageSize={PageSize}&ForceLoad={ForceLoad}";
            var response = await ApiClient.GetAsync(Url);
            return response.IsSuccessStatusCode ? await response.Content.ReadAsAsync<T>() : null;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error in ApiProcessor");
        }
        return default;
    }

    public async Task<ItemsCollection_DTO<ChequeModel>> GetChequeCollectionAsync(string Key, string OrderBy, OrderType orderType, ChequeListQueryStatus? listQueryStatus = ChequeListQueryStatus.FromNowOn, int Page = 1, string SearchText = "", int PageSize = 50, bool ForceLoad = false)
    {
        try
        {
            var Url = $"{Key}?OrderBy={OrderBy}&OrderType={orderType}&listQueryStatus={listQueryStatus}&Page={Page}&SearchText={SearchText}&PageSize={PageSize}&ForceLoad={ForceLoad}";
            var response = await ApiClient.GetAsync(Url);
            return response.IsSuccessStatusCode ? await response.Content.ReadAsAsync<ItemsCollection_DTO<ChequeModel>>() : null;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error in ApiProcessor");
        }
        return null;
    }

    public async Task<T> GetItemAsync<T>(string Key, string Id) where T : class
    {
        try
        {
            var Url = $"{Key}/{Id}";
            var response = await ApiClient.GetAsync(Url);
            return response.IsSuccessStatusCode ? await response.Content.ReadAsAsync<T>() : null;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error in ApiProcessor");
        }
        return default;
    }

    public async Task<U> CreateItemAsync<T, U>(string Key, T model) where U : class
    {
        try
        {
            var Url = $"{Key}";
            var response = await ApiClient.PostAsJsonAsync(Url, model);
            return response.IsSuccessStatusCode ? await response.Content.ReadAsAsync<U>() : null;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error in ApiProcessor");
        }
        return default;
    }

    public async Task<U> UpdateItemAsync<T, U>(string Key, int Id, T model) where U : class
    {
        try
        {
            var Url = $"{Key}/{Id}";
            var response = await ApiClient.PutAsJsonAsync(Url, model);
            return response.IsSuccessStatusCode ? await response.Content.ReadAsAsync<U>() : null;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error in ApiProcessor");
        }
        return default;
    }

    public async Task<bool> DeleteItemAsync(string Key, int Id)
    {
        try
        {
            var Url = $"{Key}?Id={Id}";
            var response = await ApiClient.DeleteAsync(Url);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error in ApiProcessor");
        }
        return false;
    }

    public async Task<T?> GetValueOrNullAsync<T>(string Key, params int[] Ids) where T : struct
    {
        try
        {
            if (Ids == null || Ids.Length == 0) return null;
            string ids = "";
            foreach (var id in Ids)
            {
                ids += "/" + id.ToString();
            }
            var Url = $"{Key}{ids}";
            var response = await ApiClient.GetAsync(Url);
            return response.IsSuccessStatusCode ? await response.Content.ReadAsAsync<T>() : null;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error in ApiProcessor");
        }
        return null;
    }

    public async Task<double> GetValueOrZeroAsync<T>(string Key, params int[] Ids)
    {
        try
        {
            if (Ids == null || Ids.Length == 0) return 0;
            string ids = "";
            foreach (var id in Ids)
            {
                ids += "/" + id.ToString();
            }
            var Url = $"{Key}{ids}";
            var response = await ApiClient.GetAsync(Url);
            return response.IsSuccessStatusCode ? await response.Content.ReadAsAsync<double>() : 0;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error in ApiProcessor");
        }
        return 0;
    }
}
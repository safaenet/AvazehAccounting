using AvazehApiClient.DataAccess.Interfaces;
using FluentValidation.Results;
using SharedLibrary.DalModels;
using SharedLibrary.DtoModels;
using SharedLibrary.Enums;
using SharedLibrary.Validators;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace AvazehApiClient.DataAccess.CollectionManagers;

public class ChequeCollectionManagerAsync : IChequeCollectionManagerAsync
{
    public ChequeCollectionManagerAsync(IApiProcessor apiProcessor)
    {
        ApiProcessor = apiProcessor;
    }
    private const string Key = "Cheque";
    public IApiProcessor ApiProcessor { get; init; }

    public ObservableCollection<ChequeModel> Items { get; set; }

    public string SearchValue { get; set; }
    public string QueryOrderBy { get; set; } = "DueDate";
    public OrderType QueryOrderType { get; set; } = OrderType.DESC;

    public int PageSize { get; set; } = 50;
    public int PagesCount { get; private set; }
    public int CurrentPage { get; private set; }
    public int TotalItemCount { get; private set; }
    public ChequeListQueryStatus? ListQueryStatus { get; set; } = ChequeListQueryStatus.FromNowOn;

    public ChequeModel GetItemFromCollectionById(int Id)
    {
        return Items.SingleOrDefault(i => i.Id == Id);
    }

    public async Task<ChequeModel> GetItemById(int Id)
    {
        return await ApiProcessor.GetItemAsync<ChequeModel>(Key, Id.ToString());
    }

    public async Task<ChequeModel> CreateItemAsync(ChequeModel item)
    {
        if (item == null || !ValidateItem(item).IsValid) return null;
        var newItem = item.AsDto();
        return await ApiProcessor.CreateItemAsync<ChequeModel_DTO_Create_Update, ChequeModel>(Key, newItem);
    }

    public async Task<ChequeModel> UpdateItemAsync(ChequeModel item)
    {
        if (item == null || !ValidateItem(item).IsValid) return null;
        var newItem = item.AsDto();
        return await ApiProcessor.UpdateItemAsync<ChequeModel_DTO_Create_Update, ChequeModel>(Key, item.Id, newItem);
    }

    public async Task<bool> DeleteItemAsync(int Id)
    {
        if (await ApiProcessor.DeleteItemAsync(Key, Id))
        {
            var item = GetItemFromCollectionById(Id);
            if (item != null)
                Items.Remove(item);
            return true;
        }
        return false;
    }

    public ValidationResult ValidateItem(ChequeModel item)
    {
        ChequeValidator validator = new();
        var result = validator.Validate(item);
        return result;
    }

    public async Task<List<ChequeModel>> GetCloseCheques()
    {
        var list = await ApiProcessor.GetCollectionAsync<List<ChequeModel>>("Cheque/CloseCheques", 0);
        return list;
    }

    public async Task<int> GotoPageAsync(int PageNumber, bool Refresh = false)
    {
        var collection = await ApiProcessor.GetChequeCollectionAsync(Key, QueryOrderBy, QueryOrderType, ListQueryStatus, PageNumber, SearchValue, PageSize);
        Items = collection?.Content.AsObservable();
        CurrentPage = collection is null ? 0 : collection.CurrentPage;
        PagesCount = collection is null ? 0 : collection.PagesCount;
        return Items == null ? 0 : Items.Count;
    }

    public async Task<int> RefreshPage()
    {
        return await GotoPageAsync(CurrentPage, true);
    }

    public async Task<int> LoadFirstPageAsync()
    {
        var result = await GotoPageAsync(1);
        return result;
    }

    public async Task<int> LoadPreviousPageAsync()
    {
        if (CurrentPage == PagesCount) return 0;
        var result = await GotoPageAsync(CurrentPage + 1);
        return result;
    }

    public async Task<int> LoadNextPageAsync()
    {
        if (CurrentPage == 1) return 0;
        var result = await GotoPageAsync(CurrentPage - 1);
        return result;
    }
}
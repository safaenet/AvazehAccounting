using DataLibraryCore.DataAccess.Interfaces;
using DataLibraryCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLibraryCore.DataAccess.CollectionManagers
{
    public partial class ProductCollectionManager : IProductCollectionManager
    {
        public async Task<ProductModel> GetItemFromCollectionByIdAsync(int Id)
        {
            return await Task.FromResult(Items.SingleOrDefault(i => i.Id == Id));
        }
        public async Task<bool> DeleteItemFromCollectionByIdAsync(int Id)
        {
            return await Task.FromResult(Items.Remove(await GetItemFromCollectionByIdAsync(Id)));
        }

        public async Task<bool> DeleteItemFromDbByIdAsync(int Id)
        {
            if (Processor.DeleteItemById(Id) > 0)
            {
                DeleteItemFromCollectionById(Id);
                return await Task.FromResult(true);
            }
            return await Task.FromResult(false);
        }

        public async Task<int> GotoPageAsync(int PageNumber)
        {
            if (!Initialized)
            {
                TotalQueryCount = Processor.GetTotalQueryCount(WhereClause);
                if (TotalQueryCount == 0) return await Task.FromResult(0);
                Initialized = true;
            }
            PageNumber = PageNumber < 1 ? 1 : PageNumber;
            PageNumber = PageNumber > PagesCount ? PagesCount : PageNumber;
            Items = Processor.LoadManyItems((PageNumber - 1) * PageSize, PageSize, WhereClause);
            CurrentPage = Items == null || Items.Count == 0 ? 0 : PageNumber;
            return await Task.FromResult(Items == null ? 0 : Items.Count);
        }

        public async Task<int> LoadFirstPageAsync()
        {
            var result = GotoPage(1);
            FirstPageLoaded?.Invoke(this, null);
            return await Task.FromResult(result);
        }

        public async Task<int> LoadPreviousPageAsync()
        {
            PageLoadEventArgs eventArgs = new();
            PreviousPageLoading?.Invoke(this, eventArgs);
            if (eventArgs.Cancel) return await Task.FromResult(0);
            var result = GotoPage(CurrentPage - 1);
            PreviousPageLoaded?.Invoke(this, null);
            return await Task.FromResult(result);
        }

        public async Task<int> LoadNextPageAsync()
        {
            PageLoadEventArgs eventArgs = new();
            NextPageLoading?.Invoke(this, eventArgs);
            if (eventArgs.Cancel) return await Task.FromResult(0);
            var result = GotoPage(CurrentPage + 1);
            NextPageLoaded?.Invoke(this, null);
            return await Task.FromResult(result);
        }

        public async Task<int> GenerateWhereClauseAsync(string val, SqlSearchMode mode = SqlSearchMode.OR)
        {
            if (val == SearchValue) return await Task.FromResult(0);
            SearchValue = val;
            WhereClause = Processor.GenerateWhereClause(val, mode);
            return await Task.FromResult(Items == null ? 0 : Items.Count);
        }
    }
}
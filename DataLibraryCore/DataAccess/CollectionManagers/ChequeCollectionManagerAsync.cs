using DataLibraryCore.DataAccess.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLibraryCore.DataAccess.CollectionManagers
{
    public partial class ChequeCollectionManager : IChequeCollectionManager
    {
        public async Task<bool> DeleteItemFromDbByIdAsync(int Id)
        {
            if (await Processor.DeleteItemByIdAsync(Id) > 0)
            {
                DeleteItemFromCollectionById(Id);
                return true;
            }
            return false;
        }

        public async Task<int> GotoPageAsync(int PageNumber)
        {
            if (!Initialized)
            {
                TotalQueryCount = await Processor.GetTotalQueryCountAsync(WhereClause);
                if (TotalQueryCount == 0)
                {
                    Items = null;
                    return 0;
                }
                Initialized = true;
            }
            if (PagesCount == 0) PageNumber = 1;
            else if (PageNumber > PagesCount) PageNumber = PagesCount;
            else if (PageNumber < 1) PageNumber = 1;
            Items = await Processor.LoadManyItemsAsync((PageNumber - 1) * PageSize, PageSize, WhereClause);
            CurrentPage = Items == null || Items.Count == 0 ? 0 : PageNumber;
            return Items == null ? 0 : Items.Count;
        }

        public async Task<int> LoadFirstPageAsync()
        {
            var result = await GotoPageAsync(1);
            FirstPageLoaded?.Invoke(this, null);
            return result;
        }

        public async Task<int> LoadPreviousPageAsync()
        {
            PageLoadEventArgs eventArgs = new();
            PreviousPageLoading?.Invoke(this, eventArgs);
            if (eventArgs.Cancel) return 0;
            var result = await GotoPageAsync(CurrentPage - 1);
            PreviousPageLoaded?.Invoke(this, null);
            return result;
        }

        public async Task<int> LoadNextPageAsync()
        {
            PageLoadEventArgs eventArgs = new();
            NextPageLoading?.Invoke(this, eventArgs);
            if (eventArgs.Cancel) return 0;
            var result = await GotoPageAsync(CurrentPage + 1);
            NextPageLoaded?.Invoke(this, null);
            return result;
        }
    }
}
using DataLibraryCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLibraryCore.DataAccess.Interfaces
{
    public partial interface IProductCollectionManager
    {
        Task<bool> DeleteItemFromCollectionByIdAsync(int Id);
        Task<bool> DeleteItemFromDbByIdAsync(int Id);
        Task<int> GenerateWhereClauseAsync(string val, SqlSearchMode mode = SqlSearchMode.OR);
        Task<ProductModel> GetItemFromCollectionByIdAsync(int Id);
        Task<int> GotoPageAsync(int PageNumber);
        Task<int> LoadFirstPageAsync();
        Task<int> LoadNextPageAsync();
        Task<int> LoadPreviousPageAsync();
    }
}
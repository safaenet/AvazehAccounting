using AvazehWpfApiClient.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvazehWpfApiClient.DataAccess.Interfaces
{
    public partial interface IProductCollectionManager
    {
        Task<bool> DeleteItemFromDbByIdAsync(int Id);
        Task<int> GotoPageAsync(int PageNumber);
        Task<int> LoadFirstPageAsync();
        Task<int> LoadNextPageAsync();
        Task<int> LoadPreviousPageAsync();
    }
}
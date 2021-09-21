using DataLibraryCore.Models;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace DataLibraryCore.DataAccess.Interfaces
{
    public partial interface IProductProcessor
    {
        Task<int> CreateItemAsync(ProductModel product);
        Task<int> DeleteItemByIdAsync(int Id);
        Task<int> GetTotalQueryCountAsync(string WhereClause);
        Task<ObservableCollection<ProductModel>> LoadManyItemsAsync(int OffSet, int FetcheSize, string WhereClause, OrderType Order = OrderType.ASC, string OrderBy = "Id");
        Task<ProductModel> LoadSingleItemAsync(int Id);
        Task<int> UpdateItemAsync(ProductModel product);
    }
}
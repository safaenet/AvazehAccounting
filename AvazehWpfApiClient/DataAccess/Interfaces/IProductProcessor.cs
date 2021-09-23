using AvazehWpfApiClient.Models;
using FluentValidation.Results;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace AvazehWpfApiClient.DataAccess.Interfaces
{
    public partial interface IProductProcessor
    {
        int CreateItem(ProductModel product);
        int DeleteItemById(int Id);
        int GetTotalQueryCount(string WhereClause);
        ObservableCollection<ProductModel> LoadManyItems(int OffSet, int FetcheSize, string WhereClause, OrderType Order = OrderType.ASC, string OrderBy = "Id");
        ProductModel LoadSingleItem(int Id);
        int UpdateItem(ProductModel product);
        string GenerateWhereClause(string val, SqlSearchMode mode = SqlSearchMode.OR);
        ValidationResult ValidateItem(ProductModel product);
    }
}
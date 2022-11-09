using FluentValidation.Results;
using SharedLibrary.Enums;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace DataLibraryCore.DataAccess.Interfaces
{
    public interface IGeneralProcessor<TModel> : IProcessorBase<TModel>
    {
        string GenerateWhereClause(string val, SqlSearchMode mode);
        Task<ObservableCollection<TModel>> LoadManyItemsAsync(int OffSet, int FetcheSize, string WhereClause, string OrderBy, OrderType Order);
    }
}
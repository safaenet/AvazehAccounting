using FluentValidation.Results;
using SharedLibrary.DalModels;
using SharedLibrary.DtoModels;
using SharedLibrary.Enums;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace DataLibraryCore.DataAccess.Interfaces
{
    public interface IChequeProcessorBase<TModel> : IProcessorBase<TModel>
    {
        string GenerateWhereClause(string val, ChequeListQueryStatus? listQueryStatus, SqlSearchMode mode);
        Task<ObservableCollection<TModel>> LoadManyItemsAsync(int OffSet, int FetcheSize, string WhereClause, string OrderBy, OrderType Order);
        Task<List<string>> GetBanknames();
    }
}
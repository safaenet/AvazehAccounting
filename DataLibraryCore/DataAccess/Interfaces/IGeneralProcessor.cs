using SharedLibrary.Enums;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataLibraryCore.DataAccess.Interfaces;

public interface IGeneralProcessor<TModel> : IProcessorBase<TModel>
{
    string GenerateWhereClause(string val, SqlSearchMode mode);
    Task<IEnumerable<TModel>> LoadManyItemsAsync(int OffSet, int FetcheSize, string WhereClause, string OrderBy, OrderType Order);
}
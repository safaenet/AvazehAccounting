using SharedLibrary.DalModels;
using SharedLibrary.Enums;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace DataLibraryCore.DataAccess.Interfaces
{
    public interface ICollectionManager2<TModel, TProcessor> : ICollectionManagerBase<TModel>
    {
        TProcessor Processor { get; init; }
        string QueryOrderBy { get; }
        OrderType QueryOrderType { get; }
        int GenerateWhereClause(string val, string OrderBy, OrderType orderType, bool run = false, SqlSearchMode mode = SqlSearchMode.OR);
    }
}
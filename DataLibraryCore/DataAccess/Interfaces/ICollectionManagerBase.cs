using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataLibraryCore.DataAccess.Interfaces;

public interface ICollectionManagerBase<TModel>
{
    int CurrentPage { get; }
    bool Initialized { get; set; }
    IEnumerable<TModel> Items { get; set; }
    int PagesCount { get; }
    int PageSize { get; set; }
    string SearchValue { get; }
    string WhereClause { get; set; }
    Task<int> GotoPageAsync(int PageNumber);
}
﻿using SharedLibrary.Enums;

namespace DataLibraryCore.DataAccess.Interfaces;

public interface IChequeCollectionManagerBase<TModel> : ICollectionManagerBase<TModel>
{
    IChequeProcessor Processor { get; init; }
    string QueryOrderBy { get; }
    OrderType QueryOrderType { get; }
    ChequeListQueryStatus? ListQueryStatus { get; }
    int GenerateWhereClause(string val, string OrderBy, OrderType orderType, ChequeListQueryStatus? listQueryStatus = ChequeListQueryStatus.FromNowOn, bool run = false, SqlSearchMode mode = SqlSearchMode.OR);
}
namespace SharedLibrary.Enums
{
    public enum SqlSearchMode { AND, OR }
    public enum PaginationQueryMode { Previous, Next, Refresh }
    public enum SearchIdType { Min, Max }
    public enum ChequeEventTypes
    {
        None = 0,
        Holding = 1,
        Sold = 2,
        NonSufficientFund = 3,
        Cashed = 4
    }
    public enum DiscountTypes
    {
        Percent = 0,
        Amount = 1
    }
    public enum InvoiceFinancialStatus
    {
        Balanced = 0,
        Deptor = 1,
        Creditor = 2
    }
    public enum InvoiceLifeStatus
    {
        Active = 0,
        Inactive = 1,
        Archive = 2,
        Deleted = 3
    }

    public enum TransactionFinancialStatus
    {
        Balanced = 0,
        Positive = 1,
        Negative = 2
    }
    public enum OrderType
    {
        ASC,
        DESC
    }
}
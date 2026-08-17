using SharedLibrary.Enums;

namespace SharedLibrary.Contracts;

/// <summary>
/// This model is for viewing Transactions in ListView
/// </summary>
public class TransactionSummaryDTO
{
    public int Id { get; set; }
    public string FileName { get; set; }
    public string DateCreated { get; set; }
    public string TimeCreated { get; set; }
    public string DateUpdated { get; set; }
    public string TimeUpdated { get; set; }
    public string Descriptions { get; set; }
    public decimal TotalPositiveItemsSum { get; set; }
    public decimal TotalNegativeItemsSum { get; set; }
}
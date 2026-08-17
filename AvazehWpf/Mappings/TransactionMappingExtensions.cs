using AvazehWpf.Models;
using SharedLibrary.Contracts;

namespace AvazehWpf.Mappings;

public static class TransactionMappingExtensions
{
    public static TransactionListExtraDetailsModel ToListExtraDetails(this TransactionSummaryDTO source)
    {
        return new TransactionListExtraDetailsModel
        {
            Id = source.Id,
            FileName = source.FileName,

            DateCreated = source.DateCreated,
            TimeCreated = source.TimeCreated,
            DateUpdated = source.DateUpdated,
            TimeUpdated = source.TimeUpdated,

            Descriptions = source.Descriptions,

            TotalPositiveItemsSum = source.TotalPositiveItemsSum,
            TotalNegativeItemsSum = source.TotalNegativeItemsSum
        };
    }
}
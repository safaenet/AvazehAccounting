using SharedLibrary.Contracts;
using AvazehWpf.Models;

namespace AvazehWpf.Mappings;

public static class InvoiceMappingExtensions
{
    public static InvoiceSummaryExtraDetailsModel ToListExtraDetails(this InvoiceSummaryDTO source)
    {
        return new InvoiceSummaryExtraDetailsModel
        {
            Id = source.Id,
            CustomerId = source.CustomerId,
            CustomerFullName = source.CustomerFullName,
            About = source.About,

            DateCreated = source.DateCreated,
            TimeCreated = source.TimeCreated,
            DateUpdated = source.DateUpdated,
            TimeUpdated = source.TimeUpdated,

            TotalInvoiceSum = source.TotalInvoiceSum,
            TotalPayments = source.TotalPayments,

            LifeStatus = source.LifeStatus,

            PrevInvoiceId = source.PrevInvoiceId,
            PrevInvoiceBalance = source.PrevInvoiceBalance,
            FwdInvoiceId = source.FwdInvoiceId
        };
    }
}
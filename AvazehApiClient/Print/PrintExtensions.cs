using SharedLibrary.Models;

namespace AvazehApiClient.Print;

public static class PrintExtensions
{
    public static void AsPrintModel(this InvoiceModel invoice, PrintInvoiceModel piv)
    {
        if (invoice == null) return;
        piv.InvoiceId = invoice.Id;
        piv.Products = new();
        if (invoice.Items != null && invoice.Items.Count > 0)
            foreach (var item in invoice.Items)
            {
                InvoiceItemForPrintModel i = new();
                i.CountString = item.CountString;
                i.DateCreated = item.DateCreated;
                i.DateUpdated = item.DateUpdated;
                i.Delivered = item.Delivered;
                i.Descriptions = item.Descriptions;
                i.Id = item.Id;
                i.ProductName = item.Product.ProductName;
                i.ProductUnitName = item.Unit == null ? "" : item.Unit.UnitName;
                i.SellPrice = item.SellPrice;
                i.TotalPrice = item.TotalSellValue;
                piv.Products.Add(i);
            }
        piv.CustomerId = invoice.Customer.Id;
        piv.CustomerFullName = invoice.Customer.FullName;
        piv.CustomerPhoneNumber = (invoice.Customer.PhoneNumbers == null || invoice.Customer.PhoneNumbers.Count == 0) ? "" : invoice.Customer.PhoneNumbers[0].PhoneNumber;
        piv.InvoiceDateCreated = invoice.DateCreated.ToString();
        piv.CustomerPostAddress = string.IsNullOrEmpty(invoice.Customer.PostAddress) ? "" : invoice.Customer.PostAddress;
        piv.CustomerDescription = string.IsNullOrEmpty(invoice.Customer.Descriptions) ? "" : invoice.Customer.Descriptions;
        piv.InvoiceDescription = string.IsNullOrEmpty(invoice.Descriptions) ? "" : invoice.Descriptions;
        piv.InvoiceFinStatus = invoice.InvoiceFinancialStatus.ToString();
        piv.TotalBalance = invoice.TotalBalance;
        piv.TotalDiscountAmount = invoice.TotalDiscountAmount;
        piv.TotalItemsSellSum = invoice.TotalItemsSellSum;
        piv.TotalInvoiceSum = invoice.TotalInvoiceSum;
        piv.TotalPayments = invoice.TotalPayments;
    }

    public static void AsPrintModel(this TransactionModel transaction, PrintTransactionModel ptm)
    {
        if (transaction == null) return;
        ptm.TransactionId = transaction.Id;
        ptm.FileName = transaction.FileName;
        ptm.Items = new();
        if (transaction.Items != null && transaction.Items.Count > 0)
            foreach (var item in transaction.Items)
            {
                TransactionItemForPrintModel i = new();
                i.Id = item.Id;
                i.Title = item.Title;
                i.CountString = item.CountString;
                i.Amount = item.Amount;
                i.TotalPrice = item.TotalValue;
                i.DateCreated = item.DateCreated;
                i.DateUpdated = item.DateUpdated;
                i.Descriptions = item.Descriptions;
                ptm.Items.Add(i);
            }
        ptm.TransactionDateCreated = transaction.DateCreated.ToString();
        ptm.TransactionDescription = string.IsNullOrEmpty(transaction.Descriptions) ? "" : transaction.Descriptions;
        ptm.TransactionFinStatus = transaction.TransactionFinancialStatus.ToString();
        ptm.TotalPositiveItemsSum = transaction.TotalPositiveItemsSum;
        ptm.TotalNegativeItemsSum = transaction.TotalNegativeItemsSum;
        ptm.TotalBalance = transaction.TotalBalance;
    }
}

using SharedLibrary.SecurityAndSettingsModels;
using System.Collections.Generic;

namespace SharedLibrary.DalModels;

public class PrintInvoiceModel
{
    public int InvoiceId;
    public List<InvoiceItemForPrintModel> Products;
    public int CustomerId;
    public string CustomerFullName;
    public string CustomerPhoneNumber;
    public string InvoiceDateCreated;
    public string CustomerPostAddress;
    public string CustomerDescription = "";
    public string InvoiceDescription = "";
    public string InvoiceFinStatus;
    public decimal TotalBalance;
    public decimal TotalDiscountAmount;
    public decimal TotalItemsSellSum;
    public decimal TotalInvoiceSum;
    public decimal TotalPayments;

    public decimal CustomerPreviousBalance;
    public int InvoiceType;
    public bool PrintInvoiceDescription;
    public bool PrintCustomerDescription;
    public bool PrintUserDescription;
    public bool PrintCustomerPostAddress;
    public bool PrintInvoiceId = true;
    public bool PrintDate = true;
    public bool PrintCustomerPhoneNumber = true;
    public PrintSettingsModel PrintSettings = new();
}
using AvazehApiClient.DataAccess.Interfaces;
using SharedLibrary.DalModels;
using SharedLibrary.DtoModels;
using System.Globalization;
using System.Threading.Tasks;
using System.Linq;
using SharedLibrary.SecurityAndSettingsModels;

namespace AvazehApiClient.DataAccess;

public static class Extensions
{
    public static ProductModel_DTO_Create_Update AsDto(this ProductModel model)
    {
        return new ProductModel_DTO_Create_Update()
        {
            ProductName = model.ProductName,
            BuyPrice = model.BuyPrice,
            SellPrice = model.SellPrice,
            Barcode = model.Barcode,
            CountString = model.CountString,
            Descriptions = model.Descriptions,
            IsActive = model.IsActive,
        };
    }

    public static CustomerModel_DTO_Create_Update AsDto(this CustomerModel model)
    {
        return new CustomerModel_DTO_Create_Update()
        {
            FirstName = model.FirstName,
            LastName = model.LastName,
            CompanyName = model.CompanyName,
            EmailAddress = model.EmailAddress,
            PostAddress = model.PostAddress,
            DateJoined = model.DateJoined,
            PhoneNumbers = model.PhoneNumbers,
            Descriptions = model.Descriptions
        };
    }

    public static ChequeModel_DTO_Create_Update AsDto(this ChequeModel model)
    {
        return new ChequeModel_DTO_Create_Update()
        {
            Drawer = model.Drawer,
            Orderer = model.Orderer,
            PayAmount = model.PayAmount,
            About = model.About,
            IssueDate = model.IssueDate,
            DueDate = model.DueDate,
            BankName = model.BankName,
            Serial = model.Serial,
            Identifier = model.Identifier,
            Descriptions = model.Descriptions,
            Events = model.Events
        };
    }

    public static InvoiceModel_DTO_Create_Update AsDto(this InvoiceModel model)
    {
        return new InvoiceModel_DTO_Create_Update()
        {
            CustomerId = model.Customer.Id,
            DateCreated = model.DateCreated,
            DiscountType = model.DiscountType,
            DiscountValue = model.DiscountValue,
            Descriptions = model.Descriptions,
            LifeStatus = model.LifeStatus
        };
    }

    public static InvoiceListModel AsListModel(this InvoiceModel model)
    {
        return new InvoiceListModel()
        {
            Id = model.Id,
            CustomerId = model.Customer.Id,
            CustomerFullName = model.Customer.FullName,
            DateCreated = model.DateCreated,
            DateUpdated = model.DateUpdated,
            TotalInvoiceSum = model.TotalInvoiceSum,
            TotalPayments = model.TotalPayments,
            LifeStatus = model.LifeStatus
        };
    }

    public static InvoiceItemModel_DTO_Create_Update AsDto(this InvoiceItemModel model)
    {
        return new InvoiceItemModel_DTO_Create_Update()
        {
            InvoiceId = model.InvoiceId,
            ProductId = model.Product.Id,
            BuyPrice = model.BuyPrice,
            SellPrice = model.SellPrice,
            CountString = model.CountString,
            Unit = model.Unit == null || model.Unit.Id == 0 ? null : model.Unit,
            Delivered = model.Delivered,
            Descriptions = model.Descriptions
        };
    }

    public static InvoicePaymentModel_DTO_Create_Update AsDto(this InvoicePaymentModel model)
    {
        return new InvoicePaymentModel_DTO_Create_Update()
        {
            InvoiceId = model.InvoiceId,
            PayAmount = model.PayAmount,
            Descriptions = model.Descriptions
        };
    }

    internal static TransactionItemModel AsDaL(this TransactionItemModel_DTO_Create_Update model)
    {
        return new TransactionItemModel()
        {
            TransactionId = model.TransactionId,
            Title = model.Title,
            Amount = model.Amount,
            CountString = model.CountString,
            Descriptions = model.Descriptions
        };
    }

    internal static TransactionItemModel_DTO_Create_Update AsDto(this TransactionItemModel model)
    {
        return new TransactionItemModel_DTO_Create_Update()
        {
            TransactionId = model.TransactionId,
            Title = model.Title,
            Amount = model.Amount,
            CountString = model.CountString,
            Descriptions = model.Descriptions
        };
    }

    internal static TransactionModel_DTO_Create_Update AsDto(this TransactionModel model)
    {
        return new TransactionModel_DTO_Create_Update()
        {
            FileName = model.FileName,
            DateCreated = model.DateCreated,
            Descriptions = model.Descriptions
        };
    }

    public static void Clone(this ProductModel From, ProductModel To)
    {
        if (From == null) return;
        if (To == null) To = new();
        To.Id = From.Id;
        To.ProductName = From.ProductName;
        To.BuyPrice = From.BuyPrice;
        To.SellPrice = From.SellPrice;
        To.Barcode = From.Barcode;
        To.CountString = From.CountString;
        To.DateCreated = From.DateCreated;
        To.DateUpdated = From.DateUpdated;
        To.Descriptions = From.Descriptions;
        To.IsActive = From.IsActive;
    }

    public static void Clone(this CustomerModel From, CustomerModel To)
    {
        if (From == null) return;
        if (To == null) To = new();
        To.Id = From.Id;
        To.FirstName = From.FirstName;
        To.LastName = From.LastName;
        To.CompanyName = From.CompanyName;
        To.EmailAddress = From.EmailAddress;
        To.PostAddress = From.PostAddress;
        To.DateJoined = From.DateJoined;
        To.Descriptions = From.Descriptions;
        if (From.PhoneNumbers != null)
        {
            To.PhoneNumbers = new();
            foreach (var phone in From.PhoneNumbers)
                To.PhoneNumbers.Add(phone);
        }
    }

    public static void Clone(this ChequeModel From, ChequeModel To)
    {
        if (From == null) return;
        if (To == null) To = new();
        To.Id = From.Id;
        To.Drawer = From.Drawer;
        To.Orderer = From.Orderer;
        To.PayAmount = From.PayAmount;
        To.About = From.About;
        To.IssueDate = From.IssueDate;
        To.DueDate = From.DueDate;
        To.BankName = From.BankName;
        To.Serial = From.Serial;
        To.Identifier = From.Identifier;
        To.Descriptions = From.Descriptions;
        if (From.Events != null)
        {
            To.Events = new();
            foreach (var Event in From.Events)
            {
                To.Events.Add(Event);
            }
        }
    }

    public static void Clone(this InvoiceItemModel From, InvoiceItemModel To)
    {
        if (From == null) return;
        if (To == null) To = new();
        To.Id = From.Id;
        To.InvoiceId = From.InvoiceId;
        To.BuyPrice = From.BuyPrice;
        To.SellPrice = From.SellPrice;
        To.CountString = From.CountString;
        if (From.Unit != null)
        {
            To.Unit = new();
            To.Unit.Id = From.Unit.Id;
            To.Unit.UnitName = From.Unit.UnitName;
        }
        else To.Unit = null;
        To.DateCreated = From.DateCreated;
        To.DateUpdated = From.DateUpdated;
        To.Delivered = From.Delivered;
        To.Descriptions = From.Descriptions;
        From.Product.Clone(To.Product);
    }

    public static void Clone(this InvoicePaymentModel From, InvoicePaymentModel To)
    {
        if (From == null) return;
        if (To == null) To = new();
        To.Id = From.Id;
        To.InvoiceId = From.InvoiceId;
        To.DateCreated = From.DateCreated;
        To.DateUpdated = From.DateUpdated;
        To.PayAmount = From.PayAmount;
        To.Descriptions = From.Descriptions;
    }

    public static void Clone(this TransactionItemModel From, TransactionItemModel To)
    {
        if (From == null) return;
        if (To == null) To = new();
        To.Id = From.Id;
        To.TransactionId = From.TransactionId;
        To.Title = From.Title;
        To.Amount = From.Amount;
        To.CountString = From.CountString;
        To.DateCreated = From.DateCreated;
        To.DateUpdated = From.DateUpdated;
        To.Descriptions = From.Descriptions;
    }

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

    public static async Task<ProductModel> GetItemByBarCodeAsync(this ICollectionManager<ProductModel> manager, string BarCode)
    {
        return await manager.ApiProcessor.GetItemAsync<ProductModel>("Product/BarCode", BarCode);
    }

    public static string GetPersianDate(this PersianCalendar pCal)
    {
        var now = System.DateTime.Now;
        return string.Format("{0:0000}/{1:00}/{2:00}", pCal.GetYear(now), pCal.GetMonth(now), pCal.GetDayOfMonth(now));
    }

    public static UserSettingsModel Clone(this UserSettingsModel From)
    {
        UserSettingsModel To = new();
        To.ColorNewItem = From.ColorNewItem;
        To.ColorSoldItem = From.ColorSoldItem;
        To.ColorNonSufficientFundItem = From.ColorNonSufficientFundItem;
        To.ColorCashedItem = From.ColorCashedItem;
        To.ColorChequeNotification = From.ColorChequeNotification;
        To.ColorUpdatedItem = From.ColorUpdatedItem;
        To.ColorBalancedItem = From.ColorBalancedItem;
        To.ColorDeptorItem = From.ColorDeptorItem;
        To.ColorCreditorItem = From.ColorCreditorItem;
        To.ColorInactiveItem = From.ColorInactiveItem;
        To.ColorArchivedItem = From.ColorArchivedItem;
        To.ColorDeletedItem = From.ColorDeletedItem;
        To.ColorNegativeProfit = From.ColorNegativeProfit;
        To.ColorPositiveItem = From.ColorPositiveItem;
        To.ColorNegativeItem = From.ColorNegativeItem;

        To.DataGridFontSize = From.DataGridFontSize;
        To.ChequeListPageSize = From.ChequeListPageSize;
        To.ChequeListQueryOrderType = From.ChequeListQueryOrderType;
        To.ChequeNotifyDays = From.ChequeNotifyDays;
        To.ChequeNotify = From.ChequeNotify;
        To.InvoicePageSize = From.InvoicePageSize;
        To.InvoiceListQueryOrderType = From.InvoiceListQueryOrderType;
        To.InvoiceDetailQueryOrderType = From.InvoiceDetailQueryOrderType;
        To.TransactionListPageSize = From.TransactionListPageSize;
        To.TransactionDetailPageSize = From.TransactionDetailPageSize;
        To.TransactionListQueryOrderType = From.TransactionListQueryOrderType;
        To.TransactionDetailQueryOrderType = From.TransactionDetailQueryOrderType;
        To.AutoSelectPersianLanguage = From.AutoSelectPersianLanguage;
        To.TransactionShortcut1Id = From.TransactionShortcut1Id;
        To.TransactionShortcut2Id = From.TransactionShortcut2Id;
        To.TransactionShortcut3Id = From.TransactionShortcut3Id;
        To.TransactionShortcut1Name = From.TransactionShortcut1Name;
        To.TransactionShortcut2Name = From.TransactionShortcut2Name;
        To.TransactionShortcut3Name = From.TransactionShortcut3Name;
        To.AskToAddNotExistingProduct = From.AskToAddNotExistingProduct;
        To.SearchWhenTyping = From.SearchWhenTyping;
        To.CustomerListPageSize = From.CustomerListPageSize;
        To.CustomerListQueryOrderType = From.CustomerListQueryOrderType;
        To.ProductListPageSize = From.ProductListPageSize;
        To.ProductListQueryOrderType = From.ProductListQueryOrderType;
        return To;
    }

    public static PrintSettingsModel Clone(this PrintSettingsModel From)
    {
        PrintSettingsModel To = new();
        To.MainHeaderText = From.MainHeaderText;
        To.HeaderDescription1 = From.HeaderDescription1;
        To.HeaderDescription2 = From.HeaderDescription2;
        To.LeftHeaderImage = From.LeftHeaderImage;
        To.RightHeaderImage = From.RightHeaderImage;
        To.FooterTextLeft = From.FooterTextLeft;
        To.FooterTextRight = From.FooterTextRight;
        To.MainHeaderTextFontSizeA5P = From.MainHeaderTextFontSizeA5P;
        To.HeaderDescriptionFontSizeA5P = From.HeaderDescriptionFontSizeA5P;
        To.TypeTextFontSizeA5P = From.TypeTextFontSizeA5P;
        To.MainHeaderTextFontSizeA5L = From.MainHeaderTextFontSizeA5L;
        To.HeaderDescriptionFontSizeA5L = From.HeaderDescriptionFontSizeA5L;
        To.TypeTextFontSizeA5L = From.TypeTextFontSizeA5L;
        To.MainHeaderTextFontSizeA4P = From.MainHeaderTextFontSizeA4P;
        To.HeaderDescriptionFontSizeA4P = From.HeaderDescriptionFontSizeA4P;
        To.TypeTextFontSizeA4P = From.TypeTextFontSizeA4P;
        To.PageHeaderFontSize = From.PageHeaderFontSize;
        To.DetailsFontSize = From.DetailsFontSize;
        To.PageFooterFontSize = From.PageFooterFontSize;
        To.DescriptionFontSize = From.DescriptionFontSize;
        To.DefaultPrintLayout = From.DefaultPrintLayout;
        To.DefaultPaperSize = From.DefaultPaperSize;
        if (From.UserDescriptions != null)
            To.UserDescriptions = From.UserDescriptions.ToList();
        return To;
    }

    public static GeneralSettingsModel Clone(this GeneralSettingsModel From)
    {
        GeneralSettingsModel To = new()
        {
            BarcodeAddItemCount = From.BarcodeAddItemCount,
            CanHaveDuplicateItemsInInvoice = From.CanHaveDuplicateItemsInInvoice,
            CompanyName = From.CompanyName
        };
        if (From.ProductUnits != null) To.ProductUnits = From.ProductUnits.ToList();
        return To;
    }
}
using AvazehApiClient.DataAccess.Interfaces;
using SharedLibrary.DalModels;
using SharedLibrary.DtoModels;
using System.Globalization;
using System.Threading.Tasks;
using System.Linq;

namespace AvazehApiClient.DataAccess
{
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
                Descriptions = model.Descriptions
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
                TimeCreated = model.TimeCreated,
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
                TimeCreated = model.TimeCreated,
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
            To.TimeCreated = From.TimeCreated;
            To.DateUpdated = From.DateUpdated;
            To.TimeUpdated = From.TimeUpdated;
            To.Descriptions = From.Descriptions;
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
            To.TimeCreated = From.TimeCreated;
            To.DateUpdated = From.DateUpdated;
            To.TimeUpdated = From.TimeUpdated;
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
            To.TimeCreated = From.TimeCreated;
            To.DateUpdated = From.DateUpdated;
            To.TimeUpdated = From.TimeUpdated;
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
            To.TimeCreated = From.TimeCreated;
            To.DateUpdated = From.DateUpdated;
            To.TimeUpdated = From.TimeUpdated;
            To.Descriptions = From.Descriptions;
        }

        public static void AsPrintModel(this InvoiceModel invoice, PrintInvoiceModel piv)
        {
            if (invoice == null) return;
            piv.Products = new();
            piv.CustomerDescription = string.IsNullOrEmpty(invoice.Customer.Descriptions) ? "" : invoice.Customer.Descriptions;
            piv.CustomerFullName = invoice.Customer.FullName;
            piv.CustomerId = invoice.Customer.Id;
            piv.InvoiceDateCreated = invoice.DateCreated;
            piv.InvoiceDescription = string.IsNullOrEmpty(invoice.Descriptions) ? "" : invoice.Descriptions;
            piv.InvoiceFinStatus = invoice.InvoiceFinancialStatus.ToString();
            piv.InvoiceId = invoice.Id;
            piv.CustomerPhoneNumber = (invoice.Customer.PhoneNumbers == null || invoice.Customer.PhoneNumbers.Count == 0) ? "" : invoice.Customer.PhoneNumbers[0].ToString();
            piv.TotalBalance = invoice.TotalBalance;
            piv.TotalDiscountAmount = invoice.TotalDiscountAmount;
            piv.TotalItemsSellSum = invoice.TotalItemsSellSum;
            piv.TotalInvoiceSum = invoice.TotalInvoiceSum;
            piv.TotalPayments = invoice.TotalPayments;
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
                    i.TimeCreated= item.TimeCreated;
                    i.TimeUpdated= item.TimeUpdated;
                    i.TotalPrice = item.TotalSellValue;
                    piv.Products.Add(i);
                }
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
    }
}
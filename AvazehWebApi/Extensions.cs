using DataLibraryCore.DataAccess.Interfaces;
using DataLibraryCore.DataAccess.SqlServer;
using SharedLibrary.DalModels;
using SharedLibrary.DtoModels;
using SharedLibrary.Enums;
using SharedLibrary.Validators;
using System.Linq;
using System.Threading.Tasks;

namespace AvazehWeb
{
    internal static class Extensions
    {
        internal static ProductModel AsDaL(this ProductModel_DTO_Create_Update model)
        {
            return new ProductModel()
            {
                ProductName = model.ProductName,
                BuyPrice = model.BuyPrice,
                SellPrice = model.SellPrice,
                Barcode = model.Barcode,
                CountString = model.CountString,
                Descriptions = model.Descriptions,
                IsActive = model.IsActive
            };
        }

        internal static ProductModel_DTO_Create_Update AsDto(this ProductModel model)
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

        internal static ItemsCollection_DTO<TDal> AsDto<TDal>(this IGeneralCollectionManager<TDal, IGeneralProcessor<TDal>> manager)
        {
            return new ItemsCollection_DTO<TDal>()
            {
                Items = manager.Items,
                PagesCount = manager.PagesCount,
                CurrentPage = manager.CurrentPage
            };
        }

        internal static ItemsCollection_DTO<InvoiceListModel> AsDto(this IInvoiceCollectionManager manager)
        {
            return new ItemsCollection_DTO<InvoiceListModel>()
            {
                Items = manager.Items,
                PagesCount = manager.PagesCount,
                CurrentPage = manager.CurrentPage
            };
        }

        internal static ItemsCollection_DTO<TransactionListModel> AsDto(this ITransactionCollectionManager manager)
        {
            return new ItemsCollection_DTO<TransactionListModel>()
            {
                Items = manager.Items,
                PagesCount = manager.PagesCount,
                CurrentPage = manager.CurrentPage
            };
        }

        internal static ItemsCollection_DTO<ChequeModel> AsDto(this IChequeCollectionManager manager)
        {
            return new ItemsCollection_DTO<ChequeModel>()
            {
                Items = manager.Items,
                PagesCount = manager.PagesCount,
                CurrentPage = manager.CurrentPage
            };
        }

        internal static ItemsCollection_DTO<TransactionItemModel> AsDto(this ITransactionItemCollectionManager manager)
        {
            return new ItemsCollection_DTO<TransactionItemModel>()
            {
                Items = manager.Items,
                PagesCount = manager.PagesCount,
                CurrentPage = manager.CurrentPage
            };
        }

        internal static CustomerModel AsDaL(this CustomerModel_DTO_Create_Update model)
        {
            return new CustomerModel()
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

        internal static CustomerModel_DTO_Create_Update AsDto(this CustomerModel model)
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

        internal static ChequeModel AsDaL(this ChequeModel_DTO_Create_Update model)
        {
            return new ChequeModel()
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

        internal static ChequeModel_DTO_Create_Update AsDto(this ChequeModel model)
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

        internal static InvoiceModel AsDaL(this InvoiceModel_DTO_Create_Update model)
        {
            var processor = new SqlCustomerProcessor<CustomerModel, PhoneNumberModel, CustomerValidator>(new SqlDataAccess()); //Should be edited to get instance from DI container
            return new InvoiceModel()
            {
                Customer = processor.LoadSingleItemAsync(model.CustomerId).Result,
                DateCreated = model.DateCreated,
                TimeCreated = model.TimeCreated,
                DiscountType = model.DiscountType,
                DiscountValue = model.DiscountValue,
                Descriptions = model.Descriptions,
                LifeStatus = model.LifeStatus
            };
        }

        internal static InvoiceModel_DTO_Create_Update AsDto(this InvoiceModel model)
        {
            return new InvoiceModel_DTO_Create_Update()
            {
                CustomerId = model.Customer.Id,
                DiscountType = model.DiscountType,
                DiscountValue = model.DiscountValue,
                Descriptions = model.Descriptions,
                LifeStatus = model.LifeStatus
            };
        }

        internal static InvoiceItemModel AsDaL(this InvoiceItemModel_DTO_Create_Update model)
        {
            var processor = new SqlProductProcessor<ProductModel, ProductValidator>(new SqlDataAccess()); //Should be edited to get instance from DI container
            return new InvoiceItemModel()
            {
                InvoiceId = model.InvoiceId,
                Product = processor.LoadSingleItemAsync(model.ProductId).Result,
                BuyPrice = model.BuyPrice,
                SellPrice = model.SellPrice,
                CountString = model.CountString,
                Unit = model.Unit == null || model.Unit.Id == 0 ? null : model.Unit,
                Delivered = model.Delivered,
                Descriptions = model.Descriptions
            };
        }

        internal static InvoiceItemModel_DTO_Create_Update AsDto(this InvoiceItemModel model)
        {
            return new InvoiceItemModel_DTO_Create_Update()
            {
                InvoiceId = model.InvoiceId,
                ProductId = model.Product.Id,
                BuyPrice = model.BuyPrice,
                SellPrice = model.SellPrice,
                CountString = model.CountString,
                Delivered = model.Delivered,
                Descriptions = model.Descriptions
            };
        }

        internal static InvoicePaymentModel AsDaL(this InvoicePaymentModel_DTO_Create_Update model)
        {
            return new InvoicePaymentModel()
            {
                InvoiceId = model.InvoiceId,
                PayAmount = model.PayAmount,
                Descriptions = model.Descriptions
            };
        }

        internal static InvoicePaymentModel_DTO_Create_Update AsDto(this InvoicePaymentModel model)
        {
            return new InvoicePaymentModel_DTO_Create_Update()
            {
                InvoiceId = model.InvoiceId,
                PayAmount = model.PayAmount,
                Descriptions = model.Descriptions
            };
        }

        internal static TransactionModel AsDaL(this TransactionModel_DTO_Create_Update model)
        {
            return new TransactionModel()
            {
                FileName = model.FileName,
                DateCreated = model.DateCreated,
                TimeCreated = model.TimeCreated,
                Descriptions = model.Descriptions
            };
        }

        internal static TransactionModel_DTO_Create_Update AsDto(this TransactionModel model)
        {
            return new TransactionModel_DTO_Create_Update()
            {
                FileName=model.FileName,
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

        public static async Task<ProductModel> LoadSingleItemByBarcodeAsync(this IGeneralProcessor<ProductModel> processor, string Id)
        {
            var outPut = await processor.LoadManyItemsAsync(0, 1, $"[BarCode] LIKE '{ Id }'", "ProductName", OrderType.ASC);
            return outPut.FirstOrDefault();
        }
    }
}
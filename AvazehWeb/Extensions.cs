using AvazehWeb.Models;
using DataLibraryCore.DataAccess.Interfaces;
using DataLibraryCore.DataAccess.SqlServer;
using DataLibraryCore.Models;
using DataLibraryCore.Models.Validators;
using System;
using System.Collections.Generic;
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
                Descriptions = model.Descriptions
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

        internal static ItemsCollection_DTO<TDal> AsDto<TDal>(this ICollectionManager<TDal, IProcessor<TDal>> manager)
        {
            return new ItemsCollection_DTO<TDal>()
            {
                Items = manager.Items,
                PagesCount = manager.PagesCount,
                CurrentPage = manager.CurrentPage
            };
        }

        internal static ItemsCollection_DTO<TDalList> AsDto<TDalList>(this IInvoiceCollectionManager manager)
        {
            return new ItemsCollection_DTO<TDalList>()
            {
                Items = manager.Items as IEnumerable<TDalList>,
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
                Customer = processor.LoadSingleItem(model.CustomerId),
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
    }
}
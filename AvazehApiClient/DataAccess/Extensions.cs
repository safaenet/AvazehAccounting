using AvazehApiClient.Models;
using FluentValidation;
using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                Delivered = model.Delivered,
                Descriptions = model.Descriptions
            };
        }
    }
}
using AvazehWeb.Models;
using DataLibraryCore.DataAccess.Interfaces;
using DataLibraryCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AvazehWeb
{
    internal static class Statics
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

        internal static ProductItemsCollection_DTO AsDto(this IProductCollectionManager manager)
        {
            return new ProductItemsCollection_DTO()
            {
                Items = manager.Items,
                PagesCount = manager.PagesCount,
                CurrentPage = manager.CurrentPage,
                SearchValue = manager.SearchValue
            };
        }

        internal static CustomerItemsCollection_DTO AsDto(this ICustomerCollectionManager manager)
        {
            return new CustomerItemsCollection_DTO()
            {
                Items = manager.Items,
                PagesCount = manager.PagesCount,
                CurrentPage = manager.CurrentPage,
                PageSize = manager.PageSize,
                SearchValue = manager.SearchValue
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
    }
}
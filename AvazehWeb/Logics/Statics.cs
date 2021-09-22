using AvazehWeb.Models;
using DataLibraryCore.DataAccess.Interfaces;
using DataLibraryCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AvazehWeb.Logics
{
    internal static class Statics
    {
        internal static ProductModel AsDaLModel(this ProductModel_DTO_Create_Update model)
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

        internal static ProductItemsCollection_DTO AsDto(this IProductCollectionManager manager)
        {
            return new ProductItemsCollection_DTO()
            {
                Items = manager.Items,
                PagesCount = manager.PagesCount,
                CurrentPage = manager.CurrentPage,
                PageSize = manager.PageSize,
                SearchValue = manager.SearchValue
            };
        }
    }
}
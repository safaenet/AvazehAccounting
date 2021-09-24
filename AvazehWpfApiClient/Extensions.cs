using AvazehWpfApiClient.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvazehWpfApiClient
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
    }
}
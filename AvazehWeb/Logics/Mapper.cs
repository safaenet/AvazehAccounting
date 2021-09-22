using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace AvazehWeb.Logics
{
    public static class Mapper
    {
        public static IEnumerable<Models.ProductModel_Dto> MapProductModel(IEnumerable<DataLibraryCore.Models.ProductModel> src)
        {
            if (src == null) return null;
            List<Models.ProductModel_Dto> outPut = new();
            foreach (var item in src)
            {
                outPut.Add(new()
                {
                    Id = item.Id,
                    ProductName = item.ProductName,
                    BuyPrice = item.BuyPrice,
                    SellPrice = item.SellPrice,
                    Barcode = item.Barcode,
                    CountString = item.CountString,
                    DateCreated = item.DateCreated,
                    TimeCreated = item.TimeCreated,
                    DateUpdated = item.DateUpdated,
                    TimeUpdated = item.TimeUpdated,
                    Descriptions = item.Descriptions
                });
            }
            return outPut;
        }

        public static Models.ProductModel_Dto MapProductModel(DataLibraryCore.Models.ProductModel src)
        {
            if (src == null) return null;
            Models.ProductModel_Dto outPut = new();
            outPut.Id = src.Id;
            outPut.ProductName = src.ProductName;
            outPut.BuyPrice = src.BuyPrice;
            outPut.SellPrice = src.SellPrice;
            outPut.Barcode = src.Barcode;
            outPut.CountString = src.CountString;
            outPut.DateCreated = src.DateCreated;
            outPut.TimeCreated = src.TimeCreated;
            outPut.DateUpdated = src.DateUpdated;
            outPut.TimeUpdated = src.TimeUpdated;
            outPut.Descriptions = src.Descriptions;
            return outPut;
        }

        public static IEnumerable<DataLibraryCore.Models.ProductModel> MapProductModel(List<Models.ProductModel_Dto> src)
        {
            if (src == null) return null;
            ObservableCollection<DataLibraryCore.Models.ProductModel> outPut = new();
            foreach (var item in src)
            {
                outPut.Add(new()
                {
                    Id = item.Id,
                    ProductName = item.ProductName,
                    BuyPrice = item.BuyPrice,
                    SellPrice = item.SellPrice,
                    Barcode = item.Barcode,
                    CountString = item.CountString,
                    DateCreated = item.DateCreated,
                    TimeCreated = item.TimeCreated,
                    DateUpdated = item.DateUpdated,
                    TimeUpdated = item.TimeUpdated,
                    Descriptions = item.Descriptions
                });
            }
            return outPut;
        }

        public static DataLibraryCore.Models.ProductModel MapProductModel(Models.ProductModel_Dto src)
        {
            if (src == null) return null;
            DataLibraryCore.Models.ProductModel outPut = new();
            outPut.Id = src.Id;
            outPut.ProductName = src.ProductName;
            outPut.BuyPrice = src.BuyPrice;
            outPut.SellPrice = src.SellPrice;
            outPut.Barcode = src.Barcode;
            outPut.CountString = src.CountString;
            outPut.DateCreated = src.DateCreated;
            outPut.TimeCreated = src.TimeCreated;
            outPut.DateUpdated = src.DateUpdated;
            outPut.TimeUpdated = src.TimeUpdated;
            outPut.Descriptions = src.Descriptions;
            return outPut;
        }
    }
}
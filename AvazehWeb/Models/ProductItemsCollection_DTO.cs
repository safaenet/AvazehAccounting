using DataLibraryCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AvazehWeb.Models
{
    public class ProductItemsCollection_DTO
    {
        public IEnumerable<ProductModel> Items { get; init; }
        public int PagesCount { get; init; }
        public int CurrentPage { get; init; }
        public string SearchValue { get; init; }
    }
}
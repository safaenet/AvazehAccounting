using AvazehWpfApiClient.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace AvazehWpfApiClient.Models
{
    public class ProductItemsCollection_DTO
    {
        public ObservableCollection<ProductModel> Items { get; init; }
        public int PagesCount { get; init; }
        public int CurrentPage { get; init; }
        public string SearchValue { get; init; }
    }
}
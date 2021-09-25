using AvazehApiClient.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace AvazehApiClient.Models
{
    public class ItemsCollection_DTO<T>
    {
        public ObservableCollection<T> Items { get; init; }
        public int PagesCount { get; init; }
        public int CurrentPage { get; init; }
        public string SearchValue { get; init; }
    }
}
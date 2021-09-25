using DataLibraryCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AvazehWeb.Models
{
    public class ItemsCollection_DTO<T>
    {
        public IEnumerable<T> Items { get; init; }
        public int PagesCount { get; init; }
        public int CurrentPage { get; init; }
    }
}
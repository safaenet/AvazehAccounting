using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.DtoModels
{
    /// <summary>
    /// ProductName Or Id Transaction FileName and Id for Combobox
    /// </summary>
    public class ItemsForComboBox
    {
        public int Id { get; set; }
        public string ItemName { get; set; }
        public bool IsChecked { get; set; }
    }
}
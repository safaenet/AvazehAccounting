using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.DalModels
{
    public class UserDescriptionModel
    {
        public int Id { get; set; }
        public string DescriptionTitle { get; set; }
        public string DescriptionText { get; set; }
    }
}
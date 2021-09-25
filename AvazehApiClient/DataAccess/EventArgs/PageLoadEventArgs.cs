using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvazehApiClient.DataAccess.CollectionManagers
{
    public class PageLoadEventArgs : EventArgs
    {
        public bool Cancel { get; set; }
    }
}

using SharedLibrary.DalModels;
using SharedLibrary.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvazehApiClient.DataAccess.Interfaces
{
    public interface IChequeCollectionManagerAsync : ICollectionManager<ChequeModel>
    {
        ChequeListQueryStatus? ListQueryStatus { get; set; }
    }
}
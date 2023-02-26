using SharedLibrary.DalModels;
using SharedLibrary.Enums;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AvazehApiClient.DataAccess.Interfaces;

public interface IChequeCollectionManagerAsync : ICollectionManager<ChequeModel>
{
    ChequeListQueryStatus? ListQueryStatus { get; set; }
    Task<List<ChequeModel>> GetCloseCheques();
}
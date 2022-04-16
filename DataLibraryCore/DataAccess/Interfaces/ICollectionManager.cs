using SharedLibrary.Enums;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace DataLibraryCore.DataAccess.Interfaces
{
    public interface ICollectionManager<TModel, TProcessor> : ICollectionManagerBase<TModel>
    {
    }
}
using FluentValidation.Results;
using SharedLibrary.Enums;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace AvazehApiClient.DataAccess.Interfaces
{
    public interface ICollectionManager<T> : ICollectionManagerBase<T>
    {
        ObservableCollection<T> Items { get; set; }
        T GetItemFromCollectionById(int Id);
    }
}
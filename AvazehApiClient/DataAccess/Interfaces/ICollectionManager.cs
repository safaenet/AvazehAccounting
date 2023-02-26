using System.Collections.ObjectModel;

namespace AvazehApiClient.DataAccess.Interfaces;

public interface ICollectionManager<T> : ICollectionManagerBase<T>
{
    ObservableCollection<T> Items { get; set; }
    T GetItemFromCollectionById(int Id);
}
using System.Collections.ObjectModel;

namespace SharedLibrary.DtoModels;

public class ItemsCollection_DTO<T>
{
    public ObservableCollection<T> Items { get; init; }
    public int PagesCount { get; init; }
    public int CurrentPage { get; init; }
}
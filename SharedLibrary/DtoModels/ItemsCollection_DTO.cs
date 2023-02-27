using System.Collections.Generic;

namespace SharedLibrary.DtoModels;

public class ItemsCollection_DTO<T>
{
    public List<T> Items { get; init; }
    public int PagesCount { get; init; }
    public int CurrentPage { get; init; }
}
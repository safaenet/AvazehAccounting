using System.Collections.Generic;

namespace SharedLibrary.Contracts;

public class ItemsCollection_DTO<T>
{
    public IEnumerable<T> Items { get; init; }
    public int PagesCount { get; init; }
    public int CurrentPage { get; init; }
}
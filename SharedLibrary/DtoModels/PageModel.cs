using System.Collections.ObjectModel;

namespace SharedLibrary.DtoModels;

public class PageModel<T>
{
    public ObservableCollection<T> Content { get; init; }
    public Pageable Pageable { get; set; }
    public bool Last { get; set; }
    public int TotalPages { get; set; }
    public int Size { get; set; }
    public int Number { get; set; }
    public Sort Sort { get; set; }
    public bool First { get; set; }
    public int NumberOfElements { get; set; }
    public bool Empty { get; set; }
}

public class Pageable
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public Sort Sort { get; set; }
    public int Offset { get; set; }
    public bool Paged { get; set; }
    public bool Unpaged { get; set; }
}

public class Sort
{
    public bool Empty { get; set; }
    public bool Sorted { get; set; }
    public bool Unsorted { get; set; }
}
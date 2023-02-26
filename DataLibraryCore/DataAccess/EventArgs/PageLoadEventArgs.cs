using System;

namespace DataLibraryCore.DataAccess.CollectionManagers;

public class PageLoadEventArgs : EventArgs
{
    public bool Cancel { get; set; }
}
using System;

namespace AvazehApiClient.DataAccess.CollectionManagers;

public class PageLoadEventArgs : EventArgs
{
    public bool Cancel { get; set; }
}
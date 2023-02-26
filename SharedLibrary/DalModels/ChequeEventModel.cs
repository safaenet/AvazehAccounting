using SharedLibrary.Enums;
using System;

namespace SharedLibrary.DalModels;

public class ChequeEventModel
{
    public int ChequeId { get; set; }
    public DateTime EventDate { get; set; }
    public ChequeEventTypes EventType { get; set; }
    public string EventText { get; set; }
    public string EventTypeString
    {
        get => EventType.ToString();
        set { if (Enum.IsDefined(typeof(ChequeEventTypes), value)) EventType = Enum.Parse<ChequeEventTypes>(value); }
    }
    public int EventTypeValue
    {
        get => (int)EventType;
        set { if (Enum.IsDefined(typeof(ChequeEventTypes), value)) EventType = (ChequeEventTypes)value; }
    }
}
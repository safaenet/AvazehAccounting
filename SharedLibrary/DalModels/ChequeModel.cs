using SharedLibrary.Enums;
using System.Collections.ObjectModel;

namespace SharedLibrary.DalModels
{
    public class ChequeModel
    {
        public int Id { get; set; }
        public string Drawer { get; set; }
        public string  Orderer { get; set; }
        public long PayAmount { get; set; }
        public string About { get; set; }
        public string IssueDate { get; set; }
        public string DueDate { get; set; }
        public string BankName { get; set; }
        public string Serial { get; set; }
        public string Identifier { get; set; } //Sayyaad Code
        public string Descriptions { get; set; }
        public ObservableCollection<ChequeEventModel> Events { get; set; }
        public string PayAmountInPersian { get; }
        public ChequeEventModel LastEvent => Events == null || Events.Count == 0 ? null : Events[^1];
        public string LastEventString => Events == null || Events.Count == 0 ? ChequeEventTypes.None.ToString() : LastEvent.EventTypeString;
        public string LastEventText => LastEvent == null ? string.Empty : LastEvent.EventText;
    }
}
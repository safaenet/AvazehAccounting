using Caliburn.Micro;
using SharedLibrary.DtoModels;
using SharedLibrary.SettingsModels.WindowsApplicationSettingsModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using AvazehApiClient.DataAccess;

namespace AvazehWpf.ViewModels
{
    public class SettingsViewModel : ViewAware
    {
        public SettingsViewModel()
        {

        }
        private AppSettingsModel appSettings;
        private ObservableCollection<ItemsForComboBox> transactionItemsForComboBox;
        private ItemsForComboBox selectedTransactionItem1;
        private ItemsForComboBox selectedTransactionItem2;
        private ItemsForComboBox selectedTransactionItem3;

        public AppSettingsModel AppSettings
        {
            get { return appSettings; }
            set { appSettings = value; NotifyOfPropertyChange(() => AppSettings); }
        }
        public ObservableCollection<ItemsForComboBox> TransactionItemsForComboBox { get => transactionItemsForComboBox; set { transactionItemsForComboBox = value; NotifyOfPropertyChange(() => TransactionItemsForComboBox); } }
        public ItemsForComboBox SelectedTransactionItem1
        {
            get => selectedTransactionItem1;
            set { selectedTransactionItem1 = value; AppSettings.GeneralSettings.TransactionShortcut1.TransactionId = selectedTransactionItem1.Id; NotifyOfPropertyChange(() => SelectedTransactionItem1); }
        }
        public ItemsForComboBox SelectedTransactionItem2
        {
            get => selectedTransactionItem2;
            set { selectedTransactionItem2 = value; AppSettings.GeneralSettings.TransactionShortcut2.TransactionId = selectedTransactionItem2.Id; NotifyOfPropertyChange(() => SelectedTransactionItem2); }
        }
        public ItemsForComboBox SelectedTransactionItem3
        {
            get => selectedTransactionItem3;
            set { selectedTransactionItem3 = value; AppSettings.GeneralSettings.TransactionShortcut3.TransactionId = selectedTransactionItem3.Id; NotifyOfPropertyChange(() => SelectedTransactionItem3); }
        }
        public Color NewInvoiceColor { get => AppSettings.InvoiceSettings.NewItemColor.ToColor(); set { AppSettings.InvoiceSettings.NewItemColor = value.ToHex(); NotifyOfPropertyChange(() => NewInvoiceColor); } }
    }
}
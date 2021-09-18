using Caliburn.Micro;
using DataLibraryCore.DataAccess.Interfaces;
using DataLibraryCore.DataAccess.SqlServer;
using DataLibraryCore.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace AvazehWpf.ViewModels
{
    class ChequeListViewModel : Screen
    {
        public ChequeListViewModel(IChequeCollectionManager manager)
        {
            _CCM = manager;
            _SelectedCheque = new ChequeModel();
            CCM.GotoPage(1);
        }

        private IChequeCollectionManager _CCM;
        private ChequeModel _SelectedCheque;

        public ChequeModel SelectedCheque
        {
            get { return _SelectedCheque; }
            set { _SelectedCheque = value; NotifyOfPropertyChange(() => SelectedCheque); }
        }


        public IChequeCollectionManager CCM
        {
            get { return _CCM; }
            set
            {
                _CCM = value;
                NotifyOfPropertyChange(() => CCM);
                NotifyOfPropertyChange(() => Cheques);
            }
        }

        public ObservableCollection<ChequeModel> Cheques
        {
            get { return CCM.Items; }
            set
            {
                CCM.Items = value;
                NotifyOfPropertyChange(() => CCM);
                NotifyOfPropertyChange(() => Cheques);
            }
        }

        public string SearchText { get; set; }

        public void AddNewCheque()
        {
            ChequeModel newCheque = new();
            WindowManager wm = new();
            wm.ShowDialogAsync(new ChequeDetailViewModel(CCM, newCheque));
            if (newCheque != null) Cheques.Add(newCheque);
        }

        public void PreviousPage()
        {
            CCM.LoadPreviousPage();
            NotifyOfPropertyChange(() => Cheques);
        }

        public void NextPage()
        {
            CCM.LoadNextPage();
            NotifyOfPropertyChange(() => Cheques);
        }

        public void Search()
        {
            CCM.GenerateWhereClause(SearchText);
            NotifyOfPropertyChange(() => Cheques);
        }

        public void SearchBoxKeyDownHandler(ActionExecutionContext context)
        {
            if (context.EventArgs is KeyEventArgs keyArgs && keyArgs.Key == Key.Enter)
            {
                Search();
            }
        }

        public void EditCheque()
        {
            WindowManager wm = new();
            wm.ShowDialogAsync(new ChequeDetailViewModel(CCM, SelectedCheque));
            NotifyOfPropertyChange(() => Cheques);
            NotifyOfPropertyChange(() => SelectedCheque);
        }

        public void DeleteCheque()
        {
            if (SelectedCheque == null) return;
            var result = MessageBox.Show("Are you sure ?", $"Delete cheque from {SelectedCheque.Drawer}", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
            if (result == MessageBoxResult.No) return;
            var output = CCM.Processor.DeleteItemByID(SelectedCheque.Id);
            if (output > 0) Cheques.Remove(SelectedCheque);
            else MessageBox.Show($"Cheque with ID: {SelectedCheque.Id} was not found in the Database", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}

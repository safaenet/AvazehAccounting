using Caliburn.Micro;
using DataLibraryCore.DataAccess.SqlServer;
using DataLibraryCore.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DataLibraryCore.DataAccess.Interfaces;

namespace AvazehWpf.ViewModels
{
    public class ChequeDetailViewModel : ViewAware
    {
        public ChequeDetailViewModel(IChequeCollectionManager manager, ChequeModel Cheque)
        {
            Manager = manager;
            if (Cheque != null)
            {
                if (Cheque.Id == 0) Cheque.IssueDate = PersianCalendarModel.GetCurrentPersianDate();
                this.Cheque = Cheque;
                _BackupCheque = new ChequeModel();
                CloneCheque(Cheque, ref _BackupCheque);
            }
        }

        private readonly IChequeCollectionManager Manager;
        private ChequeModel _Cheque;
        private readonly ChequeModel _BackupCheque;
        private bool _CancelAndClose = true;

        public ChequeModel Cheque
        {
            get { return _Cheque; }
            set { _Cheque = value; NotifyOfPropertyChange(() => Cheque); }
        }

        public void AddNewEvent()
        {
            ChequeEventModel newEvent = new();
            if (Cheque.Events == null)
            {
                Cheque.Events = new();
                NotifyOfPropertyChange(() => Cheque);
            }
            newEvent.ChequeId = Cheque.Id;
            Cheque.Events.Add(newEvent);
        }

        public void DeleteEvent()
        {
            if (Cheque != null && Cheque.Events != null && Cheque.Events.Any()) Cheque.Events.RemoveAt(Cheque.Events.Count - 1);
        }

        public void DeleteAndClose()
        {
            if (Cheque == null) return;
            var result = MessageBox.Show("Are you sure ?", $"Delete cheque of {Cheque.Drawer}", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
            if (result == MessageBoxResult.No) return;
            if (Manager.Processor.DeleteItemByID(Cheque.Id) == 0) MessageBox.Show($"Cheque with ID: {Cheque.Id} was not found in the Database", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            _CancelAndClose = false;
            CloseWindow();
        }

        public void CloseWindow()
        {
            (GetView() as Window).Close();
        }

        public void SaveAndNew()
        {
            if (SaveToDatabase() == 0) return;
            _CancelAndClose = false;
            Cheque = new ChequeModel();
            WindowManager wm = new();
            wm.ShowWindowAsync(new ChequeDetailViewModel(Manager, Cheque));
            CloseWindow();
        }

        public void CancelAndClose()
        {
            _CancelAndClose = true;
            CloseWindow();
        }

        public void SaveAndClose()
        {
            if (SaveToDatabase() == 0) return;
            _CancelAndClose = false;
            CloseWindow();
        }

        private int SaveToDatabase()
        {
            if (String.IsNullOrEmpty(Cheque.IssueDate)) Cheque.IssueDate = PersianCalendarModel.GetCurrentPersianDate();
            var validate = Manager.Processor.ValidateItem(Cheque);
            if (validate.IsValid)
            {
                if (Cheque == null)
                {
                    var result = MessageBox.Show("Cheque is not assigned, Nothing will be saved; Close anyway ?", "Close", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result == MessageBoxResult.Yes) CloseWindow();
                }
                int outPut;
                if (Cheque.Id == 0) //It's a new Cheque
                {
                    outPut = Manager.Processor.CreateItem(Cheque);
                    if (outPut == 0) MessageBox.Show($"There was a problem when saving to Database", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else //Update Cheque
                {

                    outPut = Manager.Processor.UpdateItem(Cheque);
                    if (outPut == 0) MessageBox.Show($"There was a problem when updating the Database", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                return outPut;
            }
            else
            {
                string str = "";
                foreach (var error in validate.Errors)
                {
                    str += error.ErrorMessage + "\n";
                }
                MessageBox.Show(str);
                return 0;
            }
        }

        private static void CloneCheque(ChequeModel From, ref ChequeModel To)
        {
            if (From == null || To == null) return;
            To.Id = From.Id;
            To.Drawer = From.Drawer;
            To.Orderer = From.Orderer;
            To.PayAmount = From.PayAmount;
            To.About = From.About;
            To.IssueDate = From.IssueDate;
            To.DueDate = From.DueDate;
            To.BankName = From.BankName;
            To.Serial = From.Serial;
            To.Identifier = From.Identifier;
            To.Descriptions = From.Descriptions;
            if (From.Events != null)
                To.Events = new(From.Events);
        }

        public void ClosingWindow()
        {
            if (_CancelAndClose)
            {
                CloneCheque(_BackupCheque, ref _Cheque);
            }
        }

        public void ShowSel(object sender, EventArgs e)
        {
            //MessageBox.Show(sender.ToString());
        }
    }
}
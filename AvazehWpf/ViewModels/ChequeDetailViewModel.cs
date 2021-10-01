using AvazehApiClient.DataAccess;
using AvazehApiClient.DataAccess.Interfaces;
using Caliburn.Micro;
using SharedLibrary.DalModels;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace AvazehWpf.ViewModels
{
    public class ChequeDetailViewModel : ViewAware
    {
        public ChequeDetailViewModel(ICollectionManager<ChequeModel> manager, ChequeModel Cheque, Func<Task> callBack)
        {
            Manager = manager;
            CallBackFunc = callBack;
            if (Cheque is not null)
            {
                BackupCheque = new();
                this.Cheque = Cheque;
                Cheque.Clone(BackupCheque);
            }
        }

        private readonly ICollectionManager<ChequeModel> Manager;
        private ChequeModel _Cheque;
        private ChequeModel _BackupCheque;
        private Func<Task> CallBackFunc;

        public ChequeModel Cheque
        {
            get => _Cheque;
            set { _Cheque = value; NotifyOfPropertyChange(() => Cheque); }
        }

        public ChequeModel BackupCheque
        {
            get => _BackupCheque;
            set
            {
                _BackupCheque = value;
                NotifyOfPropertyChange(() => BackupCheque);
            }
        }

        public void AddNewEvent()
        {
            ChequeEventModel newEvent = new();
            if (BackupCheque.Events == null)
            {
                BackupCheque.Events = new();
                NotifyOfPropertyChange(() => BackupCheque);
            }
            newEvent.ChequeId = BackupCheque.Id;
            BackupCheque.Events.Add(newEvent);
        }

        public void DeleteEvent()
        {
            if (BackupCheque == null || BackupCheque.Events == null || !BackupCheque.Events.Any()) return;
            BackupCheque.Events.RemoveAt(BackupCheque.Events.Count - 1);
        }

        public async Task DeleteAndClose()
        {
            if (Cheque == null) return;
            var result = MessageBox.Show("Are you sure ?", $"Delete cheque of {Cheque.Drawer}", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
            if (result == MessageBoxResult.No) return;
            if (await Manager.DeleteItemAsync(Cheque.Id) == false) MessageBox.Show($"Cheque with ID: {Cheque.Id} was not found in the Database", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            CloseWindow();
        }

        public void CloseWindow()
        {
            (GetView() as Window).Close();
        }

        public async Task SaveAndNew()
        {
            if (await SaveToDatabase() == false) return;
            var newCheque = new ChequeModel();
            WindowManager wm = new();
            await wm.ShowWindowAsync(new ChequeDetailViewModel(Manager, newCheque, CallBackFunc));
            CloseWindow();
        }

        public void CancelAndClose()
        {
            CloseWindow();
        }

        public async Task SaveAndClose()
        {
            if (await SaveToDatabase() == false) return;
            CloseWindow();
        }

        private async Task<bool> SaveToDatabase()
        {
            if (BackupCheque == null) return false;
            var validate = Manager.ValidateItem(BackupCheque);
            if (validate.IsValid)
            {
                ChequeModel outPut;
                if (Cheque.Id == 0) //It's a new Cheque
                    outPut = await Manager.CreateItemAsync(BackupCheque);
                else //Update Cheque
                    outPut = await Manager.UpdateItemAsync(BackupCheque);
                if (outPut is null)
                {
                    MessageBox.Show($"There was a problem when saving to Database", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
                outPut.Clone(BackupCheque);
                BackupCheque.Clone(Cheque);
                return true;
            }
            else
            {
                var str = "";
                foreach (var error in validate.Errors)
                {
                    str += error.ErrorMessage + "\n";
                }
                MessageBox.Show(str);
                return false;
            }
        }

        public void ClosingWindow()
        {
            Cheque.Clone(BackupCheque);
            CallBackFunc();
        }
    }
}
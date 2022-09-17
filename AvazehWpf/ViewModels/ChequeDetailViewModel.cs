using AvazehApiClient.DataAccess;
using AvazehApiClient.DataAccess.Interfaces;
using AvazehUserControlLibraryWpf;
using Caliburn.Micro;
using Microsoft.VisualStudio.PlatformUI;
using SharedLibrary.DalModels;
using SharedLibrary.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace AvazehWpf.ViewModels
{
    public class ChequeDetailViewModel : ViewAware
    {
        public ChequeDetailViewModel(ICollectionManager<ChequeModel> manager, ChequeModel cheque, Func<Task> callBack)
        {
            Manager = manager;
            CallBackFunc = callBack;
            if (cheque is not null)
            {
                Cheque = cheque;
                WindowTitle = "چک " + cheque.Orderer + " به " + cheque.Drawer;
            }
            else
            {
                Cheque = new();
                Cheque.IssueDate = pCal.GetPersianDate();
                Cheque.DueDate = pCal.GetPersianDate();
                WindowTitle = "چک جدید";
            }
        }

        private readonly ICollectionManager<ChequeModel> Manager;
        private ChequeModel _Cheque;
        private Func<Task> CallBackFunc;
        private string windowTitle;
        PersianCalendar pCal = new();

        public string WindowTitle
        {
            get { return windowTitle; }
            set { windowTitle = value; NotifyOfPropertyChange(() => WindowTitle); }
        }

        public ChequeModel Cheque
        {
            get => _Cheque;
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
            newEvent.EventDate = pCal.GetPersianDate();
            Cheque.Events.Add(newEvent);
        }

        public void DeleteEvent()
        {
            if (Cheque == null || Cheque.Events == null || !Cheque.Events.Any()) return;
            Cheque.Events.RemoveAt(Cheque.Events.Count - 1);
        }

        public async Task DeleteAndClose()
        {
            if (Cheque == null || Cheque.Id == 0) return;
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
            if (Cheque == null) return false;
            var validate = Manager.ValidateItem(Cheque);
            if (validate.IsValid)
            {
                ChequeModel outPut;
                if (Cheque.Id == 0) //It's a new Cheque
                    outPut = await Manager.CreateItemAsync(Cheque);
                else //Update Cheque
                    outPut = await Manager.UpdateItemAsync(Cheque);
                if (outPut is null)
                {
                    MessageBox.Show($"There was a problem when saving to Database", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
                outPut.Clone(Cheque);
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

        public async Task ClosingWindow()
        {
            await CallBackFunc?.Invoke();
        }

        public void Window_PreviewKeyDown(object window, KeyEventArgs e)
        {
            if (e.Key == Key.Escape) (GetView() as Window).Close();
        }
    }

    public static class EventTypeItems //For ComboBoxes
    {
        public static Dictionary<int, string> GetPersianEventTypeItems()
        {
            Dictionary<int, string> choices = new();
            for (int i = 0; i < Enum.GetNames(typeof(ChequeEventTypes)).Length; i++)
            {
                if (Enum.GetName(typeof(ChequeEventTypes), i) == ChequeEventTypes.None.ToString())
                    choices.Add((int)ChequeEventTypes.None, "هیچ");
                else if (Enum.GetName(typeof(ChequeEventTypes), i) == ChequeEventTypes.Holding.ToString())
                    choices.Add((int)ChequeEventTypes.Holding, "عادی");
                else if (Enum.GetName(typeof(ChequeEventTypes), i) == ChequeEventTypes.Sold.ToString())
                    choices.Add((int)ChequeEventTypes.Sold, "منتقل شده");
                else if (Enum.GetName(typeof(ChequeEventTypes), i) == ChequeEventTypes.NonSufficientFund.ToString())
                    choices.Add((int)ChequeEventTypes.NonSufficientFund, "برگشت خورده");
                else if (Enum.GetName(typeof(ChequeEventTypes), i) == ChequeEventTypes.Cashed.ToString())
                    choices.Add((int)ChequeEventTypes.Cashed, "وصول شده");
            }
            return choices;
        }
    }
}
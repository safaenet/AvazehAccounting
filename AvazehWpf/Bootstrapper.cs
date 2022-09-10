using AvazehApiClient.DataAccess;
using AvazehApiClient.DataAccess.CollectionManagers;
using AvazehApiClient.DataAccess.Interfaces;
using AvazehWpf.ViewModels;
using Caliburn.Micro;
using SharedLibrary.DalModels;
using SharedLibrary.DtoModels;
using SharedLibrary.Validators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace AvazehWpf
{
    public class Bootstrapper : BootstrapperBase
    {
        public SimpleContainer Container = new();
        protected override object GetInstance(Type serviceType, string key)
        {
            return Container.GetInstance(serviceType, key);
        }

        protected override void Configure()
        {
            Container.Instance(Container)
                .Singleton<IWindowManager, WindowManager>()
                .Singleton<IEventAggregator, EventAggregator>();
            GetType().Assembly.GetTypes()
                .Where(type => type.IsClass)
                .Where(type => type.Name.EndsWith("ViewModel"))
                .ToList()
                .ForEach(ViewModelType => Container.RegisterPerRequest(
                    ViewModelType, ViewModelType.ToString(), ViewModelType));

            Container
                .Singleton<IApiProcessor, ApiProcessor>();
            Container
                .PerRequest<ICollectionManager<ProductModel>, ProductCollectionManagerAsync<ProductModel, ProductModel_DTO_Create_Update, ProductValidator>>()
                .PerRequest<ICollectionManager<CustomerModel>, CustomerCollectionManagerAsync<CustomerModel, CustomerModel_DTO_Create_Update, CustomerValidator>>()
                .PerRequest<ICollectionManager<ChequeModel>, ChequeCollectionManagerAsync<ChequeModel, ChequeModel_DTO_Create_Update, ChequeValidator>>()
                .PerRequest<IInvoiceCollectionManager, InvoiceCollectionManagerAsync>()
                .PerRequest<ITransactionCollectionManager, TransactionCollectionManagerAsync>()
                .PerRequest<IInvoiceDetailManager, InvoiceDetailManager>()
                .PerRequest<ITransactionDetailManager, TransactionDetailManager>()
                .PerRequest<IAppSettingsManager, AppSettingsManager>()
                .Singleton<SingletonClass>();
        }

        protected override IEnumerable<object> GetAllInstances(Type serviceType)
        {
            return Container.GetAllInstances(serviceType);
        }

        protected override void BuildUp(object instance)
        {
            Container.BuildUp(instance);
        }
        public Bootstrapper()
        {
            Initialize();
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            DisplayRootViewFor<MainWindowViewModel>();
        }
        
    }
}
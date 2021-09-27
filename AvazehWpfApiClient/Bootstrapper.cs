using AvazehApiClient.DataAccess;
using AvazehApiClient.DataAccess.Interfaces;
using AvazehWpf.ViewModels;
using Caliburn.Micro;
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
            Container.Instance(Container);
            Container
                .Singleton<IWindowManager, WindowManager>()
                .Singleton<IEventAggregator, EventAggregator>()
                .Singleton<IApiProcessor, ApiProcessor>();

            GetType().Assembly.GetTypes()
                .Where(type => type.IsClass)
                .Where(type => type.Name.EndsWith("ViewModel"))
                .ToList()
                .ForEach(ViewModelType => Container.RegisterPerRequest(
                    ViewModelType, ViewModelType.ToString(), ViewModelType));
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
            DisplayRootViewFor<ProductListViewModel>();
        }
    }
}
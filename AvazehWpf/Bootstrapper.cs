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
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Controls.Primitives;
using AvazehApiClient.DataAccess.Interfaces;
using SharedLibrary.DalModels;
using SharedLibrary.DtoModels;
using System.Threading.Tasks;
using AvazehApiClient.DataAccess;
using AvazehApiClient.DataAccess.CollectionManagers;
using SharedLibrary.Validators;
using System.Windows.Input;
using System.Windows.Data;
using System.Collections.ObjectModel;
using System.Windows.Threading;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Xml;
using System.IO;
using System.Diagnostics;
using SharedLibrary.SettingsModels.WindowsApplicationSettingsModels;
using SharedLibrary.SettingsModels;
using System.Globalization;

namespace AvazehWpf.ViewModels
{
    public class MainWindowViewModel : ViewAware
    {
        public MainWindowViewModel(IAppSettingsManager settingsManager)
        {
            ASM = settingsManager;
            LoadSettings().ConfigureAwait(true);
        }

        private readonly IAppSettingsManager ASM;
        private GeneralSettingsModel generalSettings;
        private bool settingsLoaded;

        public bool SettingsLoaded
        {
            get { return settingsLoaded; }
            set { settingsLoaded = value; NotifyOfPropertyChange(() => SettingsLoaded); }
        }


        public GeneralSettingsModel GeneralSettings { get => generalSettings; private set { generalSettings = value; NotifyOfPropertyChange(() => GeneralSettings); } }

        private async Task LoadSettings()
        {
            var Settings = await ASM.LoadAllAppSettings();
            if (Settings == null) Settings = new();
            GeneralSettings = Settings.GeneralSettings;
            SettingsLoaded = true;
        }
    }
}
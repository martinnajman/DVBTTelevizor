﻿using Xamarin.Forms;
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using LoggerService;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System.Threading;
using Newtonsoft.Json;

namespace DVBTTelevizor
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        protected ILoggingService _loggingService;
        protected IDialogService _dialogService;
        protected DVBTDriverManager _driver;
        protected DVBTTelevizorConfiguration _config;

        private string _status;

        bool isBusy = false;

        #region INotifyPropertyChanged

        protected bool SetProperty<T>(ref T backingStore, T value,
           [CallerMemberName]string propertyName = "",
           Action onChanged = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            onChanged?.Invoke();
            OnPropertyChanged(propertyName);
            return true;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var changed = PropertyChanged;
            if (changed == null)
                return;

            changed.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        public BaseViewModel(ILoggingService loggingService, IDialogService dialogService, DVBTDriverManager driver, DVBTTelevizorConfiguration config)
        {
            _loggingService = loggingService;
            _dialogService = dialogService;
            _driver = driver;
            _config = config;

            MessagingCenter.Subscribe<string>(this, "DVBTDriverConfiguration", (message) =>
            {
                if (!_driver.Started)
                {
                    _driver.Configuration = JsonConvert.DeserializeObject<DVBTDriverConfiguration>(message);
                    Status = $"Initialized ({_driver.Configuration.DeviceName})";
                    _driver.Start();
                }
            });

            MessagingCenter.Subscribe<string>(this, "DVBTDriverConfigurationFailed", (message) =>
            {
                Status = $"Initialization failed ({message})";
            });
        }

        public bool IsBusy
        {
            get { return isBusy; }
            set
            {
                //SetProperty(ref isBusy, value);
                //isBusy = value;
                OnPropertyChanged(nameof(IsBusy));
            }
        }

        public string Status
        {
            get { return _status; }
            set
            {
                _status = value;
                OnPropertyChanged(nameof(Status));
            }
        }

        public static bool ChannelExists(ObservableCollection<DVBTChannel> channels, long frequency, string name, long ProgramMapPID)
        {
            foreach (var ch in channels)
            {
                if (ch.Frequency == frequency &&
                    ch.Name == name &&
                    ch.ProgramMapPID == ProgramMapPID)
                {
                    return true;
                }
            }

            return false;
        }

        public async Task RunWithStoragePermission(Func<Task> action)
        {
            try
            {
                var status = await CrossPermissions.Current.CheckPermissionStatusAsync<StoragePermission>();

                if (status != PermissionStatus.Granted)
                {
                    if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Permission.Storage))
                    {
                        await _dialogService.Information("Application requires storage permission", "Information");
                    }

                    status = await CrossPermissions.Current.RequestPermissionAsync<StoragePermission>();
                }

                if (status == PermissionStatus.Granted)
                {
                    await action();
                }
                else
                {
                    await _dialogService.Error("Storage not granted", "Error");
                }
            }
            catch (Exception ex)
            {
                _loggingService.Error(ex);
            }
        }
    }
}
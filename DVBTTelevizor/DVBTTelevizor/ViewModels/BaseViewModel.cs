﻿using Xamarin.Forms;
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using LoggerService;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Threading;
using Newtonsoft.Json;
using DVBTTelevizor.Models;
using System.IO;

namespace DVBTTelevizor
{
    public class BaseViewModel : ConfigViewModel
    {
        protected ILoggingService _loggingService;
        protected IDialogService _dialogService;
        protected IDVBTDriverManager _driver;
        protected bool _isRefreshing = false;

        public const string MSG_DVBTDriverConfiguration = "DVBTDriverConfiguration";
        public const string MSG_DVBTDriverConfigurationFailed = "DVBTDriverConfigurationFailed";
        public const string MSG_EnableFullScreen = "EnableFullScreen";
        public const string MSG_DisableFullScreen = "DisableFullScreen";
        public const string MSG_PlayStream = "PlayStream";
        public const string MSG_StopStream = "StopStream";
        public const string MSG_UpdateDriverState = "UpdateDriverState";
        public const string MSG_Init = "Init";
        public const string MSG_KeyDown = "KeyDown";
        public const string MSG_ToastMessage = "ShowToastMessage";
        public const string MSG_LongToastMessage = "ShowLongToastMessage";
        public const string MSG_EditChannel = "EditChannel";
        public const string MSG_CheckBatterySettings = "CheckBatterySettings";
        public const string MSG_RequestBatterySettings = "RequestBatterySettings";
        public const string MSG_SetBatterySettings = "SetBatterySettings ";
        public const string MSG_PlayNextChannel = "PlayNextChannel";
        public const string MSG_PlayPreviousChannel = "PlayPreviousChannel";
        public const string MSG_ShareFile = "ShareFile";

        public const string MSG_PlayInBackgroundNotification = "PlayInBackgroundNotification";
        public const string MSG_StopPlayInBackgroundNotification = "StopPlayInBackgroundNotification";

        public const string MSG_ImportChannelsList = "ImportChannelsList";

        public BaseViewModel(ILoggingService loggingService, IDialogService dialogService, IDVBTDriverManager driver, DVBTTelevizorConfiguration config)
              : base(config)
        {
            _loggingService = loggingService;
            _dialogService = dialogService;
            _driver = driver;

            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;

                do
                {
                    Xamarin.Forms.Device.BeginInvokeOnMainThread(
                        new Action(
                            delegate
                            {
                                OnPropertyChanged(nameof(DataStreamInfo));
                            }));

                    // 2 secs delay
                    Thread.Sleep(2 * 1000);

                } while (true);
            }).Start();
        }

        // cannot run async!
        public void ConnectDriver(string message)
        {
            _driver.Configuration = JsonConvert.DeserializeObject<DVBTDriverConfiguration>(message);
            _driver.Start();

            MessagingCenter.Send($"Device {_driver.Configuration.DeviceName} connected", BaseViewModel.MSG_ToastMessage);

            MessagingCenter.Send("", BaseViewModel.MSG_UpdateDriverState);
        }

        public async Task DisconnectDriver()
        {
            await _driver.Disconnect();

            MessagingCenter.Send($"Device {_driver.Configuration.DeviceName} disconnected", BaseViewModel.MSG_ToastMessage);

            UpdateDriverState();
        }

        public void UpdateDriverState()
        {
            OnPropertyChanged(nameof(DriverConnected));
            OnPropertyChanged(nameof(DriverDisConnected));
            OnPropertyChanged(nameof(DriverConnectedIcon));
        }

        public string DataStreamInfo
        {
            get
            {
                return _driver.DataStreamInfo;
            }
        }

        public bool DriverConnected
        {
            get
            {
                return _driver.Started;
            }
        }

        public string DriverConnectedIcon
        {
            get
            {
                if (DriverConnected)
                    return "Connected.png";

                return "Disconnected.png";
            }
        }

        public bool DriverDisConnected
        {
            get
            {
                return !DriverConnected;
            }
        }

        public bool IsRefreshing
        {
            get { return _isRefreshing; }
            set
            {
                OnPropertyChanged(nameof(IsRefreshing));
            }
        }

        public static string AndroidAppDirectory
        {
            get
            {
                return GetAndroidDirectory(null);
            }
        }

        private static string GetAndroidDirectory(string specialFolder)
        {
            try
            {
                if (specialFolder == null)
                {
                    // internal storage - always writable directory
                    try
                    {
                        var pathToExternalMediaDirs = Android.App.Application.Context.GetExternalMediaDirs();

                        if (pathToExternalMediaDirs.Length == 0)
                            throw new DirectoryNotFoundException("No external media directory found");

                        return pathToExternalMediaDirs[0].AbsolutePath;
                    }
                    catch
                    {
                        // fallback for older API:

                        var internalStorageDir = Android.App.Application.Context.GetExternalFilesDir(Environment.SpecialFolder.MyDocuments.ToString());

                        return internalStorageDir.AbsolutePath;
                    }
                }
                else
                {
                    // external storage

                    // Android 11 has no access to external files -> using internal storage
                    if (Android.OS.Build.VERSION.SdkInt > Android.OS.BuildVersionCodes.P)
                    {
                        var internalStorageDir = Android.App.Application.Context.GetExternalFilesDir(Environment.SpecialFolder.MyDocuments.ToString());
                        return internalStorageDir.AbsolutePath;
                    }
                    else
                    {

                        var externalFolderPath = Android.OS.Environment.GetExternalStoragePublicDirectory(specialFolder);
                        return externalFolderPath.AbsolutePath;
                    }
                }

            } catch
            {
                var dir = Android.App.Application.Context.GetExternalFilesDir("");

                return dir.AbsolutePath;
            }
        }
    }
}

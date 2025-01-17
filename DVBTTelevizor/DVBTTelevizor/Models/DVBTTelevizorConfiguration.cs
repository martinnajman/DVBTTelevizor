﻿using Android.Content;
using Android.Preferences;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using Xamarin.Forms;

namespace DVBTTelevizor
{
    public class DVBTTelevizorConfiguration : CustomSharedPreferencesObject
    {
        public ObservableCollection<DVBTChannel> Channels
        {
            get
            {
                var val = GetPersistingSettingValue<string>("ChannelsJson");
                if (!string.IsNullOrEmpty(val))
                {
                    return JsonConvert.DeserializeObject<ObservableCollection<DVBTChannel>>(val);
                }
                return null;
            }
            set
            {
                SavePersistingSettingValue<string>("ChannelsJson", JsonConvert.SerializeObject(value));
            }
        }

        public bool AutoInitAfterStart
        {
            get
            {
               return !DoNotAutoInitAfterStart;
            }
            set
            {
                DoNotAutoInitAfterStart = value;
            }
        }


        public bool DoNotAutoInitAfterStart
        {
            get
            {
                return GetPersistingSettingValue<bool>("DoNotAutoInitAfterStart");
            }
            set
            {
                SavePersistingSettingValue<bool>("DoNotAutoInitAfterStart", value);
            }
        }


        public bool ShowTVChannels
        {
            get
            {
                return !HideTVChannels;
            }
            set
            {
                HideTVChannels = !value;
            }
        }

        public bool HideTVChannels
        {
            get
            {
                return GetPersistingSettingValue<bool>("HideTVChannels", true);
            }
            set
            {
                SavePersistingSettingValue<bool>("HideTVChannels", value);
            }
        }

        public bool ShowRadioChannels
        {
            get
            {
                return GetPersistingSettingValue<bool>("ShowRadioChannels");
            }
            set
            {
                SavePersistingSettingValue<bool>("ShowRadioChannels", value);
            }
        }

        public bool ShowOtherChannels
        {
            get
            {
                return GetPersistingSettingValue<bool>("ShowOtherChannels");
            }
            set
            {
                SavePersistingSettingValue<bool>("ShowOtherChannels", value);
            }
        }

        public bool ShowServiceMenu
        {
            get
            {
                return GetPersistingSettingValue<bool>("ShowServiceMenu");
            }
            set
            {
                SavePersistingSettingValue<bool>("ShowServiceMenu", value);
            }
        }

        public bool EnableLogging
        {
            get
            {
                return GetPersistingSettingValue<bool>("EnableLogging");
            }
            set
            {
                SavePersistingSettingValue<bool>("EnableLogging", value);
            }
        }

        public AppFontSizeEnum AppFontSize
        {
            get
            {
                var index = GetPersistingSettingValue<int>("AppFontSize");
                return (AppFontSizeEnum)index;
            }
            set
            {
                SavePersistingSettingValue<int>("AppFontSize", (int)value);
            }
        }

        public bool Fullscreen
        {
            get
            {
                return GetPersistingSettingValue<bool>("Fullscreen");
            }
            set
            {
                SavePersistingSettingValue<bool>("Fullscreen", value);
            }
        }

        public bool PlayOnBackground
        {
            get
            {
                return GetPersistingSettingValue<bool>("PlayOnBackground");
            }
            set
            {
                SavePersistingSettingValue<bool>("PlayOnBackground", value);
            }
        }

        public bool ScanEPG
        {
            get
            {
                return GetPersistingSettingValue<bool>("ScanEPG");
            }
            set
            {
                SavePersistingSettingValue<bool>("ScanEPG", value);
            }
        }
    }
}

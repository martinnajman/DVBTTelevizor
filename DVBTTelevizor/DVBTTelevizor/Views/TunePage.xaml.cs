﻿using LoggerService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DVBTTelevizor
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TunePage : ContentPage
    {
        private TunePageViewModel _viewModel;
        protected ILoggingService _loggingService;
        protected IDialogService _dialogService;
        protected DVBTDriverManager _driver;
        protected DVBTTelevizorConfiguration _config;

        public TunePage(ILoggingService loggingService, IDialogService dialogService, DVBTDriverManager driver, DVBTTelevizorConfiguration config)
        {
            InitializeComponent();

            _loggingService = loggingService;
            _dialogService = dialogService;
            _driver = driver;
            _config = config;

            BindingContext = _viewModel = new TunePageViewModel(_loggingService, _dialogService, _driver, _config);
        }
    }
}
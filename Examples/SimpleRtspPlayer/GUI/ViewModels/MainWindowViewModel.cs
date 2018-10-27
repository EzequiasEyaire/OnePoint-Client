using System;
using System.ComponentModel;
using System.Net;
using System.Runtime.CompilerServices;
using System.Windows;
using GalaSoft.MvvmLight.Command;
using RtspClientSharp;
using Client.GUI.Models;

namespace Client.GUI.ViewModels
{
    class MainWindowViewModel : INotifyPropertyChanged
    {
        private const string RtspPrefix = "rtsp://";
        private const string HttpPrefix = "http://";

        private string _status = string.Empty;
        private readonly IMainWindowModel _mainWindowModel;
        private bool _startButtonEnabled = true;
        private bool _stopButtonEnabled;

        public string DeviceAddress { get; set; } 

        public string Login { get; set; } 
        public string Password { get; set; } 

        public IVideoSource VideoSource => _mainWindowModel.VideoSource;

        public RelayCommand StartClickCommand { get; }
        public RelayCommand StopClickCommand { get; }
        public RelayCommand<CancelEventArgs> ClosingCommand { get; }

        public string Status
        {
            get => _status;
            set
            {
                _status = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public MainWindowViewModel(IMainWindowModel mainWindowModel)
        {
            _mainWindowModel = mainWindowModel ?? throw new ArgumentNullException(nameof(mainWindowModel));
            
            StartClickCommand = new RelayCommand(OnStartButtonClick, () => _startButtonEnabled);
            StopClickCommand = new RelayCommand(OnStopButtonClick, () => _stopButtonEnabled);
            ClosingCommand = new RelayCommand<CancelEventArgs>(OnClosing);
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void OnStartButtonClick()
        {



         DeviceAddress = "rtsp://184.72.239.149/vod/mp4:BigBuckBunny_175k.mov";
         Login  = "admin";
         Password  = "123456";





        string address = DeviceAddress;

            if (!address.StartsWith(RtspPrefix) && !address.StartsWith(HttpPrefix))
                address = RtspPrefix + address;

            if (!Uri.TryCreate(address, UriKind.Absolute, out Uri deviceUri))
            {
                MessageBox.Show("Invalid device address", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Create Credencial 
            var credential = new NetworkCredential(Login, Password);
            var connectionParameters = new ConnectionParameters(deviceUri, credential);
            connectionParameters.CancelTimeout = TimeSpan.FromSeconds(1);

            _mainWindowModel.Start(connectionParameters);
            _mainWindowModel.StatusChanged += MainWindowModelOnStatusChanged;

            _startButtonEnabled = false;
            StartClickCommand.RaiseCanExecuteChanged();
            _stopButtonEnabled = true;
            StopClickCommand.RaiseCanExecuteChanged();
        }

        private void OnStopButtonClick()
        {
            _mainWindowModel.Stop();
            _mainWindowModel.StatusChanged -= MainWindowModelOnStatusChanged;

            _stopButtonEnabled = false;
            StopClickCommand.RaiseCanExecuteChanged();
            _startButtonEnabled = true;
            StartClickCommand.RaiseCanExecuteChanged();
            Status = string.Empty;
        }

        private void MainWindowModelOnStatusChanged(object sender, string s)
        {
            Application.Current.Dispatcher.Invoke(() => Status = s);
        }

        private void OnClosing(CancelEventArgs args)
        {
            _mainWindowModel.Stop();
        }
    }
}
using SerialComm.Model;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO.Ports;
using System.Windows.Input;
using System.Management;
using System.Windows.Threading;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using NLog;
using System.Windows;
namespace SerialComm.ViewModel
{
    public class SerialCommViewModel : ViewModelBase
    {
        #region Private Fields
        private static Logger logger;
        private SerialPort _SerialPort;
        private string _InputText;
        private string _OutputText;
        private string _SettingsCheck;
        private string _CheckConnectionStatus;
        private ObservableCollection<CommPort> _CommPorts;
        private CommPort _SelectedCommPort;
        private List<SerialCommModel> _BaudRates;
        private int _SelectedBaudRate;
        private List<SerialCommModel> _Parities;
        private Parity _SelectedParity;
        private List<SerialCommModel> _StopBitsList;
        private StopBits _SelectedStopBits;
        private int[] _DataBits;
        private int _SelectedDataBits;
        private List<SerialCommModel> _LineEndings;
        private string _SelectedLineEnding;
        private bool _IsDTR;
        private bool _IsRTS;
        private bool _IsAutoscrollChecked;
        private bool _AutoscrollChecked;
        private bool _EnableDisableSettings;
        private ICommand _Open;
        private ICommand _Close;
        private ICommand _Send;
        private ICommand _Clear;
        private ICommand _OpenLink;
        #endregion

        #region Public Properties
        public string InputText
        {
            get { return _InputText; }
            set
            {
                _InputText = value;
                OnPropertyChanged("InputText");
            }
        }

        public string OutputText
        {
            get { return _OutputText; }
            set
            {
                _OutputText = value;
                OnPropertyChanged("OutputText");
            }
        }

        public string SettingsCheck
        {
            get
            {
                _SettingsCheck = GetSettingsCheck();
                return _SettingsCheck;
            }
        }

        public string CheckConnectionStatus
        {
            get
            {
                _CheckConnectionStatus = GetConnectionStatus();
                return _CheckConnectionStatus;
            }
        }

        public ObservableCollection<CommPort> CommPorts
        {
            get
            {
                if (_CommPorts == null)
                {
                    _CommPorts = GetCommPorts();
                    OnPropertyChanged("CommPorts");
                }
                return _CommPorts;
            }
        }

        public CommPort SelectedCommPort
        {
            get { return _SelectedCommPort; }
            set
            {
                _SelectedCommPort = value;
                OnPropertyChanged("SelectedCommPort");
                OnPropertyChanged("SettingsCheck");
            }
        }

        public List<SerialCommModel> BaudRates
        {
            get { return _BaudRates; }
            set
            {
                _BaudRates = value;
                OnPropertyChanged("BaudRates");
            }
        }

        public int SelectedBaudRate
        {
            get { return _SelectedBaudRate; }
            set
            {
                _SelectedBaudRate = value;
                OnPropertyChanged("SelectedBaudRate");
                OnPropertyChanged("SettingsCheck");
            }
        }

        public List<SerialCommModel> Parities
        {
            get { return _Parities; }
            set
            {
                _Parities = value;
                OnPropertyChanged("Parities");
            }
        }
        
        public Parity SelectedParity
        {
            get { return _SelectedParity; }
            set
            {
                _SelectedParity = value;
                OnPropertyChanged("SelectedParity");
                OnPropertyChanged("SettingsCheck");
            }
        }
        
        public List<SerialCommModel> StopBitsList
        {
            get { return _StopBitsList; }
            set
            {
                _StopBitsList = value;
                OnPropertyChanged("StopBitsList");
            }
        }
        
        public StopBits SelectedStopBits
        {
            get { return _SelectedStopBits; }
            set
            {
                _SelectedStopBits = value;
                OnPropertyChanged("SelectedStopBits");
                OnPropertyChanged("SettingsCheck");
            }
        }
        
        public int[] DataBits
        {
            get { return _DataBits; }
            set
            {
                _DataBits = value;
                OnPropertyChanged("DataBits");
            }
        }
        
        public int SelectedDataBits
        {
            get { return _SelectedDataBits; }
            set
            {
                _SelectedDataBits = value;
                OnPropertyChanged("SelectedDataBits");
                OnPropertyChanged("SettingsCheck");
            }
        }
        
        public List<SerialCommModel> LineEndings
        {
            get { return _LineEndings; }
            set
            {
                _LineEndings = value;
                OnPropertyChanged("LineEndings");
            }
        }
        
        public string SelectedLineEnding
        {
            get { return _SelectedLineEnding; }
            set
            {
                _SelectedLineEnding = value;
                OnPropertyChanged("SelectedLineEnding");
            }
        }
        
        public bool IsDTR
        {
            get { return _IsDTR; }
            set
            {
                _IsDTR = value;
                OnPropertyChanged("IsDTR");
                OnPropertyChanged("SettingsCheck");
            }
        }
        
        public bool IsRTS
        {
            get { return _IsRTS; }
            set
            {
                _IsRTS = value;
                OnPropertyChanged("IsRTS");
                OnPropertyChanged("SettingsCheck");
            }
        }
        
        public bool IsAutoscrollChecked
        {
            get
            {
                if (AutoscrollChecked)
                {
                    _IsAutoscrollChecked = true;
                }
                else
                {
                    _IsAutoscrollChecked = false;
                }
                return _IsAutoscrollChecked;
            }
            set
            {
                _IsAutoscrollChecked = value;
                if (_IsAutoscrollChecked)
                {
                    AutoscrollChecked = true;
                }
                else
                {
                    AutoscrollChecked = false;
                }
                OnPropertyChanged("IsAutoscrollChecked");
                OnPropertyChanged("AutoscrollChecked");
            }
        }
        
        public bool AutoscrollChecked
        {
            get { return _AutoscrollChecked; }
            set
            {
                _AutoscrollChecked = value;
                OnPropertyChanged("AutoscrollChecked");
                OnPropertyChanged("IsAutoscrollChecked");
                OnPropertyChanged("SettingsCheck");
            }
        }

        public bool EnableDisableSettings
        {
            get { return _EnableDisableSettings; }
            set
            {
                _EnableDisableSettings = value;
                OnPropertyChanged("EnableDisableSettings");
            }
        }
        #endregion

        #region Public ICommands
        public ICommand Open
        {
            get
            {
                _Open = new RelayCommand(
                    param => StartListening(),
                    param => StartListeningCanExecute());
                return _Open;
            }
        }
        
        public ICommand Close
        {
            get
            {
                _Close = new RelayCommand(
                    param => StopListening(),
                    param => StopListeningCanExecute());
                return _Close;
            }
        }
        
        public ICommand Send
        {
            get
            {
                _Send = new RelayCommand(
                    param => WriteData(),
                    param => WriteDataCanExecute());
                return _Send;
            }
        }

        public ICommand Clear
        {
            get
            {
                _Clear = new RelayCommand(
                    param => ClearOutputText());
                return _Clear;
            }
        }

        public ICommand OpenLink
        {
            get
            {
                _OpenLink = new RelayCommand(
                    param => OpenLinkMethod());
                return _OpenLink;
            }
        }
        #endregion

        #region Constructor
        public SerialCommViewModel()
        {
            logger = LogManager.GetCurrentClassLogger();
            logger.Log(LogLevel.Debug, "Application started.");

            SelectedCommPort = CommPorts[0];

            SerialCommModel _SerialCommModel = new SerialCommModel();
            BaudRates = _SerialCommModel.getBaudRates();
            SelectedBaudRate = 9600; // Default
            LineEndings = _SerialCommModel.getLineEndings();
            SelectedLineEnding = "";
            Parities = _SerialCommModel.getParities();
            SelectedParity = Parity.None;
            StopBitsList = _SerialCommModel.getStopBits();
            SelectedStopBits = StopBits.One;
            DataBits = _SerialCommModel.getDataBits;
            SelectedDataBits = 8;

            AutoscrollChecked = true;
            IsDTR = true;
            IsRTS = true;

            EnableDisableSettings = true;
        }
        #endregion

        #region Events
        private void DataReceivedEvent(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                var receivedInput = _SerialPort.ReadLine();
                OutputText += receivedInput + SelectedLineEnding;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                logger.Log(LogLevel.Error, ex.Message);
            }
        }
        #endregion

        #region Public Methods
        public void OnWindowClosing(object sender, CancelEventArgs e)
        {
            if (_SerialPort != null && _SerialPort.IsOpen)
            {
                try
                {
                    _SerialPort.Close();
                    logger.Log(LogLevel.Debug, "_SerialPort.Close() initiated on Application's closing (without pressing STOP COMMUNICATION button).");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    logger.Log(LogLevel.Error, ex.Message);
                    logger.Log(LogLevel.Debug, "Error above occured at OnWindowClosing() method.");
                }
            }
        }
        #endregion

        #region Private Methods
        private string GetSettingsCheck()
        {
            var getPort = SelectedCommPort.DeviceID;
            var getBaud = SelectedBaudRate;
            var getConn = CheckConnectionStatus;
            var getParity = SelectedParity;
            var getStopBits = SelectedStopBits;
            var getDataBits = SelectedDataBits;
            return "PortName = " + getPort.ToString() + " | BaudRate = " + getBaud.ToString() + " | Parity = " + getParity.ToString() + " | DataBits = " + getDataBits.ToString() + " | StopBits = " + getStopBits.ToString() + " | ConnectionStatus = " + getConn + " | DTR = " + IsDTR + " | RTS = " + IsRTS + " | Autoscroll = " + AutoscrollChecked.ToString();
        }

        private ObservableCollection<CommPort> GetCommPorts()
        {
            var results = new ObservableCollection<CommPort>();
            var mc = new ManagementClass("Win32_SerialPort");

            foreach (var m in mc.GetInstances()) using (m)
                {
                    results.Add(new CommPort()
                        {
                            DeviceID = (string)m.GetPropertyValue("DeviceID"),
                            Description = (string)m.GetPropertyValue("Caption")
                        });
                }
            return results;
        }

        private void WriteData()
        {
            try
            {
                _SerialPort.Write(InputText);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                logger.Log(LogLevel.Error, ex.Message);
                logger.Log(LogLevel.Debug, "Error above occured at WriteData() method.");
            }
            InputText = String.Empty;
        }

        private bool WriteDataCanExecute()
        {
            return _SerialPort != null && _SerialPort.IsOpen;
        }

        private void StartListening()
        {
            try
            {
                _SerialPort = new SerialPort(SelectedCommPort.DeviceID, SelectedBaudRate, SelectedParity, SelectedDataBits, SelectedStopBits);
                _SerialPort.DataReceived += new SerialDataReceivedEventHandler(DataReceivedEvent);
                _SerialPort.Open();
                logger.Log(LogLevel.Debug, "_SerialPort.Open() initiated.");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                logger.Log(LogLevel.Error, ex.Message);
                logger.Log(LogLevel.Debug, "Error above occured at StartListening() method.");
            }
            _SerialPort.DtrEnable = IsDTR;
            _SerialPort.RtsEnable = IsRTS;

            OutputText = "";

            OnPropertyChanged("CheckConnectionStatus");
            OnPropertyChanged("SettingsCheck");

            logger.Log(LogLevel.Info, "Parameter Settings Check: " + GetSettingsCheck());

            EnableDisableSettings = false;
        }

        private bool StartListeningCanExecute()
        {
            return _SerialPort == null || !_SerialPort.IsOpen;
        }

        private void StopListening()
        {
            _SerialPort.Close();
            OnPropertyChanged("CheckConnectionStatus");
            OnPropertyChanged("SettingsCheck");

            logger.Log(LogLevel.Debug, "_SerialPort.Close() initiated.");

            EnableDisableSettings = true;
        }

        private bool StopListeningCanExecute()
        {
            return _SerialPort != null && _SerialPort.IsOpen;
        }

        private string GetConnectionStatus()
        {
            if (_SerialPort != null && _SerialPort.IsOpen)
                return "Connected";
            else
                return "Disconnected";
        }

        private void ClearOutputText()
        {
            OutputText = "";
        }

        private void OpenLinkMethod()
        {
            System.Diagnostics.Process.Start("https://github.com/heiswayi/SerialComm");
        }
        #endregion
    }
}

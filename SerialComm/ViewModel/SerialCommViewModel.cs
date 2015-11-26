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
        private static string AppTitle = "SerialComm Monitor V1";
        private static Logger logger;
        private SerialPort _SerialPort;
        private string _InputText;
        private string _OutputText;
        private string _CheckConnectionStatus;
        private string _WindowTitle;
        private ObservableCollection<SerialPortSettingsModel.CommPort> _CommPorts;
        private SerialPortSettingsModel.CommPort _SelectedCommPort;
        private List<SerialPortSettingsModel> _BaudRates;
        private int _SelectedBaudRate;
        private List<SerialPortSettingsModel> _Parities;
        private Parity _SelectedParity;
        private List<SerialPortSettingsModel> _StopBitsList;
        private StopBits _SelectedStopBits;
        private int[] _DataBits;
        private int _SelectedDataBits;
        private List<SerialPortSettingsModel> _LineEndings;
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
        private ICommand _RefreshPorts;
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

        public string CheckConnectionStatus
        {
            get
            {
                _CheckConnectionStatus = GetConnectionStatus();
                return _CheckConnectionStatus;
            }
        }

        public string WindowTitle
        {
            get { return _WindowTitle; }
            set
            {
                _WindowTitle = value;
                OnPropertyChanged("WindowTitle");
            }
        }

        public ObservableCollection<SerialPortSettingsModel.CommPort> CommPorts
        {
            get { return _CommPorts; }
            set
            {
                _CommPorts = value;
                OnPropertyChanged("CommPorts");
            }
        }

        public SerialPortSettingsModel.CommPort SelectedCommPort
        {
            get { return _SelectedCommPort; }
            set
            {
                _SelectedCommPort = value;
                OnPropertyChanged("SelectedCommPort");
            }
        }

        public List<SerialPortSettingsModel> BaudRates
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
            }
        }

        public List<SerialPortSettingsModel> Parities
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
            }
        }
        
        public List<SerialPortSettingsModel> StopBitsList
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
            }
        }
        
        public List<SerialPortSettingsModel> LineEndings
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
            }
        }
        
        public bool IsRTS
        {
            get { return _IsRTS; }
            set
            {
                _IsRTS = value;
                OnPropertyChanged("IsRTS");
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
                    param => _SerialPort != null && _SerialPort.IsOpen);
                return _Close;
            }
        }
        
        public ICommand Send
        {
            get
            {
                _Send = new RelayCommand(
                    param => WriteData(),
                    param => _SerialPort != null && _SerialPort.IsOpen);
                return _Send;
            }
        }

        public ICommand Clear
        {
            get
            {
                _Clear = new RelayCommand(
                    param => OutputText = "");
                return _Clear;
            }
        }

        public ICommand OpenLink
        {
            get
            {
                _OpenLink = new RelayCommand(
                    param => System.Diagnostics.Process.Start("https://github.com/heiswayi/SerialComm"));
                return _OpenLink;
            }
        }

        public ICommand RefreshPorts
        {
            get
            {
                _RefreshPorts = new RelayCommand(
                    param => RefreshPortsMethod(),
                    param => (_SerialPort == null || !_SerialPort.IsOpen));
                return _RefreshPorts;
            }
        }
        #endregion

        #region Constructor
        public SerialCommViewModel()
        {
            logger = LogManager.GetCurrentClassLogger();
            logger.Log(LogLevel.Debug, "Application started.");

            try
            {
                CommPorts = SerialPortSettingsModel.Instance.GetCommPorts();
            }
            catch (Exception ex)
            {
                logger.Log(LogLevel.Error, ex.Message);
                logger.Log(LogLevel.Debug, "Error above occured when something's wrong with ManagementClass(\"Win32_SerialPort\") on application startup.");
            }
            if (CommPorts != null) SelectedCommPort = CommPorts[0];
            BaudRates = SerialPortSettingsModel.Instance.getBaudRates();
            SelectedBaudRate = 9600; // Default
            LineEndings = SerialPortSettingsModel.Instance.getLineEndings();
            SelectedLineEnding = "";
            Parities = SerialPortSettingsModel.Instance.getParities();
            SelectedParity = Parity.None;
            StopBitsList = SerialPortSettingsModel.Instance.getStopBits();
            SelectedStopBits = StopBits.One;
            DataBits = SerialPortSettingsModel.Instance.getDataBits;
            SelectedDataBits = 8;

            AutoscrollChecked = true;
            IsDTR = true;
            IsRTS = true;

            OnPropertyChanged("CheckConnectionStatus");
            WindowTitle = AppTitle + " (" + CheckConnectionStatus + ")";

            EnableDisableSettings = true;
        }
        #endregion

        #region Events
        /// <summary>
        /// Receive data event from serial port.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
        /// <summary>
        /// Close port if port is open when user closes MainWindow.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnWindowClosing(object sender, CancelEventArgs e)
        {
            try
            {
                if (_SerialPort != null && _SerialPort.IsOpen)
                {
                    _SerialPort.Close();
                    logger.Log(LogLevel.Debug, "_SerialPort.Close() initiated on Application's closing (without pressing STOP COMMUNICATION button).");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                logger.Log(LogLevel.Error, ex.Message);
                logger.Log(LogLevel.Debug, "Error above occured at OnWindowClosing() method.");
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Send data to serial port.
        /// </summary>
        private void WriteData()
        {
            try
            {
                _SerialPort.Write(InputText);
                InputText = String.Empty;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                logger.Log(LogLevel.Error, ex.Message);
                logger.Log(LogLevel.Debug, "Error above occured at WriteData() method.");
            }
        }

        /// <summary>
        /// Initiate serial port communication.
        /// </summary>
        private void StartListening()
        {
            try
            {
                _SerialPort = new SerialPort(SelectedCommPort.DeviceID, SelectedBaudRate, SelectedParity, SelectedDataBits, SelectedStopBits);
                _SerialPort.DataReceived += new SerialDataReceivedEventHandler(DataReceivedEvent);
                _SerialPort.Open();
                _SerialPort.DtrEnable = IsDTR;
                _SerialPort.RtsEnable = IsRTS;
                logger.Log(LogLevel.Debug, "_SerialPort.Open() initiated.");

                OutputText = "";

                logger.Log(LogLevel.Info, "Parameter Settings Check: " + SelectedCommPort.DeviceID + ", " + SelectedBaudRate.ToString() + " baud, Parity." + SelectedParity.ToString() + ", " + SelectedDataBits.ToString() + ", StopBits." + SelectedStopBits.ToString() + ", RTS=" + IsRTS.ToString() + ", DTR=" + IsDTR.ToString());

                EnableDisableSettings = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                logger.Log(LogLevel.Error, ex.Message);
                logger.Log(LogLevel.Debug, "Error above occured at StartListening() method.");
            }

            OnPropertyChanged("CheckConnectionStatus");
            WindowTitle = AppTitle + " (" + CheckConnectionStatus + ")";
        }

        /// <summary>
        /// Allow/disallow StartListening() to be executed.
        /// </summary>
        /// <returns>True/False</returns>
        private bool StartListeningCanExecute()
        {
            if (CommPorts == null)
            {
                return false;
            }
            else
            {
                if (_SerialPort == null || !_SerialPort.IsOpen)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Terminate serial port communication.
        /// </summary>
        private void StopListening()
        {
            try
            {
                _SerialPort.Close();
                logger.Log(LogLevel.Debug, "_SerialPort.Close() initiated.");
                EnableDisableSettings = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                logger.Log(LogLevel.Error, ex.Message);
                logger.Log(LogLevel.Debug, "Error above occured at StopListening() method.");
            }

            OnPropertyChanged("CheckConnectionStatus");
            WindowTitle = AppTitle + " (" + CheckConnectionStatus + ")";
        }

        /// <summary>
        /// Get connection/communication status.
        /// </summary>
        /// <returns>String of Connected/Disconnect</returns>
        private string GetConnectionStatus()
        {
            if (_SerialPort != null && _SerialPort.IsOpen)
                return "Connected";
            else
                return "Not Connected";
        }

        private void RefreshPortsMethod()
        {
            try
            {
                CommPorts = SerialPortSettingsModel.Instance.GetCommPorts();
                SelectedCommPort = CommPorts[0];
            }
            catch (Exception ex)
            {
                logger.Log(LogLevel.Error, ex.Message);
                logger.Log(LogLevel.Debug, "Error above occured when something's wrong with ManagementClass(\"Win32_SerialPort\") on COM* ports refresh.");
            }
        }
        #endregion
    }
}

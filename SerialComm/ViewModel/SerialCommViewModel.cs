using SerialComm.Model;
using SerialComm.Helper;
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
using System.Reflection;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Text;
namespace SerialComm.ViewModel
{
    public class SerialCommViewModel : ViewModelBase
    {
        #region Private Fields
        private static string AppTitle = "SerialComm Monitor V2";
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private SerialPort _SerialPort;
        private DispatcherTimer timer = null;
        private string _FileName = "output_data";
        private bool _IsAutoscrollChecked = true;
        private ICommand _Open;
        private ICommand _Close;
        private ICommand _Send;
        private ICommand _Clear;
        private ICommand _OpenLink;
        private ICommand _RefreshPorts;
        private ICommand _ChangeFileLocation;
        private ICommand _ExportTXTFile;
        #endregion

        #region Public Properties
        public string InputText { get; private set; }
        public string OutputText { get; private set; }
        public string WindowTitle { get; private set; }
        public List<SerialPortSettingsModel> CommPorts { get; private set; }
        public SerialPortSettingsModel SelectedCommPort { get; private set; }
        public List<SerialPortSettingsModel> BaudRates { get; private set; }
        public int SelectedBaudRate { get; private set; }
        public List<SerialPortSettingsModel> Parities { get; private set; }
        public Parity SelectedParity { get; private set; }
        public List<SerialPortSettingsModel> StopBitsList { get; private set; }
        public StopBits SelectedStopBits { get; private set; }
        public int[] DataBits { get; private set; }
        public int SelectedDataBits { get; private set; }
        public List<SerialPortSettingsModel> LineEndings { get; private set; }
        public string SelectedLineEnding { get; private set; }
        public bool IsDTR { get; private set; }
        public bool IsRTS { get; private set; }
        public bool EnableDisableSettings { get; private set; }
        public string FileLocation { get; private set; }
        public static string AssemblyDirectory
        {
            get
            {
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }
        public string[] FileExtensions { get; private set; }
        public string SelectedFileExtension { get; private set; }
        public string FileName
        {
            get { return _FileName; }
            set
            {
                _FileName = value;
                OnPropertyChanged("ExportFile");
            }
        }
        public string ExportStatus { get; private set; }
        public bool ExportStatusSuccess { get; private set; }

        // Disable interaction from UI
        // TODO: Trigger TextBoxAutomaticScrollingExtension.cs or Scroll to
        //       end the TextBox when CheckBox is checked for second time.
        public string ScrollConfirm
        {
            get
            {
                // Debug only
                //return "Autoscroll (" + ScrollOnTextChanged.ToString() + ")";
                return "Autoscroll";
            }
        }
        public bool ScrollOnTextChanged { get; private set; }
        public bool IsAutoscrollChecked
        {
            get { return _IsAutoscrollChecked; }
            set
            {
                _IsAutoscrollChecked = value;
                if (_IsAutoscrollChecked == true)
                {
                    ScrollOnTextChanged = true;
                }
                else
                {
                    ScrollOnTextChanged = false;
                }

                OnPropertyChanged("IsAutoscrollChecked");
                OnPropertyChanged("ScrollOnTextChanged");
                OnPropertyChanged("ScrollConfirm");
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

        public ICommand ChangeFileLocation
        {
            get
            {
                _ChangeFileLocation = new RelayCommand(
                    param => ChangeFileLocationMethod());
                return _ChangeFileLocation;
            }
        }

        public ICommand ExportFile
        {
            get
            {
                _ExportTXTFile = new RelayCommand(
                    param => ExportFileMethod(),
                    param => ExportFileCanExecute());
                return _ExportTXTFile;
            }
        }
        #endregion

        #region Constructor
        public SerialCommViewModel()
        {
            logger.Log(LogLevel.Info, "--- PROGRAM STARTED ---");

            _SerialPort = new SerialPort();
            logger.Log(LogLevel.Debug, "New instance of SerialPort() is initialized.");

            // Get lists of settings objects
            try
            {
                CommPorts = SerialPortSettingsModel.Instance.getCommPorts();
                BaudRates = SerialPortSettingsModel.Instance.getBaudRates();
                Parities = SerialPortSettingsModel.Instance.getParities();
                DataBits = SerialPortSettingsModel.Instance.getDataBits;
                StopBitsList = SerialPortSettingsModel.Instance.getStopBits();
                LineEndings = SerialPortSettingsModel.Instance.getLineEndings();
                FileExtensions = FileExportSettingsModel.Instance.getFileExtensions;
                logger.Log(LogLevel.Debug, "All lists of settings objects are loaded.");
            }
            catch (Exception ex)
            {
                logger.Log(LogLevel.Error, "EXCEPTION raised: " + ex.ToString());
            }

            // Set default values
            if (CommPorts != null) SelectedCommPort = CommPorts[0];
            SelectedBaudRate = 9600;
            SelectedParity = Parity.None;
            SelectedDataBits = 8;
            SelectedStopBits = StopBits.One;
            SelectedLineEnding = "";
            IsDTR = true;
            IsRTS = true;
            FileLocation = AssemblyDirectory;
            SelectedFileExtension = FileExtensions[0];
            WindowTitle = AppTitle + " (" + GetConnectionStatus() + ")";
            EnableDisableSettings = true;
            ScrollOnTextChanged = true;
            logger.Log(LogLevel.Debug, "All default values are set. End of SerialCommViewModel() constructor!");
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
            if (_SerialPort.IsOpen)
            {
                try
                {
                    //string inData = _SerialPort.ReadLine();
                    //OutputText += inData.ToString() + SelectedLineEnding;
                    //OnPropertyChanged("OutputText");

                    byte[] data = new byte[_SerialPort.BytesToRead];
                    _SerialPort.Read(data, 0, data.Length);
                    string s = Encoding.GetEncoding("Windows-1252").GetString(data);
                    OutputText += s + SelectedLineEnding;
                    OnPropertyChanged("OutputText");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    logger.Log(LogLevel.Error, "EXCEPTION raised: " + ex.ToString());
                }
            }
        }

        private void TimerTick(object send, EventArgs e)
        {
            if (timer != null)
            {
                timer.Stop();
                timer = null;
                ExportStatus = "";
                OnPropertyChanged("ExportStatus");
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
                    _SerialPort.DataReceived -= DataReceivedEvent;
                    _SerialPort.Dispose();
                    _SerialPort.Close();
                    logger.Log(LogLevel.Debug, "SerialPort.Dispose() & SerialPort.Close() are executed on OnWindowClosing() method.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                logger.Log(LogLevel.Error, "EXCEPTION raised: " + ex.ToString());
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Send data to serial port.
        /// </summary>
        private void WriteData()
        {
            if (_SerialPort.IsOpen)
            {
                try
                {
                    _SerialPort.Write(InputText);
                    InputText = String.Empty;
                    OnPropertyChanged("InputText");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    logger.Log(LogLevel.Error, "EXCEPTION raised: " + ex.ToString());
                }
            }
        }

        /// <summary>
        /// Initiate serial port communication.
        /// </summary>
        private void StartListening()
        {
            try
            {
                if (_SerialPort != null && _SerialPort.IsOpen)
                {
                    _SerialPort.Dispose();
                    _SerialPort.Close();
                    logger.Log(LogLevel.Debug, "SerialPort.Dispose() & SerialPort.Close() are executed on StartListening() method.");
                }

                _SerialPort.PortName = SelectedCommPort.DeviceID;
                _SerialPort.BaudRate = SelectedBaudRate;
                _SerialPort.Parity = SelectedParity;
                _SerialPort.DataBits = SelectedDataBits;
                _SerialPort.StopBits = SelectedStopBits;
                _SerialPort.Open();
                logger.Log(LogLevel.Debug, "SerialPort.Open() is executed.");
                _SerialPort.DtrEnable = IsDTR;
                _SerialPort.RtsEnable = IsRTS;

                OutputText = "";
                OnPropertyChanged("OutputText");

                EnableDisableSettings = false;
                OnPropertyChanged("EnableDisableSettings");

                logger.Log(LogLevel.Info, "Connected to: " + SelectedCommPort.DeviceID + ", " + SelectedBaudRate.ToString() + " baud, Parity." + SelectedParity.ToString() + ", " + SelectedDataBits.ToString() + ", StopBits." + SelectedStopBits.ToString() + ", RTS=" + IsRTS.ToString() + ", DTR=" + IsDTR.ToString());

                _SerialPort.DataReceived += new SerialDataReceivedEventHandler(DataReceivedEvent);
                logger.Log(LogLevel.Debug, "Ready to receive data...");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                logger.Log(LogLevel.Error, "EXCEPTION raised: " + ex.ToString());
            }

            WindowTitle = AppTitle + " (" + GetConnectionStatus() + ")";
            OnPropertyChanged("WindowTitle");
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
            if (_SerialPort != null && _SerialPort.IsOpen)
            {
                try
                {
                    _SerialPort.DataReceived -= DataReceivedEvent;
                    _SerialPort.Dispose();
                    _SerialPort.Close();

                    EnableDisableSettings = true;
                    OnPropertyChanged("EnableDisableSettings");

                    logger.Log(LogLevel.Info, "Disconnected from " + SelectedCommPort.DeviceID);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    logger.Log(LogLevel.Error, "EXCEPTION raised: " + ex.ToString());
                }
            }

            WindowTitle = AppTitle + " (" + GetConnectionStatus() + ")";
            OnPropertyChanged("WindowTitle");
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

        /// <summary>
        /// Rescan avaiable ports
        /// </summary>
        private void RefreshPortsMethod()
        {
            try
            {
                CommPorts = SerialPortSettingsModel.Instance.getCommPorts();
                OnPropertyChanged("CommPorts");
                SelectedCommPort = CommPorts[0];
                OnPropertyChanged("SelectedCommPort");
                logger.Log(LogLevel.Debug, "New list of COM* ports are repopulated.");
            }
            catch (Exception ex)
            {
                logger.Log(LogLevel.Error, "EXCEPTION raised: " + ex.ToString());
            }
        }

        private void ChangeFileLocationMethod()
        {
            var dlg = new CommonOpenFileDialog();
            dlg.Title = "File Location";
            dlg.IsFolderPicker = true;
            dlg.InitialDirectory = FileLocation;

            dlg.AddToMostRecentlyUsedList = false;
            dlg.AllowNonFileSystemItems = false;
            dlg.DefaultDirectory = FileLocation;
            dlg.EnsureFileExists = true;
            dlg.EnsurePathExists = true;
            dlg.EnsureReadOnly = false;
            dlg.EnsureValidNames = true;
            dlg.Multiselect = false;
            dlg.ShowPlacesList = true;

            if (dlg.ShowDialog() == CommonFileDialogResult.Ok)
            {
                var folder = dlg.FileName;
                // Do something with selected folder string
                FileLocation = folder.ToString();
                OnPropertyChanged("FileLocation");
            }
        }

        private void ExportFileMethod()
        {
            try
            {
                if (File.Exists(FileLocation + @"\" + FileName + SelectedFileExtension))
                {
                    MessageBoxResult msgBoxResult = MessageBox.Show(
                        "File " + FileName + SelectedFileExtension + " already exists!\n Select 'Yes' to overwrite the existing file or\n'No' to create a new file with timestamp suffix or\n 'Cancel' to cancel?",
                        "Overwrite Confirmation",
                        MessageBoxButton.YesNoCancel);
                    if (msgBoxResult == MessageBoxResult.Yes)
                    {
                        File.WriteAllText(FileLocation + @"\" + FileName + SelectedFileExtension, OutputText);
                        logger.Log(LogLevel.Debug, "Output data is saved and exported into " + FileLocation + @"\" + FileName + SelectedFileExtension);
                        ExportStatus = "Done.";
                        OnPropertyChanged("ExportStatus");
                        ExportStatusSuccess = true;
                        OnPropertyChanged("ExportStatusSuccess");
                        StartTimer(10);

                    }
                    else if (msgBoxResult == MessageBoxResult.No)
                    {
                        File.WriteAllText(FileLocation + @"\" + FileName + DateTime.Now.ToString("-yyyyMMddHHmmss") + SelectedFileExtension, OutputText);
                        logger.Log(LogLevel.Debug, "Output data is saved and exported into " + FileLocation + @"\" + FileName + DateTime.Now.ToString("-yyyyMMddHHmmss") + SelectedFileExtension);
                        ExportStatus = "Done.";
                        OnPropertyChanged("ExportStatus");
                        ExportStatusSuccess = true;
                        OnPropertyChanged("ExportStatusSuccess");
                        StartTimer(10);
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    File.WriteAllText(FileLocation + @"\" + FileName + SelectedFileExtension, OutputText);
                    logger.Log(LogLevel.Debug, "Output data is saved and exported into " + FileLocation + @"\" + FileName + SelectedFileExtension);
                    ExportStatus = "Done.";
                    OnPropertyChanged("ExportStatus");
                    ExportStatusSuccess = true;
                    OnPropertyChanged("ExportStatusSuccess");
                    StartTimer(10);
                }
            }
            catch (Exception ex)
            {
                ExportStatus = "Error exporting a file!";
                OnPropertyChanged("ExportStatus");
                ExportStatusSuccess = false;
                OnPropertyChanged("ExportStatusSuccess");
                StartTimer(10);
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                logger.Log(LogLevel.Error, "EXCEPTION raised: " + ex.ToString());
            }
        }

        private bool ExportFileCanExecute()
        {
            return FileName != "";
        }

        private void StartTimer(int duration)
        {
            if (timer != null)
            {
                timer.Stop();
                ExportStatus = "";
                OnPropertyChanged("ExportStatus");
            }
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(duration);
            timer.Tick += new EventHandler(TimerTick);
            timer.Start();
        }
        #endregion
    }
}

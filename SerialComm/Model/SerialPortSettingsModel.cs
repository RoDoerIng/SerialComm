using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Management;
namespace SerialComm.Model
{
    [Serializable]
    public class SerialPortSettingsModel : SingletonBase<SerialPortSettingsModel>
    {
        public SerialPortSettingsModel()
        {
        }

        #region Comm. Port    
        public string DeviceID { get; set; }
        public string Description { get; set; }
        public string DeviceInfo { get; set; }

        public List<SerialPortSettingsModel> getCommPorts()
        {
            List<SerialPortSettingsModel> devices = new List<SerialPortSettingsModel>();

            ManagementObjectCollection moc;
            using (var searcher = new ManagementObjectSearcher(@"Select * From Win32_SerialPort"))
                moc = searcher.Get();

            foreach (var device in moc)
            {
                devices.Add(new SerialPortSettingsModel()
                {
                    DeviceID = (string)device.GetPropertyValue("DeviceID"),
                    Description = (string)device.GetPropertyValue("Description"),
                    DeviceInfo = (string)device.GetPropertyValue("Description") + " (" + (string)device.GetPropertyValue("DeviceID") + ")"
                });
            }

            moc.Dispose();
            return devices;
            
        }
        #endregion

        #region Baud Rate
        public string BaudRateName { get; set; }
        public int BaudRateValue { get; set; }

        public List<SerialPortSettingsModel> getBaudRates()
        {
            List<SerialPortSettingsModel> returnBaudRates = new List<SerialPortSettingsModel>();
            returnBaudRates.Add(new SerialPortSettingsModel() { BaudRateName = "4800 baud", BaudRateValue = 4800 });
            returnBaudRates.Add(new SerialPortSettingsModel() { BaudRateName = "9600 baud", BaudRateValue = 9600 });
            returnBaudRates.Add(new SerialPortSettingsModel() { BaudRateName = "19200 baud", BaudRateValue = 19200 });
            returnBaudRates.Add(new SerialPortSettingsModel() { BaudRateName = "38400 baud", BaudRateValue = 38400 });
            returnBaudRates.Add(new SerialPortSettingsModel() { BaudRateName = "57600 baud", BaudRateValue = 57600 });
            returnBaudRates.Add(new SerialPortSettingsModel() { BaudRateName = "115200 baud", BaudRateValue = 115200 });
            returnBaudRates.Add(new SerialPortSettingsModel() { BaudRateName = "230400 baud", BaudRateValue = 230400 });
            return returnBaudRates;
        }
        #endregion

        #region Parity
        public string ParityName { get; set; }
        public Parity ParityValue { get; set; }

        public List<SerialPortSettingsModel> getParities()
        {
            List<SerialPortSettingsModel> returnParities = new List<SerialPortSettingsModel>();
            returnParities.Add(new SerialPortSettingsModel() { ParityName = "Even", ParityValue = Parity.Even });
            returnParities.Add(new SerialPortSettingsModel() { ParityName = "Mark", ParityValue = Parity.Mark });
            returnParities.Add(new SerialPortSettingsModel() { ParityName = "None", ParityValue = Parity.None });
            returnParities.Add(new SerialPortSettingsModel() { ParityName = "Odd", ParityValue = Parity.Odd });
            returnParities.Add(new SerialPortSettingsModel() { ParityName = "Space", ParityValue = Parity.Space });
            return returnParities;
        }
        #endregion

        #region DataBits
        public int[] getDataBits = { 5, 6, 7, 8 };
        #endregion

        #region StopBits
        public string StopBitsName { get; set; }
        public StopBits StopBitsValue { get; set; }

        public List<SerialPortSettingsModel> getStopBits()
        {
            List<SerialPortSettingsModel> returnStopBits = new List<SerialPortSettingsModel>();
            returnStopBits.Add(new SerialPortSettingsModel() { StopBitsName = "None", StopBitsValue = StopBits.None });
            returnStopBits.Add(new SerialPortSettingsModel() { StopBitsName = "One", StopBitsValue = StopBits.One });
            returnStopBits.Add(new SerialPortSettingsModel() { StopBitsName = "OnePointFive", StopBitsValue = StopBits.OnePointFive });
            returnStopBits.Add(new SerialPortSettingsModel() { StopBitsName = "Two", StopBitsValue = StopBits.Two });
            return returnStopBits;
        }
        #endregion

        #region Line Ending
        public string LineEndingName { get; set; }
        public string LineEndingChars { get; set; }

        public List<SerialPortSettingsModel> getLineEndings()
        {
            List<SerialPortSettingsModel> returnLineEndings = new List<SerialPortSettingsModel>();
            returnLineEndings.Add(new SerialPortSettingsModel() { LineEndingName = "No line ending", LineEndingChars = "" });
            returnLineEndings.Add(new SerialPortSettingsModel() { LineEndingName = "Newline", LineEndingChars = "\n" });
            returnLineEndings.Add(new SerialPortSettingsModel() { LineEndingName = "Carriage return", LineEndingChars = "\r" });
            returnLineEndings.Add(new SerialPortSettingsModel() { LineEndingName = "Both NL & CR", LineEndingChars = "\r\n" });
            return returnLineEndings;
        }
        #endregion
    }
}

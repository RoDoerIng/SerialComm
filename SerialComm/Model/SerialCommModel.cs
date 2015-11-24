using System.Collections.Generic;
using System.IO.Ports;
namespace SerialComm.Model
{
    public class SerialCommModel
    {
        #region Baud Rate
        public string BaudRateName { get; set; }
        public int BaudRateValue { get; set; }

        public List<SerialCommModel> getBaudRates()
        {
            List<SerialCommModel> returnBaudRates = new List<SerialCommModel>();
            returnBaudRates.Add(new SerialCommModel() { BaudRateName = "4800 baud", BaudRateValue = 4800 });
            returnBaudRates.Add(new SerialCommModel() { BaudRateName = "9600 baud", BaudRateValue = 9600 });
            returnBaudRates.Add(new SerialCommModel() { BaudRateName = "19200 baud", BaudRateValue = 19200 });
            returnBaudRates.Add(new SerialCommModel() { BaudRateName = "38400 baud", BaudRateValue = 38400 });
            returnBaudRates.Add(new SerialCommModel() { BaudRateName = "57600 baud", BaudRateValue = 57600 });
            returnBaudRates.Add(new SerialCommModel() { BaudRateName = "115200 baud", BaudRateValue = 115200 });
            returnBaudRates.Add(new SerialCommModel() { BaudRateName = "230400 baud", BaudRateValue = 230400 });
            return returnBaudRates;
        }
        #endregion

        #region Parity
        public string ParityName { get; set; }
        public Parity ParityValue { get; set; }

        public List<SerialCommModel> getParities()
        {
            List<SerialCommModel> returnParities = new List<SerialCommModel>();
            returnParities.Add(new SerialCommModel() { ParityName = "Even", ParityValue = Parity.Even });
            returnParities.Add(new SerialCommModel() { ParityName = "Mark", ParityValue = Parity.Mark });
            returnParities.Add(new SerialCommModel() { ParityName = "None", ParityValue = Parity.None });
            returnParities.Add(new SerialCommModel() { ParityName = "Odd", ParityValue = Parity.Odd });
            returnParities.Add(new SerialCommModel() { ParityName = "Space", ParityValue = Parity.Space });
            return returnParities;
        }
        #endregion

        #region DataBits
        public int[] getDataBits = { 5, 6, 7, 8 };
        #endregion

        #region StopBits
        public string StopBitsName { get; set; }
        public StopBits StopBitsValue { get; set; }

        public List<SerialCommModel> getStopBits()
        {
            List<SerialCommModel> returnStopBits = new List<SerialCommModel>();
            returnStopBits.Add(new SerialCommModel() { StopBitsName = "None", StopBitsValue = StopBits.None });
            returnStopBits.Add(new SerialCommModel() { StopBitsName = "One", StopBitsValue = StopBits.One });
            returnStopBits.Add(new SerialCommModel() { StopBitsName = "OnePointFive", StopBitsValue = StopBits.OnePointFive });
            returnStopBits.Add(new SerialCommModel() { StopBitsName = "Two", StopBitsValue = StopBits.Two });
            return returnStopBits;
        }
        #endregion

        #region Line Ending
        public string LineEndingName { get; set; }
        public string LineEndingChars { get; set; }

        public List<SerialCommModel> getLineEndings()
        {
            List<SerialCommModel> returnLineEndings = new List<SerialCommModel>();
            returnLineEndings.Add(new SerialCommModel() { LineEndingName = "No line ending", LineEndingChars = "" });
            returnLineEndings.Add(new SerialCommModel() { LineEndingName = "Newline", LineEndingChars = "\n" });
            returnLineEndings.Add(new SerialCommModel() { LineEndingName = "Carriage return", LineEndingChars = "\r" });
            returnLineEndings.Add(new SerialCommModel() { LineEndingName = "Both NL & CR", LineEndingChars = "\r\n" });
            return returnLineEndings;
        }
        #endregion
    }

    #region Comm. Port
    public class CommPort
    {
        public string DeviceID { get; set; }
        public string Description { get; set; }
    }
    #endregion
}

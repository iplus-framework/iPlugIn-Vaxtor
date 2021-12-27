using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using System.Net.Sockets;
using System.IO.Ports;

namespace soehnle.mes.processapplication
{
    public class Comm3xxxBase
    {
        #region Methods
        public bool ReadWeightData(TcpClient channel, out string result)
        {
            result = null;
            return false;
        }

        public bool ReadWeightData(SerialPort channel, out string result)
        {
            result = null;
            return false;
        }

        public Tele3xxxEDV ConvertToEDV(string result)
        {
            return new Tele3xxxEDV(result);
        }

        public bool StartReadingWeights(TcpClient channel)
        {
            return false;
        }

        public bool StartReadingWeights(SerialPort channel)
        {
            return false;
        }
        #endregion
    }
}

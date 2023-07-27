using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using System.Net.Sockets;
using System.IO.Ports;

namespace bosche.mes.processapplication
{
    public class CommBoscheMCI
    {
        #region Methods
        public bool ReadWeightData(TcpClient channel, out string result)
        {            
            result = null;
            if (channel == null || !channel.Connected)
                return false;
            NetworkStream stream = channel.GetStream();
            if (stream == null)
                return false;
            if (stream.CanRead && stream.DataAvailable)
            {
                byte[] myReadBuffer = new byte[1024];
                StringBuilder myCompleteMessage = new StringBuilder();
                int numberOfBytesRead = 0;
                do
                {
                    numberOfBytesRead = stream.Read(myReadBuffer, 0, myReadBuffer.Length);
                    myCompleteMessage.AppendFormat("{0}", Encoding.ASCII.GetString(myReadBuffer, 0, numberOfBytesRead));
                }
                while (stream.DataAvailable);

                result = myCompleteMessage.ToString();
                return result != null;
            }
            return false;
        }


        public bool ReadWeightData(SerialPort channel, int teleLength, out string result)
        {
            result = null;
            if (channel == null)
            {
                return false;
            }

            if (channel.IsOpen)
            {
                byte[] myReadBuffer = new byte[1024];
                int numberOfBytesRead = 0;
                if (teleLength <= 0)
                    teleLength = TeleContMCI.C_TelegramLength;
                int readSize = (1024 / teleLength) * teleLength;
                try
                {
                    numberOfBytesRead = channel.Read(myReadBuffer, 0, readSize);
                }
                catch(TimeoutException)
                {
                    return false;
                }

                //channel.DiscardInBuffer();
                result = string.Format("{0}", Encoding.ASCII.GetString(myReadBuffer, 0, numberOfBytesRead));
                return result != null;
            }
            return false;
        }
    

        public bool StartReadingWeights(TcpClient channel)
        {
            if (channel == null || !channel.Connected)
                return false;
            NetworkStream stream = channel.GetStream();
            if (stream == null || !stream.CanWrite)
                return false;
            stream.Write(CmdMCI.GetValueAllTime, 0, CmdMCI.C_CmdLength);
            return true;
        }


        public bool StartReadingWeights(SerialPort channel)
        {
            //if (!channel.IsOpen)
            //    return false;

            //try
            //{
            //    channel.Write(CmdMCI.GetValueAllTime, 0, CmdMCI.C_CmdLength);
            //}
            //catch (Exception)
            //{
            //    return false;
            //}
            return true;
        }

        public bool StartReadingWeights(SerialPort channel, byte[] command)
        {
            //if (!channel.IsOpen)
            //    return false;
            //channel.Write(command, 0, CmdMCI.C_CmdLength);
            return true;
        }




        public bool StopReadingWeights(TcpClient channel)
        {
            //if (channel == null || !channel.Connected)
            //    return false;
            //NetworkStream stream = channel.GetStream();
            //if (stream == null || !stream.CanWrite)
            //    return false;
            //stream.Write(CmdMCI.GetValueOneTime, 0, CmdMCI.C_CmdLength);
            return true;
        }


        public bool StopReadingWeights(SerialPort channel)
        {
            //if (!channel.IsOpen)
            //    return false;
            //channel.Write(CmdMCI.GetValueOneTime, 0, CmdMCI.C_CmdLength);
            return true;
        }

        //[ACMethodInfo("Read weight", "en{'Read weight'}de{'Gewicht ablesen'}", 701)]
        //public void ReadWeights(string source)
        //{
        //    string[] sourceSplit = source.Split(';');
        //    string targetWord = "GRO:";

        //    // gross weight from string
        //    foreach (var word in sourceSplit)
        //    {
        //        if (word.Contains(targetWord))
        //        {
        //            string result = word.Substring(5);
        //            try
        //            {
        //                double weight = Double.Parse(result);
        //                ActualValue.ValueT = weight;
        //                ActualWeight.ValueT = weight;
        //                Console.WriteLine(weight);
        //            }
        //            catch (FormatException)
        //            {
        //                Console.WriteLine($"Unable to parse '{result}'");
        //            }
        //        }
        //    }
        //}
        #endregion
    }
}

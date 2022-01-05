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


        public bool ReadWeightData(SerialPort channel, out string result)
        {
            result = null;
            if (channel == null)
                return false;
            if (channel.IsOpen && channel.BytesToRead > 0)
            {

                byte[] myReadBuffer = new byte[1024];
                int numberOfBytesRead = 0;
                numberOfBytesRead = channel.Read(myReadBuffer, 0, myReadBuffer.Length);
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
            stream.Write(Cmd3xxx.GetValueAllTime, 0, Cmd3xxx.C_CmdLength);
            return true;
        }


        public bool StartReadingWeights(SerialPort channel)
        {
            if (!channel.IsOpen)
                return false;
            channel.Write(Cmd3xxx.GetValueAllTime, 0, Cmd3xxx.C_CmdLength);
            return true;
        }


        public bool StopReadingWeights(TcpClient channel)
        {
            if (channel == null || !channel.Connected)
                return false;
            NetworkStream stream = channel.GetStream();
            if (stream == null || !stream.CanWrite)
                return false;
            stream.Write(Cmd3xxx.GetValueOneTime, 0, Cmd3xxx.C_CmdLength);
            return true;
        }


        public bool StopReadingWeights(SerialPort channel)
        {
            if (!channel.IsOpen)
                return false;
            channel.Write(Cmd3xxx.GetValueOneTime, 0, Cmd3xxx.C_CmdLength);
            return true;
        }


        public virtual bool SendReadAlibiCmd(TcpClient channel)
        {
            if (channel == null || !channel.Connected)
                return false;
            NetworkStream stream = channel.GetStream();
            if (stream == null || !stream.CanWrite)
                return false;
            stream.Write(Cmd3xxx.GetValueOneTimeStillWithAlibi, 0, Cmd3xxx.C_CmdLength);
            return true;
        }


        public virtual bool SendReadAlibiCmd(SerialPort channel)
        {
            if (!channel.IsOpen)
                return false;
            channel.Write(Cmd3xxx.GetValueOneTimeStillWithAlibi, 0, Cmd3xxx.C_CmdLength);
            return true;
        }


        public Tele3xxxEDV ConvertToEDV(string result)
        {
            return new Tele3xxxEDV(result);
        }

        #endregion
    }
}

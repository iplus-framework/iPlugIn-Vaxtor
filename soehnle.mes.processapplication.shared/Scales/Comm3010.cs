using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using System.Net.Sockets;
using System.IO.Ports;

namespace soehnle.mes.processapplication
{
    public class Comm3010 : Comm3xxxBase
    {
        #region Methods
        public override bool SendReadAlibiCmd(TcpClient channel)
        {
            if (channel == null || !channel.Connected)
                return false;
            NetworkStream stream = channel.GetStream();
            if (stream == null || !stream.CanWrite)
                return false;
            stream.Write(Cmd3xxx.GetValueOneTimeStill, 0, Cmd3xxx.C_CmdLength);
            return true;
        }

        public override bool SendReadAlibiCmd(SerialPort channel)
        {
            if (!channel.IsOpen)
                return false;
            channel.Write(Cmd3xxx.GetValueOneTimeStill, 0, Cmd3xxx.C_CmdLength);
            return true;
        }
        #endregion
    }
}
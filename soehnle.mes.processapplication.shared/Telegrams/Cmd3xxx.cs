using System;
using System.Collections.Generic;
using System.Text;

namespace soehnle.mes.processapplication
{
    public static class Cmd3xxx
    {
        public const string C_GetValueOneTime = "<A>";
        public const string C_GetValueOneTimeACK = "<a>";
        public const string C_GetValueOneTimeStillWithAlibi = "<B>";
        public const string C_GetValueOneTimeStillWithAlibiACK = "<b>";
        public const string C_GetValueAllTimeStill = "<C>";
        public const string C_GetValueAllTimeStillACK = "<c>";
        public const string C_GetValueAllTimeStillEmpty = "<D>";
        public const string C_GetValueAllTimeStillEmptyACK = "<d>";
        public const string C_GetValueAllTimeStillEmptyReleased = "<E>";
        public const string C_GetValueAllTimeStillEmptyReleasedACK = "<E>";
        public const string C_GetValueAllTime = "<F>";
        public const string C_GetValueAllTimeACK = "<f>";
        public const string C_GetValueOneTimeStill = "<H>";
        public const string C_GetValueOneTimeStillACK = "<h>";
        public const int C_CmdLength = 3;

        public const byte C_ACK = 0x6;
        public const byte C_NAK = 0x15;

        public readonly static byte[] GetValueOneTime = Encoding.ASCII.GetBytes(C_GetValueOneTime);
        public readonly static byte[] GetValueOneTimeACK = Encoding.ASCII.GetBytes(C_GetValueOneTimeACK);
        public readonly static byte[] GetValueOneTimeStillWithAlibi = Encoding.ASCII.GetBytes(C_GetValueOneTimeStillWithAlibi);
        public readonly static byte[] GetValueOneTimeStillWithAlibiACK = Encoding.ASCII.GetBytes(C_GetValueOneTimeStillWithAlibiACK);
        public readonly static byte[] GetValueAllTimeStill = Encoding.ASCII.GetBytes(C_GetValueAllTimeStill);
        public readonly static byte[] GetValueAllTimeStillACK = Encoding.ASCII.GetBytes(C_GetValueAllTimeStillACK);
        public readonly static byte[] GetValueAllTimeStillEmpty = Encoding.ASCII.GetBytes(C_GetValueAllTimeStillEmpty);
        public readonly static byte[] GetValueAllTimeStillEmptyACK = Encoding.ASCII.GetBytes(C_GetValueAllTimeStillEmptyACK);
        public readonly static byte[] GetValueAllTimeStillEmptyReleased = Encoding.ASCII.GetBytes(C_GetValueAllTimeStillEmptyReleased);
        public readonly static byte[] GetValueAllTimeStillEmptyReleasedACK = Encoding.ASCII.GetBytes(C_GetValueAllTimeStillEmptyReleasedACK);
        public readonly static byte[] GetValueAllTime = Encoding.ASCII.GetBytes(C_GetValueAllTime);
        public readonly static byte[] GetValueAllTimeACK = Encoding.ASCII.GetBytes(C_GetValueAllTimeACK);
        public readonly static byte[] GetValueOneTimeStill = Encoding.ASCII.GetBytes(C_GetValueOneTimeStill);
        public readonly static byte[] GetValueOneTimeStillACK = Encoding.ASCII.GetBytes(C_GetValueOneTimeStillACK);
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

namespace soehnle.mes.processapplication
{
    public class Tele3xxxAlibi : Tele3xxxEDV
    {
        #region c'tors
        //Alibi
        //
        // | 1 | 2 | 3 | 4 | 5 | 6 | 7 | 8 | 9 | 10 | 11 | 12 | 13 | 14 | 15 | 16 | 17 | 18 | 19 | 20 | 21 | 22 | 23 | 24 | 25 | 26 | 27 | 28 | 29 | 30 | 31 | .... | 42 |

        // | A | 0 | 0 | 0 | 0 | 0 | 0 | 1 | 0 |  0 |  0 |  1 |  0 |  1 |  N |    |    |    |    |  - |  1 |  0 |  0 |  0 |  . |  0 |    |  k |  g |    |
        //                 Alibi           |      Status      |  Scale  |  Net value with marking, presign and dimension


        public Tele3xxxAlibi(string telegram) : base(telegram)
        {
            AlibiNumber = "";
            string tele = ExtractLastPart(telegram);
            if (String.IsNullOrEmpty(tele))
                return;
            AlibiNumber = tele.Substring(0, StartOffset);
        }

        public Tele3xxxAlibi(bool isUnderLoad, bool isOverLoad, bool isStandStill, bool isEmpty, string scaleID, ScaleValueTypeEnum scaleValueType, decimal weightValue, string dimension, string alibiNumber) 
                            : base(isUnderLoad, isOverLoad, isStandStill, isEmpty, scaleID, scaleValueType, weightValue, dimension)
        {
            if (String.IsNullOrEmpty(alibiNumber) || alibiNumber.Length != 8)
                throw new ArgumentOutOfRangeException("alibiNumber");

            AlibiNumber = alibiNumber;
        }
        #endregion


        #region Properties
        public string AlibiNumber { get; private set; }

        protected override int StartOffset
        {
            get
            {
                return C_TelegramOffsetAlibi;
            }
        }

        public override int TelegramLength
        {
            get
            {
                return StartOffset + base.TelegramLength;
            }
        }

        public const int C_TelegramOffsetAlibi = 8;
        public const int C_TelegramLengthAlibi = C_TelegramLength + C_TelegramOffsetAlibi;

        public override string TelegramR
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(AlibiNumber);
                sb.Append(base.TelegramR);
                return sb.ToString();
            }
        }
        #endregion
    }
}

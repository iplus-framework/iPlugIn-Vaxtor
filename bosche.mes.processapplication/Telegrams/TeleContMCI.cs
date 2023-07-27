using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

namespace bosche.mes.processapplication
{
    public class TeleContMCI
    {
        #region c'tors
        //Telegram:
        //
        //CHN:A;RAW:+      0;Z;ST;GS;GRO:+    0.0;NET:+    0.0; TAR:+    0.0;UW:+    172;CNT:+      0;
        //


        public TeleContMCI(string telegram)
        {
            InvalidTelegram = true;
            IsUnderLoad = false;
            IsOverLoad = false;
            IsStandStill = false;
            IsEmpty = false;
            ScaleID = null;
            ScaleValueType = ScaleValueTypeEnum.NetWeight;
            _WeightValue = 0.0M;
            Dimension = C_Dim_Kilo;
            InvalidWeight = true;

            string tele = ExtractLastPart(telegram);
            if (String.IsNullOrEmpty(tele))
                return;

            string[] valueArr = tele.Split(';');
            if (valueArr == null || valueArr.Length == 0)
                return;
            foreach (string value in valueArr)
            {
                if (value.StartsWith("GRO:"))
                {
                    ScaleValueType = ScaleValueTypeEnum.GrossWeight;
                    string sWeight = value.Substring(5).Trim();
                    //if (Decimal.TryParse(sWeight, NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign, _CultureInfo, out _WeightValue))
                    if (Decimal.TryParse(sWeight, NumberStyles.AllowDecimalPoint, _CultureInfo, out _WeightValue))
                    {
                        InvalidWeight = false;
                    }
                    if (value[0] == '-')
                        _WeightValue = _WeightValue * -1;
                }
                else if (value.StartsWith("NET:"))
                {
                }
                else if (value.StartsWith("TAR:"))
                {
                }
                else if (value.StartsWith("UW:"))
                {
                }
                else if (value.StartsWith("CNT:"))
                {
                }
                else if (value.StartsWith("ST"))
                {
                    IsStandStill = true;
                }
                else if (value.StartsWith("GS"))
                {
                }
                else if (value.StartsWith("Z"))
                {
                    IsEmpty = true;
                }
            }

            //InvalidTelegram = false;
            //IsUnderLoad = tele[StartOffset + 0] == '1';
            //IsOverLoad = tele[StartOffset + 1] == '1';
            //IsStandStill = tele[StartOffset + 2] == '1';
            //IsEmpty = tele[StartOffset + 3] == '1';
            //ScaleID = tele.Substring(StartOffset + 4, 2);
            //switch (tele[StartOffset + 6])
            //{
            //    case 'N':
            //        ScaleValueType = ScaleValueTypeEnum.NetWeight;
            //        break;
            //    case 'B':
            //        ScaleValueType = ScaleValueTypeEnum.GrossWeight;
            //        break;
            //    case 'T':
            //        ScaleValueType = ScaleValueTypeEnum.TareWeight;
            //        break;
            //    default:
            //        break;
            //}
            //string sWeight = tele.Substring(StartOffset + 12, 6);
            //sWeight = sWeight.Trim();
            //if (!string.IsNullOrWhiteSpace(sWeight)
            //    && sWeight != C_EmptyWeightUnderload1
            //    && sWeight != C_EmptyWeightUnderload2
            //    && Decimal.TryParse(sWeight, NumberStyles.AllowDecimalPoint|NumberStyles.AllowLeadingSign, _CultureInfo, out _WeightValue))
            //{
            //    InvalidWeight = false;
            //    if (tele[StartOffset + 11] == '-')
            //        _WeightValue = _WeightValue * -1;
            //    if (tele.Length >= (StartOffset + 18 + 4))
            //    {
            //        string dimension = tele.Substring(StartOffset + 18, 4);
            //        dimension = dimension.Trim();
            //        if (!String.IsNullOrEmpty(dimension))
            //            Dimension = dimension.ToLower();
            //    }
            //}
            //else
            //{
            //    InvalidWeight = true;
            //}
        }

        public TeleContMCI(bool isUnderLoad, bool isOverLoad, bool isStandStill, bool isEmpty, string scaleID, ScaleValueTypeEnum scaleValueType, decimal weightValue, string dimension)
        {
            IsUnderLoad = isUnderLoad;
            IsOverLoad = isOverLoad;
            IsStandStill = isStandStill;
            IsEmpty = isEmpty;
            ScaleID = scaleID;
            ScaleValueType = scaleValueType;
            _WeightValue = weightValue;
            InvalidWeight = false;
            Dimension = C_Dim_Kilo;
            if (!String.IsNullOrEmpty(dimension))
                Dimension = dimension;
        }
        #endregion


        #region Properties
        protected virtual int StartOffset
        {
            get
            {
                return 0;
            }
        }

        public virtual int TelegramLength
        {
            get
            {
                return C_TelegramLength;
            }
        }

        public const int C_TelegramLength = 92;

        private static CultureInfo _CultureInfo = new CultureInfo("en-US");
        public const string C_EmptyWeightUnderload1 = "______";
        public const string C_EmptyWeightUnderload2 = "^^^^^^";
        public const string C_Dim_Gramm = "g";
        public const string C_Dim_Kilo = "kg";
        public const string C_Dim_Ton = "to";

        public bool IsUnderLoad { get; private set; }
        public bool IsOverLoad { get; private set; }
        public bool IsStandStill { get; private set; }
        public bool IsEmpty{ get; private set; }
        public bool IsSerialComm { get; private set; }
        public string ScaleID { get; private set; }
        public ScaleValueTypeEnum ScaleValueType { get; private set; }

        private decimal _WeightValue;
        public decimal WeightValue
        {
            get
            {
                return _WeightValue;
            }
            private set
            {
                _WeightValue = value;
            }
        }
        public string Dimension { get; private set; }

        public double WeightKg
        {
            get
            {
                if (String.IsNullOrEmpty(Dimension) || Dimension == C_Dim_Kilo)
                    return Convert.ToDouble(WeightValue);
                else if (Dimension == C_Dim_Gramm)
                    return Convert.ToDouble(WeightValue) * 0.001;
                else
                    return Convert.ToDouble(WeightValue) / 1000;
            }
        }

        public bool InvalidWeight { get; private set; }

        public bool InvalidTelegram { get; private set; }

        public virtual string TelegramR
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(IsUnderLoad ? '1' : '0');
                sb.Append(IsOverLoad ? '1' : '0');
                sb.Append(IsStandStill ? '1' : '0');
                sb.Append(IsEmpty ? '1' : '0');
                string tmp = "00" + ScaleID;
                sb.Append(tmp.Substring(tmp.Length - 2, 2));
                switch (ScaleValueType)
                {
                    case ScaleValueTypeEnum.NetWeight:
                        sb.Append('N');
                        break;
                    case ScaleValueTypeEnum.GrossWeight:
                        sb.Append('B');
                        break;
                    case ScaleValueTypeEnum.TareWeight:
                        ScaleValueType = ScaleValueTypeEnum.TareWeight;
                        sb.Append('T');
                        break;
                    default:
                        sb.Append('N');
                        break;
                }
                sb.Append("    ");
                sb.Append(_WeightValue < 0 ? '-' : '+');
                string s = String.Format("{0:    0.0}", Math.Abs(_WeightValue));
                sb.Append(s.Substring(s.Length - 6, 6));
                tmp = " " + Dimension + "    ";
                sb.Append(tmp.Substring(4));
                return sb.ToString();
            }
        }
        #endregion


        #region Methods
        protected virtual string ExtractLastPart(string telegram)
        {
            if (   String.IsNullOrEmpty(telegram)
                || telegram.Length < TelegramLength)
                return null;
            else
            {
                int startOfLastSegment = telegram.LastIndexOf("CHN:");
                if (startOfLastSegment < 0)
                    return null;
                return telegram.Substring(startOfLastSegment);
            }
        }

        public override string ToString()
        {
            return TelegramR;
        }
        #endregion
    }
}

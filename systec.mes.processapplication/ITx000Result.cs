using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;


namespace systec.mes.processapplication
{
    [ACSerializeableInfo]
    [DataContract]
    public class ITx000Result
    {
        public ITx000Result(double bruttoWeight, string uniqueIdenNo, string errorMsg)
        {
            BruttoWeight = bruttoWeight;
            UniqueIdentifierNo = uniqueIdenNo;
            ErrorMessage = errorMsg;
        }

        [DataMember]
        public double BruttoWeight
        {
            get;
            set;
        }

        [DataMember]
        public string UniqueIdentifierNo
        {
            get;
            set;
        }

        [DataMember]
        public string ErrorMessage
        {
            get;
            set;
        }

        [DataMember]
        public string Date
        {
            get;
            set;
        }

        [DataMember]
        public string Time
        {
            get;
            set;
        }

        public override string ToString()
        {
            return String.Format("BruttoWeight={0}; UniqueIdentifierNo={1}, ErrorMessage={2}, Date={3}, Time={4}", BruttoWeight, UniqueIdentifierNo, ErrorMessage, Date, Time);
        }
    }

}

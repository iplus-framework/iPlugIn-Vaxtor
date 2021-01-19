using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace systec.mes.processapplication
{
    public class ITx000Result
    {
        public ITx000Result(double bruttoWeight, string uniqueIdenNo, string errorMsg)
        {
            BruttoWeight = bruttoWeight;
            UniqueIdentifierNo = uniqueIdenNo;
            ErrorMessage = errorMsg;
        }

        public double BruttoWeight
        {
            get;
            set;
        }

        public string UniqueIdentifierNo
        {
            get;
            set;
        }

        public string ErrorMessage
        {
            get;
            set;
        }
    }
}

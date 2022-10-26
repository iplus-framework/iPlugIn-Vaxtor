using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace advantech.mes.processapplication
{
    public class Filter
    {

        public Filter()
        {
            FltrEnum = FltrEnum.NoFilter;
            UID = 1;
            TmF = 1;
        }

        public int Fltr { get; set; }

        public FltrEnum FltrEnum
        {
            get
            {
                return (FltrEnum)Fltr;
            }
            set
            {
                Fltr = (int)value;
            }
        }

        public int UID { get; set; }
        public int MAC { get; set; }
        public int TmF { get; set; }
        public int SysTk { get; set; }

        public long Amt { get; set; }
        public long TFst { get; set; }
        public long TLst { get; set; }
        public long TSt { get; set; }
        public long TEnd { get; set; }

    }
}

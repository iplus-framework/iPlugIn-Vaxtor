using System;

namespace advantech.mes.processapplication
{
    public class LogMsg
    {
        public int PE { get; set; }
        public string UID { get; set; }
        public DateTime TIM { get; set; }

        public int[][] Record { get; set; }
    }
}
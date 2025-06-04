using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace vaxtor.mes.processapplication
{
    [XmlRoot("resultset")]
    public class ResultSet
    {
        [XmlAttribute("results")]
        public int Results { get; set; }

        [XmlAttribute("limit")]
        public int Limit { get; set; }

        [XmlAttribute("count")]
        public int Count { get; set; }

        [XmlElement("container")]
        public List<Container> Containers
        {
            get;
            set;
        }

        public ResultSet()
        {
            Containers = new List<Container>();
        }
    }
}
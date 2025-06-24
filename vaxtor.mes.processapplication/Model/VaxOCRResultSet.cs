using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace vaxtor.mes.processapplication
{
    [DataContract]
    [ACSerializeableInfo]
    [ACClassInfo(Const.PackName_VarioSystem, "en{'Vaxtor OCR result set'}de{'Vaxtor OCR result set'}", Global.ACKinds.TACClass, Global.ACStorableTypes.NotStorable, true, false)]
    [XmlRoot("resultset")]
    public class VaxOCRResultSet
    {
        [DataMember]
        [XmlAttribute("results")]
        public int Results { get; set; }

        [DataMember]
        [XmlAttribute("limit")]
        public int Limit { get; set; }

        [DataMember]
        [XmlAttribute("count")]
        public int Count { get; set; }

        [DataMember]
        [XmlElement("container")]
        public List<VaxOCRContainer> Containers
        {
            get;
            set;
        }

        public VaxOCRResultSet()
        {
            Containers = new List<VaxOCRContainer>();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace vaxtor.mes.processapplication
{
    public class Container
    {
        public Container()
        {

        }

        [XmlAttribute("container_id")]
        public string ContainerID
        {
            get;
            set;
        }

        [XmlAttribute("container_code")]
        public string ContainerCode
        {
            get;
            set;
        }

        [XmlAttribute("serial_code")]
        public string SerialCode
        {
            get;
            set;
        }

        [XmlAttribute("control_digit")]
        public string ControlDigit
        {
            get;
            set;
        }

        [XmlAttribute("size_type_code")]
        public string SizeTypeCode
        {
            get;
            set;
        }

        [XmlAttribute("size_type")]
        public string SizeType
        {
            get;
            set;
        }

        [XmlAttribute("size_description")]
        public string SizeDescription
        {
            get;
            set;
        }

        [XmlAttribute("owner_code")]
        public string OwnerCode
        {
            get;
            set;
        }

        [XmlAttribute("owner_company")]
        public string OwnerCompany
        {
            get;
            set;
        }
    }
}
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
    [ACClassInfo(Const.PackName_VarioSystem, "en{'Vaxtor OCR Container'}de{'Vaxtor OCR Container'}", Global.ACKinds.TACClass, Global.ACStorableTypes.NotStorable, true, false)]
    [XmlRoot("container")]
    public class VaxOCRContainer
    {
        public VaxOCRContainer()
        {

        }

        [DataMember]
        [XmlAttribute("id")]
        public string ID { get; set; }

        [DataMember]
        [XmlAttribute("container_code")]
        public string ContainerCode { get; set; }

        [DataMember]
        [XmlAttribute("serial_code")]
        public string SerialCode { get; set; }

        [DataMember]
        [XmlAttribute("control_digit")]
        public string ControlDigit { get; set; }

        [DataMember]
        [XmlAttribute("size_type_code")]
        public string SizeTypeCode { get; set; }

        [DataMember]
        [XmlAttribute("size_type")]
        public string SizeType { get; set; }

        [DataMember]
        [XmlAttribute("size_description")]
        public string SizeDescription { get; set; }

        [DataMember]
        [XmlAttribute("owner_code")]
        public string OwnerCode { get; set; }

        [DataMember]
        [XmlAttribute("owner_company")]
        public string OwnerCompany { get; set; }

        [DataMember]
        [XmlAttribute("owner_city")]
        public string OwnerCity { get; set; }

        [DataMember]
        [XmlAttribute("owner_country")]
        public string OwnerCountry { get; set; }

        [DataMember]
        [XmlAttribute("timestamp")]
        public string Timestamp { get; set; }

        [DataMember]
        [XmlAttribute("confidence")]
        public string Confidence { get; set; }

        public double ConfidenceValue
        {
            get
            {
                if (!double.TryParse(Confidence, out double value))
                    value = 0.0;
                return value;
            }
        }

        [DataMember]
        [XmlAttribute("image_path")]
        public string ImagePath { get; set; }

        [DataMember]
        [XmlAttribute("image_width")]
        public string ImageWidth { get; set; }

        [DataMember]
        [XmlAttribute("image_height")]
        public string ImageHeight { get; set; }

        [DataMember]
        [XmlAttribute("bbox_x")]
        public string BoundingBoxX { get; set; }

        [DataMember]
        [XmlAttribute("bbox_y")]
        public string BoundingBoxY { get; set; }

        [DataMember]
        [XmlAttribute("bbox_width")]
        public string BoundingBoxWidth { get; set; }

        [DataMember]
        [XmlAttribute("bbox_height")]
        public string BoundingBoxHeight { get; set; }
    }

}
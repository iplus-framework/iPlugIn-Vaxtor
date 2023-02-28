using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace advantech.mes.processapplication
{
    [DataContract]
    [ACSerializeableInfo(new Type[] { typeof(ChannelResult), typeof(List<ChannelResult>) })]
    [ACClassInfo(Const.PackName_VarioSystem, "en{'ChannelResult'}de{'ChannelResult'}", Global.ACKinds.TACSimpleClass, Global.ACStorableTypes.NotStorable, true, false)]
    public class ChannelResult
    {
        [DataMember]
        public int Channel { get; set; }

        [DataMember]
        public long Count { get; set; }

        public override string ToString()
        {
            return String.Format("Channel: {0}, Count: {1}", Channel, Count);
        }
    }
}

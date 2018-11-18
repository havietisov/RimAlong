using RimWorld;
using System.Linq;
using System.Runtime.Serialization;
using Verse;

namespace CooperateRim
{
    public class BillRepeatModeDefSurrogate : ISerializationSurrogate
    {
        public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
        {
            BillRepeatModeDef def = (BillRepeatModeDef)obj;
            info.AddValue("defName", def.defName);
        }

        public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
        {
            string defName = info.GetString("defName");
            return DefDatabase<BillRepeatModeDef>.AllDefsListForReading.First(u=> u.defName == defName);
        }
    }
}

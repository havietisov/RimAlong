using RimWorld;
using System.Linq;
using System.Runtime.Serialization;
using Verse;

namespace CooperateRim
{
    public class TimeAssignmentDefSurrogate : ISerializationSurrogate
    {
        public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
        {
            info.AddValue("defname", ((TimeAssignmentDef)obj).defName);
        }

        public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
        {
            string defname = info.GetString("defname");
            return DefDatabase<TimeAssignmentDef>.AllDefs.First(u => u.defName == defname);
        }
    }
}

using System.Runtime.Serialization;
using Verse;
using System.Linq;

namespace CooperateRim
{
    public class ResearchProjectDef_surrogate : ISerializationSurrogate
    {
        public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
        {
            info.AddValue("defname", ((ResearchProjectDef)obj).defName);
        }

        public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
        {
            string defname = info.GetString("defname");
            return DefDatabase<ResearchProjectDef>.AllDefs.First(u => u.defName == defname);
        }
    }
}

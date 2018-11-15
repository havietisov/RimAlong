using RimWorld;
using System.Runtime.Serialization;
using Verse;

namespace CooperateRim
{
    public class WorkGiverDefSurrogate : ISerializationSurrogate
    {
        public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
        {
            WorkGiverDef df = (WorkGiverDef)obj;
            info.AddValue("defname", df.defName);
        }

        public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
        {
            string defname = info.GetString("defname");

            foreach (var rec in DefDatabase<WorkGiverDef>.AllDefsListForReading)
            {
                if (rec.defName == defname)
                {
                    return rec;
                }
            }

            CooperateRimming.Log("Could not locate defname");
            return null;
        }
    }
}

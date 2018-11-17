using System.Runtime.Serialization;
using Verse;

namespace CooperateRim
{
    public class ThingDefSurrogate : ISerializationSurrogate
    {
        public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
        {
            ThingDef s = (ThingDef)obj;
            info.AddValue("thingDefName", s.defName);
        }

        public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
        {
            string str = info.GetString("thingDefName");


            foreach (ThingDef def in DefDatabase<ThingDef>.AllDefs)
            {
                if (str == def.defName)
                {
                    return def;
                }
            }

            return null;
        }
    }

    public class JobDefSurrogate : ISerializationSurrogate
    {
        public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
        {
            JobDef inst = (JobDef)obj;

            int v = 0; 

            foreach (JobDef def in DefDatabase<JobDef>.AllDefs)
            {
                if (def == inst)
                {
                    info.AddValue("deftable_index", v);
                    return;
                }
                v++;
            }

            CooperateRimming.Log("could not serialize jobdef");
            info.AddValue("deftable_index", -1);
        }

        public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
        {
            int v = 0;
            int indx = info.GetInt32("deftable_index");

            foreach (JobDef def in DefDatabase<JobDef>.AllDefs)
            {
                if (indx == v)
                {
                    return def;
                }
                v++;
            }

            return null;
        }
    }
}

using System.Runtime.Serialization;
using Verse;

namespace CooperateRim
{
    public class SpecialThingFilterDefSurrogate : ISerializationSurrogate
    {
        public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
        {
            SpecialThingFilterDef s = (SpecialThingFilterDef)obj;
            info.AddValue("specialThingFilterDefName", s.defName);
        }

        public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
        {
            string str = info.GetString("specialThingFilterDefName");


            foreach (SpecialThingFilterDef def in DefDatabase<SpecialThingFilterDef>.AllDefs)
            {
                if (str == def.defName)
                {
                    return def;
                }
            }

            return null;
        }
    }

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
}

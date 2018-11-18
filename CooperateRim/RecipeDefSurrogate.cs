using System.Linq;
using System.Runtime.Serialization;
using Verse;

namespace CooperateRim
{
    public class RecipeDefSurrogate : ISerializationSurrogate
    {
        public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
        {
            RecipeDef rdef = (RecipeDef)obj;
            info.AddValue("defName", rdef.defName);
        }

        public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
        {
            string defName = info.GetString("defName");
            return DefDatabase<RecipeDef>.AllDefsListForReading.First(u=>u.defName == defName);
        }
    }
}

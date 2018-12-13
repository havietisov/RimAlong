using Verse;
using System.Runtime.Serialization;
using System.Linq;

public class DesignationDefSurrogate : ISerializationSurrogate
{
    public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
    {
        info.AddValue("name", ((DesignationDef)obj).defName);
    }

    public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
    {
        string defname = info.GetString("name");
        return DefDatabase<DesignationDef>.AllDefs.First(u=> u.defName == defname);
    }
}

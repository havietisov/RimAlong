using Verse;
using System.Runtime.Serialization;

public class Designation_surrogate : ISerializationSurrogate
{
    public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
    {
        Designation des = (Designation)obj;
        info.AddValue("target", des.target);
        info.AddValue("def", des.def);
    }

    public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
    {
        DesignationDef def = (DesignationDef)info.GetValue("def", typeof(DesignationDef));
        return new Designation((LocalTargetInfo)info.GetValue("target", typeof(LocalTargetInfo)), def) { designationManager = Find.CurrentMap.designationManager };
    }
}

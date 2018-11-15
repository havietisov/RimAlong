using System;
using System.Runtime.Serialization;
using Verse;

namespace CooperateRim
{
    public class FloatMenuOptionSurrogate : ISerializationSurrogate
    {
        public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
        {
            FloatMenuOption opt = (FloatMenuOption)obj;
            info.AddValue("opt.Label", opt.Label);
            info.AddValue("opt.Action", opt.action);
        }

        public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
        {
            FloatMenuOption opt = new FloatMenuOption(info.GetString("opt.Label"), (Action)info.GetValue("opt.Action", typeof(Action)));
            return opt;
        }
    }
}

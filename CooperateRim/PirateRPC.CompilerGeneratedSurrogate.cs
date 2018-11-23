using System.Reflection;
using System.Runtime.Serialization;
using CooperateRim;

namespace PirateRPC
{
    public partial class PirateRPC
    {
        public class CompilerGeneratedSurrogate : ISerializationSurrogate
        {
            public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
            {
                int id = 0;
                foreach (FieldInfo fi in obj.GetType().GetFields())
                {
                    CooperateRim.Utilities.RimLog.Message("-Surrogator : " + fi.Name + "|" + fi.GetValue(obj));
                    SerializationService.GetSurrogateFor(fi.FieldType).GetObjectData(fi.GetValue(obj), info, context);
                    //sinfo.AddValue((id++) + "_" + fi.Name, fi.GetValue(obj));
                }

                foreach (FieldInfo fi in obj.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance))
                {
                    CooperateRim.Utilities.RimLog.Message("--Surrogator : " + fi.Name + "|" + fi.GetValue(obj));
                }

                foreach (FieldInfo fi in obj.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance))
                {
                    CooperateRim.Utilities.RimLog.Message("--Surrogator : " + fi.Name + "|" + fi.GetValue(obj));
                    SerializationService.GetSurrogateFor(fi.FieldType).GetObjectData(fi.GetValue(obj), info, context);
                    //info.AddValue((id++) + "_" + fi.Name, fi.GetValue(obj), fi.FieldType);
                }
            }

            public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
            {
                int id = 0;
                foreach (FieldInfo fi in obj.GetType().GetFields())
                {
                    CooperateRim.Utilities.RimLog.Message("+Surrogator : " + fi.Name + "|" + fi.FieldType);
                    object o = SerializationService.GetSurrogateFor(fi.FieldType).SetObjectData(null, info, context, selector);
                    CooperateRim.Utilities.RimLog.Message("+Surrogator : " + o);
                    fi.SetValue(obj, o);
                    //fi.SetValue(obj, info.GetValue((id++) + "_" + fi.Name, fi.FieldType));
                }

                foreach (FieldInfo fi in obj.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance))
                {
                    CooperateRim.Utilities.RimLog.Message("++Surrogator : " + fi.Name);
                    object o = SerializationService.GetSurrogateFor(fi.FieldType).SetObjectData(null, info, context, selector);
                    CooperateRim.Utilities.RimLog.Message("+++Surrogator : " + o);
                    fi.SetValue(obj, o);
                }

                return obj;
            }
        }
    }
}
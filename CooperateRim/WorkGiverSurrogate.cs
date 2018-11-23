using RimWorld;
using System.Runtime.Serialization;
using Verse;

namespace CooperateRim
{
    public class WorkGiverSurrogate : ISerializationSurrogate
    {
        public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
        {
            WorkGiver wg = (WorkGiver)obj;
            info.AddValue("workgiverdef", wg.def);
        }

        public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
        {
            WorkGiverDef wgd = (WorkGiverDef)info.GetValue("workgiverdef", typeof(WorkGiverDef));

            foreach (WorkTypeDef workTypeDef in DefDatabase<WorkTypeDef>.AllDefsListForReading)
            {
                for (int i = 0; i < workTypeDef.workGiversByPriority.Count; i++)
                {
                    WorkGiverDef workGiver = workTypeDef.workGiversByPriority[i];

                    if (workGiver == wgd)
                    {
                        return workGiver.Worker;
                    }
                }
            }

            Utilities.RimLog.Message("Could not locate workgiver by it's workgiverdef");
            return null;
        }
    }
}

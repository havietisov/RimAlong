using System.Runtime.Serialization;
using Verse;
using Verse.AI;

namespace CooperateRim
{
    public class JobSurrogate : ISerializationSurrogate
    {
        public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
        {
            Job j = (Job)obj;
            //new SyncTickData.TemporaryJobData() { pawn = pawn, targetThing = t, forced = forced, __result = __result, jobTargetA = __result != null ? __result.targetA : null, jobTargetB = __result != null ? __result.targetB : null, jobTargetC = __result != null ? __result.targetC : null }
            info.AddValue(nameof(Job.targetA), j.targetA);
            info.AddValue(nameof(Job.targetB), j.targetB);
            info.AddValue(nameof(Job.targetC), j.targetC);
            info.AddValue(nameof(Job.playerForced), j.playerForced);
            info.AddValue(nameof(Job.def), j.def);
            info.AddValue(nameof(Job.count), j.count);
        }

        public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
        {
            Job j = new Job
            (
                (JobDef)(info.GetValue(nameof(Job.def), typeof(JobDef)))
                , (LocalTargetInfo)info.GetValue(nameof(Job.targetA), typeof(LocalTargetInfo))
                , (LocalTargetInfo)info.GetValue(nameof(Job.targetB), typeof(LocalTargetInfo))
                , (LocalTargetInfo)info.GetValue(nameof(Job.targetC), typeof(LocalTargetInfo))
            );
            j.count = info.GetInt32(nameof(Job.count));
            j.playerForced = info.GetBoolean(nameof(Job.playerForced));
            return j;
        }
    }
}

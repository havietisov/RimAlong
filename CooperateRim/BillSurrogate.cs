using Harmony;
using RimWorld;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Verse;

namespace CooperateRim
{

    public class BillSurrogate : ISerializationSurrogate
    {
        public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
        {
            Bill b = (Bill)obj;
            BillStack st = b.billStack;
            info.AddValue("bill_stack", st);
            info.AddValue("recipedef", b.recipe.defName);
        }

        public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
        {
            string recipeDefName = info.GetString("recipedef");
            BillStack st = (BillStack)info.GetValue("bill_stack", typeof(BillStack));
            Utilities.RimLog.Message("bill lacks pawn restriction in surrogate!");
            Utilities.RimLog.Message("billstack is null ? " + (st == null ? "yes" : "no"));
            Utilities.RimLog.Message("bill giver as thing : " + (st.billGiver as Thing));
            Utilities.RimLog.Message("bill giver def as thing : " + (st.billGiver as Thing).def);

            foreach (var rec in (st.billGiver as Thing).def.recipes)
            {
                if (rec.defName == recipeDefName)
                {
                    return BillUtility.MakeNewBill(rec);
                }
            }

            Utilities.RimLog.Message("could not make bill!");
            return null;
        }
    }
}

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
            CooperateRimming.Log("bill lacks pawn restriction in surrogate!");
            CooperateRimming.Log("billstack is null ? " + (st == null ? "yes" : "no"));
            CooperateRimming.Log("bill giver as thing : " + (st.billGiver as Thing));
            CooperateRimming.Log("bill giver def as thing : " + (st.billGiver as Thing).def);

            foreach (var rec in (st.billGiver as Thing).def.recipes)
            {
                if (rec.defName == recipeDefName)
                {
                    return BillUtility.MakeNewBill(rec);
                }
            }

            CooperateRimming.Log("could not make bill!");
            return null;
        }
    }
}

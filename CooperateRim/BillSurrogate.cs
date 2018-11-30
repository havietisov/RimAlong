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
        public virtual void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
        {
            Bill b = (Bill)obj;
            BillStack st = b.billStack;
            info.AddValue("bill_stack", st);
            info.AddValue("recipedef", b.recipe.defName);
        }

        public virtual object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
        {
            int stage = 0;
            int iter = -1;
            try
            {
                string recipeDefName = info.GetString("recipedef");
                BillStack st = (BillStack)info.GetValue("bill_stack", typeof(BillStack));
                stage = 1;
                Utilities.RimLog.Message("bill lacks pawn restriction in surrogate!");
                stage = 2;
                Utilities.RimLog.Message("billstack is null ? " + (st == null ? "yes" : "no"));
                stage = 3;
                Utilities.RimLog.Message("bill giver as thing : " + (st.billGiver as Thing));
                stage = 4;
                Utilities.RimLog.Message("bill giver def as thing : " + (st.billGiver as Thing).def);
                stage = 5;
                iter++;
                Utilities.RimLog.Message("bill giver def recipes : " + (st.billGiver as Thing).def.AllRecipes);
                foreach (var rec in (st.billGiver as Thing).def.AllRecipes)
                {
                    Utilities.RimLog.Message("rec is " + (rec == null || rec.defName == null ? "<null>" : rec.ToString()));
                    if (rec.defName == recipeDefName)
                    {
                        iter++;
                        return BillUtility.MakeNewBill(rec);
                    }
                }
            }
            catch (System.Exception ee)
            {
                Utilities.RimLog.Error("stage " + stage + " at iter " + iter +  "could not make bill! " + ee.ToString());
            }
            return null;
        }
    }
}

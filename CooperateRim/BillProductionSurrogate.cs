using CooperateRim.Utilities;
using RimWorld;
using System.Runtime.Serialization;
using Verse;

namespace CooperateRim
{
    public class BillProductionSurrogate : ISerializationSurrogate
    {
        public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
        {
            Bill_Production bp = (Bill_Production)obj;
            info.AddValue("recipedef", bp.recipe.defName);
            info.AddValue("loadID", bp.GetUniqueLoadID());
        }

        public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
        {
            /*
            ISurrogateSelector _selector;
            ISerializationSurrogate billmaker = selector.GetSurrogate(typeof(Bill), context, out _selector);
            Bill b = (Bill)billmaker.SetObjectData(obj, info, context, _selector);
            return (Bill_Production)b;*/
            string recipeDefName = info.GetString("recipedef");
            string loadID = info.GetString("loadID");

            foreach (var b in BillUtility.GlobalBills())
            {
                if (b.GetUniqueLoadID() == loadID)
                {
                    return b;
                }
            }

            foreach (var rec in DefDatabase<RecipeDef>.AllDefsListForReading)
            {
                if (rec.defName == recipeDefName)
                {
                    RimLog.Message("bill production restored via recipedef!");
                    Bill b = BillUtility.MakeNewBill(rec);
                    return b;
                }
            }

            RimLog.Message("could not make bill_production!");
            return null;
        }
    }
}

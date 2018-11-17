using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using RimWorld;
using Verse;

namespace CooperateRim
{
    public class FoodRestrictionSurrogate : ISerializationSurrogate
    {
        public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
        {
            FoodRestriction fs = (FoodRestriction)obj;
            info.AddValue("food_restr_id", fs.id);
        }

        public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
        {
            int oID = info.GetInt32("food_restr_id");
            return Current.Game.foodRestrictionDatabase.AllFoodRestrictions.First(u => u.id == oID);
        }
    }

    public class OutfitSerializationSurrogate : ISerializationSurrogate
    {
        public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
        {
            Outfit o = (Outfit)obj;
            //
            info.AddValue("outfit_id", o.uniqueId);
        }

        public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
        {
            int oID = info.GetInt32("outfit_id");
            return Current.Game.outfitDatabase.AllOutfits.First(u=> u.uniqueId == oID);
        }
    }
}

using RimWorld;
using Verse;


namespace CooperateRim
{
    public class thingfilter_methods
    {
        public static bool avoidInternalLoop = false;

        public static void SetAllowAllFor(object o)
        {
            avoidInternalLoop = true;

            if (o is Zone_Stockpile)
            {
                (o as Zone_Stockpile).settings.filter.SetAllowAll(null);
            }

            if (o is Outfit)
            {
                (o as Outfit).filter.SetAllowAll(null);
            }

            if (o is FoodRestriction)
            {
                (o as FoodRestriction).filter.SetAllowAll(null);
            }

            if (o is Bill)
            {
                (o as Bill).ingredientFilter.SetAllowAll(null);
            }

            //Utilities.RimLog.Message("Setallowall for " + o);
            avoidInternalLoop = false;
        }

        public static void SetDisallowAllFor(object o)
        {
            avoidInternalLoop = true;

            if (o is Zone_Stockpile)
            {
                (o as Zone_Stockpile).settings.filter.SetDisallowAll();
            }

            if (o is Outfit)
            {
                (o as Outfit).filter.SetDisallowAll();
            }

            if (o is FoodRestriction)
            {
                (o as FoodRestriction).filter.SetDisallowAll();
            }

            if (o is Bill)
            {
                (o as Bill).ingredientFilter.SetDisallowAll();
            }

            //Utilities.RimLog.Message("Setdisallowall for " + o);
            avoidInternalLoop = false;
        }

        public static void SetAllowance(object o, ThingDef def, bool isSpecial, bool isAllow)
        {
            avoidInternalLoop = true;

            if (o is Zone_Stockpile)
            {
                (o as Zone_Stockpile).settings.filter.SetAllow(def, isAllow);
            }

            if (o is Outfit)
            {
                (o as Outfit).filter.SetAllow(def, isAllow);
            }

            if (o is FoodRestriction)
            {
                (o as FoodRestriction).filter.SetAllow(def, isAllow);
            }

            if (o is Bill)
            {
                (o as Bill).ingredientFilter.SetAllow(def, isAllow);
            }

           // Utilities.RimLog.Message("SetAllowance for " + o + "::" + def + "::" + isAllow);
            avoidInternalLoop = false;
        }

        public static void SetAllowance(object o, SpecialThingFilterDef def, bool isSpecial, bool isAllow)
        {
            avoidInternalLoop = true;

            if (o is Zone_Stockpile)
            {
                (o as Zone_Stockpile).settings.filter.SetAllow(def, isAllow);
            }

            if (o is Outfit)
            {
                (o as Outfit).filter.SetAllow(def, isAllow);
            }

            if (o is FoodRestriction)
            {
                (o as FoodRestriction).filter.SetAllow(def, isAllow);
            }

            if (o is Bill)
            {
                (o as Bill).ingredientFilter.SetAllow(def, isAllow);
            }

           // Utilities.RimLog.Message("SetAllowance for " + o + "::" + def + "::" + isAllow);
            avoidInternalLoop = false;
        }
    }
}

using System.Collections.Generic;
using Verse;
using CooperateRim.Utilities;
using CooperateRim;
using System.Reflection;

public class designator_methods : common_patch_fields
{
    public static void parrot_designate_multiple_cell(IEnumerable<IntVec3> cells, System.Type iType)
    {
        use_native = true;
        try
        {
            MethodInfo mi = ExpressionHelper.GetMethodInfo(()=> DesignatorUtility.FindAllowedDesignator<Designator>());
            MethodInfo mii = mi.GetGenericMethodDefinition().MakeGenericMethod(iType);
            Designator d = (Designator)mii.Invoke(null, new object[] { });
            d.DesignateMultiCell(cells);
        }
        finally
        {
            use_native = false;
        }
    }
    
    public static bool RegularDesignateMultipleCells(IEnumerable<IntVec3> cells, Designator __instance)
    {
        if (use_native)
        {
            return true;
        }
        else
        {
            parrot_designate_multiple_cell(cells, __instance.GetType());
            return false;
        }
    }

    public static void parrot_designate_thing(Thing t, System.Type iType)
    {
        use_native = true;
        try
        {
            MethodInfo mi = ExpressionHelper.GetMethodInfo(() => DesignatorUtility.FindAllowedDesignator<Designator>());
            MethodInfo mii = mi.GetGenericMethodDefinition().MakeGenericMethod(iType);
            Designator d = (Designator)mii.Invoke(null, new object[] { });
            d.DesignateThing(t);
        }
        finally
        {
            use_native = false;
        }
    }

    public static bool RegularDesignateThing(Thing t, Designator __instance)
    {
        if (use_native)
        {
            return true;
        }
        else
        {
            parrot_designate_thing(t, __instance.GetType());
            return false;
        }
    }
    
    public static void designation_mgr_parrot_RemoveAllDesignationsOn(Thing t, bool standardCanceling)
    {
        use_native = true;
        try
        {
            Find.CurrentMap.designationManager.RemoveAllDesignationsOn(t, standardCanceling);
        }
        finally
        {
            use_native = false;
        }
    }

    public static void designation_mgr_parrot_RemoveDesignation(Designation des)
    {
        use_native = true;
        try
        {
            Find.CurrentMap.designationManager.RemoveDesignation(des);
        }
        finally
        {
            use_native = false;
        }
    }

    public static bool designation_mgr_regular_RemoveDesignation(Designation des)
    {
        if (use_native)
        {
            return true;
        }
        else
        {
            designation_mgr_parrot_RemoveDesignation(des);
            return false;
        }
    }

    public static bool designation_mgr_regular_RemoveAllDesignationsOn(Thing t, bool standardCanceling)
    {
        if (use_native)
        {
            return true;
        }
        else
        {
            designation_mgr_parrot_RemoveAllDesignationsOn(t, standardCanceling);
            return false;
        }
    }

    public static void designation_mgr_parrot_AddDesignation(Designation newDes)
    {
        use_native = true;
        try
        {
            Find.CurrentMap.designationManager.AddDesignation(newDes);
        }
        finally
        {
            use_native = false;
        }
    }

    public static bool designation_mgr_regular_AddDesignation(Designation newDes)
    {
        if (use_native)
        {
            return true;
        }
        else
        {
            designation_mgr_parrot_AddDesignation(newDes);
            return false;
        }
    }
}

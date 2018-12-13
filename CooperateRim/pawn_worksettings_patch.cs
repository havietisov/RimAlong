using Verse;

public class pawn_worksettings_patch : common_patch_fields
{
    public static void parrot_set_priority(WorkTypeDef w, int priority, Pawn p)
    {
        use_native = true;
        try
        {
            p.workSettings.SetPriority(w, priority);
        }
        finally
        {
            use_native = false;
        }
    }

    public static bool prefix_set_priority(WorkTypeDef w, int priority, Pawn ___pawn)
    {
        if (use_native)
        {
            return true;
        }
        else
        {
            parrot_set_priority(w, priority, ___pawn);
            return false;
        }
    }
}

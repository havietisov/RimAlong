public class common_patch_fields
{
    private static int counter;

    protected static bool use_native
    {
        get
        {
            return counter > 0;
        }
        set
        {
            counter += value ? 1 : -1;
        }
    }
}

using CooperateRim;
using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Verse;

[HarmonyPatch(typeof(Verse.Root))]
[HarmonyPatch("Update")]
public partial class root_play_upd
{
    public static ulong ui_state;
    public static ulong game_state;
    public static ulong portraits_state;
    public static ulong atk_targets_state;
    public static ulong audio_state;
    public static ulong story_state;
    public static ulong prefs_state;
    public static ulong realtime_state;

    public static ulong map_state;
    public static ulong tick_manager_state;

    public static string scopename;
}

public class CRand
{
    static RandomNumberGenerator_BasicHash saved_rng = new RandomNumberGenerator_BasicHash();
    static MethodInfo get_comp_state = typeof(Rand).GetMethod("get_StateCompressed", BindingFlags.Static | BindingFlags.NonPublic);
    static MethodInfo set_comp_state = typeof(Rand).GetMethod("set_StateCompressed", BindingFlags.Static | BindingFlags.NonPublic);

    public static ulong get_state()
    {
        return (ulong)get_comp_state.Invoke(null, new object[] { });
    }

    public static void set_state(ulong state)
    {
        set_comp_state.Invoke(null, new object[] { state });
    }
}
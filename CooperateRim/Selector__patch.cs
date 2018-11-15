using Harmony;
using RimWorld;

namespace CooperateRim
{
    /*
     * some SPOOKY shit about geysers
     > CooperateRim.getValuePatch::get_Value
> Verse.Rand::get_Value_Patch1
> Verse.Rand::Range
> Verse.Mote::.ctor
> Verse.MoteThrown::.ctor
> System.Reflection.MonoCMethod::InternalInvoke
> System.Reflection.MonoCMethod::Invoke
> System.Reflection.MonoCMethod::Invoke
> System.Reflection.ConstructorInfo::Invoke
> System.Activator::CreateInstance
> System.Activator::CreateInstance
> Verse.ThingMaker::MakeThing
> RimWorld.MoteMaker::NewBaseAirPuff
> RimWorld.MoteMaker::ThrowAirPuffUp
> RimWorld.IntermittentSteamSprayer::SteamSprayerTick
> RimWorld.Building_SteamGeyser::Tick
> Verse.TickList::Tick_Patch1
> Verse.TickManager::DoSingleTick
> CooperateRim.TickManagerPatch::Prefix
> Verse.TickManager::TickManagerUpdate_Patch2
> Verse.Game::UpdatePlay
> Verse.Root_Play::Update
         */
    /*
[HarmonyPatch(typeof(SoundStarter), "PlayOneShotOnCamera")]
public class SoundStarterPatchPlayOneShotOnCamera
{
   [HarmonyPrefix]
   public static bool Prefix()
   {
       getValuePatch.GuardedPush();
       return false;
   }

   [HarmonyPostfix]
   public static void postfix()
   {
       getValuePatch.GuardedPop();
   }
}*/

    /*
    [HarmonyPatch(typeof(SoundStarter), "PlayOneShot")]
    public class SoundStarterPatchPlayOneShot
    {
        [HarmonyPrefix]
        public static bool Prefix()
        {
            getValuePatch.GuardedPush();
            return false;
        }

        [HarmonyPostfix]
        public static void postfix()
        {
            getValuePatch.GuardedPop();
        }
    }*/

    /*
    [HarmonyPatch(typeof(SustainerManager))]
    [HarmonyPatch("SustainerManagerUpdate")]
    public class SustainerManagerUpdate_patch
    {
        [HarmonyPrefix]
        public static bool Prefix()
        {
            getValuePatch.GuardedPush();
            return false;
        }

        [HarmonyPostfix]
        public static void postfix()
        {
            getValuePatch.GuardedPop();
        }
    }*/

    [HarmonyPatch(typeof(Selector), "Select")]
    public class Selector__patch
    {
        [HarmonyPrefix]
        public static void Prefix(ref bool playSound)
        {
            //playSound = false;
            //getValuePatch.GuardedPush();
        }

        [HarmonyPostfix]
        public static void postfix()
        {
            //getValuePatch.GuardedPop();
        }
    }
}

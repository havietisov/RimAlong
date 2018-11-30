using Harmony;
using RimWorld;
using System.Collections.Generic;
using System.Reflection;
using Verse;
using System;
using UnityEngine;
using HugsLib;
using RimWorld.Planet;
using Verse.AI;
using Verse.Sound;
using CooperateRim.Utilities;
using System.Diagnostics;

namespace CooperateRim
{

    public partial class CooperateRimming : ModBase
    {
        public override string ModIdentifier => "CooperateRim.CooperateRimming";
        public static List<Pawn> initialPawnList = new List<Pawn>();
        public static bool dumpRand = false;
        public static CooperateRimming inst;

        public CooperateRimming ()
        {
            RimLog.Init(this.Logger);
        }

        public HarmonyInstance harmonyInst
        {
            get
            {
                return base.HarmonyInst;
            }
        }
        
        public static void GenerateWorld()
        {
            ThingIDMakerPatch.stopID = true;
            
            ThinkTreeKeyAssigner.Reset();

            foreach (var def in DefDatabase<ThinkTreeDef>.AllDefsListForReading)
            {
                ThinkTreeKeyAssigner.AssignKeys(def.thinkRoot, 0);
            }


            if (SyncTickData.cliendID == 0)
            {
                Find.WindowStack.Add(new Dialog_SaveFileList_Load());
            }
            else
            {
                LongEventHandler.QueueLongEvent(() => 
                {
                    NetDemo.LoadFromRemoteSFD();
                    for (; NetDemo.GetSFD() == null ;)
                    {

                    }
                }, "Downloading savefile".Translate(), true, e => { RimLog.Error(e.ToString()); });
            }
        }

        [HarmonyPatch(typeof(Dialog_SaveFileList_Load), "DoFileInteraction")]
        public class Dialog_SaveFileList_Load_patch//actually, server should do checks about this
        {
            [HarmonyPrefix]
            public static bool Prefix(string saveFileName)
            {
                string fileName = saveFileName;
                string fileContent = System.IO.File.ReadAllText(GenFilePaths.FilePathForSavedGame(saveFileName));

                if (SyncTickData.cliendID == 0)
                {
                    PirateRPC.PirateRPC.SendInvocation(NetDemo.ns, u => 
                    {
                        NetDemo.SetSFD(new NetDemo.SaveFileData() { tcontext = fileContent, partial_name = fileName + "_received" });

                        PirateRPC.PirateRPC.SendInvocation(u, uu => { NetDemo.LoadFromRemoteSFD(); });
                    });

                    LongEventHandler.QueueLongEvent(() =>
                    {
                        for (; NetDemo.GetSFD() == null;)
                        {

                        }
                    }, "Downloading savefile".Translate(), true, e => { RimLog.Error(e.ToString()); });
                }

                return false;
            }
        }

        [HarmonyPatch(typeof(WorldPawns), "AddPawn")]
        public class wpPatch
        {
            [HarmonyAfter]
            public static void postfix(Pawn p)
            {
                StackTrace st = new StackTrace();

                RimLog.Message("==========");
                foreach (var a in st.GetFrames())
                {
                    RimLog.Message(a.GetMethod().Name + "::" + a.GetMethod().DeclaringType);
                }

                RimLog.Message("pawn added to world : " + p);
                RimLog.Message("///==========");
            }
        }

        [HarmonyPatch(typeof(Dialog_ManageOutfits), MethodType.Constructor, new Type[] { typeof(Outfit) })]
        public class Dialog_ManageOutfits_patch
        {
            [HarmonyPrefix]
            public static void Prefix()
            {
                ThingFilterPatch.avoidThingFilterUsage = true;
            }

            [HarmonyPostfix]
            public static void Postfix()
            {
                ThingFilterPatch.avoidThingFilterUsage = false;
            }
        }

        [HarmonyPatch(typeof(Dialog_ManageFoodRestrictions), MethodType.Constructor, new Type[] { typeof(FoodRestriction) })]
        public class Dialog_ManageFoodRestrictions_patch
        {
            [HarmonyPrefix]
            public static void Prefix()
            {
                ThingFilterPatch.avoidThingFilterUsage = true;
            }

            [HarmonyPostfix]
            public static void Postfix()
            {
                ThingFilterPatch.avoidThingFilterUsage = false;
            }
        }

        static void InitBullshit()
        {
            /*
            SerializationService.Initialize();
            ParrotWrapper.Initialize();
            
            CooperateRim.ParrotWrapper.ParrotPatchExpressiontarget<Action<tester, int, string>>((__instance, ___internal_value, name_something) => __instance.DoSomething(name_something));
            tester t = new tester();
            t.DoSomething("1");
            SyncTickData.AvoidLoop = true;
            byte[] b;
            var _dat = SerializationService.DeserializeFrom(b = SerializationService.Flush());

            foreach (var dat in _dat)
            {
                if (dat.methodContext > -1)
                {
                    ParrotWrapper.IndexedCall(dat.methodContext, dat.dataObjects.ToArray());
                }
            }*/

            Prefs.PauseOnLoad = false;
            Prefs.RunInBackground = true;
            SerializationService.Initialize();
            ParrotWrapper.Initialize();

            SerializationService.AppendSurrogate(typeof(Vector3), new Vector3Surrogate());
            SerializationService.AppendSurrogate(typeof(IntVec3), new IntVec3Surrogate());
            SerializationService.AppendSurrogate(typeof(BillStack), new BillStackSurrogate());
            SerializationService.AppendSurrogate(typeof(Bill), new BillSurrogate());

            foreach (Assembly @as in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (Type t in @as.GetTypes())
                {
                    if (t.IsSubclassOf(typeof(Thing)))
                    {
                        SerializationService.AppendSurrogate(t, new ThingSurrogate());
                    }
                }
            }
            
            SerializationService.AppendSurrogate(typeof(Bill_Production), new BillProductionSurrogate());
            SerializationService.AppendSurrogate(typeof(JobDef), new JobDefSurrogate());
            SerializationService.AppendSurrogate(typeof(ThingDef), new ThingDefSurrogate());
            SerializationService.AppendSurrogate(typeof(SpecialThingFilterDef), new SpecialThingFilterDefSurrogate());
            SerializationService.AppendSurrogate(typeof(LocalTargetInfo), new LocalTargetInfoSurrogate());
            SerializationService.AppendSurrogate(typeof(Job), new JobSurrogate());
            SerializationService.AppendSurrogate(typeof(WorkGiver), new WorkGiverSurrogate());
            SerializationService.AppendSurrogate(typeof(WorkGiver_Scanner), new WorkGiverSurrogate());
            SerializationService.AppendSurrogate(typeof(WorkGiverDef), new WorkGiverDefSurrogate());
            SerializationService.AppendSurrogate(typeof(FloatMenuOption), new FloatMenuOptionSurrogate());
            SerializationService.AppendSurrogate(typeof(Zone_Stockpile), new Zone_StockPileSurrogate());
            SerializationService.AppendSurrogate(typeof(Outfit), new OutfitSerializationSurrogate());
            SerializationService.AppendSurrogate(typeof(FoodRestriction), new FoodRestrictionSurrogate());
            SerializationService.AppendSurrogate(typeof(RecipeDef), new RecipeDefSurrogate());
            SerializationService.AppendSurrogate(typeof(BillRepeatModeDef), new BillRepeatModeDefSurrogate());

            PirateRPC.PirateRPC.CompilerGeneratedSurrogate sur = new PirateRPC.PirateRPC.CompilerGeneratedSurrogate();
            
            MainTabWindow_Work_patch.useWorkPriorities_index = 
            MemberTracker<bool>.TrackPublicField<Func<bool>>(() => Current.Game.playSettings.useWorkPriorities, 
            u => 
            {
                Current.Game.playSettings.useWorkPriorities = u;
                
                foreach (Pawn pawn in PawnsFinder.AllMapsWorldAndTemporary_Alive)
                {
                    if (pawn.Faction == Faction.OfPlayer && pawn.workSettings != null)
                    {
                        pawn.workSettings.Notify_UseWorkPrioritiesChanged();
                    }
                }
            });
            ParrotWrapper.ParrotPatchExpressiontarget<Action<bool, int>>((bool newVal, int index) => MemberTracker<bool>.ApplyChange(newVal, index));

            //MemberTracker<bool>.ApplyChange(true, 0);
            //ParrotWrapper.ParrotPatchExpressiontarget<Action<BillStack, Bill>>((BillStack __instance, Bill bill) => __instance.AddBill(bill));

            //ParrotWrapper.ParrotPatchExpressiontarget<Action<string, bool, int, bool>>((string thingDefName, bool allow, int thingIDNumber, bool isSpecial) => ThingFilter_wrapper.Thingfilter_setallow_wrap(thingDefName, allow, thingIDNumber, isSpecial));
            //ParrotWrapper.ParrotPatchExpressiontarget<Action<string, bool, int, bool>>((string thingDefName, bool allow, int zoneID, bool isSpecial) => ThingFilter_wrapper.Thingfilter_setallowzone_wrap(thingDefName, allow, zoneID, isSpecial));
            //ThingFilter_setallow_bill_with_billgiver(string thingDefName, bool allow, int thingIDNumber, bool isSpecial, int billIndex)

            //ParrotWrapper.ParrotPatchExpressiontarget<thing_filter_wrapper_1>((string thingDefName, bool allow, int thingIDNumber, bool isSpecial, int billIndex) => ThingFilter_wrapper.ThingFilter_setallow_bill_with_billgiver(thingDefName, allow, thingIDNumber, isSpecial, billIndex));
            //ParrotWrapper.ParrotPatchExpressiontarget<Action<int, bool>>((int thingIDNumber, bool actuallyDisallow) => ThingFilter_setallowall_wrapper.ThingFilter_setallowall_zone(thingIDNumber, actuallyDisallow));
            //ParrotWrapper.ParrotPatchExpressiontarget<Action<Action>>((Action a) => FloatMenuOptionPatch.InvokeAction(a));

            ParrotWrapper.ParrotPatchExpressiontarget<Action<Bill_Production, int>>((Bill_Production bill, int count) => BillRepeatModeUtilityPatch.SetBillTargetCount(bill, count));
            ParrotWrapper.ParrotPatchExpressiontarget<Action<Bill_Production, int>>((Bill_Production bill, int count) => BillRepeatModeUtilityPatch.SetBillRepeatCount(bill, count));
            ParrotWrapper.ParrotPatchExpressiontarget<Action<Bill_Production, BillRepeatModeDef>>((Bill_Production bill, BillRepeatModeDef repeatMode) => BillRepeatModeUtilityPatch.SetBillRepeatType(bill, repeatMode));
            ParrotWrapper.ParrotPatchExpressiontarget<Action<BillStack, int>>((BillStack stack, int index) => bill_delete_patch.RemoveAt(stack, index));
            ParrotWrapper.ParrotPatchExpressiontarget<Action<BillStack, int, int>>((BillStack stack, int index, int offset) => bill_reorder_patch.ReorderAt(stack, index, offset));
            ParrotWrapper.ParrotPatchExpressiontarget<floatMenuMakerpatch.delUseIndexedFloatMenuEntryAt>((Vector3 clickPos, Pawn pawn, int index, int totalCount, string delegateName)=> floatMenuMakerpatch.UseIndexedFloatMenuEntryAt(clickPos, pawn, index, totalCount, delegateName));
            ParrotWrapper.ParrotPatchExpressiontarget<Action<object>>((object o) => thingfilter_methods.SetAllowAllFor(o));
            ParrotWrapper.ParrotPatchExpressiontarget<Action<object>>((object o) => thingfilter_methods.SetDisallowAllFor(o));
            ParrotWrapper.ParrotPatchExpressiontarget<Action<Building_WorkTable, RecipeDef>>((Building_WorkTable table, RecipeDef recipe) => BillStackPatch.MakeNewBillAt(table, recipe));
            ParrotWrapper.ParrotPatchExpressiontarget<Action<object, ThingDef, bool, bool>>((object o, ThingDef def, bool isSpecial, bool isAllow) => thingfilter_methods.SetAllowance(o, def, isSpecial, isAllow));
            ParrotWrapper.ParrotPatchExpressiontarget<Action<object, SpecialThingFilterDef, bool, bool>>((object o, SpecialThingFilterDef def, bool isSpecial, bool isAllow) => thingfilter_methods.SetAllowance(o, def, isSpecial, isAllow));
            

            //RandRootContext<Verse.Pawn>.ApplyPatch("Tick");
            RandRootContext<Verse.Sound.SoundRoot>.ApplyPatch("Update");
            RandRootContext<UnityEngine.GUI>.ApplyPatch("CallWindowDelegate");
            RandRootContext<MusicManagerPlay>.ApplyPatch("StartNewSong");
            RandRootContext<MusicManagerPlay_placeholder1>.ApplyPatch("MusicUpdate", typeof(MusicManagerPlay));
            RandRootContext<Verse.MapDrawer>.ApplyPatch("MapMeshDrawerUpdate_First");
            RandRootContext<TickManagerPatch>.ApplyPatch("Prefix");

            RandRootContext<Map>.ApplyPatch("MapUpdate");
            RandRootContext<mapPreTick_placeholder>.ApplyPatch("MapPreTick", typeof(Map));
            RandRootContext<mapPostTick_placeholder>.ApplyPatch("MapPostTick", typeof(Map));

            RandRootContext<TickManager>.ApplyPatch("DoSingleTick");
            RandRootContext<TickList>.ApplyPatch("Tick");
            RandRootContext<mapPreTick_placeholder>.ApplyPatch("MapPreTick", typeof(Map));
            RandRootContext<mapPostTick_placeholder>.ApplyPatch("MapPostTick", typeof(Map));
            RandRootContext<GameInfo>.ApplyPatch("GameInfoUpdate");
            RandRootContext<World>.ApplyPatch("WorldUpdate");
            RandRootContext<UIRoot_Play>.ApplyPatch("UIRootUpdate");

            //RandRootContext<Verse.Root>.ApplyPatch("OnGUI");

            //separating UpdatePlay calls from each other
            /*RandRootContext<TickManager>.ApplyPatch("DoSingleTick");
            RandRootContext<LetterStack>.ApplyPatch("LetterStackUpdate");
            RandRootContext<World>.ApplyPatch("WorldUpdate");
            RandRootContext<Map>.ApplyPatch("MapUpdate");
            RandRootContext<GameInfo>.ApplyPatch("GameInfoUpdate");
            RandRootContext<Need_Food>.ApplyPatch("get_MalnutritionSeverityPerInterval");
            RandRootContext<Verse.AI.Pawn_PathFollower>.ApplyPatch("PatherTick");
            RandRootContext<RimWorld.WildPlantSpawner>.ApplyPatch("WildPlantSpawnerTickInternal");
            RandRootContext<TickList>.ApplyPatch("Tick");
            RandRootContext<mapPreTick_placeholder>.ApplyPatch("MapPreTick", typeof(Map));
            RandRootContext<mapPostTick_placeholder>.ApplyPatch("MapPostTick", typeof(Map));
            RandRootContext<Verse.Pawn>.ApplyPatch("Tick");*/
            RandRootContext<SubEffecter_Sprayer>.ApplyPatch("MakeMote");
            RandRootContext<SubEffecter_DrifterEmote>.ApplyPatch("MakeMote");
            RandRootContext<SubEffecter_InteractSymbol>.ApplyPatch("SubEffectTick");

            RandRootContext<SubEffecter_ProgressBar>.ApplyPatch("SubEffectTick");
            RandRootContext<SubEffecter_SoundIntermittent>.ApplyPatch("SubEffectTick");
            RandRootContext<SubEffecter_SoundTriggered>.ApplyPatch("SubTrigger");
            RandRootContext<SubEffecter_Sustainer>.ApplyPatch("SubEffectTick");
            
            RandRootContext<SubEffecterDef>.ApplyPatch("Spawn");
            RandRootContext<effecter_effecttick_placeholder>.ApplyPatch("EffectTick", typeof(Effecter));
            RandRootContext<effecter_Trigger_placeholder>.ApplyPatch("Trigger", typeof(Effecter));
            RandRootContext<effecter_Cleanup_placeholder>.ApplyPatch("Cleanup", typeof(Effecter));
            

            RandRootContext<GameComponentUtility_placeholder>.ApplyPatch("GameComponentUpdate", typeof(GameComponentUtility));
            //finishing separating UpdatePlay calls from each other

            foreach (MethodInfo mi in typeof(MoteMaker).GetMethods())
            {
                if (!mi.IsConstructor && !mi.IsAbstract && mi.Name != "GetType")
                {
                    RandRootContext<MoteBullshit_placeholder>.ApplyPatch(mi, typeof(MoteMaker));
                }
            }

            //RandRootContext<Hediff>.ApplyPatch("Tick");
            //RandRootContext<LetterStack>.ApplyPatch("LetterStackUpdate");
            //RandRootContext<World>.ApplyPatch("WorldUpdate");
            //RandRootContext<Map>.ApplyPatch("MapUpdate");
            //RandRootContext<GameInfo>.ApplyPatch("GameInfoUpdate");
            //RandRootContext<Game>.ApplyPatch("LoadGame");
            RandRootContext<SustainerManager>.ApplyPatch("SustainerManagerUpdate");
            RandRootContext<SoundStarterPlaceholder>.ApplyPatch("PlayOneShot", typeof(SoundStarter));
            RandRootContext<SoundStarterPlaceholder2>.ApplyPatch("PlayOneShotOnCamera", typeof(SoundStarter));
            //RandRootContext<GamePlaceholder_1>.ApplyPatch("InitNewGame", typeof(Game));
            //RandRootContext<ThingOwner>.ApplyPatch("ThingOwnerTick");
        }
        /*
        [HarmonyPatch(typeof(Hediff), "Tick")]
        class hediff_tick_cancel
        {
            [HarmonyPrefix]
            public static bool prefix()
            {
                return false;
            }
        }*/

        class effecter_effecttick_placeholder { };
        class effecter_Trigger_placeholder { };
        class effecter_Cleanup_placeholder { };

        class mapPreTick_placeholder { };
        class mapPostTick_placeholder { };
        public class GameComponentUtility_placeholder { };
        public class MoteBullshit_placeholder { };
        class MusicManagerPlay_placeholder1 { };
        class GamePlaceholder_1 { }
        class SoundStarterPlaceholder { };
        class SoundStarterPlaceholder2 { };
        class WorldGenPlaceholder { };
        class InitNewGamePlaceholder { };

        
        public delegate void  thing_filter_wrapper_1(string thingDefName, bool allow, int thingIDNumber, bool isSpecial, int billIndex);

        public override void Initialize()
        {
            inst = this;
            NetDemo.setupCallbacks();
            (typeof(Rand).GetField("random", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null) as RandomNumberGenerator).seed = 0;

            HarmonyInstance harmony = HarmonyInst;
            RimLog.Message(System.Diagnostics.Process.GetCurrentProcess().StartInfo.Arguments);
            List<Type> typesToPatch = new List<Type>();
            List<Type> designatorInheritees = new List<Type>();
            List<Type> leftOverTypes = new List<Type>();

            InitBullshit();
            


            foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (Type t in a.GetTypes())
                {
                    if (!t.IsAbstract)
                    {
                        if (t.IsSubclassOf(typeof(RimWorld.WorkGiver_Scanner)))
                        {
                            typesToPatch.Add(t);
                            leftOverTypes.Add(t);
                        }
                    }
                }
            }

            foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (Type t in a.GetTypes())
                {
                    if (!t.IsAbstract)
                    {
                        if (t.IsSubclassOf(typeof(Thing)))
                        {
                            MethodInfo targetmethod = AccessTools.Method(t, "GetGizmos");
                            HarmonyMethod postfix = new HarmonyMethod(typeof(CooperateRim.GenericGizmoPatch).GetMethod("Postfix"));
                            harmony.Patch(targetmethod, null, postfix, null);
                        }
                    }
                }
            }

            List<Type> doNotPatchDesignators = new List<Type>(new [] { typeof(Designator_ZoneDelete), typeof(Designator_Build), typeof(Designator) });

            foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (Type t in a.GetTypes())
                {
                    if (!t.IsAbstract && t.IsSubclassOf(typeof(Designator)) && !doNotPatchDesignators.Contains(t))
                    {
                        MethodInfo targetmethod = AccessTools.Method(t, "DesignateSingleCell");
                        HarmonyMethod prefix_1 = new HarmonyMethod(typeof(CooperateRim.Designator_patch).GetMethod("DesignateSingleCell_1"));
                        HarmonyMethod prefix_2 = new HarmonyMethod(typeof(CooperateRim.Designator_patch).GetMethod("DesignateSingleCell_2"));
                        try
                        {
                            harmony.Patch(targetmethod, prefix_1, null, null);
                        }
                        catch (Exception ee)
                        {
                            harmony.Patch(targetmethod, prefix_2, null, null);
                        }
                    }
                }
            }

            foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (Type t in a.GetTypes())
                {
                    if (!t.IsAbstract && t.IsSubclassOf(typeof(Designator)) && !doNotPatchDesignators.Contains(t))
                    {
                        MethodInfo targetmethod = AccessTools.Method(t, "DesignateMultiCell");
                        HarmonyMethod prefix = new HarmonyMethod(typeof(CooperateRim.Designator_patch).GetMethod("DesignateMultiCell"));
                        harmony.Patch(targetmethod, prefix, null, null);
                    }
                }
            }

            foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (Type t in a.GetTypes())
                {
                    if (!t.IsAbstract && t.IsSubclassOf(typeof(Designator)) && !doNotPatchDesignators.Contains(t))
                    {
                        MethodInfo targetmethod = AccessTools.Method(t, "DesignateThing");
                        HarmonyMethod prefix = new HarmonyMethod(typeof(CooperateRim.Designator_patch).GetMethod("DesignateThing"));
                        harmony.Patch(targetmethod, prefix, null, null);
                    }
                }
            }
            
            Logger.Message("Field : " + (typeof(Building_Trap)).GetField("autoRearm", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.IgnoreCase));

            foreach (MethodInfo mi in new[] 
            {
                typeof(CooperateRim.WorkGiverPatch).GetMethod("JobOnThing_1"),
                typeof(CooperateRim.WorkGiverPatch).GetMethod("JobOnThing_2"),
            })
            {
                foreach (Type t in typesToPatch)
                {
                    try
                    {
                        MethodInfo targetmethod = AccessTools.Method(t, "JobOnThing");
                        HarmonyMethod postfix = new HarmonyMethod(mi);
                        //Logger.Message("Patched type : " + t);
                        harmony.Patch(targetmethod, null, postfix, null);
                        leftOverTypes.Remove(t);
                    }
                        catch (Exception ee)
                    {
                        //Logger.Message("Patching exception : " + ee.ToString());
                    }
                }
            }

            if (leftOverTypes.Count > 0)
            {
                foreach (Type t in leftOverTypes)
                {
                    Logger.Message("Unpatched job issuer : " + t);
                }
            }
            else
            {
                Logger.Message("All job issuers patched");
            }

            if (System.Diagnostics.Process.GetCurrentProcess().StartInfo.Arguments.Contains("network_launch"))
            {
                //harmony.PatchAll(Assembly.GetExecutingAssembly());
            }

        }
        
        [HarmonyPatch(typeof(MainMenuDrawer))]
        [HarmonyPatch("DoMainMenuControls")]
        [HarmonyBefore]
        [HarmonyPatch(new Type[] { typeof(Rect), typeof(bool) })]
        class Patch
        {
            static void Prefix(Rect rect, bool anyMapFiles)
            {
                rect.xMin -= 170f;
                rect.yMin += 52;
                GUI.BeginGroup(rect);
                Rect rect2 = new Rect(0f, 0f, 170f, rect.height);

                Rect rect3 = new Rect(rect2.xMax + 17f, 0f, 145f, rect.height);
                Rect rect4 = new Rect(0, 0, 170f, rect.height);

                Text.Font = GameFont.Small;
                List<ListableOption> list = new List<ListableOption>();
                string label = "Cooperative";
                list.Add(new ListableOption(label, CooperativeButtonPressed));
                OptionListingUtility.DrawOptionListing(rect2, list);
            }

            static void CooperativeButtonPressed()
            {
                CooperateRimming.inst.Logger.Trace("coop pressed");
                //CloseMainTab();
                Find.WindowStack.Add(new Dialog_Coop());
            }
        }

        [HarmonyPatch(typeof(MainMenuDrawer))]
        [HarmonyPatch("DoMainMenuControls")]
        [HarmonyAfter]
        [HarmonyPatch(new Type[] { typeof(Rect), typeof(bool) })]
        class Patch2
        {
            static void Prefix(Rect rect, bool anyMapFiles)
            {
                GUI.EndGroup();
            }
        }

        static string hostName = "localhost";

        static void IterateOverThinkNodes(IEnumerable<ThinkNode> nodes, string name)
        {
            List<string> s = new List<string>();

            foreach (var node in nodes)
            {
                s.Add(node.ToString() + "\r\n");
            }
        }
    }
}

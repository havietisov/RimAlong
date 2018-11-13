using Harmony;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Verse;
using System;
using UnityEngine;
using HugsLib;
using System.Runtime.InteropServices;
using RimWorld.Planet;
using Verse.AI;
using Verse.Sound;

namespace CooperateRim
{

    public class CooperateRimming : ModBase
    {
        public override string ModIdentifier => "CooperateRim.CooperateRimming";
        public static List<Pawn> initialPawnList = new List<Pawn>();
        public static bool dumpRand = false;
        public static CooperateRimming inst;

        static string cachedStuff = "";

        public HarmonyInstance harmonyInst
        {
            get
            {
                return base.HarmonyInst;
            }
        }

        public static void Log(string val)
        {
            if (inst != null && inst.Logger != null)
            {
                if (cachedStuff.Length != 0)
                {
                    inst.Logger.Message(cachedStuff);
                    cachedStuff = "";
                }

                inst?.Logger?.Message(val);
            }
            else
            {
                cachedStuff += val + "\n";
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

            SerializationService.AppendSurrogate(typeof(IntVec3), new IntVec3Surrogate());
            SerializationService.AppendSurrogate(typeof(BillStack), new BillStackSurrogate());
            SerializationService.AppendSurrogate(typeof(Bill), new BillSurrogate());
            SerializationService.AppendSurrogate(typeof(Pawn), new IndexedPawnSurrogate());
            SerializationService.AppendSurrogate(typeof(Bill_Production), new BillProductionSurrogate());

            PirateRPC.PirateRPC.CompilerGeneratedSurrogate sur = new PirateRPC.PirateRPC.CompilerGeneratedSurrogate();

            DateTime dt = new DateTime();
            int entryCount = 0;

            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (Type t in asm.GetTypes())
                {
                    if (t.Name.StartsWith("<>c__DisplayClass") || t.Name.Contains("c__AnonStorey"))
                    {
                        
                        object[] objs = t.GetCustomAttributes(false);

                        foreach (object o in objs)
                        {
                            if (o is System.Runtime.CompilerServices.CompilerGeneratedAttribute)
                            {
                                //CooperateRimming.Log("Surrogate for : " + t.DeclaringType);
                                SerializationService.AppendSurrogate(t, sur);
                                entryCount++;
                            }
                        }
                    }
                }
            }

            CooperateRimming.Log("Surrogate creation took " + (DateTime.Now - dt) + " for " + entryCount + " entries");

            MainTabWindow_Work_patch.useWorkPriorities_index = 
            MemberTracker<bool>.TrackPublicField<Func<bool>>(() => Current.Game.playSettings.useWorkPriorities, 
            u => 
            {
                Current.Game.playSettings.useWorkPriorities = u;
                CooperateRimming.Log("hey, it works?");
                
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
            ParrotWrapper.ParrotPatchExpressiontarget<Action<BillStack, Bill>>((BillStack __instance, Bill bill) => __instance.AddBill(bill));
            ParrotWrapper.ParrotPatchExpressiontarget<Action<BillStack, int>>((BillStack stack, int index) => bill_delete_patch.RemoveAt(stack, index));
            ParrotWrapper.ParrotPatchExpressiontarget<Action<BillStack, int, int>>((BillStack stack, int index, int offset) => bill_reorder_patch.ReorderAt(stack, index, offset));
            ParrotWrapper.ParrotPatchExpressiontarget<Action<string, bool, int, bool>>((string thingDefName, bool allow, int thingIDNumber, bool isSpecial) => ThingFilter_wrapper.Thingfilter_setallow_wrap(thingDefName, allow, thingIDNumber, isSpecial));
            ParrotWrapper.ParrotPatchExpressiontarget<Action<string, bool, int, bool>>((string thingDefName, bool allow, int zoneID, bool isSpecial) => ThingFilter_wrapper.Thingfilter_setallowzone_wrap(thingDefName, allow, zoneID, isSpecial));
            //ThingFilter_setallow_bill_with_billgiver(string thingDefName, bool allow, int thingIDNumber, bool isSpecial, int billIndex)
            ParrotWrapper.ParrotPatchExpressiontarget<thing_filter_wrapper_1>((string thingDefName, bool allow, int thingIDNumber, bool isSpecial, int billIndex) => ThingFilter_wrapper.ThingFilter_setallow_bill_with_billgiver(thingDefName, allow, thingIDNumber, isSpecial, billIndex));
            ParrotWrapper.ParrotPatchExpressiontarget<Action<int, bool>>((int thingIDNumber, bool actuallyDisallow) => ThingFilter_setallowall_wrapper.ThingFilter_setallowall_zone(thingIDNumber, actuallyDisallow));
            ParrotWrapper.ParrotPatchExpressiontarget<Action<Action>>((Action a) => FloatMenuOptionPatch.InvokeAction(a));
            RandRootContext<Pawn_JobTracker>.ApplyPatch("JobTrackerTick");
            RandRootContext<Game>.ApplyPatch("UpdatePlay");
            RandRootContext<UIRoot_Play>.ApplyPatch("UIRootOnGUI");
            //RandRootContext<Hediff>.ApplyPatch("Tick");
            //RandRootContext<LetterStack>.ApplyPatch("LetterStackUpdate");
            //RandRootContext<World>.ApplyPatch("WorldUpdate");
            //RandRootContext<Map>.ApplyPatch("MapUpdate");
            //RandRootContext<GameInfo>.ApplyPatch("GameInfoUpdate");
            //RandRootContext<Game>.ApplyPatch("LoadGame");
            RandRootContext<SustainerManager>.ApplyPatch("SustainerManagerUpdate");
            RandRootContext<SoundStarterPlaceholder>.ApplyPatch("PlayOneShot", typeof(SoundStarter));
            RandRootContext<SoundStarterPlaceholder2>.ApplyPatch("PlayOneShotOnCamera", typeof(SoundStarter));
            RandRootContext<WorldPawns>.ApplyPatch("WorldPawnsTick");
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

        class GamePlaceholder_1 { }
        class SoundStarterPlaceholder { };
        class SoundStarterPlaceholder2 { };

        public delegate void  thing_filter_wrapper_1(string thingDefName, bool allow, int thingIDNumber, bool isSpecial, int billIndex);

        public override void Initialize()
        {
            inst = this;
            NetDemo.setupCallbacks();
            (typeof(Rand).GetField("random", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null) as RandomNumberGenerator).seed = 0;

            HarmonyInstance harmony = HarmonyInst;
            Log(System.Diagnostics.Process.GetCurrentProcess().StartInfo.Arguments);
            List<Type> typesToPatch = new List<Type>();
            List<Type> designatorInheritees = new List<Type>();
            List<Type> leftOverTypes = new List<Type>();

            InitBullshit();

            //thingfilter patch
            {
                /*
                MethodInfo[] targetmethod = typeof(Verse.ThingFilter).GetMethods();

                foreach (var m in targetmethod)
                {
                    if 
                    (
                        m.Name == "SetAllow" && m.GetParameters().Length == 2 
                        && m.GetParameters()[0].ParameterType == typeof(ThingDef) 
                        && m.GetParameters()[1].ParameterType == typeof(bool)
                    )
                    {
                        Log("+++" + m);
                    }
                }*/
            }

            /*
            foreach (string methodName in new[] { "get_Int", "Gaussian" })
            {
                foreach (var method in typeof(Rand).GetMethods().Where(u => u.Name == methodName))
                {
                    HarmonyMethod prefix = new HarmonyMethod(typeof(CooperateRim.GeneralRandPatch).GetMethod("Prefix"));
                    HarmonyMethod postfix = new HarmonyMethod(typeof(CooperateRim.GeneralRandPatch).GetMethod("Postfix"));
                    harmony.Patch(method, prefix, postfix, null);
                    Log()
                }
            }*/

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
            /*
            foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (Type t in a.GetTypes())
                {
                    if (!t.IsAbstract && t.IsSubclassOf(typeof(Designator)))
                    {
                        MethodInfo targetmethod = AccessTools.Method(t, "FinalizeDesignationSucceeded");
                        HarmonyMethod prefix_1 = new HarmonyMethod(typeof(CooperateRim.Designator_patch).GetMethod("FinalizeDesignationSucceeded"));
                        harmony.Patch(targetmethod, prefix_1, null, null);
                    }
                }
            }*/
            
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
                        Log("designator multicell patch " + t.FullName);
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

            foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (Type t in a.GetTypes())
                {
                    if (!t.IsAbstract && t.IsSubclassOf(typeof(ThinkNode)))
                    {
                        MethodInfo targetmethod = AccessTools.Method(t, "TryIssueJobPackage");
                        HarmonyMethod prefix = new HarmonyMethod(typeof(CooperateRim.ThinkNode_patch).GetMethod("TryIssueJobPackage"));
                        harmony.Patch(targetmethod, prefix, null, null);
                    }
                }
            }

            foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (Type t in a.GetTypes())
                {
                    if (!t.IsAbstract && t.IsSubclassOf(typeof(ThinkNode_JobGiver)))
                    {
                        MethodInfo targetmethod = AccessTools.Method(t, "TryGiveJob");
                        HarmonyMethod prefix = new HarmonyMethod(typeof(CooperateRim.JobGiver_patch).GetMethod("TryGiveJob"));
                        harmony.Patch(targetmethod, null, prefix, null);
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
                        Logger.Message("Patched type : " + t);
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
            public static bool Prepare()
            {
                UnityEngine.Debug.Log("Command line args : " + System.Diagnostics.Process.GetCurrentProcess().StartInfo.Arguments);
                Log("" + System.Diagnostics.Process.GetCurrentProcess().StartInfo.Arguments.Contains("network_launch"));
                return !System.Diagnostics.Process.GetCurrentProcess().StartInfo.Arguments.Contains("network_launch");
            }

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

        static string hostName = "LENOVO";

        static void IterateOverThinkNodes(IEnumerable<ThinkNode> nodes, string name)
        {
            List<string> s = new List<string>();

            foreach (var node in nodes)
            {
                s.Add(node.ToString() + "\r\n");
            }

            System.IO.File.WriteAllLines("G:/CoopReplays/" + SyncTickData.cliendID + "/"+ name + "thinkdump_.txt", s.ToArray());
        }

        public class Dialog_Coop : Window
        {
            public override void DoWindowContents(Rect inRect)
            {
                int size = 52;
                Rect r = inRect;
                r.height = size - 1;
                r.width = 150;
                if (Widgets.ButtonText(r, "Connect to "))
                {
                    NetDemo.WaitForConnection(hostName);
                    Rand.PushState(8000);

                    ThinkTreeKeyAssigner.Reset();

                    foreach (var def in DefDatabase<ThinkTreeDef>.AllDefsListForReading)
                    {
                        ThinkTreeKeyAssigner.AssignKeys(def.thinkRoot, 0);
                    }

                    Current.Game = new Game();
                    Current.Game.InitData = new GameInitData();
                    Current.Game.Scenario = ScenarioDefOf.Crashlanded.scenario;
                    Find.Scenario.PreConfigure();
                    string stringseed = GenText.RandomSeedString();
                    Current.Game.storyteller = new Storyteller(StorytellerDefOf.Cassandra, DifficultyDefOf.Rough);
                    Current.Game.World = WorldGenerator.GenerateWorld(0.05f, stringseed, OverallRainfall.Normal, OverallTemperature.Normal);
                    for (int i = 0; i < 500; i++)
                    {
                        if (TileFinder.IsValidTileForNewSettlement(i))
                        {
                            Current.Game.InitData.startingTile = i;
                            Log("Choosen tile : " + i);
                            break;
                        }
                    }

                    foreach (Pawn p in Current.Game.InitData.startingAndOptionalPawns)
                    {
                        Log("++++++++++++" + p.ToString());
                    }
                    
                    TickManagerPatch.nextFrameTime = DateTime.Now;
                    TickManagerPatch.nextProcessionTick = 0;
                    Page firstConfigPage = Current.Game.Scenario.GetFirstConfigPage();

                    foreach (var p in Find.GameInitData.startingAndOptionalPawns)
                    {
                        initialPawnList.Add(p);
                    }

                    foreach (var def in DefDatabase<ThinkTreeDef>.AllDefsListForReading)
                    {
                        IterateOverThinkNodes(def.thinkRoot.ThisAndChildrenRecursive, def.ToString());
                    }

                    PageUtility.InitGameStart();
                    Rand.PopState();
                    Log("Startseed : " + stringseed);
                }
                r.y += size;
                Widgets.ButtonText(r, "MEANINGLESS BUTTON"); r.y += size;
                hostName = Widgets.TextArea(r, hostName);
                //Widgets.Label(r, "hey, motherfuckers!");
            }

            static void ErrorHandler(System.Exception ex)
            {
                Log(ex.ToString());
            }

            static void GenerateMap()
            {
                if (Find.Root)
                {
                    //Find.Root.Start();
                }
                else {
                    Log("Null root");
                }
            }
        }
    }
}

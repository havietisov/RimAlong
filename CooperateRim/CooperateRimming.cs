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
using System.Linq.Expressions;
using System.Reflection.Emit;

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

            SerializationService.AppendSurrogate(typeof(Vector3), new Vector3Surrogate());
            SerializationService.AppendSurrogate(typeof(IntVec3), new IntVec3Surrogate());
            SerializationService.AppendSurrogate(typeof(BillStack), new BillStackSurrogate());
            SerializationService.AppendSurrogate(typeof(Bill), new BillSurrogate());
            SerializationService.AppendSurrogate(typeof(Thing), new ThingSurrogate());
            SerializationService.AppendSurrogate(typeof(Area), new AreaSurrogate());
            SerializationService.AppendSurrogate(typeof(Area_Allowed), new AreaSurrogate());
            SerializationService.AppendSurrogate(typeof(AreaManager), new AreaManaegerSurrogate());
            SerializationService.AppendSurrogate(typeof(Bill_Production), new BillProductionSurrogate());
            SerializationService.AppendSurrogate(typeof(Bill_ProductionWithUft), new Bill_ProductionWithUft_surrogate());
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
            SerializationService.AppendSurrogate(typeof(DesignationDef), new DesignationDefSurrogate());
            SerializationService.AppendSurrogate(typeof(Designation), new Designation_surrogate());
            SerializationService.AppendSurrogate(typeof(Rot4), new Rot4Surrogate());
            SerializationService.AppendSurrogate(typeof(Zone), new ZoneSurrogate());
            SerializationService.AppendSurrogate(typeof(Zone_Growing), new ZoneSurrogate());
            SerializationService.AppendSurrogate(typeof(WorkTypeDef), new WorkTypeDef_surrogate());
            SerializationService.AppendSurrogate(typeof(ResearchProjectDef), new ResearchProjectDef_surrogate());
            ParrotWrapper.FastPatch<Action<DesignationManager, Designation>, Action<Designation>>((__inst, des) => __inst.RemoveDesignation(des), (des) => designator_methods.designation_mgr_regular_RemoveDesignation(des));
            ParrotWrapper.FastPatch<Action<DesignationManager, Designation>, Action<Designation>>( (__inst, newDes) => __inst.AddDesignation(newDes), (newDes) => designator_methods.designation_mgr_regular_AddDesignation(newDes) );
            ParrotWrapper.FastPatch<Action<DesignationManager, Thing, bool>, Action<Thing, bool>>( (__inst, t, b) => __inst.RemoveAllDesignationsOn(t, b), ( t, standardCanceling) => designator_methods.designation_mgr_regular_RemoveAllDesignationsOn(t, standardCanceling));
            ParrotWrapper.FastPatch<Action<Designator_Build, IntVec3>, Action<IntVec3, Rot4, BuildableDef, ThingDef>>( (u,c) => u.DesignateSingleCell(c), (IntVec3 c, Rot4 ___placingRot, BuildableDef ___entDef, ThingDef ___stuffDef) => designator_build_methods.prefix_designate_single_cell(c, ___placingRot, ___entDef, ___stuffDef));
            ParrotWrapper.FastPatch<Action<Designator_Cancel, IntVec3>, Action<IntVec3>>((u, c) => u.DesignateSingleCell(c), (IntVec3 c) => designator_cancel_methods.prefix_designate_single_cell(c));
            ParrotWrapper.FastPatch<Action<Designator_Cancel, Thing>, Action<Thing>>((u, t) => u.DesignateThing(t), (Thing t) => designator_cancel_methods.prefix_designate_thing(t));
            ParrotWrapper.FastPatch<Action<Designator_Forbid, Thing>, Action<Thing>>((u, t) => u.DesignateThing(t), (Thing t) => designator_forbid_methods.prefix_designate_thing(t));
            ParrotWrapper.FastPatch<Action<Designator_Forbid, IntVec3>, Action<IntVec3>>((u, c) => u.DesignateSingleCell(c), (IntVec3 c) => designator_forbid_methods.prefix_designate_cell(c));
            ParrotWrapper.FastPatch<Action<Designator_Claim, Thing>, Action<Thing>>((u, c) => u.DesignateThing(c), (Thing t) => designator_claim_methods.prefix_designate_thing(t));
            ParrotWrapper.FastPatch<Action<Designator_Unforbid, Thing>, Action<Thing>>((u, t) => u.DesignateThing(t), (Thing t) => designator_unforbid_methods.prefix_designate_thing(t));
            ParrotWrapper.FastPatch<Action<Designator_Unforbid, IntVec3>, Action<IntVec3>>((u, c) => u.DesignateSingleCell(c), (IntVec3 c) => designator_unforbid_methods.prefix_designate_cell(c));
            ParrotWrapper.FastPatch<Action<Designator_Plan, IntVec3>, Action<IntVec3, DesignateMode>>((u, c) => u.DesignateSingleCell(c), (IntVec3 c, DesignateMode ___mode) => designator_plan_methods.prefix_designate_cell(c, ___mode));
            ParrotWrapper.FastPatch<Action<Designator_Hunt, Thing>, Action<Thing>>((u, t) => u.DesignateThing(t), (Thing t) => designator_hunt_methods.prefix_designate_thing(t));
            ParrotWrapper.FastPatch<Action<Designator_Tame, Thing>, Action<Thing>>((u, t) => u.DesignateThing(t), (Thing t) => designator_tame_methods.prefix_designate_thing(t));
            ParrotWrapper.FastPatch<Action<Designator_ZoneAdd, IEnumerable<IntVec3>>, Action<IEnumerable<IntVec3>, Designator_ZoneAdd>>((u, c) => u.DesignateMultiCell(c), (IEnumerable<IntVec3> cells, Designator_ZoneAdd __instance) => designator_zone_methods.prefix_designate_multicell(cells, __instance));
            ParrotWrapper.FastPatch<Action<Designator_ZoneDelete, IntVec3>, Action<IntVec3, Designator_ZoneDelete>>((u, c) => u.DesignateSingleCell(c), (IntVec3 c, Designator_ZoneDelete __instance) => designator_zone_delete_methods.prefix_designate_single_cell(c, __instance));
            ParrotWrapper.FastPatch<Action<Designator_Install, IntVec3>, Action<IntVec3, Rot4, Designator_Install>>( (u, c) => u.DesignateSingleCell(c), (IntVec3 c, Rot4 ___placingRot, Designator_Install __instance) => designator_install_methods.prefix_designate_single_cell(c, ___placingRot, __instance));
            ParrotWrapper.FastPatch<Action<Designator_AreaBuildRoof, IntVec3>, Action<IntVec3>>((u,c) => u.DesignateSingleCell(c), c => designator_area_buildoof_methods.prefix_designate_single_cell(c));
            ParrotWrapper.FastPatch<Action<Designator_AreaHomeExpand, IntVec3>, Action<IntVec3, Designator_AreaHome>>((u, c) => u.DesignateSingleCell(c), (c, __instance) => designator_area_home_methods.prefix_designate_single_cell(c, __instance));
            ParrotWrapper.FastPatch<Action<Designator_AreaNoRoof, IntVec3>, Action<IntVec3>>((u, c) => u.DesignateSingleCell(c), c => designator_area_no_roof_methods.prefix_designate_single_cell(c));
            ParrotWrapper.FastPatch<Action<Designator_AreaIgnoreRoof, IntVec3>, Action<IntVec3>>((u, c) => u.DesignateSingleCell(c), c => designator_area_ignore_roof_methods.prefix_designate_single_cell(c));
            ParrotWrapper.FastPatch<Action<Designator_AreaSnowClear, IntVec3>, Action<IntVec3, Designator_AreaSnowClear>>((u,c)=> u.DesignateSingleCell(c), (c, __instance) => designator_area_snowclear_methods.prefix_designate_single_cell(c, __instance));
            ParrotWrapper.FastPatch<Action<Pawn_WorkSettings, WorkTypeDef, int>, Action<WorkTypeDef, int, Pawn>>((u,c,i)=> u.SetPriority(c,i), (w, priority, ___pawn) => pawn_worksettings_patch.prefix_set_priority(w, priority, ___pawn));
            ParrotWrapper.FastPatch<Action<Designator_AreaAllowedExpand, IntVec3>, Action<IntVec3>>((u,c)=> u.DesignateSingleCell(c), c => designator_area_allowed_expand_methods.prefix_designate_single_cell(c));
            ParrotWrapper.ParrotPatchExpressiontarget<Action<Thing, bool>>((t, standardCanceling) => designator_methods.designation_mgr_parrot_RemoveAllDesignationsOn(t, standardCanceling));
            ParrotWrapper.ParrotPatchExpressiontarget<Action<Designation>>((newDes) => designator_methods.designation_mgr_parrot_AddDesignation(newDes));
            ParrotWrapper.ParrotPatchExpressiontarget<Action<Designation>>((des) => designator_methods.designation_mgr_parrot_RemoveDesignation(des));
            ParrotWrapper.ParrotPatchExpressiontarget<Action<IntVec3, ThingDef, Rot4, BuildableDef>>((IntVec3 c, ThingDef stuffDef, Rot4 placingRot, BuildableDef entDef) => designator_build_methods.parrot_designate_single_cell(c, stuffDef, placingRot, entDef));
            ParrotWrapper.ParrotPatchExpressiontarget<Action<IntVec3>>((c) => designator_cancel_methods.parrot_designate_single_cell(c));
            ParrotWrapper.ParrotPatchExpressiontarget<Action<Thing>>((t) => designator_cancel_methods.parrot_designate_thing(t));
            ParrotWrapper.ParrotPatchExpressiontarget<Action<Thing>>((t) => designator_forbid_methods.parrot_designate_thing(t));
            ParrotWrapper.ParrotPatchExpressiontarget<Action<IntVec3>>((c) => designator_forbid_methods.parrot_designate_cell(c));
            ParrotWrapper.ParrotPatchExpressiontarget<Action<Thing>>((t) => designator_unforbid_methods.parrot_designate_thing(t));
            ParrotWrapper.ParrotPatchExpressiontarget<Action<IntVec3>>((c) => designator_unforbid_methods.parrot_designate_cell(c));
            ParrotWrapper.ParrotPatchExpressiontarget<Action<IntVec3, DesignateMode>>((c, mode) => designator_plan_methods.parrot_designate_cell(c, mode));
            ParrotWrapper.ParrotPatchExpressiontarget<Action<Thing>>((t) => designator_hunt_methods.parrot_designate_thing(t));
            ParrotWrapper.ParrotPatchExpressiontarget<Action<Thing>>((t) => designator_tame_methods.parrot_designate_thing(t));
            ParrotWrapper.ParrotPatchExpressiontarget<Action<Thing>>((t) => designator_claim_methods.parrot_designate_thing(t));
            ParrotWrapper.ParrotPatchExpressiontarget<Action<IEnumerable<IntVec3>, Type, Zone>> ((cells, designator_type, z) => designator_zone_methods.parrot_designate_multicell(cells, designator_type, z));
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
            ParrotWrapper.ParrotPatchExpressiontarget<Action<AreaManager>>((inst) => TryMakeNewAllowed_patch.TryMakeNewArea(inst));
            ParrotWrapper.ParrotPatchExpressiontarget<Action<Thing, Pawn>>((t, p) => InterfaceDrop_patch.DropGear(t, p));
            ParrotWrapper.ParrotPatchExpressiontarget<Action<Thing, int>>((inst, index) => GenericGizmoMethods.CallByIndex(inst, index));
            ParrotWrapper.ParrotPatchExpressiontarget<Action<ThingDef, Zone_Growing>>((plantDef, inst) => Zone_growing_setplantdef_patch.parrot(plantDef, inst));
            ParrotWrapper.ParrotPatchExpressiontarget<Action<Zone>>((inst) => zone_delete_patch.parrot(inst));
            ParrotWrapper.ParrotPatchExpressiontarget<Action<IntVec3, Rot4, Thing>>((IntVec3 c, Rot4 placingRot, Thing thingToInstall) => designator_install_methods.parrot_designate_single_cell(c, placingRot, thingToInstall));
            ParrotWrapper.ParrotPatchExpressiontarget<Action<IntVec3, Type>>((c, t) => designator_zone_delete_methods.parrot_designate_single_cell(c,t));
            ParrotWrapper.ParrotPatchExpressiontarget<Action<IntVec3>>(c => designator_area_buildoof_methods.parrot_designate_single_cell(c));
            ParrotWrapper.ParrotPatchExpressiontarget<Action<IntVec3, Type>>((c,t) => designator_area_home_methods.parrot_designate_single_cell(c, t));
            ParrotWrapper.ParrotPatchExpressiontarget<Action<IntVec3>>((c) => designator_area_no_roof_methods.parrot_designate_single_cell(c));
            ParrotWrapper.ParrotPatchExpressiontarget<Action<IntVec3>>((c) => designator_area_ignore_roof_methods.parrot_designate_single_cell(c));
            ParrotWrapper.ParrotPatchExpressiontarget<Action<IntVec3, Type>>((c,t)=> designator_area_snowclear_methods.parrot_designate_single_cell(c,t));
            ParrotWrapper.ParrotPatchExpressiontarget<Action<WorkTypeDef, int, Pawn>>((w, priority, p) => pawn_worksettings_patch.parrot_set_priority(w, priority, p));
            ParrotWrapper.ParrotPatchExpressiontarget<Action<ResearchProjectDef>>(project => MainTabWindow_Research_patch.parrotSetCurrentResearch(project));
            ParrotWrapper.ParrotPatchExpressiontarget<Action<bool>>(val=> MainTabWindow_Work_patch.ChangeUseWorkPriorities(val));
            ParrotWrapper.ParrotPatchExpressiontarget<Action<IntVec3, Area>>((c,z) => designator_area_allowed_expand_methods.parrot_designate_single_cell(c,z));
            ParrotWrapper.ParrotPatchExpressiontarget<Action<Area_Allowed, string>>((area, label) => designator_area_allowed_label.setLabel(area, label));
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

        public static void PatchDesignateSingleCellFor(Type t, Delegate delPatch)
        {
            MethodInfo targetmethod = AccessTools.Method(t, "DesignateSingleCell");
            HarmonyMethod prefix = new HarmonyMethod(delPatch.Method);
            CooperateRimming.inst.harmonyInst.Patch(targetmethod, prefix);
        }
        
        
        public delegate void  thing_filter_wrapper_1(string thingDefName, bool allow, int thingIDNumber, bool isSpecial, int billIndex);

        public override void Initialize()
        {
            inst = this;
            NetDemo.setupCallbacks();
            (typeof(Rand).GetField("random", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null) as RandomNumberGenerator).seed = 0;

            HarmonyInstance harmony = HarmonyInst;
            RimLog.Message(System.Diagnostics.Process.GetCurrentProcess().StartInfo.Arguments);
            List<Type> designatorInheritees = new List<Type>();
            InitBullshit();

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

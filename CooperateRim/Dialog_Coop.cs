using RimWorld;
using Verse;
using System;
using UnityEngine;
using RimWorld.Planet;

namespace CooperateRim
{

    public partial class CooperateRimming
    {
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

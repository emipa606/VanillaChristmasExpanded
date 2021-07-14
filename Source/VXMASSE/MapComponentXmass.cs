using System.Linq;
using RimWorld;
using Verse;
using Verse.AI.Group;

namespace VXMASSE
{
    // Token: 0x02000006 RID: 6
    public class MapComponentXmass : MapComponent
    {
        private int lastPartyYear = -1;

        // Token: 0x04000008 RID: 8
        private bool once;

        // Token: 0x04000009 RID: 9
        private bool party;

        // Token: 0x06000018 RID: 24 RVA: 0x000026CF File Offset: 0x000008CF
        public MapComponentXmass(Map map) : base(map)
        {
        }

        // Token: 0x06000019 RID: 25 RVA: 0x000026E1 File Offset: 0x000008E1
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref party, "party");
            Scribe_Values.Look(ref once, "once", true);
        }

        // Token: 0x0600001A RID: 26 RVA: 0x00002714 File Offset: 0x00000914
        public override void MapComponentTick()
        {
            base.MapComponentTick();
            if (Find.TickManager.TicksGame % 30000 != 0)
            {
                return;
            }

            if (lastPartyYear == GenDate.YearsPassed)
            {
                //Log.Message(lastPartyYear.ToString());
                return;
            }

            once = true;

            if (!map.listerBuildings.AllBuildingsColonistOfDef(ThingDef.Named("ChristmasTreeBase")).Any() &&
                !map.listerBuildings.AllBuildingsColonistOfDef(ThingDef.Named("ChristmasTreeB")).Any() &&
                !map.listerBuildings.AllBuildingsColonistOfDef(ThingDef.Named("ChristmasTreeC")).Any() &&
                !map.listerBuildings.AllBuildingsColonistOfDef(ThingDef.Named("ChristmasTreeD")).Any())
            {
                //Log.Message("no tree");
                return;
            }

            var season = GenDate.Season(GenTicks.TicksAbs, Find.WorldGrid.LongLatOf(map.Tile));
            var doTheThing = false;
            if (season == Season.PermanentSummer || season == Season.PermanentWinter)
            {
                //Log.Message("permanent season");
                if (GenDate.Quadrum(GenTicks.TicksAbs, Find.WorldGrid.LongLatOf(map.Tile).x) != Quadrum.Decembary)
                {
                    //Log.Message("not december");
                    return;
                }

                doTheThing = true;
            }

            if (!doTheThing && (season != Season.Winter ||
                                GenDate.DayOfSeason(GenTicks.TicksAbs, Find.WorldGrid.LongLatOf(map.Tile).x) < 2) ||
                !once)
            {
                //Log.Message("not winter");
                return;
            }

            //Log.Message("party");
            once = false;
            ExposeData();
            var incidentParms = StorytellerUtility.DefaultParmsNow(IncidentCategoryDefOf.Misc, map);
            incidentParms.forced = true;
            incidentParms.target = map;
            Find.Storyteller.incidentQueue.Add(XDefOf.PresentDrop, Find.TickManager.TicksGame, incidentParms, 240000);
            TryStartParty();
            lastPartyYear = GenDate.YearsPassed;
        }

        // Token: 0x0600001B RID: 27 RVA: 0x000028D0 File Offset: 0x00000AD0
        public bool TryStartParty()
        {
            var pawn = GatheringsUtility.FindRandomGatheringOrganizer(Faction.OfPlayer, map, GatheringDefOf.Party);
            bool result;
            if (pawn == null)
            {
                result = false;
            }
            else
            {
                if (!RCellFinder.TryFindGatheringSpot(pawn, GatheringDefOf.Party, true, out var intVec))
                {
                    result = false;
                }
                else
                {
                    LordMaker.MakeNewLord(pawn.Faction, new LordJob_Joinable_CParty(intVec, pawn), map);
                    Find.LetterStack.ReceiveLetter("CParyLabel".Translate(), "CParyLetter".Translate(),
                        LetterDefOf.PositiveEvent, new TargetInfo(intVec, map));
                    result = true;
                }
            }

            return result;
        }
    }
}
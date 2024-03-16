using RimWorld;
using Verse;
using Verse.AI.Group;

namespace VXMASSE;

public class MapComponentXmass(Map map) : MapComponent(map)
{
    private int lastPartyYear = -1;

    private bool once;

    private bool party;

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref party, "party");
        Scribe_Values.Look(ref once, "once", true);
    }

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

    public void TryStartParty()
    {
        var pawn = GatheringsUtility.FindRandomGatheringOrganizer(Faction.OfPlayer, map, GatheringDefOf.Party);
        if (pawn == null)
        {
            return;
        }

        if (!RCellFinder.TryFindGatheringSpot(pawn, GatheringDefOf.Party, true, out var intVec))
        {
            return;
        }

        LordMaker.MakeNewLord(pawn.Faction, new LordJob_Joinable_CParty(intVec, pawn), map);
        Find.LetterStack.ReceiveLetter("CParyLabel".Translate(), "CParyLetter".Translate(),
            LetterDefOf.PositiveEvent, new TargetInfo(intVec, map));
    }
}
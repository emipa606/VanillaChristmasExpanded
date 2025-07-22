using RimWorld;
using Verse;
using Verse.AI.Group;

namespace VXMASSE;

public class LordJob_Joinable_CParty : LordJob_VoluntarilyJoinable
{
    private Pawn organizer;

    private IntVec3 spot;

    private Trigger_TicksPassed timeoutTrigger;

    public LordJob_Joinable_CParty()
    {
    }

    public LordJob_Joinable_CParty(IntVec3 spot, Pawn organizer)
    {
        this.spot = spot;
        this.organizer = organizer;
    }

    public override bool AllowStartNewGatherings => false;

    public Pawn Organizer => organizer;

    public override StateGraph CreateGraph()
    {
        var stateGraph = new StateGraph();
        var lordToilCParty = new LordToil_CParty(spot);
        stateGraph.AddToil(lordToilCParty);
        var lordToilEnd = new LordToil_End();
        stateGraph.AddToil(lordToilEnd);
        var transition = new Transition(lordToilCParty, lordToilEnd);
        transition.AddTrigger(new Trigger_TickCondition(shouldBeCalledOff));
        transition.AddTrigger(new Trigger_PawnKilled());
        transition.AddPreAction(new TransitionAction_Message("MessagePartyCalledOff".Translate(),
            MessageTypeDefOf.NegativeEvent, new TargetInfo(spot, Map)));
        stateGraph.AddTransition(transition);
        timeoutTrigger = new Trigger_TicksPassed(Rand.RangeInclusive(5000, 15000));
        var transition2 = new Transition(lordToilCParty, lordToilEnd);
        transition2.AddTrigger(timeoutTrigger);
        transition2.AddPreAction(new TransitionAction_Message("MessagePartyFinished".Translate(),
            MessageTypeDefOf.SituationResolved, new TargetInfo(spot, Map)));
        stateGraph.AddTransition(transition2);
        return stateGraph;
    }

    private bool shouldBeCalledOff()
    {
        return !GatheringsUtility.AcceptableGameConditionsToContinueGathering(Map) ||
               !spot.Roofed(Map) && !JoyUtility.EnjoyableOutsideNow(Map);
    }

    public override float VoluntaryJoinPriorityFor(Pawn p)
    {
        if (!isInvited(p))
        {
            return 0f;
        }

        if (!GatheringsUtility.ShouldPawnKeepGathering(p, GatheringDefOf.Party))
        {
            return 0f;
        }

        if (spot.IsForbidden(p))
        {
            return 0f;
        }

        if (!lord.ownedPawns.Contains(p) && isPartyAboutToEnd())
        {
            return 0f;
        }

        return VoluntarilyJoinableLordJobJoinPriorities.SocialGathering;
    }

    public override void ExposeData()
    {
        Scribe_Values.Look(ref spot, "cspot");
        Scribe_References.Look(ref organizer, "corganizer");
    }

    private bool isPartyAboutToEnd()
    {
        return timeoutTrigger.TicksLeft < 1200;
    }

    private bool isInvited(Pawn p)
    {
        return lord.faction != null && p.Faction == lord.faction;
    }
}
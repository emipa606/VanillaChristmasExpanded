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
        var lordToil_CParty = new LordToil_CParty(spot);
        stateGraph.AddToil(lordToil_CParty);
        var lordToil_End = new LordToil_End();
        stateGraph.AddToil(lordToil_End);
        var transition = new Transition(lordToil_CParty, lordToil_End);
        transition.AddTrigger(new Trigger_TickCondition(ShouldBeCalledOff));
        transition.AddTrigger(new Trigger_PawnKilled());
        transition.AddPreAction(new TransitionAction_Message("MessagePartyCalledOff".Translate(),
            MessageTypeDefOf.NegativeEvent, new TargetInfo(spot, Map)));
        stateGraph.AddTransition(transition);
        timeoutTrigger = new Trigger_TicksPassed(Rand.RangeInclusive(5000, 15000));
        var transition2 = new Transition(lordToil_CParty, lordToil_End);
        transition2.AddTrigger(timeoutTrigger);
        transition2.AddPreAction(new TransitionAction_Message("MessagePartyFinished".Translate(),
            MessageTypeDefOf.SituationResolved, new TargetInfo(spot, Map)));
        stateGraph.AddTransition(transition2);
        return stateGraph;
    }

    private bool ShouldBeCalledOff()
    {
        return !GatheringsUtility.AcceptableGameConditionsToContinueGathering(Map) ||
               !spot.Roofed(Map) && !JoyUtility.EnjoyableOutsideNow(Map);
    }

    public override float VoluntaryJoinPriorityFor(Pawn p)
    {
        if (!IsInvited(p))
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

        if (!lord.ownedPawns.Contains(p) && IsPartyAboutToEnd())
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

    public virtual string GetReport()
    {
        return "LordReportAttendingParty".Translate();
    }

    private bool IsPartyAboutToEnd()
    {
        return timeoutTrigger.TicksLeft < 1200;
    }

    private bool IsInvited(Pawn p)
    {
        return lord.faction != null && p.Faction == lord.faction;
    }
}
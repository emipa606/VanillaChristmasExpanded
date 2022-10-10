using RimWorld;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace VXMASSE;

public class LordToil_CParty : LordToil
{
    private const int DefaultTicksPerPartyPulse = 600;

    private readonly IntVec3 spot;

    private readonly int ticksPerPartyPulse;
    private int ticksToNextPulse;

    public LordToil_CParty(IntVec3 spot, int ticksPerPartyPulse = DefaultTicksPerPartyPulse)
    {
        this.spot = spot;
        this.ticksPerPartyPulse = ticksPerPartyPulse;
        data = new LordToilData_Gathering();
        ticksToNextPulse = ticksPerPartyPulse;
    }

    private LordToilData_Gathering Data => (LordToilData_Gathering)data;

    public override ThinkTreeDutyHook VoluntaryJoinDutyHookFor(Pawn p)
    {
        return DutyDefOf.Party.hook;
    }

    public override void UpdateAllDuties()
    {
        foreach (var pawn in lord.ownedPawns)
        {
            pawn.mindState.duty = new PawnDuty(VDutyDefOf.CParty, spot);
        }
    }

    public override void LordToilTick()
    {
        var unused = Data;
        var num = ticksToNextPulse - 1;
        ticksToNextPulse = num;
        if (num > 0)
        {
            return;
        }

        ticksToNextPulse = ticksPerPartyPulse;
        var ownedPawns = lord.ownedPawns;
        foreach (var pawn in ownedPawns)
        {
            if (!GatheringsUtility.InGatheringArea(pawn.Position, spot, Map))
            {
                continue;
            }

            pawn.needs.mood.thoughts.memories.TryGainMemory(XDefOf.FeelingFestive);
            if (lord.LordJob is LordJob_Joinable_CParty lordJob_Joinable_CParty)
            {
                TaleRecorder.RecordTale(TaleDefOf.AttendedParty, pawn, lordJob_Joinable_CParty.Organizer);
            }
        }
    }
}
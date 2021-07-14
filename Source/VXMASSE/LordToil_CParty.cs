using RimWorld;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace VXMASSE
{
    // Token: 0x02000003 RID: 3
    public class LordToil_CParty : LordToil
    {
        // Token: 0x04000006 RID: 6
        private const int DefaultTicksPerPartyPulse = 600;

        // Token: 0x04000004 RID: 4
        private readonly IntVec3 spot;

        // Token: 0x04000005 RID: 5
        private readonly int ticksPerPartyPulse;
        private int ticksToNextPulse;

        // Token: 0x0600000D RID: 13 RVA: 0x00002349 File Offset: 0x00000549
        public LordToil_CParty(IntVec3 spot, int ticksPerPartyPulse = DefaultTicksPerPartyPulse)
        {
            this.spot = spot;
            this.ticksPerPartyPulse = ticksPerPartyPulse;
            data = new LordToilData_Gathering();
            ticksToNextPulse = ticksPerPartyPulse;
        }

        // Token: 0x17000003 RID: 3
        // (get) Token: 0x0600000E RID: 14 RVA: 0x00002384 File Offset: 0x00000584
        private LordToilData_Gathering Data => (LordToilData_Gathering) data;

        // Token: 0x0600000F RID: 15 RVA: 0x000023A4 File Offset: 0x000005A4
        public override ThinkTreeDutyHook VoluntaryJoinDutyHookFor(Pawn p)
        {
            return DutyDefOf.Party.hook;
        }

        // Token: 0x06000010 RID: 16 RVA: 0x000023C0 File Offset: 0x000005C0
        public override void UpdateAllDuties()
        {
            foreach (var pawn in lord.ownedPawns)
            {
                pawn.mindState.duty = new PawnDuty(VDutyDefOf.CParty, spot);
            }
        }

        // Token: 0x06000011 RID: 17 RVA: 0x00002424 File Offset: 0x00000624
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
}
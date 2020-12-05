using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace VXMASSE
{
    // Token: 0x02000003 RID: 3
    public class LordToil_CParty : LordToil
	{
		private int ticksToNextPulse;

		// Token: 0x0600000D RID: 13 RVA: 0x00002349 File Offset: 0x00000549
		public LordToil_CParty(IntVec3 spot, int ticksPerPartyPulse = DefaultTicksPerPartyPulse)
		{
			this.spot = spot;
			this.ticksPerPartyPulse = ticksPerPartyPulse;
			data = new LordToilData_Party();
			ticksToNextPulse = ticksPerPartyPulse;
		}

        // Token: 0x17000003 RID: 3
        // (get) Token: 0x0600000E RID: 14 RVA: 0x00002384 File Offset: 0x00000584
        private LordToilData_Party Data => (LordToilData_Party)data;

        // Token: 0x0600000F RID: 15 RVA: 0x000023A4 File Offset: 0x000005A4
        public override ThinkTreeDutyHook VoluntaryJoinDutyHookFor(Pawn p)
		{
			return DutyDefOf.Party.hook;
		}

		// Token: 0x06000010 RID: 16 RVA: 0x000023C0 File Offset: 0x000005C0
		public override void UpdateAllDuties()
		{
			for (var i = 0; i < lord.ownedPawns.Count; i++)
			{
				lord.ownedPawns[i].mindState.duty = new PawnDuty(VDutyDefOf.CParty, spot, -1f);
			}
		}

		// Token: 0x06000011 RID: 17 RVA: 0x00002424 File Offset: 0x00000624
		public override void LordToilTick()
		{
			LordToilData_Party data = Data;
			var num = ticksToNextPulse - 1;
			ticksToNextPulse = num;
			var flag = num <= 0;
			if (flag)
			{
				ticksToNextPulse = ticksPerPartyPulse;
				List<Pawn> ownedPawns = lord.ownedPawns;
				for (var i = 0; i < ownedPawns.Count; i++)
				{
					var flag2 = GatheringsUtility.InGatheringArea(ownedPawns[i].Position, spot, Map);
					if (flag2)
					{
						ownedPawns[i].needs.mood.thoughts.memories.TryGainMemory(XDefOf.FeelingFestive, null);
						var lordJob_Joinable_CParty = lord.LordJob as LordJob_Joinable_CParty;
						var flag3 = lordJob_Joinable_CParty != null;
						if (flag3)
						{
							TaleRecorder.RecordTale(TaleDefOf.AttendedParty, new object[]
							{
								ownedPawns[i],
								lordJob_Joinable_CParty.Organizer
							});
						}
					}
				}
			}
		}

		// Token: 0x04000004 RID: 4
		private IntVec3 spot;

		// Token: 0x04000005 RID: 5
		private readonly int ticksPerPartyPulse = 600;

		// Token: 0x04000006 RID: 6
		private const int DefaultTicksPerPartyPulse = 600;
	}
}

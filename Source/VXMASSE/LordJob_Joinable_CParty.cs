using RimWorld;
using Verse;
using Verse.AI.Group;

namespace VXMASSE
{
    // Token: 0x02000002 RID: 2
    public class LordJob_Joinable_CParty : LordJob_VoluntarilyJoinable
	{
		// Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
		public LordJob_Joinable_CParty()
		{
		}

		// Token: 0x06000002 RID: 2 RVA: 0x0000205A File Offset: 0x0000025A
		public LordJob_Joinable_CParty(IntVec3 spot, Pawn organizer)
		{
			this.spot = spot;
			this.organizer = organizer;
		}

        // Token: 0x17000001 RID: 1
        // (get) Token: 0x06000003 RID: 3 RVA: 0x00002074 File Offset: 0x00000274
        public override bool AllowStartNewGatherings => false;

        // Token: 0x17000002 RID: 2
        // (get) Token: 0x06000004 RID: 4 RVA: 0x00002088 File Offset: 0x00000288
        public Pawn Organizer => organizer;

        // Token: 0x06000005 RID: 5 RVA: 0x000020A0 File Offset: 0x000002A0
        public override StateGraph CreateGraph()
		{
			var stateGraph = new StateGraph();
			var lordToil_CParty = new LordToil_CParty(spot, 600);
			stateGraph.AddToil(lordToil_CParty);
			var lordToil_End = new LordToil_End();
			stateGraph.AddToil(lordToil_End);
			var transition = new Transition(lordToil_CParty, lordToil_End, false, true);
			transition.AddTrigger(new Trigger_TickCondition(() => ShouldBeCalledOff(), 1));
			transition.AddTrigger(new Trigger_PawnKilled());
			transition.AddPreAction(new TransitionAction_Message(Translator.Translate("MessagePartyCalledOff"), MessageTypeDefOf.NegativeEvent, new TargetInfo(spot, Map, false), null, 1f));
			stateGraph.AddTransition(transition, false);
			timeoutTrigger = new Trigger_TicksPassed(Rand.RangeInclusive(5000, 15000));
			var transition2 = new Transition(lordToil_CParty, lordToil_End, false, true);
			transition2.AddTrigger(timeoutTrigger);
			transition2.AddPreAction(new TransitionAction_Message(Translator.Translate("MessagePartyFinished"), MessageTypeDefOf.SituationResolved, new TargetInfo(spot, Map, false), null, 1f));
			stateGraph.AddTransition(transition2, false);
			return stateGraph;
		}

		// Token: 0x06000006 RID: 6 RVA: 0x000021C0 File Offset: 0x000003C0
		private bool ShouldBeCalledOff()
		{
			return !GatheringsUtility.AcceptableGameConditionsToContinueGathering(Map) || (!spot.Roofed(Map) && !JoyUtility.EnjoyableOutsideNow(Map, null));
		}

		// Token: 0x06000007 RID: 7 RVA: 0x00002208 File Offset: 0x00000408
		public override float VoluntaryJoinPriorityFor(Pawn p)
		{
			var flag = !IsInvited(p);
			float result;
			if (flag)
			{
				result = 0f;
			}
			else
			{
				var flag2 = !GatheringsUtility.ShouldPawnKeepGathering(p, GatheringDefOf.Party);
				if (flag2)
				{
					result = 0f;
				}
				else
				{
					var flag3 = spot.IsForbidden(p);
					if (flag3)
					{
						result = 0f;
					}
					else
					{
						var flag4 = !lord.ownedPawns.Contains(p) && IsPartyAboutToEnd();
						if (flag4)
						{
							result = 0f;
						}
						else
						{
							result = VoluntarilyJoinableLordJobJoinPriorities.SocialGathering;
						}
					}
				}
			}
			return result;
		}

		// Token: 0x06000008 RID: 8 RVA: 0x00002290 File Offset: 0x00000490
		public override void ExposeData()
		{
			Scribe_Values.Look(ref spot, "cspot", default, false);
			Scribe_References.Look(ref organizer, "corganizer", false);
		}

		// Token: 0x06000009 RID: 9 RVA: 0x000022CC File Offset: 0x000004CC
		public virtual string GetReport()
		{
			return Translator.Translate("LordReportAttendingParty");
		}

		// Token: 0x0600000A RID: 10 RVA: 0x000022E8 File Offset: 0x000004E8
		private bool IsPartyAboutToEnd()
		{
			return timeoutTrigger.TicksLeft < 1200;
		}

		// Token: 0x0600000B RID: 11 RVA: 0x0000230C File Offset: 0x0000050C
		private bool IsInvited(Pawn p)
		{
			return lord.faction != null && p.Faction == lord.faction;
		}

		// Token: 0x04000001 RID: 1
		private IntVec3 spot;

		// Token: 0x04000002 RID: 2
		private Pawn organizer;

		// Token: 0x04000003 RID: 3
		private Trigger_TicksPassed timeoutTrigger;
	}
}

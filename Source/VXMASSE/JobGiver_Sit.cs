using System;
using RimWorld;
using Verse;
using Verse.AI;

namespace VXMASSE
{
	// Token: 0x02000005 RID: 5
	public class JobGiver_Sit : JobGiver_Wander
	{
		// Token: 0x06000013 RID: 19 RVA: 0x00002538 File Offset: 0x00000738
		protected override Job TryGiveJob(Pawn pawn)
		{
			var flag = pawn.CurJob != null && pawn.CurJob.def == JobDefOf.GotoWander;
			var nextMoveOrderIsWait = pawn.mindState.nextMoveOrderIsWait;
			var flag2 = !flag;
			if (flag2)
			{
				pawn.mindState.nextMoveOrderIsWait = !pawn.mindState.nextMoveOrderIsWait;
			}
			var flag3 = nextMoveOrderIsWait && !flag;
			Job result;
			if (flag3)
			{
				result = new Job(JobDefOf.Wait_Wander)
				{
					expiryInterval = 1000
				};
			}
			else
			{
				IntVec3 exactWanderDest = GetExactWanderDest(pawn);
				var flag4 = !exactWanderDest.IsValid;
				if (flag4)
				{
					pawn.mindState.nextMoveOrderIsWait = false;
					result = null;
				}
				else
				{
					result = new Job(JobDefOf.GotoWander, exactWanderDest)
					{
						locomotionUrgency = locomotionUrgency,
						expiryInterval = 1000,
						checkOverrideOnExpire = true
					};
				}
			}
			return result;
		}

		// Token: 0x06000014 RID: 20 RVA: 0x00002620 File Offset: 0x00000820
		protected override IntVec3 GetWanderRoot(Pawn pawn)
		{
			throw new NotImplementedException();
		}

		// Token: 0x06000015 RID: 21 RVA: 0x00002628 File Offset: 0x00000828
		protected override IntVec3 GetExactWanderDest(Pawn pawn)
		{
			IntVec3 position = pawn.Position;
            bool validator(Thing x)
            {
                bool result;
                if (x.def.building != null && x.def.building.isSittable)
                {
                    result = x.Position.GetThingList(pawn.Map).FindAll((Thing p) => p.def.race != null && p.def.race.IsFlesh).Count <= 0;
                }
                else
                {
                    result = false;
                }
                return result;
            }
            return GenClosest.ClosestThingReachable(position, pawn.Map, ThingRequest.ForGroup(ThingRequestGroup.BuildingArtificial), PathEndMode.OnCell, TraverseParms.For(TraverseMode.NoPassClosedDoorsOrWater, Danger.None, false), 8f, validator, null, 0, 4, false, RegionType.Set_Passable, false).Position;
		}

		// Token: 0x06000016 RID: 22 RVA: 0x00002694 File Offset: 0x00000894
		protected virtual IntVec3 GetExactWanderDestIfNull(Pawn pawn)
		{
			IntVec3 position = pawn.Position;
			return RCellFinder.RandomWanderDestFor(pawn, position, 10f, wanderDestValidator, PawnUtility.ResolveMaxDanger(pawn, Danger.None));
		}
	}
}

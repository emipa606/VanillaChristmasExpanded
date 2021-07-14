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
            var nextMoveOrderIsWait = pawn.mindState.nextMoveOrderIsWait;
            if (!(pawn.CurJob != null && pawn.CurJob.def == JobDefOf.GotoWander))
            {
                pawn.mindState.nextMoveOrderIsWait = !pawn.mindState.nextMoveOrderIsWait;
            }

            Job result;
            if (nextMoveOrderIsWait && !(pawn.CurJob != null && pawn.CurJob.def == JobDefOf.GotoWander))
            {
                result = new Job(JobDefOf.Wait_Wander)
                {
                    expiryInterval = 1000
                };
            }
            else
            {
                var exactWanderDest = GetExactWanderDest(pawn);
                if (!exactWanderDest.IsValid)
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
            var position = pawn.Position;

            bool validator(Thing x)
            {
                bool result;
                if (x.def.building is {isSittable: true})
                {
                    result = x.Position.GetThingList(pawn.Map).FindAll(p => p.def.race is {IsFlesh: true})
                        .Count <= 0;
                }
                else
                {
                    result = false;
                }

                return result;
            }

            return GenClosest.ClosestThingReachable(position, pawn.Map,
                    ThingRequest.ForGroup(ThingRequestGroup.BuildingArtificial), PathEndMode.OnCell,
                    TraverseParms.For(TraverseMode.NoPassClosedDoorsOrWater, Danger.None), 8f, validator, null, 0, 4)
                .Position;
        }

        // Token: 0x06000016 RID: 22 RVA: 0x00002694 File Offset: 0x00000894
        protected virtual IntVec3 GetExactWanderDestIfNull(Pawn pawn)
        {
            var position = pawn.Position;
            return RCellFinder.RandomWanderDestFor(pawn, position, 10f, wanderDestValidator,
                PawnUtility.ResolveMaxDanger(pawn, Danger.None));
        }
    }
}
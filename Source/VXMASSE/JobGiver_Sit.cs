using System;
using RimWorld;
using Verse;
using Verse.AI;

namespace VXMASSE;

public class JobGiver_Sit : JobGiver_Wander
{
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

    protected override IntVec3 GetWanderRoot(Pawn pawn)
    {
        throw new NotImplementedException();
    }

    protected override IntVec3 GetExactWanderDest(Pawn pawn)
    {
        var position = pawn.Position;

        return GenClosest.ClosestThingReachable(position, pawn.Map,
                ThingRequest.ForGroup(ThingRequestGroup.BuildingArtificial), PathEndMode.OnCell,
                TraverseParms.For(TraverseMode.NoPassClosedDoorsOrWater, Danger.None), 8f, validator, null, 0, 4)
            .Position;

        bool validator(Thing x)
        {
            bool result;
            if (x.def.building is { isSittable: true })
            {
                result = x.Position.GetThingList(pawn.Map).FindAll(p => p.def.race is { IsFlesh: true })
                    .Count <= 0;
            }
            else
            {
                result = false;
            }

            return result;
        }
    }

    protected virtual IntVec3 GetExactWanderDestIfNull(Pawn pawn)
    {
        var position = pawn.Position;
        return RCellFinder.RandomWanderDestFor(pawn, position, 10f, wanderDestValidator,
            PawnUtility.ResolveMaxDanger(pawn, Danger.None));
    }
}
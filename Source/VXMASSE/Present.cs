using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace VXMASSE;

public class Present : Building, IThingHolder, IOpenable
{
    private ThingOwner innerContainer;

    public Present()
    {
        innerContainer = new ThingOwner<Thing>(this, false);
    }

    private bool HasAnyContents => true;

    public Thing ContainedThing => innerContainer.Count != 0 ? innerContainer[0] : null;

    public bool CanOpen => HasAnyContents;

    public int OpenTicks => 100;

    public virtual void Open()
    {
        EjectContents();
    }

    public ThingOwner GetDirectlyHeldThings()
    {
        return innerContainer;
    }

    public void GetChildHolders(List<IThingHolder> outChildren)
    {
        ThingOwnerUtility.AppendThingHoldersFromThings(outChildren, GetDirectlyHeldThings());
    }

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Deep.Look(ref innerContainer, "innerContainer", this);
    }

    public override void Tick()
    {
        base.Tick();
        innerContainer.ThingOwnerTick();
    }

    public override void TickRare()
    {
        base.TickRare();
        innerContainer.ThingOwnerTickRare();
    }

    public override IEnumerable<Gizmo> GetGizmos()
    {
        foreach (var c in base.GetGizmos())
        {
            yield return c;
        }
    }

    private void EjectContents()
    {
        var random = new Random();
        var num = random.Next(1, 101);
        if (num > 10)
        {
            var list = new List<ThingCategoryDef>
            {
                ThingCategoryDefOf.BodyParts,
                ThingCategoryDefOf.Apparel,
                ThingCategoryDefOf.Drugs,
                ThingCategoryDefOf.Foods,
                ThingCategoryDefOf.MeatRaw,
                ThingCategoryDefOf.Items,
                ThingCategoryDefOf.Leathers,
                ThingCategoryDefOf.Manufactured,
                ThingCategoryDefOf.Medicine,
                ThingCategoryDefOf.ResourcesRaw,
                ThingCategoryDefOf.Weapons
            };
            var thingCategoryDef = list.RandomElement();
            var categoryDef = thingCategoryDef;
            IEnumerable<ThingStuffPair> source = ThingStuffPair.AllWith(td =>
                td.thingCategories != null && td.thingCategories.Contains(categoryDef));
            while (!source.Any())
            {
                thingCategoryDef = list.RandomElement();
                var def1 = thingCategoryDef;
                source = ThingStuffPair.AllWith(td =>
                    td.thingCategories != null && td.thingCategories.Contains(def1));
            }

            var thingStuffPair = source.RandomElement();
            var thing = ThingMaker.MakeThing(thingStuffPair.thing, thingStuffPair.stuff);
            CellFinder.TryRandomClosewalkCellNear(Position, Map, 1, out var intVec);
            if (thing.def.thingCategories.Contains(ThingCategoryDefOf.Apparel) ||
                thing.def.thingCategories.Contains(ThingCategoryDefOf.Weapons))
            {
                if (random.Next(0, 101) < 20)
                {
                    thing.TryGetComp<CompQuality>().SetQuality(QualityCategory.Poor, ArtGenerationContext.Outsider);
                }

                if (random.Next(0, 101) is > 20 and < 60)
                {
                    thing.TryGetComp<CompQuality>()
                        .SetQuality(QualityCategory.Normal, ArtGenerationContext.Outsider);
                }

                if (random.Next(0, 101) > 60 && random.Next(0, 101) < 80)
                {
                    thing.TryGetComp<CompQuality>().SetQuality(QualityCategory.Good, ArtGenerationContext.Outsider);
                }

                if (random.Next(0, 101) > 80 && random.Next(0, 101) < 90)
                {
                    thing.TryGetComp<CompQuality>()
                        .SetQuality(QualityCategory.Excellent, ArtGenerationContext.Outsider);
                }

                if (random.Next(0, 101) > 90 && random.Next(0, 101) < 95)
                {
                    thing.TryGetComp<CompQuality>()
                        .SetQuality(QualityCategory.Legendary, ArtGenerationContext.Outsider);
                }

                if (random.Next(0, 101) > 95 && random.Next(0, 101) < 101)
                {
                    thing.TryGetComp<CompQuality>()
                        .SetQuality(QualityCategory.Masterwork, ArtGenerationContext.Outsider);
                }
            }

            thing.stackCount++;
            var num3 = 0;
            while (num3 < thing.def.stackLimit * 0.02)
            {
                thing.stackCount++;
                num3++;
            }

            GenPlace.TryPlaceThing(thing, intVec, Map, ThingPlaceMode.Direct);
            DeSpawn();
        }
        else
        {
            var allDefs = DefDatabase<IncidentDef>.AllDefs;
            var list2 = new List<IncidentDef>();
            foreach (var incidentDef in allDefs)
            {
                var parms = StorytellerUtility.DefaultParmsNow(incidentDef.category, Map);
                if (incidentDef.Worker.CanFireNow(parms) && incidentDef.defName != "StrangerInBlackJoin" &&
                    incidentDef.defName != "PresentDrop" && incidentDef.defName != "ShortCircuit" &&
                    incidentDef.letterDef != null && incidentDef.letterDef.defName != "PurpleEvent" &&
                    !incidentDef.targetTags.Contains(IncidentTargetTagDefOf.World) &&
                    !incidentDef.targetTags.Contains(DefDatabase<IncidentTargetTagDef>.GetNamedSilentFail("Caravan")))
                {
                    list2.Add(incidentDef);
                }
            }

            var incidentDef2 = list2.RandomElement();
            if (incidentDef2 == null)
            {
                EjectContents();
            }

            var incidentParms = StorytellerUtility.DefaultParmsNow(incidentDef2?.category, Map);
            var pointsScaleable = incidentDef2 is { pointsScaleable: true };
            if (pointsScaleable)
            {
                var storytellerComp = Find.Storyteller.storytellerComps.First(x =>
                    x is StorytellerComp_OnOffCycle || x is StorytellerComp_RandomMain);
                incidentParms = storytellerComp.GenerateParms(incidentDef2.category, incidentParms.target);
            }

            incidentDef2?.Worker.TryExecute(incidentParms);
            DeSpawn();
        }
    }
}
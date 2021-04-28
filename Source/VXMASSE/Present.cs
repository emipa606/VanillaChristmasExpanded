using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace VXMASSE
{
    // Token: 0x02000009 RID: 9
    public class Present : Building, IThingHolder, IOpenable
    {
        // Token: 0x0400000E RID: 14
        private ThingOwner innerContainer;

        // Token: 0x0600001F RID: 31 RVA: 0x00002A81 File Offset: 0x00000C81
        public Present()
        {
            innerContainer = new ThingOwner<Thing>(this, false);
        }

        // Token: 0x17000004 RID: 4
        // (get) Token: 0x06000020 RID: 32 RVA: 0x00002A9C File Offset: 0x00000C9C
        private bool HasAnyContents => true;

        // Token: 0x17000005 RID: 5
        // (get) Token: 0x06000023 RID: 35 RVA: 0x00002AEC File Offset: 0x00000CEC
        public Thing ContainedThing => innerContainer.Count != 0 ? innerContainer[0] : null;

        // Token: 0x17000006 RID: 6
        // (get) Token: 0x06000024 RID: 36 RVA: 0x00002B1C File Offset: 0x00000D1C
        public bool CanOpen => HasAnyContents;

        // Token: 0x0600002A RID: 42 RVA: 0x00003030 File Offset: 0x00001230
        public virtual void Open()
        {
            EjectContents();
        }

        // Token: 0x06000025 RID: 37 RVA: 0x00002B34 File Offset: 0x00000D34
        public ThingOwner GetDirectlyHeldThings()
        {
            return innerContainer;
        }

        // Token: 0x06000026 RID: 38 RVA: 0x00002B4C File Offset: 0x00000D4C
        public void GetChildHolders(List<IThingHolder> outChildren)
        {
            ThingOwnerUtility.AppendThingHoldersFromThings(outChildren, GetDirectlyHeldThings());
        }

        // Token: 0x06000021 RID: 33 RVA: 0x00002AAF File Offset: 0x00000CAF
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Deep.Look(ref innerContainer, "innerContainer", this);
        }

        // Token: 0x06000022 RID: 34 RVA: 0x00002AD4 File Offset: 0x00000CD4
        public override void Tick()
        {
            base.Tick();
            innerContainer.ThingOwnerTick();
        }

        // Token: 0x06000027 RID: 39 RVA: 0x00002B5C File Offset: 0x00000D5C
        public override void TickRare()
        {
            base.TickRare();
            innerContainer.ThingOwnerTickRare();
        }

        // Token: 0x06000028 RID: 40 RVA: 0x00002B73 File Offset: 0x00000D73
        public override IEnumerable<Gizmo> GetGizmos()
        {
            foreach (var c in base.GetGizmos())
            {
                yield return c;
            }
        }

        // Token: 0x06000029 RID: 41 RVA: 0x00002B84 File Offset: 0x00000D84
        private void EjectContents()
        {
            var random = new Random();
            var num = random.Next(1, 101);
            var flag = num > 10;
            if (flag)
            {
                var list = new List<ThingCategoryDef>
                {
                    ThingCategoryDefOf.BodyParts,
                    ThingCategoryDefOf.Apparel,
                    ThingCategoryDefOf.Drugs,
                    ThingCategoryDefOf.FoodMeals,
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
                IEnumerable<ThingStuffPair> source = ThingStuffPair.AllWith(td => td.thingCategories != null && td.thingCategories.Contains(categoryDef));
                while (!source.Any())
                {
                    thingCategoryDef = list.RandomElement();
                    var def1 = thingCategoryDef;
                    source = ThingStuffPair.AllWith(td => td.thingCategories != null && td.thingCategories.Contains(def1));
                }

                var thingStuffPair = source.RandomElement();
                var thing = ThingMaker.MakeThing(thingStuffPair.thing, thingStuffPair.stuff);
                CellFinder.TryRandomClosewalkCellNear(Position, Map, 1, out var intVec);
                var flag2 = thing.def.thingCategories.Contains(ThingCategoryDefOf.Apparel) || thing.def.thingCategories.Contains(ThingCategoryDefOf.Weapons);
                if (flag2)
                {
                    var num2 = random.Next(0, 101);
                    var flag3 = num2 < 20;
                    if (flag3)
                    {
                        thing.TryGetComp<CompQuality>().SetQuality(QualityCategory.Poor, ArtGenerationContext.Outsider);
                    }

                    var flag4 = num2 > 20 && num2 < 60;
                    if (flag4)
                    {
                        thing.TryGetComp<CompQuality>().SetQuality(QualityCategory.Normal, ArtGenerationContext.Outsider);
                    }

                    var flag5 = num2 > 60 && num2 < 80;
                    if (flag5)
                    {
                        thing.TryGetComp<CompQuality>().SetQuality(QualityCategory.Good, ArtGenerationContext.Outsider);
                    }

                    var flag6 = num2 > 80 && num2 < 90;
                    if (flag6)
                    {
                        thing.TryGetComp<CompQuality>().SetQuality(QualityCategory.Excellent, ArtGenerationContext.Outsider);
                    }

                    var flag7 = num2 > 90 && num2 < 95;
                    if (flag7)
                    {
                        thing.TryGetComp<CompQuality>().SetQuality(QualityCategory.Legendary, ArtGenerationContext.Outsider);
                    }

                    var flag8 = num2 > 95 && num2 < 101;
                    if (flag8)
                    {
                        thing.TryGetComp<CompQuality>().SetQuality(QualityCategory.Masterwork, ArtGenerationContext.Outsider);
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
                    var flag9 = incidentDef.Worker.CanFireNow(parms) && incidentDef.defName != "StrangerInBlackJoin" && incidentDef.defName != "PresentDrop" && incidentDef.defName != "ShortCircuit" && incidentDef.letterDef != null && incidentDef.letterDef.defName != "PurpleEvent" && !incidentDef.targetTags.Contains(IncidentTargetTagDefOf.World) && !incidentDef.targetTags.Contains(IncidentTargetTagDefOf.Caravan);
                    if (flag9)
                    {
                        list2.Add(incidentDef);
                    }
                }

                var incidentDef2 = list2.RandomElement();
                var flag10 = incidentDef2 == null;
                if (flag10)
                {
                    EjectContents();
                }

                var incidentParms = StorytellerUtility.DefaultParmsNow(incidentDef2?.category, Map);
                var pointsScaleable = incidentDef2 != null && incidentDef2.pointsScaleable;
                if (pointsScaleable)
                {
                    var storytellerComp = Find.Storyteller.storytellerComps.First(x => x is StorytellerComp_OnOffCycle || x is StorytellerComp_RandomMain);
                    var flag11 = storytellerComp != null;
                    if (flag11)
                    {
                        incidentParms = storytellerComp.GenerateParms(incidentDef2.category, incidentParms.target);
                    }
                }

                incidentDef2?.Worker.TryExecute(incidentParms);
                DeSpawn();
            }
        }
    }
}
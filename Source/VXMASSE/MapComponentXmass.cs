using System;
using System.Linq;
using RimWorld;
using Verse;
using Verse.AI.Group;

namespace VXMASSE
{
	// Token: 0x02000006 RID: 6
	public class MapComponentXmass : MapComponent
	{
		// Token: 0x06000018 RID: 24 RVA: 0x000026CF File Offset: 0x000008CF
		public MapComponentXmass(Map map) : base(map)
		{
		}

		// Token: 0x06000019 RID: 25 RVA: 0x000026E1 File Offset: 0x000008E1
		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref party, "party", false, false);
			Scribe_Values.Look(ref once, "once", true, false);
		}

		// Token: 0x0600001A RID: 26 RVA: 0x00002714 File Offset: 0x00000914
		public override void MapComponentTick()
		{
			base.MapComponentTick();
			var flag = Find.TickManager.TicksGame % 30000 == 0;
			if (flag)
			{
				var flag2 = map.listerBuildings.AllBuildingsColonistOfDef(ThingDef.Named("ChristmasTreeBase")).Count() > 0 || map.listerBuildings.AllBuildingsColonistOfDef(ThingDef.Named("ChristmasTreeB")).Count() > 0 || map.listerBuildings.AllBuildingsColonistOfDef(ThingDef.Named("ChristmasTreeC")).Count() > 0 || map.listerBuildings.AllBuildingsColonistOfDef(ThingDef.Named("ChristmasTreeD")).Count() > 0;
				var flag3 = flag2;
				if (flag3)
				{
					Season season = GenDate.Season(GenTicks.TicksAbs, Find.WorldGrid.LongLatOf(map.Tile));
					var flag4 = season == Season.Winter && GenDate.DayOfSeason(GenTicks.TicksAbs, Find.WorldGrid.LongLatOf(map.Tile).x) >= 2;
					var flag5 = flag4 && once;
					if (flag5)
					{
						once = false;
						ExposeData();
						IncidentParms incidentParms = StorytellerUtility.DefaultParmsNow(IncidentCategoryDefOf.Misc, map);
						incidentParms.forced = true;
						incidentParms.target = map;
						Find.Storyteller.incidentQueue.Add(XDefOf.PresentDrop, Find.TickManager.TicksGame, incidentParms, 240000);
						TryStartParty();
					}
					else
					{
						var flag6 = season != Season.Winter;
						if (flag6)
						{
							var random = new Random();
							party = false;
							once = true;
						}
					}
				}
			}
		}

		// Token: 0x0600001B RID: 27 RVA: 0x000028D0 File Offset: 0x00000AD0
		public bool TryStartParty()
		{
			Pawn pawn = GatheringsUtility.FindRandomGatheringOrganizer(Faction.OfPlayer, map, GatheringDefOf.Party);
			var flag = pawn == null;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
                var flag2 = !RCellFinder.TryFindGatheringSpot_NewTemp(pawn, GatheringDefOf.Party, true, out IntVec3 intVec);
                if (flag2)
				{
					result = false;
				}
				else
				{
					LordMaker.MakeNewLord(pawn.Faction, new LordJob_Joinable_CParty(intVec, pawn), map, null);
					Find.LetterStack.ReceiveLetter(Translator.Translate("CParyLabel"), Translator.Translate("CParyLetter"), LetterDefOf.PositiveEvent, new TargetInfo(intVec, map, false), null, null);
					result = true;
				}
			}
			return result;
		}

		// Token: 0x04000008 RID: 8
		private bool once;

		// Token: 0x04000009 RID: 9
		private bool party = false;
	}
}

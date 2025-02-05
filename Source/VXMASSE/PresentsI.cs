using System;
using RimWorld;
using Verse;

namespace VXMASSE;

internal class PresentsI : IncidentWorker
{
    protected override bool CanFireNowSub(IncidentParms parms)
    {
        return VXMASSEMod.instance.Settings.VCEGifts && base.CanFireNowSub(parms);
    }

    protected override bool TryExecuteWorker(IncidentParms parms)
    {
        var map = (Map)parms.target;
        var random = new Random();
        var num = random.Next(1, 6);
        TryFindPresentDropCell(map.Center, map, 300, out var pos);
        var skyfaller = SkyfallerMaker.SpawnSkyfaller(XDefOf.PresentIncoming, XDefOf.Present, pos, map);
        for (var i = 0; i <= num - 1; i++)
        {
            TryFindPresentDropCell(map.Center, map, 300, out pos);
            skyfaller = SkyfallerMaker.SpawnSkyfaller(XDefOf.PresentIncoming, XDefOf.Present, pos, map);
        }

        string text = "PresentLabel".Translate();
        string text2 = "PresentLetter".Translate();
        Find.LetterStack.ReceiveLetter(text, text2, LetterDefOf.PositiveEvent,
            new TargetInfo(skyfaller.Position, map));
        return true;
    }

    private void TryFindPresentDropCell(IntVec3 nearLoc, Map map, int maxDist, out IntVec3 pos)
    {
        var presentIncoming = XDefOf.PresentIncoming;
        CellFinderLoose.TryFindSkyfallerCell(presentIncoming, map, out pos, 10, nearLoc, maxDist, false);
    }
}
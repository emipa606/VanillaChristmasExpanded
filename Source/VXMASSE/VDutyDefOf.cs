using RimWorld;
using Verse.AI;

namespace VXMASSE;

[DefOf]
public static class VDutyDefOf
{
    public static DutyDef CParty;

    static VDutyDefOf()
    {
        DefOfHelper.EnsureInitializedInCtor(typeof(DutyDefOf));
    }
}
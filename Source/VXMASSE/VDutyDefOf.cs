using RimWorld;
using Verse.AI;

namespace VXMASSE
{
    // Token: 0x02000004 RID: 4
    [DefOf]
    public static class VDutyDefOf
    {
        // Token: 0x04000007 RID: 7
        public static DutyDef CParty;

        // Token: 0x06000012 RID: 18 RVA: 0x00002523 File Offset: 0x00000723
        static VDutyDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(DutyDefOf));
        }
    }
}
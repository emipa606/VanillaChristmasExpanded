using System.Text;
using Verse;

namespace VXMASSE
{
    // Token: 0x0200000A RID: 10
    internal class Xtree : Building
    {
        // Token: 0x0600002C RID: 44 RVA: 0x00003044 File Offset: 0x00001244
        public override string GetInspectString()
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append(base.GetInspectString());
            if (stringBuilder.Length != 0)
            {
                stringBuilder.AppendLine();
            }

            stringBuilder.Append("DaysUntilPresent".Translate());
            return stringBuilder.ToString();
        }
    }
}
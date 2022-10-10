using System.Text;
using Verse;

namespace VXMASSE;

internal class Xtree : Building
{
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
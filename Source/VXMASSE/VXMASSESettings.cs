using Verse;

namespace VXMASSE;

/// <summary>
///     Definition of the settings for the mod
/// </summary>
internal class VXMASSESettings : ModSettings
{
    public bool VCEGifts = true;

    /// <summary>
    ///     Saving and loading the values
    /// </summary>
    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref VCEGifts, "VCEGifts", true);
    }
}
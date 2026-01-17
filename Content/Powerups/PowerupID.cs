using ReLogic.Reflection;
using Terraria.ID;
using Terraria.ModLoader;

namespace TerrariaXMario.Content.Powerups;

internal class PowerupID
{
    [ReinitializeDuringResizeArrays]
    internal static class Sets
    {
        internal static SetFactory Factory = new(PowerupLoader.powerups.Count, $"{nameof(TerrariaXMario)}/{nameof(PowerupID)}", Search);

        internal static bool[] ShowTail = Factory.CreateNamedSet("ShowTail")
            .Description("Powerups that should have a tail")
            .RegisterBoolSet(false, ModContent.GetInstance<SuperLeafData>().Type, ModContent.GetInstance<TanookiSuitData>().Type);

        /*internal static bool[] ShowCape = Factory.CreateNamedSet("ShowCape")
            .Description("Powerups that should have a cape")
            .RegisterBoolSet(false, ModContent.GetInstance<CapeFeatherData>().Type);*/

        internal static bool[] DisableGroundPound = Factory.CreateNamedSet("DisableGroundPound")
            .Description("Powerups that prevent usage of the ground pound ability")
            .RegisterBoolSet(false, ModContent.GetInstance<FrogSuitData>().Type);
    }

    internal static IdDictionary Search = IdDictionary.Create<PowerupID, int>();
}

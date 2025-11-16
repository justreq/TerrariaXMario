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

        //internal static bool[] NonBoss = Factory.CreateNamedSet("NonBoss")
        //    .Description("Victory poses in this set are options to be chosen when defeating a regular enemy")
        //    .RegisterBoolSet(false);
    }

    internal static IdDictionary Search = IdDictionary.Create<PowerupID, int>();
}

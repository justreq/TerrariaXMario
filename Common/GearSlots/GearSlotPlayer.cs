using Terraria;
using Terraria.ModLoader;

namespace TerrariaXMario.Common.GearSlots;
internal class GearSlotPlayer : ModPlayer
{
    private int lastVanillaLoadout = 0;

    private EquipmentLoadout gearLoadout = new();

    private bool showGearSlots;
    internal bool ShowGearSlots
    {
        get => showGearSlots;
        set
        {
            showGearSlots = value;

            if (Player.whoAmI != Main.myPlayer || (!(Player.itemTime > 0 || Player.itemAnimation > 0) && !Player.CCed && !Player.dead))
            {
                if (value)
                {
                    lastVanillaLoadout = Player.CurrentLoadoutIndex;
                    Player.Loadouts[lastVanillaLoadout].Swap(Player);
                    gearLoadout.Swap(Player);
                }
                else
                {
                    gearLoadout.Swap(Player);
                    Player.Loadouts[lastVanillaLoadout].Swap(Player);
                }
            }
        }
    }

    public override void Initialize()
    {
        lastVanillaLoadout = Player.CurrentLoadoutIndex;
    }
}
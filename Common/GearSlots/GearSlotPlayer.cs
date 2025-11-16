using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.UI;

namespace TerrariaXMario.Common.GearSlots;
internal class GearSlotPlayer : ModPlayer
{
    private readonly EquipmentLoadout gearLoadout = new();
    private int lastUsedVanillaLoadout;

    internal bool ShowGearSlots
    {
        get => field;
        set
        {
            field = value;

            if (Player.whoAmI != Main.myPlayer || (!(Player.itemTime > 0 || Player.itemAnimation > 0) && !Player.CCed && !Player.dead))
            {
                if (value)
                {
                    Player.TrySwitchingLoadout(0);
                    SwitchFromVanillaToGearLoadout();
                }
                else SwitchFromGearToVanillaLoadout();
            }
        }
    }

    private void SwitchFromVanillaToGearLoadout() // Nearly identical to Terraria.Player.TrySwitchingLoadout, using gearLoadout rather than a loadout index
    {
        Player.Loadouts[Player.CurrentLoadoutIndex].Swap(Player);
        gearLoadout.Swap(Player);
        Player.CurrentLoadoutIndex = -1;
        if (Player.whoAmI == Main.myPlayer)
        {
            CloneLoadouts(Main.clientPlayer);
            Main.mouseLeftRelease = false;
            ItemSlot.RecordLoadoutChange();
            SoundEngine.PlaySound(SoundID.MenuTick);
            NetMessage.TrySendData(MessageID.SyncLoadout, -1, -1, null, Player.whoAmI, -1);
            ParticleOrchestrator.RequestParticleSpawn(clientOnly: false, ParticleOrchestraType.LoadoutChange, new ParticleOrchestraSettings
            {
                PositionInWorld = Player.Center,
                UniqueInfoPiece = -1
            }, Player.whoAmI);
        }
    }

    private void SwitchFromGearToVanillaLoadout() // Nearly identical to Terraria.Player.TrySwitchingLoadout, using gearLoadout rather than a loadout index
    {
        gearLoadout.Swap(Player);
        Player.Loadouts[lastUsedVanillaLoadout].Swap(Player);
        Player.CurrentLoadoutIndex = lastUsedVanillaLoadout;
        if (Player.whoAmI == Main.myPlayer)
        {
            CloneLoadouts(Main.clientPlayer);
            Main.mouseLeftRelease = false;
            ItemSlot.RecordLoadoutChange();
            SoundEngine.PlaySound(SoundID.MenuTick);
            NetMessage.TrySendData(MessageID.SyncLoadout, -1, -1, null, Player.whoAmI, lastUsedVanillaLoadout);
            ParticleOrchestrator.RequestParticleSpawn(clientOnly: false, ParticleOrchestraType.LoadoutChange, new ParticleOrchestraSettings
            {
                PositionInWorld = Player.Center,
                UniqueInfoPiece = lastUsedVanillaLoadout
            }, Player.whoAmI);
        }
    }

    private void CloneLoadouts(Player clonePlayer) // Identical to Terraria.Player.CloneLoadouts
    {
        Item[] array = Player.armor;
        Item[] array2 = clonePlayer.armor;
        for (int i = 0; i < array.Length; i++)
        {
            array[i].CopyNetStateTo(array2[i]);
        }
        array = Player.dye;
        array2 = clonePlayer.dye;
        for (int j = 0; j < array.Length; j++)
        {
            array[j].CopyNetStateTo(array2[j]);
        }
        for (int k = 0; k < Player.Loadouts.Length; k++)
        {
            array = Player.Loadouts[k].Armor;
            array2 = clonePlayer.Loadouts[k].Armor;
            for (int l = 0; l < array.Length; l++)
            {
                array[l].CopyNetStateTo(array2[l]);
            }
            array = Player.Loadouts[k].Dye;
            array2 = clonePlayer.Loadouts[k].Dye;
            for (int m = 0; m < array.Length; m++)
            {
                array[m].CopyNetStateTo(array2[m]);
            }
        }
    }

    public override void Initialize()
    {
        lastUsedVanillaLoadout = Player.CurrentLoadoutIndex;
    }

    public override void OnEquipmentLoadoutSwitched(int oldLoadoutIndex, int loadoutIndex)
    {
        lastUsedVanillaLoadout = loadoutIndex;
    }

    public override void SaveData(TagCompound tag)
    {
        tag[nameof(lastUsedVanillaLoadout)] = lastUsedVanillaLoadout;
        tag[nameof(ShowGearSlots)] = ShowGearSlots;
    }

    public override void LoadData(TagCompound tag)
    {
        if (tag.ContainsKey(nameof(lastUsedVanillaLoadout))) lastUsedVanillaLoadout = tag.GetInt(nameof(lastUsedVanillaLoadout));
        if (tag.ContainsKey(nameof(ShowGearSlots))) ShowGearSlots = tag.GetBool(nameof(ShowGearSlots));
    }
}
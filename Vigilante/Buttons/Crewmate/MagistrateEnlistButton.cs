using HarmonyLib;
using MiraAPI.GameOptions;
using MiraAPI.Keybinds;
using MiraAPI.LocalSettings;
using MiraAPI.Modifiers;
using MiraAPI.Translation;
using MiraAPI.Utilities;
using MiraAPI.Utilities.Assets;
using Reactor.Utilities;
using TownOfUs;
using TownOfUs.Assets;
using TownOfUs.Buttons;
using TownOfUs.Modifiers;
using TownOfUs.Modules;
using TownOfUs.Modules.Components;
using TownOfUs.Options.Roles.Crewmate;
using TownOfUs.Roles.Crewmate;
using TownOfUs.Utilities;
using UnityEngine;
using Vigilante.Assets;
using Vigilante.Modifiers.Logic.Crewmate;
using Vigilante.Options;
using Vigilante.Roles.Crewmate;

namespace Vigilante.Buttons.Crewmate;
// code from TOUM Monarch Knight Button
public sealed class MagistrateEnlistButton : TownOfUsRoleButton<MagistrateRole, PlayerControl>
{
    public override string Name => MiraLocaleManager.Get("Vigilante.Role.Magistrate.Enlist");
    public override BaseKeybind Keybind => Keybinds.PrimaryAction;
    public override Color TextOutlineColor => VigilanteColors.Magistrate;
    public override float Cooldown => OptionGroupSingleton<MagistrateOptions>.Instance.EnlistCooldown.Value + MapCooldown;
    public override LoadableAsset<Sprite> Sprite => TouCrewAssets.KnightSprite;

    //public override bool UsableFirstRound => OptionGroupSingleton<MonarchOptions>.Instance.FirstRoundUse;

    public override PlayerControl? GetTarget()
    {
        return PlayerControl.LocalPlayer.GetClosestLivingPlayer(true, Distance,
            predicate: x => !x.HasModifier<MagistrateJurorModifier>() && !x.HasModifier<BaseRevealModifier>(y => y.RevealRole && y.Visible));
    }

    protected override void OnClick()
    {
        if (Target == null)
        {
            return;
        }

        ModifierUtils.GetPlayersWithModifier<MagistrateJurorModifier>().Do(x => x.RpcRemoveModifier<MagistrateJurorModifier>());

        MagistrateRole.RpcEnlist(PlayerControl.LocalPlayer, Target);
    }

}

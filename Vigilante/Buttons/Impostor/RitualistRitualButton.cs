using MiraAPI.GameOptions;
using MiraAPI.Hud;
using MiraAPI.Keybinds;
using MiraAPI.Utilities.Assets;
using TownOfUs;
using TownOfUs.Buttons;
using TownOfUs.Roles.Crewmate;
using TownOfUs.Utilities;
using UnityEngine;
using Vigilante.Assets;
using Vigilante.Options;
using Vigilante.Roles.Impostor;

namespace Vigilante.Buttons.Impostor;

public sealed class RitualistRitualButton : TownOfUsRoleButton<RitualistRole>
{
    public override string Name => "Ritual";
    public override BaseKeybind Keybind => Keybinds.SecondaryAction;
    public override Color TextOutlineColor => TownOfUsColors.Impostor;
    public override float Cooldown => OptionGroupSingleton<RitualistOptions>.Instance.RitualCooldown.Value + (Role.IncorrectRituals * OptionGroupSingleton<RitualistOptions>.Instance.CooldownIncrease.Value);
    public override LoadableAsset<Sprite> Sprite => VigilanteAssets.RitualistRitual;
    public override ButtonLocation Location => ButtonLocation.BottomRight;
    public override bool ShouldPauseInVent => true;

    public bool MenuOpen { get; set; }

    protected override void OnClick()
    {
        Role.OpenPlayerMenu();
        MenuOpen = true;
    }

    public override void FixedUpdateHandler(PlayerControl playerControl)
    {
        if (MenuOpen && !Minigame.Instance)
        {
            MenuOpen = false;
        }

        TimerPaused = false;
        if (MenuOpen)
        {
            TimerPaused = true;
        }

        base.FixedUpdateHandler(playerControl);
    }
}
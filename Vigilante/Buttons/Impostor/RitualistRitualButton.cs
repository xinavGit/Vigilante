using MiraAPI.Hud;
using MiraAPI.Keybinds;
using MiraAPI.Utilities.Assets;
using TownOfUs;
using TownOfUs.Buttons;
using TownOfUs.Roles.Crewmate;
using TownOfUs.Utilities;
using UnityEngine;
using Vigilante.Assets;
using Vigilante.Roles.Impostor;

namespace Vigilante.Buttons.Impostor;

public sealed class RitualistRitualButton : TownOfUsRoleButton<RitualistRole>
{
    public override string Name => "Ritual";
    public override BaseKeybind Keybind => Keybinds.SecondaryAction;
    public override Color TextOutlineColor => TownOfUsColors.Impostor;
    public override float Cooldown => 1;
    public override LoadableAsset<Sprite> Sprite => VigilanteAssets.RitualistRitual;
    public override ButtonLocation Location => ButtonLocation.BottomRight;
    public override bool ShouldPauseInVent => false;

    protected override void OnClick()
    {
        var ritualist = PlayerControl.LocalPlayer.GetRole<RitualistRole>();

        if (ritualist != null)
        {
            ritualist.OpenPlayerMenu();
        }
    }
}
using MiraAPI.MeetingAbilities;
using MiraAPI.Modifiers;
using MiraAPI.Translation;
using MiraAPI.Utilities.Assets;
using TownOfUs;
using TownOfUs.Assets;
using TownOfUs.Modifiers.Crewmate;
using TownOfUs.Roles.Crewmate;
using UnityEngine;
using Vigilante.Roles.Impostor;

namespace Vigilante.MeetingAbilities.Crewmate;

public class DiplomatRevealButton : MeetingActionButton
{
    public override string Name => MiraLocaleManager.Get("Vigilante.Role.Diplomat.Reveal");

    public override float Cooldown => 0.0001f;

    public override float InitialCooldown => 0.0001f;

    public override int MaxUses => 0;

    public override LoadableAsset<Sprite> Sprite => TouAssets.RevealCleanSprite;

    public override bool HideUponWrapUp => true;
    public override bool DisableUponUse => true;
    public override Color TextOutlineColor => TownOfUsColors.Impostor;

    public override bool Enabled(RoleBehaviour? role)
    {
        return PlayerControl.LocalPlayer != null && role is DiplomatRole { isRevealed: false } &&
               !PlayerControl.LocalPlayer.HasModifier<JailedModifier>();
    }

    public override bool CanUse()
    {
        if (PlayerControl.LocalPlayer.Data.Role is not DiplomatRole diplomat)
        {
            return false;
        }

        

        return base.CanUse() &&
               MeetingHud.Instance.CurrentState is MeetingHud.MeetingStates.NotVoted or MeetingHud.MeetingStates.Voted && diplomat.CanReveal();
    }

    protected override void OnClick()
    {
        DiplomatRole.RpcReveal(PlayerControl.LocalPlayer);
    }
}

using AmongUs.GameOptions;
using MiraAPI.Roles;
using MiraAPI.Translation;
using TownOfUs.Modifiers;
using TownOfUs.Roles.Crewmate;
using TownOfUs.Utilities;

namespace Vigilante.Modifiers.Logic.Crewmate;

public sealed class InspectorPublishedModifier(RoleBehaviour role)
    : BaseRevealModifier
{
    public override ChangeRoleResult ChangeRoleResult { get; set; } = ChangeRoleResult.Nothing;
    public override RoleBehaviour? ShownRole { get; set; } = role;

    public override bool RevealRole { get; set; } = true;
    public override bool Visible { get; set; } = true;

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        if (ShownRole == null)
        {
            return;
        }

        if (PlayerControl.LocalPlayer.Data.IsDead)
        {
            SetNewInfo(true, roleTxt: $"{role.NameColor.ToTextColor()}{MiraLocaleManager.Get("Vigilante.Feedback.Inspector.PublishedWithIcon").Replace("<icon>", MiscUtils.GetRoleTmpIcon(role))}</color>", role2: Player.Data.Role);
        }
        else
        {
            SetNewInfo(true, roleTxt: $"{MiraLocaleManager.Get("Vigilante.Feedback.Inspector.Published")}", role2: ShownRole);
        }
    }
}
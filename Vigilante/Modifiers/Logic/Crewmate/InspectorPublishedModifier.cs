using AmongUs.GameOptions;
using MiraAPI.Roles;
using MiraAPI.Translation;
using MiraAPI.Utilities;
using TownOfUs.Modifiers;
using TownOfUs.Roles.Crewmate;
using TownOfUs.Utilities;
using Vigilante.Roles.Crewmate;

namespace Vigilante.Modifiers.Logic.Crewmate;

public sealed class InspectorPublishedModifier(RoleBehaviour role)
    : BaseRevealModifier
{
    public override ChangeRoleResult ChangeRoleResult { get; set; } = ChangeRoleResult.Nothing;
    public override RoleBehaviour? ShownRole { get; set; } = role;

    public override bool RevealRole { get; set; } = true;
    public override bool Visible { get; set; } = true;

    public override void OnActivate()
    {
        base.OnActivate();

        if (ShownRole == null)
        {
            return;
        }

        SetNewInfo(true, roleTxt: $"{MiraLocaleManager.Get("Vigilante.Feedback.Inspector.Published")}</color>", role2: role);
    }
}
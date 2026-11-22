using MiraAPI.Translation;
using TownOfUs.Modifiers;

namespace Vigilante.Modifiers.Logic.Neutral;

public sealed class AuditorRevealModifier(RoleBehaviour role)
    : BaseRevealModifier
{
    public override ChangeRoleResult ChangeRoleResult { get; set; } = ChangeRoleResult.UpdateInfo;
    public override RoleBehaviour? ShownRole { get; set; } = role;

    public override bool RevealRole { get; set; } = true;
    public override bool Visible { get; set; } = true;

}
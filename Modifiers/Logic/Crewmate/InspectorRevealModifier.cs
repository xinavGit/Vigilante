using MiraAPI.Translation;
using TownOfUs.Modifiers;

namespace Vigilante.Modifiers.Logic.Crewmate;

public sealed class InspectorRevealModifier(RoleBehaviour role)
    : BaseRevealModifier
{
    public override ChangeRoleResult ChangeRoleResult { get; set; } = ChangeRoleResult.Nothing;
    public override RoleBehaviour? ShownRole { get; set; } = role;

    public override bool RevealRole { get; set; } = true;
    public override bool Visible { get; set; } = true;

    public override void OnActivate()
    {
        base.OnActivate();
        SetNewInfo(true, roleTxt: MiraLocaleManager.Get("Vigilante.Feedback.Inspector.Published"));
    }
}
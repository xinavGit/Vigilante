using MiraAPI.Modifiers;
using MiraAPI.Translation;
using TownOfUs.Modifiers;
using TownOfUs.Utilities;

namespace Vigilante.Modifiers.Logic.Impostor;

public sealed class RitualistRitualizedModifier(RoleBehaviour role) : BaseRevealModifier
{
    public override ChangeRoleResult ChangeRoleResult { get; set; } = ChangeRoleResult.Nothing;
    public override RoleBehaviour? ShownRole { get; set; } = role;

    public override bool RevealRole { get; set; } = true;
    public override bool Visible { get; set; } = true;

    public override void FixedUpdate()
    {
        Visible = PlayerControl.LocalPlayer.IsImpostorAligned();
    }
}
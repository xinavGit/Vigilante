using AmongUs.GameOptions;
using MiraAPI.GameOptions;
using MiraAPI.Roles;
using MiraAPI.Translation;
using TownOfUs.Modifiers;
using Vigilante.Options;
using Vigilante.Roles.Impostor;

namespace Vigilante.Modifiers.Logic.Impostor;

public sealed class DiplomatRevealModifier(RoleBehaviour role)
    : BaseRevealModifier
{
    public override ChangeRoleResult ChangeRoleResult { get; set; } = ChangeRoleResult.Nothing;
    public override RoleBehaviour? ShownRole { get; set; } = role;

    public override bool RevealRole { get; set; } = true;
    public override bool Visible { get; set; } = true;

    public override bool? CanVent() => OptionGroupSingleton<DiplomatOptions>.Instance.RevealCanVent.Value;

    public float MeetingsLeft;

    public override void OnActivate()
    {
        MeetingsLeft = OptionGroupSingleton<DiplomatOptions>.Instance.MeetingsBeforeLeave.Value;
        base.OnActivate();
    }
}
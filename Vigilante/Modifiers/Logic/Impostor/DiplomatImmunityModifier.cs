using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using MiraAPI.Translation;
using TownOfUs.Modifiers;
using Vigilante.Options;

namespace Vigilante.Modifiers.Logic.Impostor;

public sealed class DiplomatImmunityModifier : BaseModifier
{
    public override string ModifierName => "immune to vote";
}
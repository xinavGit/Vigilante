using MiraAPI.GameOptions;
using MiraAPI.GameOptions.Attributes;
using MiraAPI.GameOptions.OptionTypes;
using MiraAPI.Utilities;
using Vigilante.Roles.Crewmate;
using TownOfUs.Roles.Crewmate;
using MiraAPI.Translation;

namespace Vigilante.Options;

public class MagistrateOptions : AbstractOptionGroup<MagistrateRole>
{
    public override string GroupName => MiraLocaleManager.Get("Vigilante.Role.Magistrate", "Magistrate");

    public ModdedNumberOption JudgementUses { get; } = new(
        "Vigilante.Options.Magistrate.JudgeUses", 2f, 1f, 5f, 1f, MiraNumberSuffixes.None);

    public ModdedToggleOption DisableSkip { get; } =
        new("Vigilante.Options.Magistrate.DisableSkip", true);
    
    public ModdedNumberOption EnlistCooldown { get; } = new(
        "Vigilante.Options.Magistrate.EnlistCooldown", 2f, 1f, 5f, 1f, MiraNumberSuffixes.Seconds);
}
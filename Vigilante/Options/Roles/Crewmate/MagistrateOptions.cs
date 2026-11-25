using MiraAPI.GameOptions;
using MiraAPI.GameOptions.Attributes;
using MiraAPI.GameOptions.OptionTypes;
using MiraAPI.Utilities;
using Vigilante.Roles.Crewmate;
using TownOfUs.Roles.Crewmate;
using MiraAPI.Translation;
using Vigilante.Roles.Neutral;

namespace Vigilante.Options;

public class MagistrateOptions : AbstractOptionGroup<MagistrateRole>
{
    public override string GroupName => MiraLocaleManager.Get("Vigilante.Role.Magistrate", "Magistrate");

    public ModdedNumberOption SummonsNeeded { get; } = new(
        "Vigilante.Options.Magistrate.SummonsNeeded", 3, 2, 5, 1, MiraNumberSuffixes.None);

    public ModdedNumberOption JudgementUses { get; } = new(
        "Vigilante.Options.Magistrate.JudgeUses", 2, 1, 5, 1, MiraNumberSuffixes.None);

    /*public ModdedToggleOption DisableSkip { get; } =
        new("Vigilante.Options.Magistrate.DisableSkip", true);*/
    
    public ModdedToggleOption CanCourtFirstRound { get; } =
        new("Vigilante.Options.Magistrate.CanCourtFirstRound", true);

}
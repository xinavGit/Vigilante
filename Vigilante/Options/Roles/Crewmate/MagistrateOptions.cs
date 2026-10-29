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
        "Vigilante.Options.Magistrate.JudgeUses", 3f, 1f, 5f, 1f, MiraNumberSuffixes.None);
    
    public ModdedNumberOption EnlistCooldown { get; } = new(
        "Vigilante.Options.Magistrate.EnlistCooldown", 2f, 1f, 5f, 1f, MiraNumberSuffixes.Seconds);

    public ModdedNumberOption EnlistDelay { get; } = new(
        "Vigilante.Options.Magistrate.EnlistDelay", 2f, 1f, 5f, 1f, MiraNumberSuffixes.Seconds);
    
    public ModdedToggleOption NotifyJuror { get; } =
        new("Vigilante.Options.Magistrate.NotifyJuror", false);

    public ModdedToggleOption BenignFriendly { get; } =
        new("Vigilante.Options.Magistrate.BenignFriendly", true);

    public ModdedToggleOption EvilFriendly { get; } =
        new("Vigilante.Options.Magistrate.EvilFriendly", true);
    
    public ModdedToggleOption OutlierFriendly { get; } =
        new("Vigilante.Options.Magistrate.OutlierFriendly", true);

    public ModdedToggleOption LeavesInShame { get; } =
        new("Vigilante.Options.Magistrate.LeavesInShame", true);
    
}
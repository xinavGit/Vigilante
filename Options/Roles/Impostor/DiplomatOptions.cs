using MiraAPI.GameOptions;
using MiraAPI.GameOptions.Attributes;
using MiraAPI.GameOptions.OptionTypes;
using MiraAPI.Utilities;
using Vigilante.Roles.Crewmate;
using TownOfUs.Roles.Crewmate;
using MiraAPI.Translation;
using Vigilante.Roles.Impostor;

namespace Vigilante.Options;

public class DiplomatOptions : AbstractOptionGroup<DiplomatRole>
{
    public override string GroupName => MiraLocaleManager.Get("Vigilante.Role.Diplomat", "Diplomat");

    public ModdedNumberOption RevealKCDIncrease { get; } =
        new($"Vigilante.Options.Diplomat.PostRevealKCDIncrease", 20f, 20f, 30f, 2.5f, MiraNumberSuffixes.Seconds);

    public ModdedNumberOption RevealVision { get; } =
        new($"Vigilante.Options.Diplomat.PostRevealVision", 0.50f, 0.05f, 1.00f, 0.05f, MiraNumberSuffixes.Multiplier, "0.0");

    public ModdedToggleOption RevealCanVent { get; } =
        new($"Vigilante.Options.Diplomat.PostRevealCanVent", false);
}
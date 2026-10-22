using MiraAPI.GameOptions;
using MiraAPI.GameOptions.Attributes;
using MiraAPI.GameOptions.OptionTypes;
using MiraAPI.Utilities;
using Vigilante.Roles.Crewmate;
using TownOfUs.Roles.Crewmate;
using MiraAPI.Translation;

namespace Vigilante.Options;

public class CrusaderOptions : AbstractOptionGroup<CrusaderRole>
{
    public override string GroupName => MiraLocaleManager.Get("Vigilante.Role.Crusader", "Crusader");

    public ModdedNumberOption SanctifyCooldown { get; } =
        new($"Vigilante.Options.Crusader.SanctifyCooldown", 0f, 0f, 60f, 2.5f, MiraNumberSuffixes.Seconds);

    public ModdedNumberOption SanctifyDuration { get; } =
        new($"Vigilante.Options.Crusader.SanctifyDuration", 30f, 5f, 120f, 2.5f, MiraNumberSuffixes.Seconds);

    public ModdedToggleOption PreventsKills { get; } =
        new($"Vigilante.Options.Crusader.PreventsKills", true);
}
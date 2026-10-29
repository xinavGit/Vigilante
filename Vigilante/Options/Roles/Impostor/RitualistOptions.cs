using MiraAPI.GameOptions;
using MiraAPI.GameOptions.Attributes;
using MiraAPI.GameOptions.OptionTypes;
using MiraAPI.Utilities;
using Vigilante.Roles.Crewmate;
using TownOfUs.Roles.Crewmate;
using MiraAPI.Translation;
using Vigilante.Roles.Impostor;

namespace Vigilante.Options;

public class RitualistOptions : AbstractOptionGroup<RitualistRole>
{
    public override string GroupName => MiraLocaleManager.Get("TownOfUsMira.Role.Ritualist", "Ritualist");

    public ModdedNumberOption RitualCooldown { get; } =
        new($"Vigilante.Options.Ritualist.RitualCooldown", 25f, 5f, 60f, 2.5f, MiraNumberSuffixes.Seconds);

    public ModdedNumberOption CooldownIncrease { get; set; } =
        new("Successful Ritual Cooldown Speed Increase", 1.25f, 1.1f, 3f, 0.05f,
            MiraNumberSuffixes.Multiplier, "0.00");
    
    public ModdedNumberOption CooldownDecrease { get; set; } =
        new("Failed Ritual Cooldown Speed Decrease", 0.8f, 0.1f, 0.95f, 0.05f,
            MiraNumberSuffixes.Multiplier, "0.00");
}
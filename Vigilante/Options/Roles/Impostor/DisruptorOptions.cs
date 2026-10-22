using MiraAPI.GameOptions;
using MiraAPI.GameOptions.Attributes;
using MiraAPI.GameOptions.OptionTypes;
using MiraAPI.Utilities;
using Vigilante.Roles.Crewmate;
using TownOfUs.Roles.Crewmate;
using MiraAPI.Translation;
using Vigilante.Roles.Impostor;

namespace Vigilante.Options;

public class DisruptorOptions : AbstractOptionGroup<DisruptorRole>
{
    public override string GroupName => MiraLocaleManager.Get("TownOfUsMira.Role.Disruptor", "Disruptor");

    public ModdedNumberOption DisruptorSaboCooldownMultiplier { get; set; } =
        new("KCD Cooldown Speed Multiplier During Sabo", 2f, 1.1f, 3f, 0.1f,
            MiraNumberSuffixes.Multiplier, "0.00"); // from overclocker
}
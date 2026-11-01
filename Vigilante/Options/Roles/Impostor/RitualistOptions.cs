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

    public ModdedToggleOption EliminateCI { get; } =
        new($"Vigilante.Options.Ritualist.CanRitCI", false);
}
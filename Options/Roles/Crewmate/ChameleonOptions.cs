using MiraAPI.GameOptions;
using MiraAPI.GameOptions.Attributes;
using MiraAPI.GameOptions.OptionTypes;
using MiraAPI.Utilities;
using Vigilante.Roles.Crewmate;
using TownOfUs.Roles.Crewmate;
using MiraAPI.Translation;

namespace Vigilante.Options;

public class ChameleonOptions : AbstractOptionGroup<ChameleonRole>
{
    public override string GroupName => MiraLocaleManager.Get("TownOfUsMira.Role.Chameleon", "Chameleon");

    public ModdedNumberOption MaxCamouflages { get; } = new(
        "VigilanteOptionChameleonUses", 10f, 0f, 15f, 1f, MiraNumberSuffixes.None, "0", true);

    public ModdedNumberOption CamouflageCooldown { get; } = new(
        "VigilanteOptionChameleonCooldown", 20f, 5f, 60f, 2.5f, MiraNumberSuffixes.Seconds);

    public ModdedNumberOption CamouflageDuration { get; } = new(
        "VigilanteOptionChameleonDuration", 7f, 1f, 60f, 0.25f, MiraNumberSuffixes.Seconds);

    public ModdedNumberOption CamouflageSpeedBoost { get; } = new(
        "VigilanteOptionChameleonSpeedBoost", 1.25f, 1.1f, 2f, 0.05f, MiraNumberSuffixes.Multiplier, "0.00");

    public ModdedToggleOption CanVent { get; } =
        new("VigilanteOptionChameleonCanVent", true);
}
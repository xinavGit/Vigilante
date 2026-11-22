using MiraAPI.GameOptions;
using MiraAPI.GameOptions.Attributes;
using MiraAPI.GameOptions.OptionTypes;
using MiraAPI.Utilities;
using Vigilante.Roles.Crewmate;
using TownOfUs.Roles.Crewmate;
using MiraAPI.Translation;
using Vigilante.Roles.Impostor;
using Vigilante.Roles.Neutral;

namespace Vigilante.Options;

public class AuditorOptions : AbstractOptionGroup<AuditorRole>
{
    public override string GroupName => MiraLocaleManager.Get("Vigilante.Role.Auditor", "Auditor");

    [ModdedNumberOption("Vigilante.Options.Auditor.SmokebombUses", 0f, 15f, 1f, MiraNumberSuffixes.None, "0", true)]
    public float MaxSmokebombs { get; set; } = 5f;

    [ModdedNumberOption("Vigilante.Options.Auditor.SmokebombCooldown", 5f, 120f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float SmokebombCooldown { get; set; } = 25f;

    [ModdedNumberOption("Vigilante.Options.Auditor.SmokebombDuration", 5f, 15f, 1f, MiraNumberSuffixes.Seconds)]
    public float SmokebombDuration { get; set; } = 10f;

    [ModdedNumberOption("Vigilante.Options.Auditor.SmokebombRadius", 0.25f, 5f, 0.25f, MiraNumberSuffixes.Distance, "0.00")]
    public float SmokebombRadius { get; set; } = 1.50f;

    [ModdedNumberOption("Vigilante.Options.Auditor.AuditUses", 1f, 5f, 1f, MiraNumberSuffixes.None, "0", true)]
    public float AuditUses { get; set; } = 1f;

    [ModdedNumberOption("Vigilante.Options.Auditor.AuditCooldown", 5f, 120f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float AuditCooldown { get; set; } = 25f;

    public ModdedToggleOption RoundOneAudit { get; } =
        new($"Vigilante.Options.Auditor.CanAuditRoundOne", false);
}
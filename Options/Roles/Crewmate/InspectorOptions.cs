using MiraAPI.GameOptions;
using MiraAPI.GameOptions.Attributes;
using MiraAPI.GameOptions.OptionTypes;
using MiraAPI.Utilities;
using Vigilante.Roles.Crewmate;
using TownOfUs.Roles.Crewmate;
using MiraAPI.Translation;

namespace Vigilante.Options;

public enum InspectorInspectableRoles
{
    CrewRoles,
    NonCrewRoles,
    AllRoles,
}

public class InspectorOptions : AbstractOptionGroup<InspectorRole>
{
    public override string GroupName => MiraLocaleManager.Get("Vigilante.Role.Inspector", "Inspector");

    public ModdedNumberOption InspectsPerMeeting { get; } = new(
        "Vigilante.Options.Inspector.InspectUsesPerMeeting", 1f, 1f, 3f, 1f, MiraNumberSuffixes.None);
    
    public ModdedNumberOption MaxPublishUses { get; } = new(
        "Vigilante.Options.Inspector.PublishUses", 1f, 1f, 3f, 1f, MiraNumberSuffixes.None);
    
    public ModdedEnumOption InspectableRoles { get; } = new(
        "Vigilante.Options.Inspector.InspectableRoles", (int)InspectorInspectableRoles.AllRoles,
        typeof(InspectorInspectableRoles),
        ["Vigilante.Options.Inspector.InspectableRolesEnumCrewmate", "Vigilante.Options.Inspector.InspectableRolesEnumNonCrewmate", "Vigilante.Options.Inspector.InspectableRolesEnumAll"]);

    public ModdedToggleOption PublishAllianceMods { get; } =
        new("Vigilante.Options.Inspector.PublishAllianceMods", true);
}
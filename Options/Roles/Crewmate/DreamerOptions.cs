using MiraAPI.GameOptions;
using MiraAPI.GameOptions.OptionTypes;
using MiraAPI.Translation;
using MiraAPI.Utilities;
using Vigilante.Roles.Crewmate;

namespace Vigilante.Options;

public enum DreamerReimagineRestriction
{
    Nothing,
    CrewmateKilling,
    CrewmatePower,
}

public enum DreamerOnDreamBreakMaxRoleCount
{
    ApplyRandom,
    DreamFail,
}

public class DreamerOptions : AbstractOptionGroup<DreamerRole>
{
    public override string GroupName => MiraLocaleManager.Get("TownOfUsMira.Role.Dreamer", "Dreamer");

    public ModdedToggleOption NotifyTargetOnAttempt { get; } =
        new("VigilanteOptionDreamerNotifyEvilTargetOnAttempt", true);
    
    public ModdedToggleOption NotifyTargetOfRoleOnAttempt { get; } =
        new("VigilanteOptionDreamerNotifyAttemptedRole", true)
        {
            Visible = () => OptionGroupSingleton<DreamerOptions>.Instance.NotifyTargetOnAttempt
        };

    public ModdedNumberOption InsomniaRounds { get; } = new(
        "VigilanteOptionDreamerInsomniaRounds", 1f, 1f, 3f, 1f, MiraNumberSuffixes.None);

    public ModdedEnumOption CannotReimagineInto { get; } = new(
        "VigilanteOptionDreamerReimagineRestriction", (int)DreamerReimagineRestriction.Nothing,
        typeof(DreamerReimagineRestriction),
        ["VigilanteOptionDreamerRestrictionEnumNoRestriction", "VigilanteOptionDreamerRestrictionEnumCrewmateKilling", "VigilanteOptionDreamerRestrictionEnumCrewmatePower"]);
    
    public ModdedEnumOption OnMaxRoleCountBroken { get; } = new(
        "VigilanteOptionDreamerOnUngivableDreamRole", (int)DreamerOnDreamBreakMaxRoleCount.ApplyRandom,
        typeof(DreamerOnDreamBreakMaxRoleCount),
        ["VigilanteOptionDreamerUngivableRoleEnumGiveRandom", "VigilanteOptionDreamerUngivableRoleEnumFailDream"]);
}

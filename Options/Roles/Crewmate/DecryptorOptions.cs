using MiraAPI.GameOptions;
using MiraAPI.GameOptions.Attributes;
using MiraAPI.GameOptions.OptionTypes;
using MiraAPI.Utilities;
using Vigilante.Roles.Crewmate;
using TownOfUs.Roles.Crewmate;
using MiraAPI.Translation;

namespace Vigilante.Options;

public class DecryptorOptions : AbstractOptionGroup<DecryptorRole>
{
    public override string GroupName => MiraLocaleManager.Get("TownOfUsMira.Role.Decryptor", "Decryptor");

    public ModdedNumberOption TasksPerLetter { get; } = new(
        "VigilanteOptionDecryptorTasksPerLetter", 1f, 1f, 3f, 1f, MiraNumberSuffixes.None);

    public ModdedToggleOption AlertEvils { get; } =
        new("VigilanteOptionDecryptorAlertEvils", true);

    public ModdedNumberOption LettersForAlert { get; } = new(
        "VigilanteOptionDecryptorLettersForAlert", 3f, 1f, 10f, 1f, MiraNumberSuffixes.None)
    {
        Visible = () => OptionGroupSingleton<DecryptorOptions>.Instance.AlertEvils
    };
}
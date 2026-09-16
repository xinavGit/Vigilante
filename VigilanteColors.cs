using UnityEngine;
using TownOfUs;
using MiraAPI.LocalSettings;

namespace Vigilante;

public static class VigilanteColors
{
    public static bool UseBasic { get; set; } =
        LocalSettingsTabSingleton<TouLocalTabPlayers>.Instance.UseCrewmateTeamColorToggle.Value;

    // Crew Colors
    public static Color Dreamer => UseBasic ? Palette.CrewmateBlue : new Color32(51, 51, 153, 255);
    public static Color Chameleon => UseBasic ? Palette.CrewmateBlue : new Color32(122, 220, 193, 255);
    public static Color Decryptor => UseBasic ? Palette.CrewmateBlue : new Color32(0, 102, 0, 255);
}
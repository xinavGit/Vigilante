using UnityEngine;
using TownOfUs;
using MiraAPI.LocalSettings;

namespace Vigilante;

public static class VigilanteColors
{
    public static bool UseBasic { get; set; } =
        LocalSettingsTabSingleton<TouLocalTabPlayers>.Instance.UseCrewmateTeamColorToggle.Value;

    // Crew Colors
    public static Color Chameleon => UseBasic ? Palette.CrewmateBlue : new Color32(122, 220, 193, 255);
    public static Color Decryptor => UseBasic ? Palette.CrewmateBlue : new Color32(0, 102, 0, 255);
    public static Color Inspector => UseBasic ? Palette.CrewmateBlue : new Color32(15, 40, 135, 255);
    public static Color Magistrate => UseBasic ? Palette.CrewmateBlue : new Color32(204, 153, 0, 255);

    //fixpoint
    public static Color Pacifist => UseBasic ? Palette.CrewmateBlue : new Color32(151, 226, 193, 255);
    public static Color Crusader => UseBasic ? Palette.CrewmateBlue : new Color32(237, 96, 0, 255);

    // Neut Colors
    public static Color Auditor => UseBasic ? Palette.CrewmateBlue : new Color32(0, 102, 0, 255);
    public static Color Accumulator => UseBasic ? Palette.CrewmateBlue : new Color32(18, 247, 192, 255);
    public static Color Starspawn => UseBasic ? Palette.CrewmateBlue : new Color32(255, 254, 159, 255);
}
using HarmonyLib;
using MiraAPI.GameOptions;
using TownOfUs.Options;
using Vigilante.Options;
using Vigilante.Roles.Crewmate;

namespace Vigilante.Patches;

[HarmonyPatch]
public static class VigilanteSkipButtonPatches
{
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Update))]
    public static void Postfix(MeetingHud __instance)
    {
        var genOpt = OptionGroupSingleton<MagistrateOptions>.Instance.DisableSkip;
        var court = MagistrateRole.CourtActive;

        if (genOpt || court)
        {
            __instance.SkipVoteButton.gameObject.SetActive(false);
        }
    }
}
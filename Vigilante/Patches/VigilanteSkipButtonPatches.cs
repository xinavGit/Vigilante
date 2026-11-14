using HarmonyLib;
using MiraAPI.GameOptions;
using MiraAPI.Roles;
using TownOfUs.Options;
using TownOfUs.Patches;
using Vigilante.Options;
using Vigilante.Roles.Crewmate;

namespace Vigilante.Patches;

[HarmonyPatch]
public static class VigilanteSkipButtonPatches
{
    [HarmonyPatch(typeof(SkipButtonPatches), nameof(SkipButtonPatches.Postfix))]
    public static void Prefix(MeetingHud __instance)
    {
        var genOpt = OptionGroupSingleton<MagistrateOptions>.Instance.DisableSkip;
        var court = MagistrateRole.CourtActive;
        var magi = CustomRoleUtils.GetActiveRolesOfType<MagistrateRole>();

        if (magi != null && genOpt && court)
        {
            __instance.SkipVoteButton.gameObject.SetActive(false);
        }

    }
}
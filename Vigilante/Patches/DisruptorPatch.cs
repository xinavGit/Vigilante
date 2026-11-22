using HarmonyLib;
using MiraAPI.GameOptions;
using TownOfUs.Options;
using TownOfUs.Patches;
using TownOfUs.Utilities;
using Vigilante.Options;
using Vigilante.Roles.Impostor;

namespace Vigilante.Patches;

[HarmonyPatch]
public static class DisruptorPatch
{
    [HarmonyPatch(typeof(SabotagePatches), nameof(SabotagePatches.CanLocalPlayerSabotage))]
    public static bool Postfix(bool __result)
    {
        var localPlayer = PlayerControl.LocalPlayer;
        if (localPlayer.IsRole<DisruptorRole>() &&
            ((DisruptorCanSabotage)OptionGroupSingleton<DisruptorOptions>.Instance.CanSabotage.Value == DisruptorCanSabotage.Never || ((DisruptorCanSabotage)OptionGroupSingleton<DisruptorOptions>.Instance.CanSabotage.Value == DisruptorCanSabotage.WhenSolo && MiscUtils.ImpAliveCount > 1)))
        {
            return false;
        }

        return __result;
    }
}
using HarmonyLib;
using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using MiraAPI.Patches;
using TownOfUs.Modifiers.Crewmate;
using TownOfUs.Modifiers.Game.Alliance;
using TownOfUs.Modifiers.Impostor;
using TownOfUs.Modifiers.Impostor.Venerer;
using TownOfUs.Modifiers.Neutral;
using TownOfUs.Options.Maps;
using TownOfUs.Options.Roles.Impostor;
using TownOfUs.Patches;
using TownOfUs.Utilities.Appearances;
using Vigilante.Modifiers.Logic.Crewmate;
using Vigilante.Modifiers.Logic.Impostor;
using Vigilante.Options;

namespace Vigilante.Patches;

[HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.CalculateLightRadius))]
public static class VigilanteVisionPatch
{
    public static void Postfix(ShipStatus __instance, NetworkedPlayerInfo player, ref float __result)
    {
        if (player._object.HasModifier<DiplomatRevealModifier>())
        {
            __result *= OptionGroupSingleton<DiplomatOptions>.Instance.RevealVision.Value;
        }
    }
}
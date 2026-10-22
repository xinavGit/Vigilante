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
using Vigilante.Options;

namespace Vigilante.Patches;

[HarmonyPatch(typeof(LogicOptions), nameof(LogicOptions.GetPlayerSpeedMod))]
public static class SpeedPatch
{
    public static void Postfix(PlayerControl pc, ref float __result)
    {
        __result *= TownOfUsMapOptions.GetMapBasedSpeedMultiplier();
        __result *= EgotistModifier.SpeedMultiplier;
        if (!(TownOfUs.Patches.HudManagerPatches.CamouflageCommsEnabled &&
             OptionGroupSingleton<AdvancedSabotageOptions>.Instance.HidePlayerSpeedInCamo))
        {
            __result *= pc.GetAppearance().Speed;
        }

        if (pc.HasModifier<ChameleonCamouflageModifier>())
        {
            __result *= OptionGroupSingleton<ChameleonOptions>.Instance.CamouflageSpeedBoost.Value;
        }
    }
}
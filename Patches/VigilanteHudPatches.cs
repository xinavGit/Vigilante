using AmongUs.GameOptions;
using HarmonyLib;
using MiraAPI.Modifiers;
using TownOfUs;
using TownOfUs.Modules.Components;
using TownOfUs.Roles;
using TownOfUs.Utilities;
using UnityEngine;
using Vigilante.Modifiers.Logic.Crewmate;
using Vigilante.Roles.Crewmate;

namespace Vigilante.Patches;

[HarmonyPatch(typeof(HudManagerHelper), "GetRoleNameText")]
public static class VigilanteHudPatches
{
    public static void Postfix(PlayerControl player, ref (Color PlayerColor, string PlayerName) __result)
    {
        if (PlayerControl.LocalPlayer == null)
        {
            return;
        }

        var (playerColor, playerName) = __result;

        if (player.HasModifier<MagistrateJurorModifier>() &&
             PlayerControl.LocalPlayer.IsRole<MagistrateRole>())
        {
            playerName += "<color=#cc9900> +</color>";
        }

        if (PlayerControl.LocalPlayer.Data.Role is DecryptorRole)
        {
            if (player.Data.IsDead)
                return;


            playerName += "\n";

            // Safely get this player's revealed letters, or an empty list if they don't have any yet
            if (!DecryptorRole.KnownCharacters.TryGetValue(player.PlayerId, out var customText))
            {
                customText = [];
            }

            foreach (var letter in customText)
            {
                playerName += $"<size=60%><color=#FFFFFF>{letter}</color></size>";
            }
        }

        __result = (playerColor, playerName);
    }
}
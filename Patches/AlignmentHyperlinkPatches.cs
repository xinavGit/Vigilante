/*using AmongUs.GameOptions;
using HarmonyLib;
using MiraAPI.Modifiers;
using TMPro;
using TownOfUs;
using TownOfUs.Modules.Components;
using TownOfUs.Patches.Misc;
using TownOfUs.Roles;
using TownOfUs.Utilities;
using UnityEngine;
using Vigilante.Modifiers.Logic.Crewmate;
using Vigilante.Roles.Crewmate;

namespace Vigilante.Patches;

[HarmonyPatch(typeof(WikiHyperLinkPatches), nameof(WikiHyperLinkPatches.CheckForTags))]
public static class AlignmentHyperlinkPatches
{
    public static void Prefix(ref (string text, TextMeshPro tmp))
    {
        if (PlayerControl.LocalPlayer.Data.Role is DecryptorRole)
        {
            if (player.Data.IsDead)
                return;

            var (playerColor, playerName) = __result;

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

            __result = (playerColor, playerName);
        }
    }
}*/
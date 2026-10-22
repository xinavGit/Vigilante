using AmongUs.GameOptions;
using HarmonyLib;
using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using MiraAPI.Translation;
using TownOfUs;
using TownOfUs.Modules.Components;
using TownOfUs.Roles;
using TownOfUs.Utilities;
using UnityEngine;
using Vigilante.Modifiers.Logic.Crewmate;
using Vigilante.Options;
using Vigilante.Roles.Crewmate;

namespace Vigilante.Patches;
//using TOUM MeetingHudTimerPatch code
[HarmonyPatch(typeof(MeetingHud))]
public static class MeetingHudTimerPatch
{
    [HarmonyPostfix]
    [HarmonyPatch(nameof(MeetingHud.UpdateTimerText))]
    public static void Postfix(MeetingHud __instance)
    {
        var newText = string.Empty;
        if (!PlayerControl.LocalPlayer || !PlayerControl.LocalPlayer.Data ||
            PlayerControl.LocalPlayer.HasDied())
        {
            return;
        }

        switch (PlayerControl.LocalPlayer.Data.Role)
        {
            case InspectorRole insp:
                newText =
                    $"\n{insp.RoundInspects} / {(int)OptionGroupSingleton<InspectorOptions>.Instance.InspectsPerMeeting} {MiraLocaleManager.Get("Vigilante.Feedback.Inspector.InspectsRemaining")}";

                break;
            case MagistrateRole magi:
                newText =
                    $"\n{magi.JudgementUsesLeft} {MiraLocaleManager.Get("Vigilante.Feedback.Magistrate.JudgementsRemaining")}";

                break;
        }

        if (newText != string.Empty)
        {
            __instance.TimerText.text += $"<color=#FFFFFF>{newText}</color>";
        }
    }
}
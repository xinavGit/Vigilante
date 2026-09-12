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
   
}
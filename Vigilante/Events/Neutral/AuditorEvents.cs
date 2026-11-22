using System.Linq;
using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Gameplay;
using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using Vigilante.Modifiers.Logic.Crewmate;
using Vigilante.Options;
using Vigilante.Roles.Crewmate;
using TownOfUs.Extensions;
using TownOfUs.Utilities;
using MiraAPI.Utilities;
using MiraAPI.Events.Vanilla.Player;
using Il2CppSystem.Runtime.InteropServices;
using MiraAPI.Roles;
using MiraAPI.Hud;
using Vigilante.Buttons.Crewmate;
using MiraAPI.Events.Vanilla.Meeting.Voting;
using TownOfUs.Modules;
using HarmonyLib;
using Vigilante.Modifiers.Logic.Neutral;
using AmongUs.GameOptions;
using Vigilante.Roles.Neutral;

namespace Vigilante.Events.Neutral;

public static class AuditorEvents
{
    [RegisterEvent]
    public static void OnRoundStart(RoundStartEvent evt)
    {
        foreach (var player in ModifierUtils.GetPlayersWithModifier<AuditorAuditTargetModifier>().ToList())
        {
            if (player.IsCrewmate())
                    {
                        var originalRole = player.GetRoleWhenAlive();
                        player.RemoveModifier<AuditorAuditTargetModifier>();
                        if (player.AmOwner)
                        {
                            player.RpcSetRole(RoleTypes.Crewmate);
                        }
                        player.AddModifier<AuditorAuditedModifier>(originalRole);
                    }
        }
    }

    [RegisterEvent]
    public static void AfterMurderEvent(AfterMurderEvent evt)
    {
        var victim = evt.Target;

        if (victim.GetRoleWhenAlive() is AuditorRole)
        {
            foreach (var player in PlayerControl.AllPlayerControls.ToArray())
            {
                Info("d");
                if (player.TryGetModifier<AuditorAuditedModifier>(out var mod))
                {
                    player.RpcSetRole(mod.CachedRole.Role);
                    player.RpcRemoveModifier<AuditorAuditedModifier>();
                }
            }
        }
    }
}
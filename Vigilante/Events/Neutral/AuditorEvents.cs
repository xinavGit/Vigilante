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
            Info("1");
            player.RemoveModifier<AuditorAuditTargetModifier>();
            if (player.IsCrewmate())
                    {
                        Info("2");
                        player.AddModifier<AuditorAuditedModifier>(player.Data.Role);
                        if (PlayerControl.LocalPlayer.IsHost())
                        {
                            Info("3");
                            player.RpcSetRole(RoleTypes.Crewmate);
                        }
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
                if (player.TryGetModifier<AuditorAuditedModifier>(out var mod))
                {
                    if (PlayerControl.LocalPlayer.IsHost())
                    {
                        player.RpcSetRole(mod.CachedRole.Role);
                        player.RpcRemoveModifier<AuditorAuditedModifier>();
                    }
                }
            }
        }
    }
}
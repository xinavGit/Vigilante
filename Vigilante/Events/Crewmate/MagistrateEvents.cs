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

namespace Vigilante.Events.Crewmate;

public static class MagistrateEvents
{
    [RegisterEvent]
    public static void OnRoundStart(RoundStartEvent evt)
    {
        if (evt.TriggeredByIntro)
        {
            return;
        }

        if (PlayerControl.LocalPlayer.IsHost())
        {
            MagistrateRole.RpcEndCourt();
        }
    }

    [RegisterEvent]
    public static void AfterMurderEvent(AfterMurderEvent evt)
    {
        var victim = evt.Target;

        if (victim.HasModifier<MagistrateJurorModifier>())
        {
            foreach (var magi in CustomRoleUtils.GetActiveRolesOfType<MagistrateRole>())
            {
                magi.ClearMenu();
            }
        }

        if ((victim == MagistrateRole.CourtTarget || victim == MagistrateRole.CourtJuror || victim.IsRole<MagistrateRole>()) && MagistrateRole.CourtActive && MagistrateRole.EnemiesCourt)
        {
            MagistrateRole.RpcEndCourt(true);
        }
    }

    [RegisterEvent]
    public static void OnMeetingSelect(MeetingSelectEvent evt)
    {
        if (GameData.Instance == null)
        return;

        var playerData = GameData.Instance.GetPlayerById((byte)evt.TargetId);
        var player = playerData?.Object;

        if (player == null || player.Data == null)
            return;

        if (player.HasModifier<MagistrateCourtImmunityModifier>())
        {
            evt.AllowSelect = false;
        }
    }
}
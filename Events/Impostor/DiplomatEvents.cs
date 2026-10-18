using System.Linq;
using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Gameplay;
using MiraAPI.Events.Vanilla.Meeting;
using MiraAPI.Events.Vanilla.Meeting.Voting;
using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using MiraAPI.Voting;
using TownOfUs.Utilities;
using Vigilante.Modifiers.Logic.Impostor;
using Vigilante.Options;
using Vigilante.Roles.Impostor;

namespace Vigilante.Events.Impostor;

public static class DiplomatEvents
{
    [RegisterEvent]
    public static void OnProcessVotes(ProcessVotesEvent evt)
    {
        var diplomat = PlayerControl.AllPlayerControls.ToArray()
            .FirstOrDefault(p => p.Data != null && p.Data.Role is DiplomatRole);

        if (diplomat == null || diplomat.Data.IsDead) return;
        if (diplomat.Data.Role is not DiplomatRole diplomatRole) return;
        if (diplomatRole.manipulatedChosen == null || diplomatRole.manipulatedChosen.Count == 0) return;

        var diplomatVote = evt.Votes.FirstOrDefault(v => v.Voter == diplomat.PlayerId);

        byte diplomatTargetId = diplomatVote.Suspect;

        foreach (var targetPlayer in diplomatRole.manipulatedChosen)
        {
            if (targetPlayer == null || targetPlayer.Data == null || targetPlayer.Data.IsDead) continue;

            var targetVoteIndex = evt.Votes.FindIndex(v => v.Voter == targetPlayer.PlayerId);
            if (targetVoteIndex < 0) continue;

            var targetVote = evt.Votes[targetVoteIndex];
            evt.Votes[targetVoteIndex] = new CustomVote(targetVote.Voter, diplomatTargetId);
        }
    }

    [RegisterEvent]
    public static void OnMeetingSelect(MeetingSelectEvent evt)
    {
        var player = GameData.Instance.GetPlayerById((byte)evt.TargetId)?.Object;
        if (player.HasModifier<DiplomatImmunityModifier>())
        {
            evt.AllowSelect = false;
        }
    }

    [RegisterEvent]
    public static void OnMeetingEnd(EndMeetingEvent evt)
    {
        foreach (var immune in ModifierUtils.GetPlayersWithModifier<DiplomatImmunityModifier>().ToList())
        {
            immune.RemoveModifier<DiplomatImmunityModifier>();
        }
    }

    [RegisterEvent]
    public static void AfterMurder(AfterMurderEvent evt)
    {
        if (evt.Source.Data.Role is DiplomatRole diplomat && diplomat.isRevealed)
        {
            evt.Source.SetKillTimer(PlayerControl.LocalPlayer.GetKillCooldown() + OptionGroupSingleton<DiplomatOptions>.Instance.RevealKCDIncrease.Value);
        }
    }
}
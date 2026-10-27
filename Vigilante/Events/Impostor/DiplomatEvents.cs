using System.Linq;
using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Gameplay;
using MiraAPI.Events.Vanilla.Meeting;
using MiraAPI.Events.Vanilla.Meeting.Voting;
using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using MiraAPI.Networking;
using MiraAPI.Roles;
using MiraAPI.Translation;
using MiraAPI.Utilities;
using MiraAPI.Voting;
using TownOfUs.Modules;
using TownOfUs.Modules.Components;
using TownOfUs.Networking;
using TownOfUs.Utilities;
using UnityEngine;
using Vigilante.Assets;
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
            .FirstOrDefault(p =>
                p != null &&
                p.Data != null &&
                p.Data.Role is DiplomatRole);

        if (diplomat?.Data == null || diplomat.Data.IsDead)
            return;

        if (diplomat.Data.Role is not DiplomatRole diplomatRole)
            return;

        if (diplomatRole.manipulatedChosen == null ||
            diplomatRole.manipulatedChosen.Count == 0)
            return;

        var diplomatVoteIndex = evt.Votes.FindIndex(
            v => v.Voter == diplomat.PlayerId);

        if (diplomatVoteIndex < 0)
            return;

        byte diplomatTargetId = evt.Votes[diplomatVoteIndex].Suspect;

        foreach (var targetPlayer in diplomatRole.manipulatedChosen)
        {
            if (targetPlayer?.Data == null || targetPlayer.Data.IsDead)
                continue;

            var targetVoteIndex = evt.Votes.FindIndex(
                v => v.Voter == targetPlayer.PlayerId);

            if (targetVoteIndex < 0)
                continue;

            var targetVote = evt.Votes[targetVoteIndex];

            evt.Votes[targetVoteIndex] = new CustomVote(
                targetVote.Voter,
                diplomatTargetId);
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

        if (player.HasModifier<DiplomatImmunityModifier>())
        {
            evt.AllowSelect = false;
        }
    }

    [RegisterEvent]
    public static void OnMeetingStart(StartMeetingEvent evt)
    {
        Info($"z");
        if (PlayerControl.LocalPlayer.IsRole<DiplomatRole>() && PlayerControl.LocalPlayer.TryGetModifier<DiplomatRevealModifier>(out var mod))
        {
            Info($"zz{mod.MeetingsLeft}");
            if (mod.MeetingsLeft > 0)
            {
                Info($"zzz{mod.MeetingsLeft}");
                mod.MeetingsLeft--;
                if (mod.MeetingsLeft == 0)
                {
                    //notif here but doesnt work
                }
            }
        }
    }

    [RegisterEvent]
    public static void OnVotingComplete(VotingCompleteEvent evt)
    {
        foreach (var immune in ModifierUtils.GetPlayersWithModifier<DiplomatImmunityModifier>().ToList())
        {
            immune.RemoveModifier<DiplomatImmunityModifier>();
        }

        foreach (var diplomat in CustomRoleUtils.GetActiveRolesOfType<DiplomatRole>())
        {
            if (diplomat.Player.HasDied())
            {
                return;
            }

            if (PlayerControl.LocalPlayer.TryGetModifier<DiplomatRevealModifier>(out var mod) && mod.MeetingsLeft == 0)
            {
                if (PlayerControl.LocalPlayer.IsHost())
                {
                    diplomat.Player.RpcMeetingMurder(diplomat.Player, MeetingAnimation.PlayerNameplateAnimation, CustomTouMurderRpcs.GetRandomMeetingAnim(DeathAnimType.Nameplate),
                        causeOfDeath: "DiplomatLeave");

                    Info("killing diplomat!");
                }
            }
            
        }
    }

    /*[RegisterEvent]
    public static void OnDisplayVotes(PopulateResultsEvent evt)
    {
        if (PlayerControl.LocalPlayer.IsRole<DiplomatRole>() && PlayerControl.LocalPlayer.TryGetModifier<DiplomatRevealModifier>(out var mod) && mod.MeetingsLeft == 0)
        {
            var player = PlayerControl.LocalPlayer;

            player.RpcMeetingMurder(player, MeetingAnimation.PlayerNameplateAnimation, CustomTouMurderRpcs.GetRandomMeetingAnim(DeathAnimType.Nameplate),
                causeOfDeath: "DiplomatLeave");
        }
    }*/

    [RegisterEvent]
    public static void AfterMurder(AfterMurderEvent evt)
    {
        if (evt.Source.Data.Role is DiplomatRole diplomat && diplomat.isRevealed)
        {
            evt.Source.SetKillTimer(PlayerControl.LocalPlayer.GetKillCooldown() + OptionGroupSingleton<DiplomatOptions>.Instance.RevealKCDIncrease.Value);
        }
    }
}
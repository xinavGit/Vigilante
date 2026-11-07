using AmongUs.GameOptions;
using System.Linq;
using Vigilante.Assets;
using Vigilante.Modifiers.Logic.Crewmate;
using Vigilante.Options;
using Il2CppInterop.Runtime.Attributes;
using Il2CppSystem.Runtime.InteropServices;
using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using MiraAPI.Patches.Stubs;
using MiraAPI.Roles;
using MiraAPI.Utilities;
using Reactor.Networking.Attributes;
using TownOfUs;
using TownOfUs.Extensions;
using TownOfUs.Modules;
using TownOfUs.Modules.Components;
using TownOfUs.Modules.Wiki;
using TownOfUs.Roles;
using TownOfUs.Roles.Crewmate;
using TownOfUs.Utilities;
using UnityEngine;
using TownOfUs.Modifiers;
using System.Text;
using MiraAPI.Translation;
//using Vigilante.Utilities;
using MiraAPI.Utilities.Assets;
using TownOfUs.Modifiers.Crewmate;
using TownOfUs.Assets;
using MiraAPI.Hud;
using TownOfUs.Buttons;
using Vigilante.Interfaces;
using Vigilante.Utilities;
using TownOfUs.Networking;
using Reactor.Utilities.Extensions;
using Reactor.Utilities;
using MiraAPI.Voting;
using Vigilante.Buttons.Crewmate;

namespace Vigilante.Roles.Crewmate;
//using TOUM monarch role code for enlist rpc
#pragma warning disable CA1001
public sealed class MagistrateRole(IntPtr cppPtr) : CrewmateRole(cppPtr), IVigilanteRole, IWikiDiscoverable, IDoomable
#pragma warning restore CA1001
{
    private MeetingMenu? judgeMenu;

    public string IdPart => "Magistrate";
    public Color RoleColor => VigilanteColors.Magistrate;
    public ModdedRoleTeams Team => ModdedRoleTeams.Crewmate;
    public RoleAlignment RoleAlignment => RoleAlignment.CrewmatePower;

    public string RoleName => MiraLocaleManager.Get($"Vigilante.Role.{IdPart}");
    public string RoleDescription => MiraLocaleManager.Get($"Vigilante.Role.{IdPart}.IntroBlurb");
    public string RoleLongDescription => MiraLocaleManager.Get($"Vigilante.Role.{IdPart}.TabDescription");

    public DoomableType DoomHintType => DoomableType.Perception;

    public float JudgementUsesLeft { get; set; }

    public static bool CourtActive { get; set; }
    public static bool EnemiesCourt { get; set; }

    [HideFromIl2Cpp] public static PlayerControl? CourtJuror { get; set; }
    [HideFromIl2Cpp] public static PlayerControl? CourtTarget { get; set; }


    public string GetAdvancedDescription()
    {
        return
            MiraLocaleManager.Get($"Vigilante.Role.{IdPart}.WikiDescription") +
            MiscUtils.AppendOptionsText(GetType());
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities
    {
        get
        {
            return
            [
                new(MiraLocaleManager.Get($"Vigilante.Role.{IdPart}.Enlist", "Enlist"),
                    MiraLocaleManager.Get($"Vigilante.Role.{IdPart}.Enlist.WikiDescription"),
                    TouRoleIcons.Prosecutor),
                new(MiraLocaleManager.Get($"Vigilante.Role.{IdPart}.Judge", "Judge"),
                    MiraLocaleManager.Get($"Vigilante.Role.{IdPart}.Judge.WikiDescription"),
                    VigilanteAssets.MagistrateJudgement)//btos2 judge sprite here!
            ];
        }
    }

    public CustomRoleConfiguration Configuration => new(this)
    {
        IconTmp = TmpSpriteUtils.CreateSpriteAsset(VigilanteAssets.JudgeIcon.LoadAsset(), "Vigilante.Role.Crewmate.Judge", 1.45f),
        Icon = VigilanteAssets.JudgeIcon,
        IntroSound = TouAudio.DetectiveIntroSound,
        MaxRoleCount = 1
    };

    public override void Initialize(PlayerControl player)
    {
        RoleBehaviourStubs.Initialize(this, player);
        CourtActive = false;
        JudgementUsesLeft = OptionGroupSingleton<MagistrateOptions>.Instance.JudgementUses.Value;

        CourtJuror = null;
        CourtTarget = null;
        EnemiesCourt = false;

        judgeMenu = new MeetingMenu(
                this,
                ClickJudge,
                MeetingAbilityType.Click,
                VigilanteAssets.MagistrateJudgement,
                null!,
                IsJudgementExempt,
                position: new Vector3(-0.35f, 0f, -3f));
    }

    public override void OnMeetingStart()
    {
        RoleBehaviourStubs.OnMeetingStart(this);
        CourtActive = false;

        var juror = ModifierUtils.GetPlayersWithModifier<MagistrateJurorModifier>().ToList().FirstOrDefault();
        var meeting = MeetingHud.Instance;
        if (Player.AmOwner && meeting != null && !Player.HasDied() && juror != null)
        {
            judgeMenu?.GenButtons(meeting, true);
        }
    }

    [HideFromIl2Cpp]
    public bool IsJudgementExempt(PlayerVoteArea voteArea)
    {
        if (JudgementUsesLeft <= 0)
        {
            return true;
        }

        if (voteArea == null || voteArea.PlayerId == Player.PlayerId)
        {
            return true;
        }

        var target = voteArea.GetPlayer();

        if (target == null || target.HasDied())// check if target has been successfully inspected again
        {
            return true;
        }

        if (target.HasModifier<MagistrateJurorModifier>())
        {
            return true;
        }

        return false;
    }

    public void ClickJudge(PlayerVoteArea voteArea, MeetingHud __)
    {
        var target = GameData.Instance.GetPlayerById(voteArea.PlayerId).Object;
        var juror = ModifierUtils.GetPlayersWithModifier<MagistrateJurorModifier>().ToList().FirstOrDefault();

        if (juror == null)
        {
            if (Player.AmOwner)
            {
                ClearMenu();
            }

            return;
        }

        RpcCourt(target, juror);

        if (Player.AmOwner)
        {
            JudgementUsesLeft--;
            ClearMenu();
        }
    }
    
    [MethodRpc((uint)VigilanteRpcs.MagistrateEndCourt)]
    public static void RpcEndCourt(bool early = false)
    {
        foreach (var jurorPlayer in ModifierUtils.GetPlayersWithModifier<MagistrateJurorModifier>().ToList())
        {
            jurorPlayer.RemoveModifier<MagistrateJurorModifier>();
        }

        foreach (var immunePlayer in ModifierUtils.GetPlayersWithModifier<MagistrateCourtImmunityModifier>().ToList())
        {
            immunePlayer.RemoveModifier<MagistrateCourtImmunityModifier>();
        }

        var button = CustomButtonSingleton<MagistrateEnlistButton>.Instance;
        ++button.UsesLeft;
        button.SetUses(button.UsesLeft);

        CourtJuror = null;
        CourtTarget = null;

        EnemiesCourt = false;

        CourtActive = false;

        if (early)
        {
            var msg = MiraLocaleManager.Get("Vigilante.Feedback.Magistrate.CourtEarlyEnd");
            var notif1 = Helpers.CreateAndShowNotification(
                $"<b>{VigilanteColors.Magistrate.ToTextColor()}{msg}</b></color>",
                Color.white, new Vector3(0f, 1f, -20f), spr: VigilanteAssets.JudgeIcon.LoadAsset());
            notif1.AdjustNotification();
        }
    }

    [MethodRpc((uint)VigilanteRpcs.MagistrateCourt)]
    public static void RpcCourt(PlayerControl target, PlayerControl juror)
    {
        var targetName = target.CachedPlayerData.PlayerName;
        var jurorName = juror.CachedPlayerData.PlayerName;

        CourtJuror = juror;
        CourtTarget = target;

        Coroutines.Start(MiscUtils.CoFlash(VigilanteColors.Magistrate));
        TouAudio.PlaySound(VigilanteAssets.CourtGavel);

        CourtActive = true;
        
        var msg = MiraLocaleManager.Get("Vigilante.Feedback.Magistrate.CourtStart");
        var notif1 = Helpers.CreateAndShowNotification(
            $"<b>{VigilanteColors.Magistrate.ToTextColor()}{msg}</b></color>",
            Color.white, new Vector3(0f, 1f, -20f), spr: VigilanteAssets.JudgeIcon.LoadAsset());
        notif1.AdjustNotification();

        if (VigilanteUtils.AreFriendly(juror, target))
        {
            var msg2a = MiraLocaleManager.Get("Vigilante.Feedback.Magistrate.CourtFriends").Replace("<player1>", $"{targetName}").Replace("<player2>", $"{jurorName}");
            var notif2a = Helpers.CreateAndShowNotification(
                $"<b>{msg2a}</b></color>",
                Color.green, new Vector3(0f, 1f, -20f), spr: VigilanteAssets.JudgeIcon.LoadAsset());
            notif2a.AdjustNotification();

            if (PlayerControl.LocalPlayer.IsHost())
            {
                target.AddModifier<MagistrateCourtImmunityModifier>();
                juror.AddModifier<MagistrateCourtImmunityModifier>();
            }
        }
        else
        {
            var msg2b = MiraLocaleManager.Get("Vigilante.Feedback.Magistrate.CourtEnemies").Replace("<player1>", $"{targetName}").Replace("<player2>", $"{jurorName}");
            var notif2b = Helpers.CreateAndShowNotification(
                $"<b>{msg2b}</b></color>",
                Color.red, new Vector3(0f, 1f, -20f), spr: VigilanteAssets.JudgeIcon.LoadAsset());
            notif2b.AdjustNotification();

            EnemiesCourt = true;

            if (PlayerControl.LocalPlayer.IsHost())
            {
                foreach (var player in PlayerControl.AllPlayerControls.ToArray())
                {
                    if (player.PlayerId == target.PlayerId || player.PlayerId == juror.PlayerId)
                    {
                        continue;
                    }

                    player.AddModifier<MagistrateCourtImmunityModifier>();
                }
            }
        }

        if (PlayerControl.LocalPlayer.IsHost())
        {
            foreach (var voter in Helpers.GetAlivePlayers())
            {
                var voteData = voter.GetVoteData();
                var votedTargets = voteData.Votes
                    .Select(vote => vote.Suspect)
                    .Distinct()
                    .ToArray();

                foreach (var suspectId in votedTargets)
                {
                    var removedVotes = voteData.Votes.RemoveAll(vote => vote.Suspect == suspectId);
                    voteData.VotesRemaining += removedVotes;

                    for (var i = 0; i < removedVotes; i++)
                    {
                        VotingUtils.RpcRemoveVote(
                            PlayerControl.LocalPlayer,
                            voter.PlayerId,
                            suspectId);
                    }
                }
            }
        }
    }

    public void ClearMenu()
    {
        judgeMenu?.HideButtons();
    }

    public override void Deinitialize(PlayerControl targetPlayer)
    {
        RoleBehaviourStubs.Deinitialize(this, targetPlayer);
        if (Player.AmOwner)
        {
            
        }
    }

    [MethodRpc((uint)VigilanteRpcs.MagistrateEnlist)]
    public static void RpcEnlist(PlayerControl player, PlayerControl target)
    {
        if (LobbyBehaviour.Instance)
        {
            MiscUtils.RunAnticheatWarning(player);
            return;
        }
        if (player.Data.Role is not MagistrateRole magi)
        {
            return;
        }
        var targetName = target.CachedPlayerData.PlayerName;
        var icon = VigilanteAssets.JudgeIcon.LoadAsset();

        if (target.HasDied())
        {
            if (player.AmOwner)
            {
                ShowNotification(MiraLocaleManager.Get("Vigilante.Feedback.Magistrate.EnlistingFail").Replace("<player>", targetName));
            }
            return;
        }

        target.AddModifier<MagistrateJurorModifier>();

        if (player.AmOwner)
        {
            ShowNotification(MiraLocaleManager.Get("Vigilante.Feedback.Magistrate.EnlistingSuccess").Replace("<player>", targetName));
        }

        if (target.AmOwner && OptionGroupSingleton<MagistrateOptions>.Instance.NotifyJuror)
        {
            ShowNotification(MiraLocaleManager.Get("Vigilante.Feedback.Magistrate.JurorNotify"));
        }


        void ShowNotification(string message)
        {
            var notif = Helpers.CreateAndShowNotification($"<b>{message}</b>", Color.white, new Vector3(0f, 1f, -20f), spr: icon);
            notif.Text.SetOutlineThickness(0.35f);
        }
    }
}
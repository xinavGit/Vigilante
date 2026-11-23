/*using AmongUs.GameOptions;
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
using HarmonyLib;
using TownOfUs.Roles.Neutral;

namespace Vigilante.Roles.Crewmate;
//using TOUM monarch role code for enlist rpc
#pragma warning disable CA1001
public sealed class MagistrateRole(IntPtr cppPtr) : NeutralRole(cppPtr), IVigilanteRole, IWikiDiscoverable, IDoomable, ICrewVariant
#pragma warning restore CA1001
{
    public override void SpawnTaskHeader(PlayerControl playerControl)
    {
        if (!playerControl.AmOwner)
        {
            return;
        }
        ImportantTextTask orCreateTask = PlayerTask.GetOrCreateTask<ImportantTextTask>(playerControl, 0);
        orCreateTask.Text = $"{TownOfUsColors.Neutral.ToTextColor()}{MiraLocaleManager.Get("NeutralEvilTaskHeader")}</color>";
        orCreateTask.name = "NeutralRoleText";
    }

    private MeetingMenu? judgeMenu;

    public string IdPart => "Magistrate";
    public Color RoleColor => VigilanteColors.Magistrate;
    public ModdedRoleTeams Team => ModdedRoleTeams.Custom;
    public RoleAlignment RoleAlignment => RoleAlignment.NeutralEvil;

    public string RoleName => MiraLocaleManager.Get($"Vigilante.Role.{IdPart}");
    public string RoleDescription => MiraLocaleManager.Get($"Vigilante.Role.{IdPart}.IntroBlurb");
    public string RoleLongDescription => MiraLocaleManager.Get($"Vigilante.Role.{IdPart}.TabDescription");

    public DoomableType DoomHintType => DoomableType.Fearmonger;
    public RoleBehaviour CrewVariant => RoleManager.Instance.GetRole((RoleTypes)RoleId.Get<ProsecutorRole>());

    public float JudgementUsesLeft { get; set; }

    public static bool CourtActive { get; set; }


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
                new(MiraLocaleManager.Get($"Vigilante.Role.{IdPart}.Court", "Court"),
                    MiraLocaleManager.Get($"Vigilante.Role.{IdPart}.Court.WikiDescription"),
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

        judgeMenu = new MeetingMenu(
                this,
                ClickJudge,
                MiraLocaleManager.Get("Vigilante.Role.Magistrate.Court"),
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
        var summoned = ModifierUtils.GetPlayersWithModifier<MagistrateJurorModifier>().ToList();

        if (__.state == MeetingHud.MeetingStates.Discussion)
        {
            return;
        }

        if (summoned.Count <= OptionGroupSingleton<MagistrateOptions>.Instance.SummonsNeeded.Value)
        {
            if (Player.AmOwner)
            {
                ClearMenu();
            }

            return;
        }

        RpcCourt(summoned);

        if (Player.AmOwner)
        {
            JudgementUsesLeft--;
            ClearMenu();
        }
    }
    
    [MethodRpc((uint)VigilanteRpcs.MagistrateEndCourt)]
    public static void RpcEndCourt(bool early = false)
    {
        if (PlayerControl.LocalPlayer.IsHost())
        {
            ModifierUtils.GetPlayersWithModifier<MagistrateJurorModifier>().Do(x => x.RpcRemoveModifier<MagistrateJurorModifier>());
            ModifierUtils.GetPlayersWithModifier<MagistrateCourtImmunityModifier>().Do(x => x.RpcRemoveModifier<MagistrateCourtImmunityModifier>());
        }

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
    public static void RpcCourt(List<PlayerControl> summoned)
    {

        var magistrate = CustomRoleUtils.GetActiveRolesOfType<MagistrateRole>().FirstOrDefault();


        if (!EnemySummoned(summoned))
        {
            if (PlayerControl.LocalPlayer == magistrate?.Player)
            {
                var msg2a = MiraLocaleManager.Get("Vigilante.Feedback.Magistrate.FailedCourt");
            var notif2a = Helpers.CreateAndShowNotification(
                $"<b>{msg2a}</b></color>",
                Color.white, new Vector3(0f, 1f, -20f), spr: VigilanteAssets.JudgeIcon.LoadAsset());
            notif2a.AdjustNotification();

                MiscUtils.AddFakeChat(magistrate.Player.Data, MiraLocaleManager.Get("Vigilante.Feedback.Magistrate.MagistrateFeedback"), MiraLocaleManager.Get("Vigilante.Feedback.Magistrate.FailedCourt"), false, true);
            }
        }
        else
        {
            Coroutines.Start(MiscUtils.CoFlash(VigilanteColors.Magistrate));
            TouAudio.PlaySound(VigilanteAssets.CourtGavel);

            var msg2b = MiraLocaleManager.Get("Vigilante.Feedback.Magistrate.CalledCourt");
            var notif2b = Helpers.CreateAndShowNotification(
                $"<b>{msg2b}</b></color>",
                Color.white, new Vector3(0f, 1f, -20f), spr: VigilanteAssets.JudgeIcon.LoadAsset());
            notif2b.AdjustNotification();

            foreach (var player in PlayerControl.AllPlayerControls.ToArray())
            {
                if (summoned.Contains(player))
                {
                    continue;
                }

                player.AddModifier<MagistrateCourtImmunityModifier>();
            }

            CourtActive = true;

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

    [HideFromIl2Cpp]
    public static bool EnemySummoned(List<PlayerControl> summoned)
    {
        foreach (var summonedPlayer in summoned)
        {
            if (summonedPlayer.IsImpostor() || summonedPlayer.Data.Role.GetRoleAlignment() == RoleAlignment.NeutralKilling)
            {
                return true;
            }

            if ((summonedPlayer.Data.Role.GetRoleAlignment() == RoleAlignment.NeutralKilling || summonedPlayer.Data.Role.GetRoleAlignment() == RoleAlignment.NeutralEvil || summonedPlayer.Data.Role.GetRoleAlignment() == RoleAlignment.NeutralBenign || summonedPlayer.Data.Role.GetRoleAlignment() == RoleAlignment.NeutralOutlier) && OptionGroupSingleton<MagistrateOptions>.Instance.NonKillingEvils.Value)
            {
                return true;
            }

            continue;
        }

        return false;
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

        void ShowNotification(string message)
        {
            var notif = Helpers.CreateAndShowNotification($"<b>{message}</b>", Color.white, new Vector3(0f, 1f, -20f), spr: icon);
            notif.Text.SetOutlineThickness(0.35f);
        }
    }
}*/
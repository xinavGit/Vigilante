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

namespace Vigilante.Roles.Crewmate;
//using TOUM monarch role code for enlist rpc
#pragma warning disable CA1001
public sealed class MagistrateRole(IntPtr cppPtr) : CrewmateRole(cppPtr), IVigilanteRole, IWikiDiscoverable, IDoomable
#pragma warning restore CA1001
{
    private MeetingMenu? judgeMenu;

    public string IdPart => "Magistrate";
    public Color RoleColor => VigilanteColors.Chameleon;
    public ModdedRoleTeams Team => ModdedRoleTeams.Crewmate;
    public RoleAlignment RoleAlignment => RoleAlignment.CrewmatePower;

    public string RoleName => MiraLocaleManager.Get($"Vigilante.Role.{IdPart}");
    public string RoleDescription => MiraLocaleManager.Get($"Vigilante.Role.{IdPart}.IntroBlurb");
    public string RoleLongDescription => MiraLocaleManager.Get($"Vigilante.Role.{IdPart}.TabDescription");

    public DoomableType DoomHintType => DoomableType.Perception;

    public float JudgementUsesLeft;

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
                    TouCrewAssets.CrewSwoopSprite)//btos2 judge sprite here!
            ];
        }
    }

    public CustomRoleConfiguration Configuration => new(this)
    {
        IconTmp = TmpSpriteUtils.CreateSpriteAsset(VigilanteAssets.JudgeIcon.LoadAsset(), "Vigilante.Role.Crewmate.Judge", 1.45f),
        Icon = VigilanteAssets.JudgeIcon,
        IntroSound = TouAudio.DetectiveIntroSound,
    };

    public override void Initialize(PlayerControl player)
    {
        RoleBehaviourStubs.Initialize(this, player);
        JudgementUsesLeft = OptionGroupSingleton<MagistrateOptions>.Instance.JudgementUses.Value;

        judgeMenu = new MeetingMenu(
                this,
                ClickJudge,
                MeetingAbilityType.Click,
                VigilanteAssets.InspectorInspect,
                null!,
                IsJudgementExempt,
                hoverColor: VigilanteColors.Inspector);
    }

    public override void OnMeetingStart()
    {
        RoleBehaviourStubs.OnMeetingStart(this);

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
        Debug.Log($"player is {voteArea.GetPlayer()}, checking exempt...");
        if (JudgementUsesLeft <= 0)
        {
            Debug.Log($"judge judgement exempt - judgement all used");
            return true;
        }

        if (voteArea == null || voteArea.PlayerId == Player.PlayerId)
        {
            Debug.Log($"null target or target is judge");
            return true;
        }

        var target = voteArea.GetPlayer();

        if (target == null || target.HasDied())// check if target has been successfully inspected again
        {
            Debug.Log($"target null or has died!");
            return true;
        }

        if (target.HasModifier<MagistrateJurorModifier>())
        {
            Debug.Log($"is the juror, exempt");
            return true;
        }

        Debug.Log($"can be judgemented");
        return false;
    }

    public void ClickJudge(PlayerVoteArea voteArea, MeetingHud __)
    {
        var target = GameData.Instance.GetPlayerById(voteArea.PlayerId).Object;
        var juror = ModifierUtils.GetPlayersWithModifier<MagistrateJurorModifier>().ToList().FirstOrDefault();

        if (juror == null && Player.AmOwner)
        {
            judgeMenu?.HideButtons();
            return;
        }

        if (VigilanteUtils.AreFriendly(target, juror))
        {
            // Even though Deputy doesn't use the nameplate normally, it should grab it anyways incase Deputy isn't meant to reveal themselves
            Player.RpcMeetingMurder(target, MeetingAnimation.RoleSpecific, CustomTouMurderRpcs.GetRandomMeetingAnim(DeathAnimType.Nameplate),
                causeOfDeath: "Judge");
        }
        else
        {
            Player.RpcMeetingMurder(Player, MeetingAnimation.RoleSpecific, CustomTouMurderRpcs.GetRandomMeetingAnim(DeathAnimType.Nameplate),
                causeOfDeath: "Judge");
            var msg = MiraLocaleManager.Get("Vigilante.Feedback.Magistrate.Judgement");
            var notif1 = Helpers.CreateAndShowNotification(
                $"<b>{VigilanteColors.Magistrate.ToTextColor()}{msg}</b></color>",
                Color.white, new Vector3(0f, 1f, -20f), spr: VigilanteAssets.JudgeIcon.LoadAsset());
            notif1.AdjustNotification();
        }

        if (Player.AmOwner)
        {
            judgeMenu?.HideButtons();
        }
    }

    /*[MethodRpc((uint)VigilanteRpcs.MagistrateJudge)]
    public static void RpcJudge(PlayerControl player, PlayerControl target, PlayerControl juror)
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
        var magiName = player.CachedPlayerData.PlayerName;
        var icon = VigilanteAssets.JudgeIcon.LoadAsset();

        if (VigilanteUtils.AreFriendly(juror, target))
        {
            // Even though Deputy doesn't use the nameplate normally, it should grab it anyways incase Deputy isn't meant to reveal themselves
            Player.RpcMeetingMurder(target, MeetingAnimation.RoleSpecific, CustomTouMurderRpcs.GetRandomMeetingAnim(DeathAnimType.Nameplate),
                causeOfDeath: "Judge");
        }
        else
        {
            Player.RpcMeetingMurder(target, MeetingAnimation.RoleSpecific, CustomTouMurderRpcs.GetRandomMeetingAnim(DeathAnimType.Nameplate),
                causeOfDeath: "Judge");
        }
    }*/

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
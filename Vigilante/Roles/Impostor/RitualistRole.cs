using HarmonyLib;
using Il2CppInterop.Runtime.Attributes;
using MiraAPI.GameOptions;
using MiraAPI.Hud;
using MiraAPI.Modifiers;
using MiraAPI.Patches.Stubs;
using MiraAPI.Roles;
using MiraAPI.Translation;
using MiraAPI.Utilities;
using MiraAPI.Utilities.Assets;
using Reactor.Networking.Attributes;
using Reactor.Utilities;
using TownOfUs;
using TownOfUs.Assets;
using TownOfUs.Extensions;
using TownOfUs.Modifiers.Crewmate;
using TownOfUs.Modules;
using TownOfUs.Modules.Components;
using TownOfUs.Modules.Wiki;
using TownOfUs.Networking;
using TownOfUs.Roles;
using TownOfUs.Roles.Crewmate;
using TownOfUs.Utilities;
using UnityEngine;
using Vigilante.Assets;
using Vigilante.Buttons.Impostor;
using Vigilante.Interfaces;
using Vigilante.Modifiers.Logic.Impostor;
using Vigilante.Options;
using Vigilante.Utilities;

namespace Vigilante.Roles.Impostor;

#pragma warning disable CA1001 // Types that own disposable fields should be disposable
public sealed class RitualistRole(IntPtr cppPtr) : ImpostorRole(cppPtr), IVigilanteRole, IWikiDiscoverable, IDoomable
#pragma warning restore CA1001 // Types that own disposable fields should be disposable
{
    private MeetingMenu? eliminateMenu;

    public DoomableType DoomHintType => DoomableType.Fearmonger;
    public string IdPart => "Ritualist";
    public string RoleMedDescriptionLocale => $"Vigilante.Role.{IdPart}.TabDescription";

    public string GetAdvancedDescription()
    {
        return
            MiraLocaleManager.Get($"Vigilante.Role.{IdPart}.WikiDescription") +
            MiscUtils.AppendOptionsText(GetType());
    }

    public Color RoleColor => TownOfUsColors.Impostor;
    public ModdedRoleTeams Team => ModdedRoleTeams.Impostor;
    public RoleAlignment RoleAlignment => RoleAlignment.ImpostorKilling;

    public CustomRoleConfiguration Configuration => new(this)
    {
        IconTmp = TmpSpriteUtils.CreateSpriteAsset(VigilanteAssets.RitualistIcon.LoadAsset(), "Vigilante.Role.Impostor.Ritualist", 1.45f),
        OptionsScreenshot = TouBanners.ImpostorRoleBanner,
        Icon = VigilanteAssets.RitualistIcon
    };

    public override void Initialize(PlayerControl player)
    {
        RoleBehaviourStubs.Initialize(this, player);
        
        eliminateMenu = new MeetingMenu(
                this,
                CheckElimination,
                MiraLocaleManager.Get("Vigilante.Role.Ritualist.Eliminate"),
                MeetingAbilityType.Click,
                VigilanteAssets.RitualistEliminate,
                null!,
                IsExempt,
                position: new Vector3(-0.35f, 0f, -3f));
    }

    public override void OnMeetingStart()
    {
        RoleBehaviourStubs.OnMeetingStart(this);

        var meeting = MeetingHud.Instance;
        if (Player.AmOwner && meeting != null && !Player.HasDied())
        {
            eliminateMenu?.GenButtons(meeting, true);
        }
    }

    [HideFromIl2Cpp]
    public bool IsExempt(PlayerVoteArea voteArea)
    {
        if (Player.HasModifier<JailedModifier>())
        {
            return true;
        }

        if (voteArea == null || voteArea.PlayerId == Player.PlayerId)
        {
            return true;
        }

        var votePlayer = voteArea.GetPlayer();

        if (votePlayer == null || votePlayer.HasDied())
        {
            return true;
        }

        if (votePlayer.HasModifier<RitualistRitualizedModifier>())
        {
            return false;
        }

        return true;
    }

    [HideFromIl2Cpp]
    public static void CheckElimination(PlayerVoteArea voteArea, MeetingHud meeting)
    {
        if (voteArea == null) return;

        var player = voteArea.GetPlayer();

        if (player == null) return;

        if (meeting.state == MeetingHud.MeetingStates.Discussion) return;

        if (!player.HasModifier<RitualistRitualizedModifier>()) return;

        MeetingMenu.Instances.Do(x => x.HideSingle(player.PlayerId));

        if (player.TryGetModifier<OracleBlessedModifier>(out var oracleMod))
        {
            OracleRole.RpcOracleBlessNotify(PlayerControl.LocalPlayer, oracleMod.Oracle, player);
            return;
        }

        RpcRitualistEliminate(PlayerControl.LocalPlayer, player);
    }

    [MethodRpc((uint)VigilanteRpcs.InspectorPublish)]
    public static void RpcRitualistEliminate(PlayerControl ritualist, PlayerControl victim)
    {
        if (ritualist == null || victim == null) return;

        if (!victim.HasModifier<RitualistRitualizedModifier>()) return;

        if (PlayerControl.LocalPlayer == ritualist)
        {
            ritualist.RpcMeetingMurder(victim,MeetingAnimation.PlayerNameplateAnimation, CustomTouMurderRpcs.GetRandomMeetingAnim(DeathAnimType.Nameplate),
                causeOfDeath: "RitualistRitualized");
        }

        Coroutines.Start(MiscUtils.CoFlash(TownOfUsColors.ImpSoft));
        TouAudio.PlaySound(VigilanteAssets.RitualistRit);

        var notif1 = Helpers.CreateAndShowNotification(
            $"<b>{MiraLocaleManager.Get("Vigilante.Feedback.Ritualist.Elimination")
            .Replace("<player>", $"{victim.Data.PlayerName}")}</b>",
            Color.white, new Vector3(0f, 1f, -20f), spr: VigilanteAssets.RitualistIcon.LoadAsset());

        notif1.AdjustNotification();

    }

    public void OpenPlayerMenu()
    {
        var playerMenu = CustomPlayerMenu.Create();

        playerMenu.Begin(
            plr => (!plr.HasDied() || !plr.HasModifier<RitualistRitualizedModifier>() ||
                    MiscUtils.GetFakePlayer(plr)
                        ?.body) && !plr.AmOwner,
            plr =>
            {
                playerMenu.ForceClose();

                if (plr != null)
                {
                    OpenRoleMenu(plr);
                }
            });
    }

    public void OpenRoleMenu(PlayerControl player)
    {
        var playerMenu = GuesserMenu.Create();
        playerMenu.Begin(IsRoleValid, ClickRoleHandle);

        void ClickRoleHandle(RoleBehaviour role)
        {
            playerMenu.Close();
            CustomButtonSingleton<RitualistRitualButton>.Instance.SetTimer(OptionGroupSingleton<RitualistOptions>.Instance.RitualCooldown.Value);

            if (player.Data.Role.Role == role.Role)
            {
                Coroutines.Start(MiscUtils.CoFlash(TownOfUsColors.Doomsayer));
                TouAudio.PlaySound(VigilanteAssets.RitualistRit);

                var notif1 = Helpers.CreateAndShowNotification(
                    $"<b>{MiraLocaleManager.Get("Vigilante.Feedback.Ritualist.RitualSuccess")
                    .Replace("<player>", $"{player.Data.PlayerName}")}</b>",
                    Color.white, new Vector3(0f, 1f, -20f), spr: VigilanteAssets.RitualistIcon.LoadAsset());

                notif1.AdjustNotification();

                var notif2 = Helpers.CreateAndShowNotification(
                    $"<b>{MiraLocaleManager.Get("Vigilante.Feedback.Ritualist.BloodSurges")}</b>",
                    Color.green, new Vector3(0f, 1f, -20f), spr: VigilanteAssets.RitualistIcon.LoadAsset());

                notif2.AdjustNotification();

                player.RpcAddModifier<RitualistRitualizedModifier>(role);
            }
            else
            {
                Coroutines.Start(MiscUtils.CoFlash(TownOfUsColors.Impostor));

                var notif1 = Helpers.CreateAndShowNotification(
                    $"<b>{MiraLocaleManager.Get("Vigilante.Feedback.Ritualist.RitualFail")
                    .Replace("<player>", $"{player.Data.PlayerName}")
                    .Replace("<role>", $"{role.GetRoleName()}")}</b>",
                    Color.white, new Vector3(0f, 1f, -20f), spr: VigilanteAssets.RitualistIcon.LoadAsset());

                notif1.AdjustNotification();

                var notif2 = Helpers.CreateAndShowNotification(
                    $"<b>{MiraLocaleManager.Get("Vigilante.Feedback.Ritualist.BloodSlows")}</b>",
                    Color.red, new Vector3(0f, 1f, -20f), spr: VigilanteAssets.RitualistIcon.LoadAsset());

                notif2.AdjustNotification();
            }
        }
    }

    private bool IsRoleValid(RoleBehaviour role)
    {
        if (role.IsDead || role is IGhostRole)
        {
            return false;
        }

        if (role.IsImpostor())
        {
            return false;// add support for ffa
        }

        if (role.GetRoleAlignment() == RoleAlignment.CrewmateInvestigative)
        {
            return OptionGroupSingleton<RitualistOptions>.Instance.EliminateCI.Value;
        }

        if (role.GetRoleAlignment() is RoleAlignment.CrewmateInvestigative || role.GetRoleAlignment() is RoleAlignment.CrewmateKilling || role.GetRoleAlignment() is RoleAlignment.CrewmatePower || role.GetRoleAlignment() is RoleAlignment.CrewmateProtective || role.GetRoleAlignment() is RoleAlignment.CrewmateSupport || role.GetRoleAlignment() is RoleAlignment.NeutralBenign || role.GetRoleAlignment() is RoleAlignment.NeutralEvil || role.GetRoleAlignment() is RoleAlignment.NeutralKilling || role.GetRoleAlignment() is RoleAlignment.NeutralOutlier)
        {
            return true;
        }

        return false;
    }

    public void FixedUpdate()
    {
        if (!Player || Player.Data.Role is not RitualistRole || Player.HasDied() || !Player.AmOwner ||
            MeetingHud.Instance)
        {
            return;
        }
        
        var value = OptionGroupSingleton<RitualistOptions>.Instance.CooldownIncrease - 1;
        
        foreach (var ability in CustomButtonManager.Buttons)
        {
            if (ability.EffectActive) continue;
            if (ability.TimerPaused) continue;

            ability.DecreaseTimer(Time.deltaTime * value);
        }

        if (Player.inVent)
        {
            Player.killTimer -= Time.deltaTime * value;
        }
    }
}
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
public sealed class KidnapperRole(IntPtr cppPtr) : ImpostorRole(cppPtr), IVigilanteRole, IWikiDiscoverable, IDoomable
#pragma warning restore CA1001 // Types that own disposable fields should be disposable
{
    private MeetingMenu? exterminateMenu;

    public DoomableType DoomHintType => DoomableType.Fearmonger;
    public string IdPart => "Kidnapper";
    public string RoleMedDescriptionLocale => $"Vigilante.Role.{IdPart}.TabDescription";

    public string GetAdvancedDescription()
    {
        return
            MiraLocaleManager.Get($"Vigilante.Role.{IdPart}.WikiDescription") +
            MiscUtils.AppendOptionsText(GetType());
    }

    public Color RoleColor => TownOfUsColors.Impostor;
    public ModdedRoleTeams Team => ModdedRoleTeams.Impostor;
    public RoleAlignment RoleAlignment => RoleAlignment.ImpostorPower;

    public bool Exterminated { get; set; }


    public CustomRoleConfiguration Configuration => new(this)
    {
        IconTmp = TmpSpriteUtils.CreateSpriteAsset(VigilanteAssets.KidnapperIcon.LoadAsset(), "Vigilante.Role.Impostor.Kidnapper", 1.45f),
        OptionsScreenshot = TouBanners.ImpostorRoleBanner,
        Icon = VigilanteAssets.KidnapperIcon
    };

    public override void Initialize(PlayerControl player)
    {
        RoleBehaviourStubs.Initialize(this, player);
        Exterminated = false;
        
        
        exterminateMenu = new MeetingMenu(
                this,
                CheckElimination,
                MiraLocaleManager.Get("Vigilante.Role.Kidnapper.Eliminate"),
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
            exterminateMenu?.GenButtons(meeting, true);
        }
    }

    [HideFromIl2Cpp]
    public bool IsExempt(PlayerVoteArea voteArea)
    {
        if (Exterminated)
        {
            return true;
        }

        if (!Player.HasModifier<KidnapperKidnappedModifier>())
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

        return false;
    }

    [HideFromIl2Cpp]
    public static void CheckElimination(PlayerVoteArea voteArea, MeetingHud meeting)
    {
        var kidnapper = PlayerControl.LocalPlayer;
        var kidnapperRole = kidnapper.GetRole<KidnapperRole>();
        if (kidnapperRole == null) return;

        if (kidnapperRole.Exterminated) return;
        
        if (voteArea == null) return;

        var player = voteArea.GetPlayer();

        if (player == null) return;

        if (meeting.state == MeetingHud.MeetingStates.Discussion) return;

        if (kidnapperRole.IsExempt(voteArea)) return;

        if (!player.HasModifier<KidnapperKidnappedModifier>()) return;

        MeetingMenu.Instances.Do(x => x.HideSingle(player.PlayerId));

        if (player.TryGetModifier<OracleBlessedModifier>(out var oracleMod))
        {
            OracleRole.RpcOracleBlessNotify(PlayerControl.LocalPlayer, oracleMod.Oracle, player);
            return;
        }

        kidnapperRole.Exterminated = true;

        RpcKidnapperExterminate(PlayerControl.LocalPlayer, player);

        kidnapperRole.exterminateMenu?.GenButtons(meeting, true);
    }

    [MethodRpc((uint)VigilanteRpcs.KidnapperExterminate)]
    public static void RpcKidnapperExterminate(PlayerControl kidnapper, PlayerControl victim)
    {

        Coroutines.Start(MiscUtils.CoFlash(TownOfUsColors.ImpSoft));
        TouAudio.PlaySound(VigilanteAssets.RitualistRit);

        var notif1 = Helpers.CreateAndShowNotification(
            $"<b>{MiraLocaleManager.Get("Vigilante.Feedback.Kidnapper.ExterminatedNotify")
            .Replace("<player>", $"{victim.Data.PlayerName}")}</b>",
            Color.white, new Vector3(0f, 1f, -20f), spr: VigilanteAssets.RitualistIcon.LoadAsset());

        notif1.AdjustNotification();

        if (PlayerControl.LocalPlayer == kidnapper)
        {
            kidnapper.RpcMeetingMurder(victim,MeetingAnimation.PlayerNameplateAnimation, CustomTouMurderRpcs.GetRandomMeetingAnim(DeathAnimType.Nameplate),
                causeOfDeath: "KidnapperExterminated");
        }

    }

}
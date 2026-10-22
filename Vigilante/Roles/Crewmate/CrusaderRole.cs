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
using MiraAPI.Utilities.Assets;
using TownOfUs.Modifiers.Crewmate;
using TownOfUs.Assets;
using MiraAPI.Hud;
using TownOfUs.Buttons;
using Vigilante.Interfaces;
using Vigilante.Buttons.Crewmate;

namespace Vigilante.Roles.Crewmate;

public sealed class CrusaderRole(IntPtr cppPtr) : CrewmateRole(cppPtr), IVigilanteRole, IWikiDiscoverable, IDoomable
{
    public string IdPart => "Crusader";
    public Color RoleColor => VigilanteColors.Crusader;
    public ModdedRoleTeams Team => ModdedRoleTeams.Crewmate;
    public RoleAlignment RoleAlignment => RoleAlignment.CrewmateProtective;

    public string RoleName => MiraLocaleManager.Get($"Vigilante.Role.{IdPart}");
    public string RoleDescription => MiraLocaleManager.Get($"Vigilante.Role.{IdPart}.IntroBlurb");
    public string RoleLongDescription => MiraLocaleManager.Get($"Vigilante.Role.{IdPart}.TabDescription");

    [HideFromIl2Cpp] public PlayerControl? Protected { get; set; }
    public bool IsProtecting { get; set; }
    public override bool IsAffectedByComms => false;

    public DoomableType DoomHintType => DoomableType.Protective;

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
                new(MiraLocaleManager.Get($"Vigilante.Role.{IdPart}.Sanctify", "Sanctify"),
                    MiraLocaleManager.Get($"Vigilante.Role.{IdPart}.Sanctify.WikiDescription"),
                    VigilanteAssets.CrusaderSanctify),
            ];
        }
    }

    public CustomRoleConfiguration Configuration => new(this)
    {
        IconTmp = TmpSpriteUtils.CreateSpriteAsset(VigilanteAssets.CrusaderIcon.LoadAsset(), "Vigilante.Role.Crewmate.Crusader", 1.45f),
        Icon = VigilanteAssets.CrusaderIcon,
        IntroSound = TouAudio.DetectiveIntroSound,
        MaxRoleCount = 1
    };

    public static bool IsPowerCrew => true;


    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        var stringB = ITownOfUsRole.SetNewTabText(this);

        stringB.AppendLine(TownOfUsPlugin.Culture, $"\n<b>{MiraLocaleManager.Get("Vigilante.Feedback.Crusader.TabProtecting")}</b>");

        if (Protected != null) stringB.AppendLine(Protected.Data.PlayerName);

        return stringB;
    }

    public void FixedUpdate()
    {
        if (!Player || Player.Data.Role is not CrusaderRole)
        {
            return;
        }

        var dced = IsProtecting && Protected == null;
        if (Protected != null && Protected.HasDied() || dced)
        {
            Clear(dced);
        }
    }

    public void Clear(bool playerLeft = false)
    {
        if (playerLeft)
        {
            IsProtecting = false;
            Protected = null;
            if (Player.AmOwner)
            {
                var button = CustomButtonSingleton<CrusaderSanctifyButton>.Instance;
                button.TargetWasValid = false;
                button.ResetCooldownAndOrEffect();
            }
        }
        SetProtectedPlayer(null);
    }

    public override void OnDeath(DeathReason reason)
    {
        RoleBehaviourStubs.OnDeath(this, reason);

        Clear();
    }

    public override void Deinitialize(PlayerControl targetPlayer)
    {
        RoleBehaviourStubs.Deinitialize(this, targetPlayer);

        Clear();
    }

    public void SetProtectedPlayer(PlayerControl? player)
    {
        IsProtecting = false;
        if (Protected == player && player != null)
        {
            if (player.TryGetModifier<CrusaderSanctifiedModifier>(out var mod2))
            {
                player.RemoveModifier(mod2);
            }

            return;
        }

        if (Protected?.TryGetModifier<CrusaderSanctifiedModifier>(out var mod) == true)
        {
            Protected.RemoveModifier(mod);
        }

        Protected = (player?.HasDied() == true) ? null : player;
        if (Protected != null)
        {
            IsProtecting = true;
            Protected.AddModifier<CrusaderSanctifiedModifier>(Player);
        }
    }

    [MethodRpc((uint)VigilanteRpcs.CrusaderSanctify)]
    public static void RpcSanctify(PlayerControl mc, PlayerControl target)
    {
        if (LobbyBehaviour.Instance)
        {
            MiscUtils.RunAnticheatWarning(mc);
            return;
        }
        if (mc.Data.Role is not CrusaderRole role)
        {
            return;
        }

        role?.SetProtectedPlayer(target);
        
        if (PlayerControl.LocalPlayer.IsRole<CrusaderRole>())
        {
            var notif1 = Helpers.CreateAndShowNotification(MiraLocaleManager.Get("Vigilante.Feedback.Crusader.SanctifySuccessful").Replace("<player>", target.Data.PlayerName),
                Color.white, new Vector3(0f, 1f, -20f), spr: VigilanteAssets.CrusaderIcon.LoadAsset());

            notif1.AdjustNotification();
        }
    }

    [MethodRpc((uint)VigilanteRpcs.CrusaderClearSanctify)]
    public static void RpcClearSanctify(PlayerControl mc)
    {
        if (LobbyBehaviour.Instance)
        {
            MiscUtils.RunAnticheatWarning(mc);
            return;
        }
        ClearSanctify(mc);
    }

    public static void ClearSanctify(PlayerControl mc)
    {
        if (mc.Data.Role is not CrusaderRole role)
        {
            return;
        }

        role?.SetProtectedPlayer(null);
    }
}
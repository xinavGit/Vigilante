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
using Reactor.Utilities;
using System.Globalization;
using Vigilante.Interfaces;
using Rewired;
using HarmonyLib;
using Il2CppSystem.IO;

namespace Vigilante.Roles.Crewmate;

#pragma warning disable CA1001 // Types that own disposable fields should be disposable
public sealed class InspectorRole(IntPtr cppPtr) : CrewmateRole(cppPtr), IVigilanteRole, IWikiDiscoverable, IDoomable
#pragma warning restore CA1001 // Types that own disposable fields should be disposable
{
    private MeetingMenu? inspectMenu;
    private MeetingMenu? publishMenu;
    private GuesserMenu? inspectGuesserMenu;

    public string IdPart => "Inspector";
    public Color RoleColor => VigilanteColors.Inspector;
    public ModdedRoleTeams Team => ModdedRoleTeams.Crewmate;
    public RoleAlignment RoleAlignment => RoleAlignment.CrewmateInvestigative;

    public string RoleName => MiraLocaleManager.Get($"Vigilante.Role.{IdPart}");
    public string RoleDescription => MiraLocaleManager.Get($"Vigilante.Role.{IdPart}.IntroBlurb");
    public string RoleLongDescription => MiraLocaleManager.Get($"Vigilante.Role.{IdPart}.TabDescription");

    public DoomableType DoomHintType => DoomableType.Insight;

    public static Dictionary<PlayerControl, RoleBehaviour> InspectedPlayers { get; } = [];
    public static List<RoleBehaviour> RoundInspectedRoles { get; } = [];
    private int playersPublished;
    public int RoundInspects { get; set; }

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
                new(MiraLocaleManager.Get($"Vigilante.Role.{IdPart}.Inspect", "Inspect"),
                    MiraLocaleManager.Get($"Vigilante.Role.{IdPart}.Inspect.WikiDescription"),
                    VigilanteAssets.InspectorInspect),
                new(MiraLocaleManager.Get($"Vigilante.Role.{IdPart}.Publish", "Publish"),
                    MiraLocaleManager.Get($"Vigilante.Role.{IdPart}.Publish.WikiDescription"),
                    TouAssets.RevealButtonSprite)
            ];
        }
    }

    public CustomRoleConfiguration Configuration => new(this)
    {
        IconTmp = TmpSpriteUtils.CreateSpriteAsset(VigilanteAssets.InspectorIcon.LoadAsset(), "Vigilante.Role.Crewmate.Inspector", 1.45f),
        Icon = VigilanteAssets.InspectorIcon,
        IntroSound = TouAudio.GlitchSound,
        MaxRoleCount = 1
    };

    public override void Initialize(PlayerControl player)
    {
        RoleBehaviourStubs.Initialize(this, player);
        InspectedPlayers.Clear();
        RoundInspectedRoles.Clear();
        RoundInspects = 0;
        playersPublished = 0;

        inspectMenu = new MeetingMenu(
                this,
                OpenInspectMenu,
                MeetingAbilityType.Click,
                VigilanteAssets.InspectorInspect,
                null!,
                IsInspectExempt,
                hoverColor: VigilanteColors.Inspector);
        
        publishMenu = new MeetingMenu(
                this,
                CheckPublish,
                MiraLocaleManager.Get("Vigilante.Role.Inspector.Publish"),
                MeetingAbilityType.Click,
                TouAssets.RevealCleanSprite,
                null!,
                IsPublishExempt,
                position: new Vector3(-0.35f, 0f, -3f));
    }
    public override void OnMeetingStart()
    {
        RoleBehaviourStubs.OnMeetingStart(this);

        foreach (var uninspectable in ModifierUtils.GetPlayersWithModifier<InspectorUninspectableModifier>().ToList())
        {
            uninspectable.RemoveModifier<InspectorUninspectableModifier>();
        }

        RoundInspectedRoles.Clear();
        
        RoundInspects = 0;
        var meeting = MeetingHud.Instance;
        if (Player.AmOwner && meeting != null && !Player.HasDied())
        {
            inspectMenu?.GenButtons(meeting, true);
            publishMenu?.GenButtons(meeting, true);
        }

        RefreshButtons();
    }

    [HideFromIl2Cpp]
    public void OpenInspectMenu(PlayerVoteArea voteArea, MeetingHud meeting)
    {
        if (meeting.state == MeetingHud.MeetingStates.Discussion || IsInspectExempt(voteArea))
        {
            return;
        }

        if (Minigame.Instance)
        {
            return;
        }

        var inspectTarget = GameData.Instance.GetPlayerById(voteArea.PlayerId)?.Object;
        if (inspectTarget == null)
        {
            return;
        }

        inspectGuesserMenu = GuesserMenu.Create();
        inspectGuesserMenu.Begin(IsRoleValid, role => OnRoleSelected(role, inspectTarget.PlayerId));
    }

    [HideFromIl2Cpp]
    public void CheckPublish(PlayerVoteArea voteArea, MeetingHud meeting)
    {
        if (meeting.state == MeetingHud.MeetingStates.Discussion || IsPublishExempt(voteArea))
        {
            return;
        }

        if (Minigame.Instance)
        {
            return;
        }

        var inspectTarget = GameData.Instance.GetPlayerById(voteArea.PlayerId)?.Object;
        if (inspectTarget == null)
        {
            return;
        }

        if (InspectedPlayers.TryGetValue(inspectTarget, out RoleBehaviour? publishRole) && publishRole != null)
        {
            RpcInspectorPublish(inspectTarget, RoleId.Get(publishRole.GetType()));
        }
    }

    [HideFromIl2Cpp]
    public static bool IsRoleValid(RoleBehaviour role)
    {
        if (role is not ITownOfUsRole)
        {
            return false;
        }

        if (role is MayorRole)
        {
            return false;
        }

        if (RoundInspectedRoles.Contains(role))
        {
            return false;
        }

        var inspectable = (InspectorInspectableRoles)OptionGroupSingleton<InspectorOptions>.Instance.InspectableRoles.Value;
        if (role.GetRoleAlignment() is RoleAlignment.CrewmateInvestigative || role.GetRoleAlignment() is RoleAlignment.CrewmateKilling || role.GetRoleAlignment() is RoleAlignment.CrewmatePower || role.GetRoleAlignment() is RoleAlignment.CrewmateProtective || role.GetRoleAlignment() is RoleAlignment.CrewmateSupport)
        {
            return inspectable is InspectorInspectableRoles.CrewRoles || inspectable is InspectorInspectableRoles.AllRoles;
        }

        if (role.GetRoleAlignment() is RoleAlignment.ImpostorConcealing || role.GetRoleAlignment() is RoleAlignment.ImpostorKilling || role.GetRoleAlignment() is RoleAlignment.ImpostorPower || role.GetRoleAlignment() is RoleAlignment.ImpostorSupport || role.GetRoleAlignment() is RoleAlignment.NeutralBenign || role.GetRoleAlignment() is RoleAlignment.NeutralEvil || role.GetRoleAlignment() is RoleAlignment.NeutralKilling || role.GetRoleAlignment() is RoleAlignment.NeutralOutlier)
        {
            return inspectable is InspectorInspectableRoles.NonCrewRoles || inspectable is InspectorInspectableRoles.AllRoles;
        }

        return false;
    }

    [HideFromIl2Cpp]
    public void OnRoleSelected(RoleBehaviour role, byte targetId)
    {
        var target = GameData.Instance.GetPlayerById(targetId)?.Object;

        if (target == null)
        {
            return;
        }

        inspectGuesserMenu?.Close();
        InspectPlayer(target, role);
    }

    [HideFromIl2Cpp]
    public bool IsInspectExempt(PlayerVoteArea voteArea)
    {
        if (Player.HasModifier<JailedModifier>())
        {
            return true;
        }

        if (RoundInspects >= OptionGroupSingleton<InspectorOptions>.Instance.InspectsPerMeeting.Value)
        {
            return true;
        }

        if (voteArea == null || voteArea.PlayerId == Player.PlayerId)
        {
            return true;
        }

        var votePlayer = voteArea.GetPlayer();

        if (votePlayer == null)
        {
            return true;
        }

        if (votePlayer.HasModifier<JailedModifier>() || votePlayer.IsRevealed())
        {
            return true;
        }

        var target = voteArea.GetPlayer();

        if (target == null || target.HasDied())
        {
            return true;
        }

        if (target.HasModifier<InspectorRevealModifier>() || target.HasModifier<InspectorPublishedModifier>() || target.HasModifier<InspectorUninspectableModifier>())
        {
            return true;
        }

        if (InspectedPlayers.ContainsKey(target))
        {
            return true;
        }

        return false;
    }

    [HideFromIl2Cpp]
    public bool IsPublishExempt(PlayerVoteArea voteArea)
    {
        if (Player.HasModifier<JailedModifier>())
        {
            return true;
        }

        if (playersPublished >= OptionGroupSingleton<InspectorOptions>.Instance.MaxPublishUses.Value)
        {
            return true;
        }

        if (voteArea == null || voteArea.PlayerId == Player.PlayerId)
        {
            return true;
        }

        var target = voteArea.GetPlayer();

        if (target == null || target.HasDied())
        {
            return true;
        }

        if (!InspectedPlayers.ContainsKey(target))
        {
            return true;
        }

        return false;
    }

    [MethodRpc((uint)VigilanteRpcs.InspectorPublish)]
    public static void RpcInspectorPublish(PlayerControl inspected, ushort publishRole)
    {
        if (Minigame.Instance)
        {
            Minigame.Instance.Close();
        }

        Coroutines.Start(MiscUtils.CoFlash(VigilanteColors.Inspector));
        TouAudio.PlaySound(VigilanteAssets.InspectorPublish);

        var notif = Helpers.CreateAndShowNotification(
            $"<b>{MiraLocaleManager.Get("Vigilante.Feedback.Inspector.InformationWasPublished")
            .Replace("<player>", $"{inspected.Data.PlayerName}")}</b>",
            Color.white, new Vector3(0f, 1f, -20f), spr: VigilanteAssets.InspectorIcon.LoadAsset());

        notif.AdjustNotification();

        InspectedPlayers.Remove(inspected);
        if (PlayerControl.LocalPlayer.Data.Role is InspectorRole insp)
        {
            inspected.RemoveModifier<InspectorRevealModifier>();
            insp.RefreshButtons();
            insp.playersPublished++;
        }

        inspected.AddModifier<InspectorPublishedModifier>(RoleManager.Instance.GetRole((RoleTypes)publishRole));
        
    }

    [HideFromIl2Cpp]
    public void InspectPlayer(PlayerControl inspected, RoleBehaviour inspectRole) //not static because of the GenButtons line
    {
        if (inspected.Data.Role.Role == inspectRole.Role)
        {
            Coroutines.Start(MiscUtils.CoFlash(TownOfUsColors.Doomsayer));

            var notif = Helpers.CreateAndShowNotification(
                $"<b>{MiraLocaleManager.Get("Vigilante.Feedback.Inspector.InspectTrue")
                .Replace("<player>", $"{inspected.Data.PlayerName}").Replace("<role>", $"{inspectRole.NameColor.ToTextColor()}{inspectRole.GetRoleName()}</color>")}</b>",
                Color.white, new Vector3(0f, 1f, -20f), spr: TouModifierIcons.DoubleShot.LoadAsset());

            notif.AdjustNotification();

            InspectedPlayers[inspected] = inspectRole;
            inspected.AddModifier<InspectorRevealModifier>(inspectRole); //should only happen on the inspector's side
        }
        else
        {
            Coroutines.Start(MiscUtils.CoFlash(TownOfUsColors.Impostor));

            var notif = Helpers.CreateAndShowNotification(
                $"<b>{MiraLocaleManager.Get("Vigilante.Feedback.Inspector.InspectFalse")
                .Replace("<player>", $"{inspected.Data.PlayerName}").Replace("<role>", $"{inspectRole.NameColor.ToTextColor()}{inspectRole.GetRoleName()}</color>")}</b>",
                Color.white, new Vector3(0f, 1f, -20f), spr: TouModifierIcons.DoubleShot.LoadAsset());

            notif.AdjustNotification();

            if (!OptionGroupSingleton<InspectorOptions>.Instance.CanInspectSamePersonTwice.Value)
            {
                inspected.AddModifier<InspectorUninspectableModifier>();
            }
        }

        if (!OptionGroupSingleton<InspectorOptions>.Instance.CanInspectSameRoleTwice)
        {
            RoundInspectedRoles.Add(inspectRole);
        }

        RoundInspects++;

        RefreshButtons();
    }

    public void RefreshButtons()
    {
        Coroutines.Start(CoRefreshButtons());
    }

    [HideFromIl2Cpp]
    private System.Collections.IEnumerator CoRefreshButtons()
    {
        // wait until the guesser overlay has actually closed
        while (Minigame.Instance != null)
        {
            yield return null;
        }
        yield return null; // one extra frame for the UI state to settle

        var meeting = MeetingHud.Instance;
        if (meeting == null) yield break;

        publishMenu?.GenButtons(meeting, true);
        inspectMenu?.GenButtons(meeting, true);
    }


}
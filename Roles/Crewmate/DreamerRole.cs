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

namespace Vigilante.Roles.Crewmate;

#pragma warning disable CA1001
public sealed class DreamerRole(IntPtr cppPtr) : CrewmateRole(cppPtr), ITownOfUsRole, IWikiDiscoverable, IDoomable
#pragma warning restore CA1001
{
    private MeetingMenu? meetingMenu;
    private GuesserMenu? dreamMenu;

    public string IdPart => "Dreamer";
    public Color RoleColor => VigilanteColors.Dreamer;
    public ModdedRoleTeams Team => ModdedRoleTeams.Crewmate;
    public RoleAlignment RoleAlignment => RoleAlignment.CrewmatePower;

    public string RoleName => MiraLocaleManager.Get($"TownOfUsMira.Role.{IdPart}");
    public string RoleDescription => MiraLocaleManager.Get($"TownOfUsMira.Role.{IdPart}.IntroBlurb");
    public string RoleLongDescription => MiraLocaleManager.Get($"TownOfUsMira.Role.{IdPart}.TabDescription");

    public override bool IsAffectedByComms => false;
    public DoomableType DoomHintType => DoomableType.Perception;

    public string GetAdvancedDescription()
    {
        return
            MiraLocaleManager.Get($"TownOfUsMira.Role.{IdPart}.WikiDescription") +
            MiscUtils.AppendOptionsText(GetType());
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities
    {
        get
        {
            return
            [
                new(MiraLocaleManager.Get($"TownOfUsMira.Role.{IdPart}Dream", "Dream"),
                    MiraLocaleManager.Get($"TownOfUsMira.Role.{IdPart}Dream.WikiDescription"),
                    VigilanteAssets.DreamerMeetingDream)
            ];
        }
    }

    public CustomRoleConfiguration Configuration => new(this)
    {
        IconTmp = TmpSpriteUtils.CreateSpriteAsset(VigilanteAssets.DreamerIcon.LoadAsset(), "TownOfUsMira.Role.Crewmate.Dreamer", 1.45f),
        Icon = VigilanteAssets.DreamerIcon,
        IntroSound = VigilanteAssets.DreamerIntro,
        MaxRoleCount = 1
    };


    public byte DreamTargetId { get; set; } = byte.MaxValue;
    public ushort DreamRoleId { get; set; }

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        var stringB = ITownOfUsRole.SetNewTabText(this);

        if (!(DreamRoleId == default) || !(DreamTargetId == byte.MaxValue))
        {
            stringB.AppendLine(TownOfUsPlugin.Culture, $"<b>{MiraLocaleManager.Get("TownOfUsMira.Role.DreamerTabHeader")}</b>");

            var targetData = GameData.Instance.GetPlayerById(DreamTargetId)?.Object;
            var roleObj = RoleManager.Instance.GetRole((RoleTypes)DreamRoleId) as ITownOfUsRole;

            stringB.AppendLine(TownOfUsPlugin.Culture, $"<b>{MiraLocaleManager.Get("TownOfUsMira.Role.DreamerTabTarget").Replace("<player>", $"{targetData?.Data.PlayerName}").Replace("<role>", $"{roleObj?.RoleColor.ToTextColor()}{roleObj?.RoleName}</color>")}</b>");
            return stringB;
        }

        return stringB;
    }

    public override void Initialize(PlayerControl player)
    {
        RoleBehaviourStubs.Initialize(this, player);

        ClearDream();

        if (Player.AmOwner)
        {
            DreamTargetId = byte.MaxValue;

            meetingMenu = new MeetingMenu(
                this,
                OpenDreamMenu,
                MiraLocaleManager.Get($"TownOfUsMira.Role.{IdPart}Dream", "Dream"),
                MeetingAbilityType.Click,
                VigilanteAssets.DreamerMeetingDream,
                exemption: IsExempt,
                position: new Vector3(-0.35f, 0f, -3f));
        }
    }

    public override void OnMeetingStart()
    {
        RoleBehaviourStubs.OnMeetingStart(this);

        ClearDream();

        var meeting = MeetingHud.Instance;
        if (Player.AmOwner && meeting != null && !Player.HasDied())
        {
            meetingMenu?.GenButtons(meeting, true);
        }
    }

    public override void OnVotingComplete()
    {
        RoleBehaviourStubs.OnVotingComplete(this);

        if (Player.AmOwner)
        {
            meetingMenu?.HideButtons();
        }
    }

    [HideFromIl2Cpp]
    public bool IsExempt(PlayerVoteArea voteArea)
    {
        if (voteArea == null || voteArea.PlayerId == Player.PlayerId)
        {
            return true;
        }

        var target = GameData.Instance.GetPlayerById(voteArea.PlayerId)?.Object;

        if (target == null || target.HasDied() || target.HasModifier<DreamerTargetDreamingModifier>() || target.HasModifier<DreamerInsomniaModifier>() || target.HasModifier<BaseRevealModifier>())
        {
            return true;
        }

        return false;
    }

    [HideFromIl2Cpp]
    public void OpenDreamMenu(PlayerVoteArea voteArea, MeetingHud meeting)
    {
        if (meeting.state == MeetingHud.MeetingStates.Discussion || IsExempt(voteArea))
        {
            return;
        }

        if (Minigame.Instance)
        {
            return;
        }

        var dreamTarget = GameData.Instance.GetPlayerById(voteArea.PlayerId)?.Object;
        if (dreamTarget == null)
        {
            return;
        }

        dreamMenu = GuesserMenu.Create();
        dreamMenu.Begin(IsRoleValid, role => OnRoleSelected(role, dreamTarget.PlayerId));
    }

    [HideFromIl2Cpp]
    public static bool IsRoleValid(RoleBehaviour role)
    {
        if (role is not ITownOfUsRole { Team: ModdedRoleTeams.Crewmate } touRole || role is DreamerRole)
        {
            return false;
        }

        if (role.GetRoleAlignment() is not RoleAlignment.CrewmateInvestigative 
        && role.GetRoleAlignment() is not RoleAlignment.CrewmateKilling 
        && role.GetRoleAlignment() is not RoleAlignment.CrewmatePower 
        && role.GetRoleAlignment() is not RoleAlignment.CrewmateProtective 
        && role.GetRoleAlignment() is not RoleAlignment.CrewmateSupport)
        {
            return false;
        }

        if (role is MayorRole or PoliticianRole or MonarchRole or TimeLordRole or ImitatorRole)
        {
            return false;
        }

        var restriction = (DreamerReimagineRestriction)OptionGroupSingleton<DreamerOptions>.Instance.CannotReimagineInto.Value;
        return restriction switch
        {
            DreamerReimagineRestriction.CrewmateKilling => touRole.RoleAlignment != RoleAlignment.CrewmateKilling,
            DreamerReimagineRestriction.CrewmatePower => touRole.RoleAlignment != RoleAlignment.CrewmatePower,
            _ => true,
        };
    }

    [HideFromIl2Cpp]
    public void OnRoleSelected(RoleBehaviour role, byte targetId)
    {
        var options = OptionGroupSingleton<DreamerOptions>.Instance;

        var target = GameData.Instance.GetPlayerById(targetId)?.Object;

        if (target == null)
        {
            return;
        }

        dreamMenu?.Close();

        var roleId = RoleId.Get(role.GetType());

        RpcSetReimagineTarget(Player, targetId, roleId);

        var roleObj = RoleManager.Instance.GetRole((RoleTypes)roleId) as ITownOfUsRole;

        var notif = Helpers.CreateAndShowNotification(
            $"<b>{MiraLocaleManager.Get("TownOfUsMira.Role.DreamerDreamTargetSelect").Replace("<player>", $"{VigilanteColors.Dreamer.ToTextColor()}{target.Data.PlayerName}</color>").Replace("<role>", $"{roleObj?.RoleColor.ToTextColor()}{roleObj?.RoleName}</color>")}</b>",
            Color.white, new Vector3(0f, 1f, -20f), spr: VigilanteAssets.DreamerIcon.LoadAsset());

        notif.AdjustNotification();
    }

    [MethodRpc((uint)VigilanteRpcs.DreamerSetReimagineTarget)]
    public static void RpcSetReimagineTarget(PlayerControl dreamer, byte targetId, ushort roleId)
    {
        if (dreamer?.Data?.Role is not DreamerRole dreamerRole)
        {
            return;
        }

        dreamerRole.DreamTargetId = targetId;
        dreamerRole.DreamRoleId = roleId;
    }

    [MethodRpc((uint)VigilanteRpcs.DreamerReimagine)]
    public static void RpcReimagine(PlayerControl dreamer, PlayerControl target, ushort dreamRoleId)
    {
        if (!AmongUsClient.Instance.AmClient)
        {
            return;
        }

        var canApplyRole = true;

        if (!IsValidDreamTarget(target, dreamer))
        {
            return;
        }

        if (dreamer?.Data?.Role is not DreamerRole dreamerRole)
        {
            return;
        }

        var role = RoleManager.Instance.GetRole((RoleTypes)dreamRoleId);
        var options = OptionGroupSingleton<DreamerOptions>.Instance;
        var onBreak = (DreamerOnDreamBreakMaxRoleCount)options.OnMaxRoleCountBroken.Value;

        if (IsBreakingMaxRoleCount(role, target))
        {
            if (onBreak == DreamerOnDreamBreakMaxRoleCount.ApplyRandom)
            {
                var randomRole = GetRandomValidRole(target);
                if (randomRole == null)
                {
                    canApplyRole = false;
                }
                else
                {
                    dreamRoleId = (ushort)randomRole.Role;
                    dreamerRole.DreamRoleId = dreamRoleId;

                    if (dreamer.AmOwner)
                    {
                        var roleObj = role as ITownOfUsRole;
                        var notif = Helpers.CreateAndShowNotification(
                        $"<b>{MiraLocaleManager.Get("TownOfUsMira.Role.DreamerAttemptedDreamRandomRole").Replace("<player>", $"{VigilanteColors.Dreamer.ToTextColor()}{target.Data.PlayerName}</color>").Replace("<role>", $"{roleObj?.RoleColor.ToTextColor()}{roleObj?.RoleName}</color>")}</b>",
                        Color.white, new Vector3(0f, 1f, -20f), spr: VigilanteAssets.DreamerIcon.LoadAsset());

                        notif.AdjustNotification();
                    }
                }
            }
            else
            {
                canApplyRole = false;
            }
        }

        if (!target.IsCrewmate())
        {
            if (options.NotifyTargetOfRoleOnAttempt.Value)
            {
                if (target.AmOwner)
                {
                    var roleObj = RoleManager.Instance.GetRole((RoleTypes)dreamerRole.DreamRoleId) as ITownOfUsRole;

                    var notif = Helpers.CreateAndShowNotification(
                    $"<b>{MiraLocaleManager.Get("TownOfUsMira.Role.DreamerAttemptedDreamWithRole").Replace("<role>", $"{roleObj?.RoleColor.ToTextColor()}{roleObj?.RoleName}</color>")}</b>",
                    Color.white, new Vector3(0f, 1f, -20f), spr: VigilanteAssets.DreamerIcon.LoadAsset());

                    notif.AdjustNotification();
                }
            }
            else if (options.NotifyTargetOnAttempt.Value)
            {
                if (target.AmOwner)
                {
                    var notif = Helpers.CreateAndShowNotification(
                    $"<b>{MiraLocaleManager.Get("TownOfUsMira.Role.DreamerAttemptedDream")}</b>",
                    Color.white, new Vector3(0f, 1f, -20f), spr: VigilanteAssets.DreamerIcon.LoadAsset());

                    notif.AdjustNotification();
                }
            }
            canApplyRole = false;
        }

        if (AmongUsClient.Instance.AmHost)
        {
            target.RpcAddModifier<DreamerInsomniaModifier>((int)options.InsomniaRounds.Value);

            if (canApplyRole)
            {
                var originalRole = (ushort)target.Data.Role.Role;
                if (target.HasModifier<ImitatorCacheModifier>())
                {
                    target.RemoveModifier<ImitatorCacheModifier>();
                    target.RpcChangeRole(RoleId.Get<ImitatorRole>(), false);
                }
                target.RpcChangeRole(dreamRoleId, false);
                target.RpcAddModifier<DreamerTargetDreamingModifier>(originalRole, dreamRoleId);
            }
        }
    }

    public static bool IsValidDreamTarget(PlayerControl? target, PlayerControl dreamer)
    {
        if (target == null || dreamer == null)
        {
            return false;
        }

        if (target.Data == null || target.Data.Disconnected)
        {
            return false;
        }

        if (target.HasDied() || target.PlayerId == dreamer.PlayerId)
        {
            return false;
        }

        if (target.HasModifier<DreamerTargetDreamingModifier>() || target.HasModifier<DreamerInsomniaModifier>() || target.HasModifier<BaseRevealModifier>())
        {
            return false;
        }

        return true;
    }

    public void ClearDream()
    {
        DreamTargetId = byte.MaxValue;
        DreamRoleId = default;
    }

   [HideFromIl2Cpp]
    public static bool IsBreakingMaxRoleCount(RoleBehaviour role, PlayerControl target)
    {
        if (role is not ITownOfUsRole touRole)
        {
            return false;
        }

        var cap = touRole.Configuration.MaxRoleCount;

        var aliveWithRole = PlayerControl.AllPlayerControls.ToArray()
            .Count(p => p != null && p.Data?.Role != null && p.Data.Role.Role == role.Role && !p.HasDied() && p != target);

        return aliveWithRole >= cap;
    }

    [HideFromIl2Cpp]
    public static RoleBehaviour? GetRandomValidRole(PlayerControl target)
    {
        var pool = MiscUtils.GetPotentialRoles()
            .Where(r => IsRoleValid(r) && !IsBreakingMaxRoleCount(r, target))
            .ToList();

        return pool.Count == 0 ? null : pool[UnityEngine.Random.Range(0, pool.Count)];
    }
}
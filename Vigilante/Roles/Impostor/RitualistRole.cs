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
using TownOfUs;
using TownOfUs.Assets;
using TownOfUs.Extensions;
using TownOfUs.Modifiers.Crewmate;
using TownOfUs.Modules;
using TownOfUs.Modules.Wiki;
using TownOfUs.Roles;
using TownOfUs.Utilities;
using UnityEngine;
using Vigilante.Assets;
using Vigilante.Interfaces;
using Vigilante.Modifiers.Logic.Impostor;
using Vigilante.Options;
using Vigilante.Utilities;

namespace Vigilante.Roles.Impostor;

public sealed class RitualistRole(IntPtr cppPtr) : ImpostorRole(cppPtr), IVigilanteRole, IWikiDiscoverable, IDoomable
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

        if (!votePlayer.HasModifier<RitualistRitualizedModifier>())
        {
            return true;
        }

        return false;
    }

    [HideFromIl2Cpp]
    public static void CheckElimination(PlayerVoteArea voteArea, MeetingHud meeting)
    {
        if (voteArea == null) return;


    }

    [MethodRpc((uint)VigilanteRpcs.InspectorPublish)]
    public static void RpcRitualistEliminate(PlayerControl ritualist, PlayerControl victim)
    {
        if (ritualist == null || victim == null) return;


    }
}
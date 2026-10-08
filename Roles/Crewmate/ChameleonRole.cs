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

namespace Vigilante.Roles.Crewmate;

#pragma warning disable CA1001
public sealed class ChameleonRole(IntPtr cppPtr) : CrewmateRole(cppPtr), IVigilanteRole, IWikiDiscoverable, IDoomable
#pragma warning restore CA1001
{
    public string IdPart => "Chameleon";
    public Color RoleColor => VigilanteColors.Chameleon;
    public ModdedRoleTeams Team => ModdedRoleTeams.Crewmate;
    public RoleAlignment RoleAlignment => RoleAlignment.CrewmateInvestigative;

    public string RoleName => MiraLocaleManager.Get($"Vigilante.Role.{IdPart}");
    public string RoleDescription => MiraLocaleManager.Get($"Vigilante.Role.{IdPart}.IntroBlurb");
    public string RoleLongDescription => MiraLocaleManager.Get($"Vigilante.Role.{IdPart}.TabDescription");

    public DoomableType DoomHintType => DoomableType.Perception;

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
                new(MiraLocaleManager.Get($"Vigilante.Role.{IdPart}.Camouflage", "Camouflage"),
                    MiraLocaleManager.Get($"Vigilante.Role.{IdPart}.Camouflage.WikiDescription"),
                    TouCrewAssets.CrewSwoopSprite)
            ];
        }
    }

    public CustomRoleConfiguration Configuration => new(this)
    {
        IconTmp = TmpSpriteUtils.CreateSpriteAsset(VigilanteAssets.ChameleonIcon.LoadAsset(), "Vigilante.Role.Crewmate.Chameleon", 1.45f),
        Icon = VigilanteAssets.ChameleonIcon,
        IntroSound = TouAudio.DetectiveIntroSound,
        GetsVentData = OptionGroupSingleton<ChameleonOptions>.Instance.CanVent.Value
    };

    public override void Initialize(PlayerControl player)
    {
        RoleBehaviourStubs.Initialize(this, player);
        if (Player.AmOwner)
        {
            CustomButtonSingleton<FakeVentButton>.Instance.Show = false;
        }
    }

    public override void Deinitialize(PlayerControl targetPlayer)
    {
        RoleBehaviourStubs.Deinitialize(this, targetPlayer);
        if (Player.AmOwner)
        {
            CustomButtonSingleton<FakeVentButton>.Instance.Show = true;
        }
    }
}
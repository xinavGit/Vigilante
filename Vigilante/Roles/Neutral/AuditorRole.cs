using System.Collections;
using AmongUs.GameOptions;
using HarmonyLib;
using Il2CppInterop.Runtime.Attributes;
using InnerNet;
using MiraAPI.GameEnd;
using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using MiraAPI.Modifiers.Types;
using MiraAPI.Patches.Stubs;
using MiraAPI.Roles;
using MiraAPI.Translation;
using MiraAPI.Utilities;
using MiraAPI.Utilities.Assets;
using Reactor.Networking.Attributes;
using Reactor.Utilities;
using TownOfUs;
using TownOfUs.Achievements;
using TownOfUs.Assets;
using TownOfUs.Events;
using TownOfUs.Extensions;
using TownOfUs.GameOver;
using TownOfUs.Modifiers;
using TownOfUs.Modifiers.Game;
using TownOfUs.Modifiers.Neutral;
using TownOfUs.Modules.Wiki;
using TownOfUs.Options;
using TownOfUs.Options.Modifiers.Alliance;
using TownOfUs.Options.Roles.Neutral;
using TownOfUs.Roles;
using TownOfUs.Roles.Crewmate;
using TownOfUs.Roles.Neutral;
using TownOfUs.Roles.Other;
using TownOfUs.Utilities;
using UnityEngine;
using Random = System.Random;

namespace Vigilante.Roles.Neutral;

public sealed class AuditorRole(IntPtr cppPtr) : NeutralRole(cppPtr), ITownOfUsRole, IWikiDiscoverable,
    IDoomable, ICrewVariant
{
    public override void SpawnTaskHeader(PlayerControl playerControl)
    {
        if (!playerControl.AmOwner)
        {
            return;
        }
        ImportantTextTask orCreateTask = PlayerTask.GetOrCreateTask<ImportantTextTask>(playerControl, 0);
        orCreateTask.Text = $"{TownOfUsColors.Neutral.ToTextColor()}{MiraLocaleManager.Get("NeutralBenignTaskHeader")}</color>";
        orCreateTask.name = "NeutralRoleText";
    }

    public RoleBehaviour CrewVariant => RoleManager.Instance.GetRole((RoleTypes)RoleId.Get<BarkeeperRole>());

    public DoomableType DoomHintType => DoomableType.Protective;
    public string IdPart => "Auditor";
    public string RoleDescription => MiraLocaleManager.Get($"Vigilante.Role.{IdPart}.IntroBlurb");
    public string RoleLongDescription => MiraLocaleManager.Get($"Vigilante.Role.{IdPart}.TabDescription");

    public string GetAdvancedDescription()
    {
        return
            MiraLocaleManager.Get($"Vigilante.Role.{IdPart}.WikiDescription")
                .Replace("<symbol>", "<color=#B3FFFFFF>★</color>") +
            MiscUtils.AppendOptionsText(GetType());
    }

    public Color RoleColor => TownOfUsColors.Chef;
    public ModdedRoleTeams Team => ModdedRoleTeams.Custom;
    public RoleAlignment RoleAlignment => RoleAlignment.NeutralBenign;

    public CustomRoleConfiguration Configuration => new(this)
    {
        IconTmp = TmpSpriteUtils.CreateSpriteAsset(TouRoleIcons.Fairy, "Vigilante.Role.Neutral.Auditor", 1.45f),
        Icon = TouRoleIcons.Fairy,
        IntroSound = TouAudio.DetectiveIntroSound,
        OptionsScreenshot = TouBanners.NeutralRoleBanner,
    };




    [HideFromIl2Cpp]
    public List<AdvancedWikiAbilityDescription> WikiAbilities
    {
        get
        {
            return
            [
                new(MiraLocaleManager.Get($"Vigilante.Role.{IdPart}Protect", "Protect"),
                    MiraLocaleManager.Get("MiraApi.AbilityType.Basic"),
                    MiraLocaleManager.Get($"Vigilante.Role.{IdPart}Protect.WikiDescription"),
                    TouNeutAssets.ProtectSprite)
            ];
        }
    }

    public override void Initialize(PlayerControl player)
    {
        RoleBehaviourStubs.Initialize(this, player);
    }

    public override void Deinitialize(PlayerControl targetPlayer)
    {
        RoleBehaviourStubs.Deinitialize(this, targetPlayer);
        TouRoleUtils.ClearTaskHeader(Player);
        if (Player.AmOwner)
        {
            var players = ModifierUtils
                .GetPlayersWithModifier<
                    GuardianAngelTargetModifier>([HideFromIl2Cpp](x) => x.OwnerId == Player.PlayerId);
            players.Do(x => x.RpcRemoveModifier<GuardianAngelTargetModifier>());
        }
    }

    public override bool DidWin(GameOverReason gameOverReason)
    {
        return (!OptionGroupSingleton<EgotistOptions>.Instance.EgotistMustSurvive || !Player.HasDied()) && !(gameOverReason is GameOverReason.CrewmatesByVote or GameOverReason.CrewmatesByTask
            or GameOverReason.ImpostorDisconnect || gameOverReason == CustomGameOver.GameOverReason<DrawGameOver>());
    }

    [MethodRpc((uint)TownOfUsRpc.SetFairyTarget)]
    public static void RpcSetFairyTarget(PlayerControl player, PlayerControl target)
    {
    }
}
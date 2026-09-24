using AmongUs.GameOptions;
using Il2CppInterop.Runtime.Attributes;
using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using MiraAPI.Patches.Stubs;
using MiraAPI.Roles;
using MiraAPI.Translation;
using MiraAPI.Utilities;
using MiraAPI.Utilities.Assets;
using TownOfUs;
using TownOfUs.Assets;
using TownOfUs.Extensions;
using TownOfUs.Interfaces;
using TownOfUs.Modifiers;
using TownOfUs.Modules.Wiki;
using TownOfUs.Options.Roles.Neutral;
using TownOfUs.Roles;
using TownOfUs.Roles.Neutral;
using TownOfUs.Utilities;
using UnityEngine;

namespace Vigilante.Roles.Neutral;

public sealed class AuditorRole(IntPtr cppPtr)
    : NeutralRole(cppPtr), ITownOfUsRole, IWikiDiscoverable, IDoomable
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

    public DoomableType DoomHintType => DoomableType.Protective;
    public string IdPart => "Auditor";
    public string RoleMedDescriptionLocale => $"TownOfUsMira.Role.{IdPart}.TabDescription";

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
                new(MiraLocaleManager.Get($"TownOfUsMira.Role.{IdPart}Safeguard", "Safeguard"),
                    MiraLocaleManager.Get($"TownOfUsMira.Role.{IdPart}Safeguard.WikiDescription"),
                    TouNeutAssets.VestSprite)
            ];
        }
    }

    public Color RoleColor => TownOfUsColors.Survivor;
    public ModdedRoleTeams Team => ModdedRoleTeams.Custom;

    public RoleAlignment RoleAlignment => RoleAlignment.NeutralBenign;

    public CustomRoleConfiguration Configuration => new(this)
    {
        IconTmp = TmpSpriteUtils.CreateSpriteAsset(TouRoleIcons.Survivor.LoadAsset(), "TouMira.Role.Neutral.Survivor", 1.45f),
        IntroSound = TouAudio.ToppatIntroSound,
        Icon = TouRoleIcons.Survivor,
        OptionsScreenshot = TouBanners.NeutralRoleBanner,
    };

    public override void Deinitialize(PlayerControl targetPlayer)
    {
        RoleBehaviourStubs.Deinitialize(this, targetPlayer);
        TouRoleUtils.ClearTaskHeader(Player);
    }

    /*public override bool DidWin(GameOverReason gameOverReason)
    {
        return !Player.HasDied();
    }*/

    public bool WinConditionMet()
    {
        var hasLivingHalters = MiscUtils.NKillersAliveCount > 0 ||
                               MiscUtils.ImpAliveCount > 0 || MiscUtils.CrewKillersAliveCount > 0 ||
                               (MiscUtils.GameHaltersAliveCount > 0 && Helpers.GetAlivePlayers().Count > 1)
                               || Helpers.GetAlivePlayers().All(x =>
                                   (x.IsCrewmate() || x.Is(RoleAlignment.NeutralBenign)) && !x.IsImpostorAligned());
        var survCount = CustomRoleUtils.GetActiveRolesOfType<SurvivorRole>().Count(x => !x.Player.HasDied());

        if (survCount == 0 || MiscUtils.NonGameEndingNeutralCount == 0 || Helpers.GetAlivePlayers().Count > 3 ||
            hasLivingHalters)
        {
            return false;
        }

        return true;
    }
}
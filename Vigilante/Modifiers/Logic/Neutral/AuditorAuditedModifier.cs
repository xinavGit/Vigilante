using AmongUs.GameOptions;
using HarmonyLib;
using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using MiraAPI.Roles;
using MiraAPI.Translation;
using MiraAPI.Utilities;
using TownOfUs;
using TownOfUs.Assets;
using TownOfUs.Events;
using TownOfUs.Extensions;
using TownOfUs.Interfaces;
using TownOfUs.Modules;
using TownOfUs.Options.Roles.Crewmate;
using TownOfUs.Roles;
using TownOfUs.Roles.Crewmate;
using TownOfUs.Roles.Neutral;
using TownOfUs.Utilities;
using UnityEngine;

namespace Vigilante.Modifiers.Logic.Neutral;

public sealed class AuditorAuditedModifier(RoleBehaviour originalRole) : BaseModifier, ICachedRole, IContinuesGame
{
    public bool CanDisplayForRole(RoleBehaviour role)
    {
        return !role.IsDead && role.Role != CachedRole.Role;
    }

    public bool ContinuesGame =>
        !Player.HasDied() && Player.IsCrewmate() && (MiscUtils.NKillersAliveCount > 0 || MiscUtils.ImpAliveCount > 0) &&
        MiscUtils.CrewKillersAliveCount == 0 && originalRole is ITouCrewRole { IsPowerCrew: true };

    public override string ModifierName => "Audited";
    //public string CachedRoleName => "Audited";
    public override bool HideOnUi => true;
    public bool ShowCurrentRoleFirst => false;

    public bool Visible => Player.AmOwner || PlayerControl.LocalPlayer.HasDied() ||
                           FairyRole.FairySeesRoleVisibilityFlag(Player);

    public CacheRoleGuess GuessMode => CacheRoleGuess.CachedRole;

    public RoleBehaviour CachedRole => originalRole;

    public override void OnActivate()
    {
        base.OnActivate();

        var notif1 = Helpers.CreateAndShowNotification(
            $"<b>{TownOfUsColors.Chef.ToTextColor()}{MiraLocaleManager.Get("Vigilante.Feedback.Auditor.Audited")}</color></b>",
            Color.white, new Vector3(0f, 1f, -150f),
            spr: TouRoleIcons.Fairy);
        notif1.AdjustNotification();
    }

    public override void OnDeactivate()
    {
        var notif1 = Helpers.CreateAndShowNotification(
            $"<b>{TownOfUsColors.Chef.ToTextColor()}{MiraLocaleManager.Get("Vigilante.Feedback.Auditor.AuditOver")}</color></b>",
            Color.white, new Vector3(0f, 1f, -150f),
            spr: TouRoleIcons.Fairy);
        notif1.AdjustNotification();

        base.OnDeactivate();
    }
}
using AmongUs.GameOptions;
using Vigilante.Assets;
using MiraAPI.Modifiers;
using MiraAPI.Utilities;
using MiraAPI.Utilities.Assets;
using TownOfUs.Roles;
using UnityEngine;
using Reactor.Utilities;
using TownOfUs.Utilities;
using TownOfUs.Extensions;
using MiraAPI.Translation;
using TownOfUs.Roles.Neutral;

namespace Vigilante.Modifiers.Logic.Crewmate;

public sealed class DreamerTargetDreamingModifier(ushort originalRoleId, ushort dreamRoleId) : BaseModifier, ICachedRole
{
    public static readonly Color DreamerColor = new Color32(51, 51, 153, 255);
    public override string ModifierName => "Dreaming";
    public override bool HideOnUi => true;
    public override LoadableAsset<Sprite>? ModifierIcon => VigilanteAssets.DreamerIcon;
    public ushort OriginalRoleId { get; set; } = originalRoleId;
    public ushort DreamRoleId { get; set; } = dreamRoleId;

    public bool ShowCurrentRoleFirst => true;
    public bool Visible => Player.AmOwner || PlayerControl.LocalPlayer.HasDied() || FairyRole.FairySeesRoleVisibilityFlag(Player);
    public CacheRoleGuess GuessMode => CacheRoleGuess.ActiveRole; // placeholder
    public RoleBehaviour CachedRole => RoleManager.Instance.GetRole((RoleTypes)OriginalRoleId);
    public string CachedRoleName => $"{VigilanteColors.Dreamer.ToTextColor()}{MiraLocaleManager.Get($"Vigilante.Role.Dreamer.DreamingShortName")}</color>";

    public override void OnActivate()
    {
        base.OnActivate();

        if (Player == null || !Player.AmOwner)
        {
            return;
        }

        var dreamRoleName = (RoleManager.Instance.GetRole((RoleTypes)DreamRoleId) as ITownOfUsRole)?.RoleName ?? "a new role";

        Helpers.CreateAndShowNotification(
            $"<b>{MiraLocaleManager.Get($"Vigilante.Role.DreamerDreamSuccess").Replace("<role>", dreamRoleName)}</b>",
            Color.white, spr: VigilanteAssets.DreamerIcon.LoadAsset());
        
        Coroutines.Start(MiscUtils.CoFlash(VigilanteColors.Dreamer));
    }

    public override void OnDeactivate()
    {
        base.OnDeactivate();

        if (Player == null || !Player.AmOwner)
        {
            return;
        }

        Helpers.CreateAndShowNotification(
            $"<b>{MiraLocaleManager.Get($"Vigilante.Role.DreamerDreamReverted")}</b>",
            Color.white, spr: VigilanteAssets.DreamerIcon.LoadAsset());
        
        Coroutines.Start(MiscUtils.CoFlash(VigilanteColors.Dreamer));
    }

    public override void OnDeath(DeathReason reason)
    {
        ModifierComponent?.RemoveModifier(this);
    }
}

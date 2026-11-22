using HarmonyLib;
using MiraAPI.GameOptions;
using MiraAPI.Keybinds;
using MiraAPI.LocalSettings;
using MiraAPI.Modifiers;
using MiraAPI.Translation;
using MiraAPI.Utilities;
using MiraAPI.Utilities.Assets;
using Reactor.Utilities;
using TownOfUs;
using TownOfUs.Assets;
using TownOfUs.Buttons;
using TownOfUs.Modifiers;
using TownOfUs.Modules;
using TownOfUs.Modules.Components;
using TownOfUs.Options.Roles.Crewmate;
using TownOfUs.Roles.Crewmate;
using TownOfUs.Utilities;
using UnityEngine;
using Vigilante.Assets;
using Vigilante.Modifiers.Logic.Crewmate;
using Vigilante.Modifiers.Logic.Neutral;
using Vigilante.Options;
using Vigilante.Roles.Crewmate;
using Vigilante.Roles.Neutral;

namespace Vigilante.Buttons.Neutral;

public sealed class AuditorAuditButton : TownOfUsRoleButton<AuditorRole, PlayerControl>
{
    public override string Name => MiraLocaleManager.Get("Vigilante.Role.Auditor.Audit");
    public override BaseKeybind Keybind => Keybinds.PrimaryAction;
    public override Color TextOutlineColor => TownOfUsColors.Chef;
    public override float Cooldown => Math.Clamp(OptionGroupSingleton<AuditorOptions>.Instance.AuditCooldown + MapCooldown, 5f, 120f);
    public override LoadableAsset<Sprite> Sprite => TouAssets.UnderclockSprite;
    public override int MaxUses => (int)OptionGroupSingleton<AuditorOptions>.Instance.AuditUses;
    public override bool UsableFirstRound => OptionGroupSingleton<AuditorOptions>.Instance.RoundOneAudit.Value;

    public override PlayerControl? GetTarget()
    {
        return PlayerControl.LocalPlayer.GetClosestLivingPlayer(true, Distance,
            predicate: x => !x.HasModifier<AuditorAuditTargetModifier>() && !x.HasModifier<AuditorAuditedModifier>());
    }

    protected override void OnClick()
    {
        if (Target == null)
        {
            return;
        }

        var notif1 = Helpers.CreateAndShowNotification(
            $"<b>{TownOfUsColors.Chef.ToTextColor()}{MiraLocaleManager.Get("Vigilante.Feedback.Auditor.Auditing").Replace("<player>", Target.Data.PlayerName)}</color></b>",
            Color.white, new Vector3(0f, 1f, -150f),
            spr: TouRoleIcons.Fairy);
        notif1.AdjustNotification();

        Target.RpcAddModifier<AuditorAuditTargetModifier>();
    }

}

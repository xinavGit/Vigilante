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
using Vigilante.Options;
using Vigilante.Roles.Crewmate;

namespace Vigilante.Buttons.Crewmate;
// code from TOUM Monarch Knight Button
public sealed class MagistrateEnlistButton : TownOfUsRoleButton<MagistrateRole, PlayerControl>
{
    public override string Name => MiraLocaleManager.Get("Vigilante.Feedback.Magistrate.Enlist");
    public override BaseKeybind Keybind => Keybinds.PrimaryAction;
    public override Color TextOutlineColor => VigilanteColors.Magistrate;
    public override float Cooldown => OptionGroupSingleton<MagistrateOptions>.Instance.EnlistCooldown.Value + MapCooldown;
    public override float EffectDuration => OptionGroupSingleton<MagistrateOptions>.Instance.EnlistDelay.Value;
    public override int MaxUses => 1;
    public override LoadableAsset<Sprite> Sprite => TouCrewAssets.KnightSprite;
    public PlayerControl? _enlistedTarget;
    private bool _isProcessingClick;

    //public override bool UsableFirstRound => OptionGroupSingleton<MonarchOptions>.Instance.FirstRoundUse;

    public override bool CanUse()
    {
        if (TimeLordRewindSystem.IsRewinding)
        {
            return false;
        }

        if (PlayerControl.LocalPlayer.HasDied() && !UsableInDeath)
        {
            return false;
        }

        if (HudManager.Instance.Chat.IsOpenOrOpening || MeetingHud.Instance)
        {
            return false;
        }

        if (!UsableFirstRound && HudManagerHelper.Instance.CurrentRound == 1 && !TutorialManager.InstanceExists)
        {
            return false;
        }

        if (!PlayerControl.LocalPlayer.CanMove ||
            PlayerControl.LocalPlayer.GetModifiers<DisabledModifier>().Any(x => !x.CanUseAbilities))
        {
            return false;
        }

        var newTarget = GetTarget();
        if (newTarget != Target)
        {
            SetOutline(false);
        }

        Target = IsTargetValid(newTarget) ? newTarget : null;
        SetOutline(true);

        return PlayerControl.LocalPlayer.moveable &&
               (EffectActive || (!EffectActive && Target != null && (!LimitedUses || UsesLeft > 0) && Timer <= 0));
    }

    public override bool CanClick()
    {
        return CanUse();
    }

    public override void ClickHandler()
    {
        if (_isProcessingClick)
        {
            return;
        }

        _isProcessingClick = true;


        try
        {
            if (!CanClick())
            {
                return;
            }

            OnClick();
        }
        finally
        {
            Coroutines.Start(ResetProcessingFlag());
        }
    }

    private System.Collections.IEnumerator ResetProcessingFlag()
    {
        yield return new WaitForSeconds(0.2f);
        _isProcessingClick = false;
    }

    public override PlayerControl? GetTarget()
    {
        return PlayerControl.LocalPlayer.GetClosestLivingPlayer(true, Distance,
            predicate: x => !x.HasModifier<KnightedModifier>());
    }

    protected override void OnClick()
    {
        if (EffectActive)
        {
            var notif2 = Helpers.CreateAndShowNotification(
                $"<b>{MiraLocaleManager.Get("Vigilante.Feedback.Magistrate.EnlistingCancel")}</b>",
                Color.white, new Vector3(0f, 1f, -20f), spr: VigilanteAssets.JudgeIcon.LoadAsset());
            notif2.Text.SetOutlineThickness(0.35f);
            _enlistedTarget = null;
            ResetCooldownAndOrEffect();
            EffectActive = false;
            Timer = 0.001f;
            return;
        }

        if (Target == null)
        {
            return;
        }

        OverrideName(MiraLocaleManager.Get("Vigilante.Feedback.Magistrate.Enlisting"));

        _enlistedTarget = Target;
        var notif = Helpers.CreateAndShowNotification(
            $"<b>{MiraLocaleManager.Get("Vigilante.Feedback.Magistrate.EnlistingPlayer") .Replace("<player>", _enlistedTarget.CachedPlayerData.PlayerName) .Replace("<seconds>", OptionGroupSingleton<MonarchOptions>.Instance.KnightDelay.ToString(TownOfUsPlugin.Culture))}</b>",
            Color.white, new Vector3(0f, 1f, -20f), spr: VigilanteAssets.JudgeIcon.LoadAsset());
        notif.Text.SetOutlineThickness(0.35f);

        if (HasEffect)
        {
            EffectActive = true;
            Timer = EffectDuration;
        }
        else
        {
            Timer = Cooldown;
        }
    }

    public override void OnEffectEnd()
    {
        OverrideName(MiraLocaleManager.Get("Vigilante.Feedback.Magistrate.Enlist"));

        if (_enlistedTarget == null) return;

        if (LimitedUses)
        {
            UsesLeft--;
            Button?.SetUsesRemaining(UsesLeft);
            TownOfUsColors.UseBasic = false;
            if (TextOutlineColor != Color.clear)
            {
                SetTextOutline(TextOutlineColor);
                Button?.usesRemainingSprite.color = TextOutlineColor;
            }

            TownOfUsColors.UseBasic = LocalSettingsTabSingleton<TouLocalTabPlayers>.Instance
                .UseCrewmateTeamColorToggle.Value;
        }

        MagistrateRole.RpcEnlist(PlayerControl.LocalPlayer, _enlistedTarget);
        _enlistedTarget = null;
    }

}

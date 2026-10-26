using MiraAPI.GameOptions;
using MiraAPI.Hud;
using MiraAPI.Keybinds;
using MiraAPI.Translation;
using MiraAPI.Utilities;
using MiraAPI.Utilities.Assets;
using Reactor.Utilities.Extensions;
using TownOfUs.Buttons;
using TownOfUs.Options.Roles.Crewmate;
using TownOfUs.Roles.Crewmate;
using TownOfUs.Utilities;
using UnityEngine;
using Vigilante.Assets;
using Vigilante.Options;
using Vigilante.Roles.Crewmate;
using Object = UnityEngine.Object;

namespace Vigilante.Buttons.Crewmate;

public sealed class CrusaderSanctifyButton : TownOfUsRoleButton<CrusaderRole>, IAftermathableButton
{
    public override string Name => MiraLocaleManager.Get("Vigilante.Role.Crusader.Sanctify", "Sanctify");
    public override BaseKeybind Keybind => Keybinds.PrimaryAction;
    public override Color TextOutlineColor => VigilanteColors.Crusader;

    public override float Cooldown =>
        Math.Clamp(OptionGroupSingleton<CrusaderOptions>.Instance.SanctifyCooldown.Value + MapCooldown, 0.001f, 120f);

    public override float EffectDuration => OptionGroupSingleton<CrusaderOptions>.Instance.SanctifyDuration.Value;
    public override LoadableAsset<Sprite> Sprite => VigilanteAssets.CrusaderSanctify;
    public override ButtonLocation Location => ButtonLocation.BottomRight;
    public override bool ShouldPauseInVent => false;
    public bool TargetWasValid { get; set; }

    public override bool CanUse()
    {
        return base.CanUse() && Role is { Protected: null } &&
               !EffectActive && Timer <= 0;
    }

    public override void ClickHandler()
    {
        if (!CanUse())
        {
            return;
        }

        OnClick();
    }

    public void AftermathHandler()
    {
        var player = PlayerControl.AllPlayerControls.ToArray().Where(plr => !plr.HasDied()).Random();
        if (player == null)
        {
            return;
        }

        CrusaderRole.RpcSanctify(PlayerControl.LocalPlayer, player);
        EffectActive = true;
        Timer = EffectDuration;
        OverrideName(MiraLocaleManager.Get("Vigilante.Feedback.Crusader.Sanctifying", "Sanctifying"));
        TargetWasValid = true;
    }

    protected override void OnClick()
    {
        /*if (!OptionGroupSingleton<GlitchOptions>.Instance.MoveWithMenu)
        {
            PlayerControl.LocalPlayer.NetTransform.Halt();
        }*/

        var playerMenu = CustomPlayerMenu.Create();
        playerMenu.Begin(
            plr => (!plr.HasDied() ||
                    Object.FindObjectsOfType<DeadBody>().FirstOrDefault(x => x.ParentId == plr.PlayerId) ||
                    MiscUtils.GetFakePlayer(plr)
                        ?.body) && !plr.AmOwner,
            plr =>
            {
                playerMenu.ForceClose();

                if (plr != null)
                {
                    CrusaderRole.RpcSanctify(PlayerControl.LocalPlayer, plr);

                    EffectActive = true;
                    Timer = EffectDuration;
                    OverrideName(MiraLocaleManager.Get("Vigilante.Role.Crusader.Sanctifying", "Sanctifying"));
                    TargetWasValid = !plr.HasDied();
                }
                else
                {
                    EffectActive = false;
                    Timer = 0.01f;
                }
            });
        foreach (var panel in playerMenu.potentialVictims)
        {
            panel.PlayerIcon.cosmetics.SetPhantomRoleAlpha(1f);
            if (panel.NameText.text != PlayerControl.LocalPlayer.Data.PlayerName)
            {
                panel.NameText.color = Color.white;
            }
        }
    }

    public override void OnEffectEnd()
    {
        var text = string.Empty;
        if (TargetWasValid)
        {
            //DecreaseUses();
        }
        else
        {
            text = MiraLocaleManager.Get("Vigilante.Feedback.Crusader.TargetDead");
        }

        // Incase the player changed roles
        if (PlayerControl.LocalPlayer.Data.Role is CrusaderRole)
        {
            if (Role.Protected != null && Role.Protected.HasDied())
            {
                return; // indirect attack death
            }
            else if (Role.Protected != null && !Role.Protected.HasDied())
            {
                return;
            }

            if (text.Contains("<player>") && Role.Protected != null)
            {
                text = text.Replace("<player>", Role.Protected.Data.PlayerName);
            }

            if (text != string.Empty && !MeetingHud.Instance)
            {
                var notif1 = Helpers.CreateAndShowNotification(text,
                    Color.white, new Vector3(0f, 1f, -20f), spr: VigilanteAssets.CrusaderIcon.LoadAsset());
                notif1.AdjustNotification();
            }
            CrusaderRole.RpcClearSanctify(PlayerControl.LocalPlayer);
        }

        TargetWasValid = false;
        OverrideName(MiraLocaleManager.Get("Vigilante.Role.Crusader.Sanctify", "Sanctify"));
    }
}
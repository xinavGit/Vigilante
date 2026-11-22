using MiraAPI.GameOptions;
using MiraAPI.Keybinds;
using MiraAPI.Modifiers;
using MiraAPI.Translation;
using MiraAPI.Utilities;
using MiraAPI.Utilities.Assets;
using TownOfUs;
using TownOfUs.Achievements;
using TownOfUs.Assets;
using TownOfUs.Buttons;
using TownOfUs.Modifiers.Impostor;
using TownOfUs.Options.Roles.Impostor;
using TownOfUs.Roles.Impostor;
using TownOfUs.Utilities;
using UnityEngine;
using Vigilante.Modifiers.Logic.Neutral;
using Vigilante.Options;
using Vigilante.Roles.Neutral;

namespace Vigilante.Buttons.Neutral;

public sealed class AuditorSmokebombButton : TownOfUsRoleButton<AuditorRole>
{
    public override string Name => MiraLocaleManager.Get("Vigilante.Role.AuditorFlash", "Flash");
    public override BaseKeybind Keybind => Keybinds.PrimaryAction;
    public override Color TextOutlineColor => TownOfUsColors.Chef;
    public override float Cooldown => Math.Clamp(OptionGroupSingleton<AuditorOptions>.Instance.SmokebombCooldown + MapCooldown, 5f, 120f);
    public override float EffectDuration => OptionGroupSingleton<AuditorOptions>.Instance.SmokebombDuration;
    public override int MaxUses => (int)OptionGroupSingleton<AuditorOptions>.Instance.MaxSmokebombs;
    public override LoadableAsset<Sprite> Sprite => TouImpAssets.FlashSprite;

    public override bool ZeroIsInfinite { get; set; } = true;

    protected override void OnClick()
    {
        var flashRadius = OptionGroupSingleton<AuditorOptions>.Instance.SmokebombRadius;
        var flashedPlayers =
            Helpers.GetClosestPlayers(PlayerControl.LocalPlayer, flashRadius * ShipStatus.Instance.MaxLightRadius);

        foreach (var player in flashedPlayers)
        {
            player.RpcAddModifier<AuditorSmokebombedModifier>();

            if (player.TryGetModifier<AuditorRevealProgressModifier>(out var mod))
            {
                mod.TimesSmokebombed++;

                if (mod.TimesSmokebombed == 3)
                {
                    player.AddModifier<AuditorRevealModifier>(player.Data.Role);
                }
            }
            else
            {
                player.RpcAddModifier<AuditorRevealProgressModifier>();
            }
        }

        HudManager.Instance.StartCoroutine(HudManager.Instance.PlayerCam.CoShakeScreen(0.2f, 2f));
        var notif1 = Helpers.CreateAndShowNotification(
            $"<b>{TownOfUsColors.Chef.ToTextColor()}{MiraLocaleManager.Get("Vigilante.Feedback.Auditor.Smokebombing")}</color></b>",
            Color.white, new Vector3(0f, 1f, -150f),
            spr: TouRoleIcons.Fairy);
        notif1.AdjustNotification();

        SoundManager.Instance.PlaySound(TouAudio.GrenadeSound, false);
    }
}
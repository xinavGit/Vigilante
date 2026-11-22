using System.Collections;
using MiraAPI.Events;
using MiraAPI.GameOptions;
using MiraAPI.Translation;
using MiraAPI.Utilities;
using Reactor.Utilities;
using TownOfUs;
using TownOfUs.Assets;
using TownOfUs.Events.TouEvents;
using TownOfUs.Modifiers;
using TownOfUs.Modules;
using TownOfUs.Options.Roles.Impostor;
using TownOfUs.Utilities;
using UnityEngine;
using Vigilante.Options;
using Color = UnityEngine.Color;

namespace Vigilante.Modifiers.Logic.Neutral;

public sealed class AuditorSmokebombedModifier() : DisabledModifier
{
    public override string ModifierName => "Flashed";
    public override bool HideOnUi => true;
    public override float Duration => OptionGroupSingleton<AuditorOptions>.Instance.SmokebombDuration + 0.5f;
    public override bool AutoStart => true;
    public override bool CanBeInteractedWith => true;
    public override bool IsConsideredAlive => true;
    public override bool CanUseAbilities => true;
    public override bool CanUseConsoles => true;
    public override bool CanOpenMap => true;
    public override bool CanReport => true;

    public override void OnActivate()
    {
        base.OnActivate();

        if (Player.AmOwner)
        {
                var notif1 = Helpers.CreateAndShowNotification(
                    $"<b>{TownOfUsColors.Chef.ToTextColor()}{MiraLocaleManager.Get("Vigilante.Modifier.AuditorFlashedNotification")}</color></b>",
                    Color.white,
                    spr: TouRoleIcons.Fairy);

                notif1.AdjustNotification();
                notif1.transform.localPosition = new Vector3(0f, 1f, -150f);
            Coroutines.Start(CoShakeScreen());
        }
    }

    private static IEnumerator CoShakeScreen()
    {
        yield return new WaitForSeconds(0.5f);

        HudManager.Instance.StartCoroutine(HudManager.Instance.PlayerCam.CoShakeScreen(0.4f, 5f));
    }

    public override void OnMeetingStart()
    {
        RemoveSelf();
    }
}
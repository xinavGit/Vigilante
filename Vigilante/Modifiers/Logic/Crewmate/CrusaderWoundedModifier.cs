using MiraAPI.GameOptions;
using MiraAPI.Hud;
using MiraAPI.Modifiers;
using MiraAPI.Translation;
using MiraAPI.Utilities;
using MiraAPI.Utilities.Assets;
using Reactor.Utilities;
using TownOfUs;
using TownOfUs.Assets;
using TownOfUs.Options.Roles.Crewmate;
using TownOfUs.Utilities;
using UnityEngine;
using Vigilante.Assets;
using Vigilante.Options;

namespace Vigilante.Modifiers.Logic.Crewmate;

public sealed class CrusaderWoundedModifier() : BaseModifier
{
    public override string ModifierName => MiraLocaleManager.Get("Vigilante.Feedback.Crusader.Wounded");
    public override bool HideOnUi => false;
    public override LoadableAsset<Sprite>? ModifierIcon => VigilanteAssets.CrusaderIcon;
    public override bool Unique => false;
    public override bool ShowInFreeplay => true;

    public bool Removing { get; set; }

    public override string GetDescription()
    {
        return "Your cooldowns are slowed until the Crusader dies.";
    }

    public override void OnActivate()
    {
        base.OnActivate();

        var notif1 = Helpers.CreateAndShowNotification(
                $"<b>{VigilanteColors.Crusader.ToTextColor()}You were wounded by a Crusader! Your cooldowns have been slowed, and will stay that way until the Crusader dies.</color></b>", Color.white,
                new Vector3(0f, 1f, -20f), spr: VigilanteAssets.CrusaderIcon);

            notif1.AdjustNotification();
        Coroutines.Start(MiscUtils.CoFlash(VigilanteColors.Crusader));
    }

    public override void FixedUpdate()
    {
        if (!Player.AmOwner) return;
        
        var value = -0.5f;
        
        foreach (var ability in CustomButtonManager.Buttons)
        {
            if (ability.EffectActive) continue;
            if (ability.TimerPaused) continue;

            ability.DecreaseTimer(Time.deltaTime * value);
        }

        Player.killTimer -= Time.deltaTime * value;
    }

    public override void OnDeactivate()
    {
        var notif1 = Helpers.CreateAndShowNotification(
                $"<b>{VigilanteColors.Crusader.ToTextColor()}You are no longer wounded!</color></b>", Color.white,
                new Vector3(0f, 1f, -20f), spr: VigilanteAssets.CrusaderIcon);

            notif1.AdjustNotification();
        Coroutines.Start(MiscUtils.CoFlash(VigilanteColors.Crusader));

        base.OnDeactivate();
    }

    public override void OnDeath(DeathReason reason)
    {
        ModifierComponent?.RemoveModifier(this);
    }
}
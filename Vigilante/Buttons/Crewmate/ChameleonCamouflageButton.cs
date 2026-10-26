using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using TownOfUs.Buttons;
using UnityEngine;
using TownOfUs;
using TownOfUs.Modifiers.Neutral;
using TownOfUs.Modifiers;
using TownOfUs.Assets;
using Vigilante.Roles.Crewmate;
using MiraAPI.Keybinds;
using Vigilante.Options;
using MiraAPI.Utilities.Assets;
using Vigilante.Modifiers.Logic.Crewmate;

namespace Vigilante.Buttons.Crewmate;

public sealed class ChameleonCamouflageButton : TownOfUsRoleButton<ChameleonRole>
{
    public override Color TextOutlineColor => VigilanteColors.Chameleon;
    public override string Name => "Camouflage";
    public override BaseKeybind Keybind => Keybinds.PrimaryAction;
    public override float Cooldown => Math.Clamp(OptionGroupSingleton<ChameleonOptions>.Instance.CamouflageCooldown.Value + MapCooldown, 5f, 120f);
    public override float EffectDuration => OptionGroupSingleton<ChameleonOptions>.Instance.CamouflageDuration.Value;
    public override int MaxUses => (int)OptionGroupSingleton<ChameleonOptions>.Instance.MaxCamouflages;
    public override LoadableAsset<Sprite> Sprite => TouCrewAssets.CrewSwoopSprite;

    public override bool ZeroIsInfinite { get; set; } = true;

    public override void ClickHandler()
    {
        if (!CanUse())
        {
            return;
        }

        OnClick();
        Button?.SetDisabled();
        if (EffectActive)
        {
            Timer = Cooldown;
            EffectActive = false;
        }
        else if (HasEffect)
        {
            EffectActive = true;
            Timer = EffectDuration;
        }
        else
        {
            Timer = Cooldown;
        }
    }

    public override bool CanUse()
    {
        if (HudManager.Instance.Chat.IsOpenOrOpening || MeetingHud.Instance)
        {
            return false;
        }

        if (PlayerControl.LocalPlayer.HasModifier<GlitchHackedModifier>() || PlayerControl.LocalPlayer
                .GetModifiers<DisabledModifier>().Any(x => !x.CanUseAbilities))
        {
            return false;
        }

        return ((Timer <= 0 && !EffectActive && (!LimitedUses || UsesLeft > 0)) ||
                (EffectActive && Timer <= EffectDuration - 2f));
    }

    protected override void OnClick()
    {
        if (!EffectActive)
        {
            PlayerControl.LocalPlayer.RpcAddModifier<ChameleonCamouflageModifier>();
            if (LimitedUses)
            {
                UsesLeft--;
                Button?.SetUsesRemaining(UsesLeft);
            }
        }
        else
        {
            OnEffectEnd();
        }
    }

    public override void OnEffectEnd()
    {
        if (!PlayerControl.LocalPlayer.HasModifier<ChameleonCamouflageModifier>())
        {
            return;
        }

        PlayerControl.LocalPlayer.RpcRemoveModifier<ChameleonCamouflageModifier>();
    }
}
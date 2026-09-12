using MiraAPI.Events;
using MiraAPI.GameOptions;
using MiraAPI.Hud;
using MiraAPI.Modifiers;
using UnityEngine;
using TownOfUs.Utilities.Appearances;
using TownOfUs.Utilities;
using TownOfUs.Options;
using TownOfUs.Assets;
using TownOfUs.Events.TouEvents;
using TownOfUs.Patches;
using TownOfUs.Modifiers;
using Vigilante.Options;
using Vigilante.Buttons.Crewmate;
using MiraAPI.Translation;

namespace Vigilante.Modifiers.Logic.Crewmate;

public sealed class ChameleonCamouflageModifier : ConcealedModifier, IVisualAppearance
{
    public override string ModifierName => MiraLocaleManager.Get("TownOfUsMira.Role.ChameleonCamouflaged", "Camoflauged");
    public override float Duration => OptionGroupSingleton<ChameleonOptions>.Instance.CamouflageDuration.Value;
    public override bool HideOnUi => true;
    public override bool AutoStart => true;
    public override bool VisibleToOthers => false;
    public bool VisualPriority => true;

    public VisualAppearance GetVisualAppearance()
    {
        var playerColor = (PlayerControl.LocalPlayer.AmOwner || (PlayerControl.LocalPlayer.DiedOtherRound() &&
                                                                      OptionGroupSingleton<GeneralOptions>
                                                                          .Instance.TheDeadKnow))
            ? new Color(0f, 0f, 0f, 0.1f)
            : Color.clear;

        return new VisualAppearance(Player.GetDefaultModifiedAppearance(), TownOfUsAppearances.Swooper)
        {
            HatId = "hat_NoHat",
            SkinId = "skin_None",
            VisorId = "visor_EmptyVisor",
            PlayerName = string.Empty,
            PetId = "pet_EmptyPet",
            RendererColor = playerColor,
            NameColor = Color.clear,
            ColorBlindTextColor = Color.clear
        };
    }

    public override void OnDeath(DeathReason reason)
    {
        Player.RemoveModifier(this);
    }

    public override void OnMeetingStart()
    {
        Player.RemoveModifier(this);
    }

    public override void OnActivate()
    {
        if (Player.AmOwner)
        {
            TouAudio.PlaySound(TouAudio.SwooperActivateSound);

            var button = CustomButtonSingleton<ChameleonCamouflageButton>.Instance;
            button.OverrideName(MiraLocaleManager.Get("TownOfUsMira.Role.ChameleonUncamouflage", "Camoflauge"));
        }

        Player.RawSetAppearance(this);
        Player.cosmetics.ToggleNameVisible(false);

        var touAbilityEvent = new TouAbilityEvent(AbilityType.SwooperSwoop, Player);
        MiraEventManager.InvokeEvent(touAbilityEvent);
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        if (VanillaSystemCheckPatches.ShroomSabotageSystem && VanillaSystemCheckPatches.ShroomSabotageSystem.IsActive)
        {
            Player.RawSetAppearance(this);
            Player.cosmetics.ToggleNameVisible(false);
        }
    }

    public override void OnDeactivate()
    {
        Player.ResetAppearance();
        Player.cosmetics.ToggleNameVisible(true);

        if (Player.AmOwner)
        {
            var button = CustomButtonSingleton<ChameleonCamouflageButton>.Instance;
            button.OverrideName(MiraLocaleManager.Get("TownOfUsMira.Role.ChameleonCamouflage", "Camoflauge"));
            if (!MeetingHud.Instance)
            {
                TouAudio.PlaySound(TouAudio.SwooperDeactivateSound);
            }
            var btn = CustomButtonSingleton<ChameleonVentButton>.Instance;
            btn.OnEffectEnd();
        }

        if (HudManagerPatches.CamouflageCommsEnabled)
        {
            Player.cosmetics.ToggleNameVisible(false);
        }

        if (VanillaSystemCheckPatches.ShroomSabotageSystem && VanillaSystemCheckPatches.ShroomSabotageSystem.IsActive)
        {
            MushroomMixUp(VanillaSystemCheckPatches.ShroomSabotageSystem, Player);
        }

        var touAbilityEvent = new TouAbilityEvent(AbilityType.SwooperUnswoop, Player);
        MiraEventManager.InvokeEvent(touAbilityEvent);
    }

    public static void MushroomMixUp(MushroomMixupSabotageSystem instance, PlayerControl player)
    {
        if (player != null && !player.Data.IsDead && instance.currentMixups.ContainsKey(player.PlayerId))
        {
            var condensedOutfit = instance.currentMixups[player.PlayerId];
            var playerOutfit = instance.ConvertToPlayerOutfit(condensedOutfit);
            playerOutfit.NamePlateId = player.Data.DefaultOutfit.NamePlateId;

            player.MixUpOutfit(playerOutfit);
        }
    }
}
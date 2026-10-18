using MiraAPI.Events;
using MiraAPI.GameOptions;
using MiraAPI.Translation;
using MiraAPI.Utilities.Assets;
using Reactor.Utilities.Extensions;
using TownOfUs.Assets;
using TownOfUs.Events.TouEvents;
using TownOfUs.Modifiers;
using TownOfUs.Modules.Anims;
using TownOfUs.Options;
using TownOfUs.Utilities;
using UnityEngine;
using Vigilante.Assets;

namespace Vigilante.Modifiers.Logic.Crewmate;

public sealed class CrusaderSanctifiedModifier(PlayerControl crusader) : BaseShieldModifier
{
    public override string ModifierName => MiraLocaleManager.Get("Vigilante.Role.Crusader.Sanctifying");
    public override LoadableAsset<Sprite>? ModifierIcon => VigilanteAssets.CrusaderIcon;

    public PlayerControl Crusader { get; } = crusader;
    public GameObject MedicShield { get; set; }
    public bool ShowShield { get; set; }

    public override bool HideOnUi => true;

    public override bool VisibleSymbol => Crusader.AmOwner;

    public override void OnActivate()
    {
        var genOpt = OptionGroupSingleton<GeneralOptions>.Instance;

        var body = UnityEngine.Object.FindObjectsOfType<DeadBody>().FirstOrDefault(x =>
            x.ParentId == PlayerControl.LocalPlayer.PlayerId && !TutorialManager.InstanceExists);
            var fakePlayer = !TutorialManager.InstanceExists ? MiscUtils.GetFakePlayer(PlayerControl.LocalPlayer.PlayerId) : null;

        ShowShield = Crusader.AmOwner ||
                     (PlayerControl.LocalPlayer.HasDied() && genOpt.TheDeadKnow && !body && !fakePlayer?.body);

        MedicShield = AnimStore.SpawnAnimBody(Player, TouAssets.MagicMirror.LoadAsset(), false, -1.1f, -0.1f, 1.5f)!;
    }

    public override void OnDeactivate()
    {
        if (MedicShield)
        {
            MedicShield.Destroy();
        }
    }

    public override void Update()
    {
        if (!Player || Crusader == null)
        {
            ModifierComponent?.RemoveModifier(this);
            return;
        }

        if (!MeetingHud.Instance && MedicShield)
        {
            MedicShield.SetActive(!Player.IsConcealed() && IsVisible && ShowShield);
        }
    }

    public override void OnDeath(DeathReason reason)
    {
        ModifierComponent?.RemoveModifier(this);
    }

    public override void OnMeetingStart()
    {
        ModifierComponent?.RemoveModifier(this);
    }
}
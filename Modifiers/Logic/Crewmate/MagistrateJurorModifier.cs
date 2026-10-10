using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using MiraAPI.Translation;
using MiraAPI.Utilities.Assets;
using TownOfUs;
using TownOfUs.Assets;
using TownOfUs.Options.Roles.Crewmate;
using UnityEngine;
using Vigilante.Assets;
using Vigilante.Options;

namespace Vigilante.Modifiers.Logic.Crewmate;

public sealed class MagistrateJurorModifier : BaseModifier
{
    public override string ModifierName => MiraLocaleManager.Get("Vigilante.Feedback.Magistrate.Juror");
    public override bool HideOnUi => !OptionGroupSingleton<MagistrateOptions>.Instance.NotifyJuror;
    public override LoadableAsset<Sprite>? ModifierIcon => VigilanteAssets.JudgeIcon;
    public override bool Unique => false;

    public override string GetDescription()
    {
        return MiraLocaleManager.Get("Vigilante.Feedback.Magistrate.JurorDescription");
    }

    public override void OnDeath(DeathReason reason)
    {
        ModifierComponent?.RemoveModifier(this);
    }
}

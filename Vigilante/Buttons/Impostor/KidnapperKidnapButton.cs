using HarmonyLib;
using MiraAPI.GameOptions;
using MiraAPI.Keybinds;
using MiraAPI.Modifiers;
using MiraAPI.Translation;
using MiraAPI.Utilities.Assets;
using TownOfUs;
using TownOfUs.Assets;
using TownOfUs.Buttons;
using TownOfUs.Modifiers.Crewmate;
using TownOfUs.Options.Roles.Crewmate;
using TownOfUs.Roles.Crewmate;
using TownOfUs.Utilities;
using UnityEngine;
using Vigilante.Modifiers.Logic.Impostor;
using Vigilante.Roles.Impostor;

namespace Vigilante.Buttons.Impostor;

public sealed class KidnapperKidnapButton : TownOfUsRoleButton<KidnapperRole, PlayerControl>, ILegacyCapable
{
    public override string Name => MiraLocaleManager.Get("TownOfUsMira.Role.Kidnapper.Kidnap", "Kidnap");
    public override BaseKeybind Keybind => Keybinds.SecondaryAction;
    public override Color TextOutlineColor => TownOfUsColors.Impostor;
    public override float Cooldown => Math.Clamp(OptionGroupSingleton<KidnapperOptions>.Instance.KidnapCooldown + MapCooldown, 1f, 120f);
    public override LoadableAsset<Sprite> Sprite => LegacyAssets.IsLegacy ? LegacyCrewAssets.JailSprite : TouCrewAssets.JailSprite;

    public override bool Enabled(RoleBehaviour? role)
    {
        return base.Enabled(role) && (!Role.Exterminated || OptionGroupSingleton<KidnapperOptions>.Instance.KidnapPostExtermination);
    }

    public override PlayerControl? GetTarget()
    {
        return PlayerControl.LocalPlayer.GetClosestLivingPlayer(true, Distance,
            predicate: player => !player.HasModifier<KidnapperKidnappedModifier>());
    }

    protected override void OnClick()
    {
        if (Target == null)
        {
            return;
        }

        ModifierUtils.GetPlayersWithModifier<KidnapperKidnappedModifier>().Do(x => x.RpcRemoveModifier<KidnapperKidnappedModifier>());
        Target?.RpcAddModifier<KidnapperKidnappedModifier>(PlayerControl.LocalPlayer.PlayerId);
        TouAudio.PlaySound(TouAudio.JailSound);
    }
}
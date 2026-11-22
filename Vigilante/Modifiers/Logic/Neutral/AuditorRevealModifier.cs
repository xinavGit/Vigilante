using MiraAPI.Translation;
using MiraAPI.Utilities;
using Rewired;
using TownOfUs;
using TownOfUs.Assets;
using TownOfUs.Modifiers;
using UnityEngine;

namespace Vigilante.Modifiers.Logic.Neutral;

public sealed class AuditorRevealModifier()
    : BaseRevealModifier
{

    public override ChangeRoleResult ChangeRoleResult { get; set; } = ChangeRoleResult.UpdateInfo;
    public override RoleBehaviour? ShownRole { get; set; }

    public override bool RevealRole { get; set; }
    public override bool Visible { get; set; }

    public int TimesSmokebombed { get; set; }

    public override void OnActivate()
    {
        base.OnActivate();
        TimesSmokebombed = 1;
        ShownRole = Player.Data.Role;
    }

    public void UpdateSmokebombInfo()
    {
        TimesSmokebombed++;

        if (TimesSmokebombed == 3)
        {
            var notif1 = Helpers.CreateAndShowNotification(
                    $"<b>{TownOfUsColors.Chef.ToTextColor()}{MiraLocaleManager.Get("Vigilante.Modifier.AuditorFlashedNotification")}</color></b>",
                    Color.red,
                    spr: TouRoleIcons.Fairy);
            
            SetNewInfo(true);
        }
    }

}
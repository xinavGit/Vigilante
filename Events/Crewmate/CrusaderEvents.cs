using System.Linq;
using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Gameplay;
using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using Vigilante.Modifiers.Logic.Crewmate;
using Vigilante.Options;
using Vigilante.Roles.Crewmate;
using TownOfUs.Extensions;
using TownOfUs.Utilities;
using MiraAPI.Utilities;
using MiraAPI.Events.Vanilla.Player;
using Il2CppSystem.Runtime.InteropServices;
using MiraAPI.Roles;
using MiraAPI.Hud;
using Vigilante.Buttons.Crewmate;
using MiraAPI.Networking;
using TownOfUs.Events.TouEvents;
using Rewired;
using MiraAPI.Events.Mira;
using TownOfUs.Buttons;

namespace Vigilante.Events.Crewmate;

public static class CrusaderEvents
{
    [RegisterEvent]
    public static void ChangeRoleEventHandler(ChangeRoleEvent @event)
    {
        if (@event.OldRole is CrusaderRole)
        {
            var ownedShield = ModifierUtils.GetActiveModifiers<CrusaderSanctifiedModifier>().FirstOrDefault(x => x.Crusader == @event.Player);
            if (ownedShield != null)
            {
                ownedShield.Player.RemoveModifier(ownedShield);
            }
        }
    }

    [RegisterEvent(-1000)]
    public static void BeforeMurderEventHandler(BeforeMurderEvent @event)
    {
        var source = @event.Source;
        var target = @event.Target;

        if (target.HasModifier<CrusaderSanctifiedModifier>())
        {
            if (OptionGroupSingleton<CrusaderOptions>.Instance.PreventsKills.Value) @event.Cancel();
            
            var crusader = PlayerControl.AllPlayerControls.ToArray().FirstOrDefault(x => x.IsRole<CrusaderRole>());
            if (crusader == null) return;
            crusader.RpcCustomMurder(source);
        }
    }

    [RegisterEvent(-1000)]
    public static void MiraButtonClickEventHandler(MiraButtonClickEvent @event)
    {
        var source = PlayerControl.LocalPlayer;
        var button = @event.Button as CustomActionButton<PlayerControl>;
        var target = button?.Target;
        if (target == null || button is not IKillButton || !button.CanClick())
        {
            return;
        }

        if (target.HasModifier<CrusaderSanctifiedModifier>())
        {
            @event.Cancel();
            var crusader = PlayerControl.AllPlayerControls.ToArray().FirstOrDefault(x => x.IsRole<CrusaderRole>());
            if (crusader == null) return;
            crusader.RpcCustomMurder(source);
        }
    }
}
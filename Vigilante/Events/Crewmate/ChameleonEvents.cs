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
using MiraAPI.Hud;
using Vigilante.Buttons.Crewmate;

namespace Vigilante.Events.Crewmate;

public static class ChameleonEvents
{
    [RegisterEvent]
    public static void CompleteTaskEvent(CompleteTaskEvent @event)
    {
        if (@event.Player.AmOwner && @event.Player.Data.Role is ChameleonRole &&
            OptionGroupSingleton<ChameleonOptions>.Instance.TaskUses)
        {
            var button = CustomButtonSingleton<ChameleonCamouflageButton>.Instance;
            ++button.UsesLeft;
            button.SetUses(button.UsesLeft);
        }
    }
    
}
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
using Vigilante.Roles.Impostor;
using Rewired;

namespace Vigilante.Events.Impostor;

public static class DisruptorEvents
{
    [RegisterEvent]
    public static void AfterMurderEvent(AfterMurderEvent evt)
    {
        if (evt.Source.Data?.Role is not DisruptorRole)
        {
            return;
        }

        if (MeetingHud.Instance) return;

        if (PlayerControl.LocalPlayer.IsHost())
        {
            DisruptorRole.SabRandom();
        }
    }
}
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

namespace Vigilante.Events.Crewmate;

public static class MagistrateEvents
{
    [RegisterEvent]
    public static void OnRoundStart(RoundStartEvent evt)
    {
        if (evt.TriggeredByIntro)
        {
            return;
        }

        foreach (var juror in ModifierUtils.GetPlayersWithModifier<MagistrateJurorModifier>().ToList())
        {
            juror.RemoveModifier<MagistrateJurorModifier>();
        }
    }
}
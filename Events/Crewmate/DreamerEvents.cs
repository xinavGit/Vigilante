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

namespace Vigilante.Events.Crewmate;

public static class DreamerEvents
{
    [RegisterEvent]
    public static void OnRoundStart(RoundStartEvent evt)
    {
        if (evt.TriggeredByIntro || !AmongUsClient.Instance.AmHost)
        {
            return;
        }

        var options = OptionGroupSingleton<DreamerOptions>.Instance;

        foreach (var insomniac in ModifierUtils.GetPlayersWithModifier<DreamerInsomniaModifier>().ToList())
        {
            var insomniaMod = insomniac.GetModifier<DreamerInsomniaModifier>();
            if (insomniaMod == null)
            {
                continue;
            }

            insomniaMod.RoundsLeft--;
            if (insomniaMod.RoundsLeft <= 0)
            {
                insomniac.RpcRemoveModifier<DreamerInsomniaModifier>();
            }
        }

        foreach (var dreaming in ModifierUtils.GetPlayersWithModifier<DreamerTargetDreamingModifier>().ToList())
        {
            var dreamMod = dreaming.GetModifier<DreamerTargetDreamingModifier>();

            if (dreamMod != null && (ushort)dreaming.Data.Role.Role == dreamMod.DreamRoleId)
            {
                dreaming.RpcChangeRole(dreamMod.OriginalRoleId, false);
            }

            dreaming.RpcRemoveModifier<DreamerTargetDreamingModifier>();
        }

        foreach (var player in PlayerControl.AllPlayerControls)
        {
            if (player == null || player.Data == null || player.Data.Role is not DreamerRole dreamer)
            {
                continue;
            }

            if (dreamer.Player == null || dreamer.Player.HasDied() || dreamer.DreamTargetId == byte.MaxValue)
            {
                continue;
            }

            var target = GameData.Instance.GetPlayerById(dreamer.DreamTargetId)?.Object;
            if (target == null)
            {
                continue;
            }

            var chosenRoleId = dreamer.DreamRoleId;

            DreamerRole.RpcReimagine(dreamer.Player, target, chosenRoleId);
        }
    }
}
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
using MiraAPI.Events.Vanilla.Meeting;

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

        var button = CustomButtonSingleton<MagistrateEnlistButton>.Instance;
        ++button.UsesLeft;
        button.SetUses(button.UsesLeft);
    }

    [RegisterEvent]
    public static void AfterMurderEvent(AfterMurderEvent evt)
    {
        var victim = evt.Target;

        if (victim.HasModifier<MagistrateJurorModifier>())
        {
            foreach (var magi in CustomRoleUtils.GetActiveRolesOfType<MagistrateRole>())
            {
                magi.ClearMenu();
            }
        }
    }

    [RegisterEvent(400)]
    public static void WrapUpEvent(EjectionEvent @event)
    {
        var player = @event.ExileController.initData.networkedPlayer?.Object;

        foreach (var pros in CustomRoleUtils.GetActiveRolesOfType<MagistrateRole>())
        {
            var hasProsecuted = pros.HasProsecuted;
            pros.Cleanup();

            if (player == null)
            {
                continue;
            }

            if (hasProsecuted)
            {
                ProsecutorRole.HasProsecutedBefore = true;
                GameHistory.UpdatePlayerDeathData(player.PlayerId, MiraLocaleManager.Get("DiedToProsecutor"), 0,
                    HudManagerHelper.Instance.CurrentRound, DeathHandlerOverride.SetFalse,
                    MiraLocaleManager.Get("DiedByStringBasic").Replace("<player>", pros.Player.Data.PlayerName),
                    lockInfo: DeathHandlerOverride.SetTrue, playerState: StoredPlayerState.Dead);

                if (IsBadJudgement(pros.Player, player) && player != pros.Player)
                {
                    ProsToAnnounceNext = pros.Player;
                    if ((BadProsecuteResult)OptionGroupSingleton<ProsecutorOptions>.Instance.WrongfulProsecutionResult.Value is BadProsecuteResult.EjectProsAndEjectTarget)
                    {
                        if (pros.Player.TryGetModifier<CelebrityModifier>(out var celeb))
                        {
                            celeb.Announced = true;
                        }
                        GameHistory.UpdatePlayerDeathData(pros.Player.PlayerId, MiraLocaleManager.Get("DiedToPunishment"), 0,
                            HudManagerHelper.Instance.CurrentRound, DeathHandlerOverride.SetFalse,
                            lockInfo: DeathHandlerOverride.SetTrue, playerState: StoredPlayerState.Dead);

                        pros.Player.Exiled();
                    }
                    else
                    {
                        pros.ProsecutionsCompleted =
                            (int)OptionGroupSingleton<ProsecutorOptions>.Instance.MaxProsecutions;
                    }
                }
            }
        }
    }
}
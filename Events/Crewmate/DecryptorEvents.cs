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

public static class DecryptorEvents
{
    [RegisterEvent]
    public static void OnTaskComplete(CompleteTaskEvent evt)
    {
        if (evt.Player == null || !evt.Player.AmOwner)
        {
            return;
        }

        if (evt.Player.Data?.Role is not DecryptorRole decryptor|| evt.Player.HasDied())
        {
            return;
        }

        DecryptorRole.TasksTowardCompletion++;

        if (DecryptorRole.TasksTowardCompletion >= (int)OptionGroupSingleton<DecryptorOptions>.Instance.TasksPerLetter.Value)
        {
            DecryptorRole.RevealLetter();

            DecryptorRole.TasksTowardCompletion = 0;
        }
    }

    [RegisterEvent]
    public static void OnRoleChange(SetRoleEvent evt)
    {
        DecryptorRole.KnownCharacters[evt.Player.PlayerId].Clear();
    }
}
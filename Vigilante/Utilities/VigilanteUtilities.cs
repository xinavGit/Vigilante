using AmongUs.GameOptions;
using TownOfUs.Roles;
using TownOfUs.Utilities;
using UnityEngine;

namespace Vigilante.Utilities;

public static class VigilanteUtils
{
    public static bool IsAnySabotageActive()
    {
        var systems = ShipStatus.Instance.Systems;

        foreach (var systemType in new[]
        {
            SystemTypes.Comms,
            SystemTypes.Reactor,
            SystemTypes.LifeSupp,
            SystemTypes.Electrical,
            SystemTypes.Laboratory,
            SystemTypes.HeliSabotage,
            SystemTypes.MushroomMixupSabotage
        })
        {
            if (!systems.TryGetValue(systemType, out var system))
                continue;

            if (system.Cast<IActivatable>().IsActive)
                return true;
        }

        return false;
    }
}
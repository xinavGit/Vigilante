using AmongUs.GameOptions;
using TownOfUs.Roles;
using TownOfUs.Utilities;
using UnityEngine;

namespace Vigilante.Utilities;

public static class VigilanteUtils
{

    public static bool AreFriendly(PlayerControl p1, PlayerControl p2)
    {
        if (p1 == null || p2 == null) return true;

        if (p1.Data?.Role == null || p2.Data?.Role == null) return true;


        if (p1.IsCrewmate() && p2.IsCrewmate()) return true;

        if (p1.IsImpostor() && p2.IsImpostor()) return true;

        if (p1.Data.Role.Role == p2.Data.Role.Role) return true;

        if (p1.Is(RoleAlignment.NeutralBenign) && p2.Is(RoleAlignment.NeutralBenign)) return true;

        if (p1.Is(RoleAlignment.NeutralEvil) && p2.Is(RoleAlignment.NeutralEvil)) return true;

        if (p1.Is(RoleAlignment.NeutralOutlier) && p2.Is(RoleAlignment.NeutralOutlier)) return true;

        if (p1.IsLover() && p2.IsLover()) return true;


        return false;
    }

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
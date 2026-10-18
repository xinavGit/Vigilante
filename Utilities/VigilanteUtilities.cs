using AmongUs.GameOptions;
using TownOfUs.Roles;
using TownOfUs.Utilities;
using UnityEngine;

namespace Vigilante.Utilities;

public static class VigilanteUtils
{
    //using seer compare logic TOUM plus more
    public static bool AreFriendly(PlayerControl p1, PlayerControl p2)
    {
        if (p1 == null || p2 == null) return true;
        Info("1");
        if (p1.Data?.Role == null || p2.Data?.Role == null) return true;
        Info("2");

        if (p1.IsCrewmate() && p2.IsCrewmate()) return true;
        Info("3");
        if (p1.IsImpostor() && p2.IsImpostor()) return true;
        Info("4");
        if (p1.Data.Role.Role == p2.Data.Role.Role) return true; // Two Cursed Soul are friendly to one another
        Info("5");
        if (p1.Is(RoleAlignment.NeutralBenign) && p2.Is(RoleAlignment.NeutralBenign)) return true;
        Info("6");
        if (p1.Is(RoleAlignment.NeutralEvil) && p2.Is(RoleAlignment.NeutralEvil)) return true;
        Info("7");
        if (p1.Is(RoleAlignment.NeutralOutlier) && p2.Is(RoleAlignment.NeutralOutlier)) return true;
        Info("8");
        if (p1.IsLover() && p2.IsLover()) return true;
        Info("9");
        // add specifics? chef etc

        // You sense that Atony and werewolves appear to be enemies!
        return false;
    }
}
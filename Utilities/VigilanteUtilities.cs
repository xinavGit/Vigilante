using AmongUs.GameOptions;
using TownOfUs.Roles;
using UnityEngine;

namespace Vigilante.Utilities;

public static class VigilanteUtils
{
    public static string GetPlayerName(byte playerId, string fallback = "them")
    {
        return GameData.Instance.GetPlayerById(playerId)?.Object?.Data?.PlayerName ?? fallback;
    }

    public static (string name, string colorHex) GetRoleDisplayInfo(ushort roleId, string fallbackName = "a new role")
    {
        var roleObj = RoleManager.Instance.GetRole((RoleTypes)roleId) as ITownOfUsRole;
        return (roleObj?.RoleName ?? fallbackName, roleObj != null ? ColorUtility.ToHtmlStringRGB(roleObj.RoleColor) : "9999FF");
    }
}
using MiraAPI.Roles;
using TownOfUs.Roles;

namespace Vigilante.Interfaces;

public interface IVigilanteRole : ITownOfUsRole
{
    string ICustomRole.IdPrefix => "Vigilante.Role";

    string ICustomRole.IdPart
    {
        get
        {
            var typeName = GetType().Name;

            return typeName.EndsWith("Role", StringComparison.Ordinal)
                ? typeName[..^4]
                : typeName;
        }
    }
}
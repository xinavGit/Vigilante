using Vigilante;
using MiraAPI.Translation;

namespace Vigilante.Modules.Localization;

public static class VigilanteLocale
{
    public static void Register()
    {
        MiraLocaleManager.Register(VigilantePlugin.Id);
    }
}
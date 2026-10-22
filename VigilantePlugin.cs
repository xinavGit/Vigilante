using System.Globalization;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using MiraAPI;
using MiraAPI.PluginLoading;
using MiraAPI.Translation;
using Reactor;
using Reactor.Networking;
using Reactor.Networking.Attributes;
using Reactor.Utilities;
using TownOfUs;
using PerfectComms;
using AchievementsAPI;
using Vigilante.Modules.Localization;
using TownOfUs.Achievements;

namespace Vigilante;

[BepInAutoPlugin("com.xinav.vigilante", "Vigilante")]
[BepInProcess("Among Us.exe")]
[BepInDependency(ReactorPlugin.Id)]
[BepInDependency(MiraApiPlugin.Id)]
[BepInDependency(TownOfUsPlugin.Id)]
[BepInDependency(AchievementsAPIHelper.PluginGuid, BepInDependency.DependencyFlags.SoftDependency)]
[BepInDependency("com.edgetel.perfectcomms", BepInDependency.DependencyFlags.SoftDependency)]
[ReactorModFlags(ModFlags.RequireOnAllClients)]
public partial class VigilantePlugin : BasePlugin, IMiraPlugin
{
    public static CultureInfo Culture => TownOfUs.TownOfUsPlugin.Culture;
    public string OptionsTitleText => "Vigi";
    public static bool IsDevBuild => true;
    public ConfigFile GetConfigFile() => Config;

    public Harmony Harmony { get; } = new(Id);

    public override void Load()
    {
        ReactorCredits.Register("Vigilante", Version, IsDevBuild, ReactorCredits.AlwaysShow);
        VigilanteLocale.Register();

        Harmony.PatchAll();
        Log.LogInfo("Vigilante loaded!");
    }
}

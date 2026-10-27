using Il2CppInterop.Runtime.Attributes;
using MiraAPI.GameOptions;
using MiraAPI.Hud;
using MiraAPI.Roles;
using MiraAPI.Translation;
using MiraAPI.Utilities.Assets;
using TownOfUs;
using TownOfUs.Assets;
using TownOfUs.Extensions;
using TownOfUs.Modules;
using TownOfUs.Modules.Wiki;
using TownOfUs.Roles;
using TownOfUs.Utilities;
using UnityEngine;
using Vigilante.Assets;
using Vigilante.Interfaces;
using Vigilante.Options;
using Vigilante.Utilities;

namespace Vigilante.Roles.Impostor;

public sealed class DisruptorRole(IntPtr cppPtr) : ImpostorRole(cppPtr), IVigilanteRole, IWikiDiscoverable, IDoomable
{
    public DoomableType DoomHintType => DoomableType.Trickster;
    public string IdPart => "Disruptor";
    public string RoleMedDescriptionLocale => $"Vigilante.Role.{IdPart}.TabDescription";

    public string GetAdvancedDescription()
    {
        return
            MiraLocaleManager.Get($"Vigilante.Role.{IdPart}.WikiDescription") +
            MiscUtils.AppendOptionsText(GetType());
    }

    public Color RoleColor => TownOfUsColors.Impostor;
    public ModdedRoleTeams Team => ModdedRoleTeams.Impostor;
    public RoleAlignment RoleAlignment => RoleAlignment.ImpostorPower;

    public CustomRoleConfiguration Configuration => new(this)
    {
        IconTmp = TmpSpriteUtils.CreateSpriteAsset(VigilanteAssets.DisruptorIcon.LoadAsset(), "Vigilante.Role.Impostor.Power", 1.45f),
        OptionsScreenshot = TouBanners.ImpostorRoleBanner,
        Icon = VigilanteAssets.DisruptorIcon
    };

    // reworked engineer fix code only broken on freeplay some maps
    [HideFromIl2Cpp]
    public static List<SystemTypes> SabotagableSabs()
    {

        List<SystemTypes> availableSabs = new List<SystemTypes>();
        switch ((ExpandedMapNames)GameOptionsManager.Instance.currentGameOptions.MapId)
        {
            case ExpandedMapNames.Skeld or ExpandedMapNames.Dleks:
                var comms1 = ShipStatus.Instance.Systems[SystemTypes.Comms].Cast<HudOverrideSystemType>();
                if (!comms1.IsActive)
                {
                    availableSabs.Add(SystemTypes.Comms);
                }

                var reactor1 = ShipStatus.Instance.Systems[SystemTypes.Reactor].Cast<ReactorSystemType>();
                if (!reactor1.IsActive)
                {
                    availableSabs.Add(SystemTypes.Reactor);
                }

                var oxygen1 = ShipStatus.Instance.Systems[SystemTypes.LifeSupp].Cast<LifeSuppSystemType>();
                if (!oxygen1.IsActive)
                {
                    availableSabs.Add(SystemTypes.LifeSupp);
                }

                var lights1 = ShipStatus.Instance.Systems[SystemTypes.Electrical].Cast<SwitchSystem>();
                if (!lights1.IsActive)
                {
                    availableSabs.Add(SystemTypes.Electrical);
                }

                break;
            case ExpandedMapNames.MiraHq:
                var comms2 = ShipStatus.Instance.Systems[SystemTypes.Comms].Cast<HqHudSystemType>();
                if (!comms2.IsActive)
                {
                    availableSabs.Add(SystemTypes.Comms);
                }

                var reactor2 = ShipStatus.Instance.Systems[SystemTypes.Reactor].Cast<ReactorSystemType>();
                if (!reactor2.IsActive)
                {
                    availableSabs.Add(SystemTypes.Reactor);
                }

                var oxygen2 = ShipStatus.Instance.Systems[SystemTypes.LifeSupp].Cast<LifeSuppSystemType>();
                if (!oxygen2.IsActive)
                {
                    availableSabs.Add(SystemTypes.LifeSupp);
                }

                var lights2 = ShipStatus.Instance.Systems[SystemTypes.Electrical].Cast<SwitchSystem>();
                if (!lights2.IsActive)
                {
                    availableSabs.Add(SystemTypes.Electrical);
                }

                break;
            case ExpandedMapNames.Polus:
                var comms3 = ShipStatus.Instance.Systems[SystemTypes.Comms].Cast<HudOverrideSystemType>();
                if (!comms3.IsActive)
                {
                    availableSabs.Add(SystemTypes.Comms);
                }

                var seismic = ShipStatus.Instance.Systems[SystemTypes.Laboratory].Cast<ReactorSystemType>();
                if (!seismic.IsActive)
                {
                    availableSabs.Add(SystemTypes.Laboratory);
                }

                var lights3 = ShipStatus.Instance.Systems[SystemTypes.Electrical].Cast<SwitchSystem>();
                if (!lights3.IsActive)
                {
                    availableSabs.Add(SystemTypes.Electrical);
                }

                break;
            case ExpandedMapNames.Airship:
                var comms4 = ShipStatus.Instance.Systems[SystemTypes.Comms].Cast<HudOverrideSystemType>();
                if (!comms4.IsActive)
                {
                    availableSabs.Add(SystemTypes.Comms);
                }

                var reactor = ShipStatus.Instance.Systems[SystemTypes.HeliSabotage].Cast<HeliSabotageSystem>();
                if (!reactor.IsActive)
                {
                    availableSabs.Add(SystemTypes.HeliSabotage);
                }

                var lights4 = ShipStatus.Instance.Systems[SystemTypes.Electrical].Cast<SwitchSystem>();
                if (!lights4.IsActive)
                {
                    availableSabs.Add(SystemTypes.Electrical);
                }

                break;
            case ExpandedMapNames.Fungle:
                var reactor7 = ShipStatus.Instance.Systems[SystemTypes.Reactor].Cast<ReactorSystemType>();
                if (!reactor7.IsActive)
                {
                    availableSabs.Add(SystemTypes.Reactor);
                }

                var comms7 = ShipStatus.Instance.Systems[SystemTypes.Comms].Cast<HqHudSystemType>();
                if (!comms7.IsActive)
                {
                    availableSabs.Add(SystemTypes.Comms);
                }

                var mushroom = ShipStatus.Instance.Systems[SystemTypes.MushroomMixupSabotage]
                    .Cast<MushroomMixupSabotageSystem>();
                if (!mushroom.IsActive)
                {
                    availableSabs.Add(SystemTypes.MushroomMixupSabotage);
                }

                break;
            case ExpandedMapNames.Submerged:
                var reactor5 = ShipStatus.Instance.Systems[SystemTypes.Reactor].Cast<ReactorSystemType>();
                if (!reactor5.IsActive)
                {
                    availableSabs.Add(SystemTypes.Reactor);
                }

                var lights5 = ShipStatus.Instance.Systems[SystemTypes.Electrical].Cast<SwitchSystem>();
                if (!lights5.IsActive)
                {
                    availableSabs.Add(SystemTypes.Electrical);
                }

                var comms5 = ShipStatus.Instance.Systems[SystemTypes.Comms].Cast<HudOverrideSystemType>();
                if (!comms5.IsActive)
                {
                    availableSabs.Add(SystemTypes.Comms);
                }


                //availableSabs.Add(SystemTypes.Comms);
                foreach (var i in PlayerControl.LocalPlayer.myTasks)
                {
                    if (i.TaskType == ModCompatibility.RetrieveOxygenMask)
                    {
                        //how do i add this :/
                        //availableSabs.Remove(SystemTypes.Comms);
                    }
                }

                break;
            case ExpandedMapNames.LevelImpostor:
                var comms6 = ShipStatus.Instance.Systems[SystemTypes.Comms].Cast<HudOverrideSystemType>();
                if (!comms6.IsActive)
                {
                    availableSabs.Add(SystemTypes.Comms);
                }

                var reactor6 = ShipStatus.Instance.Systems[SystemTypes.Reactor].Cast<ReactorSystemType>();
                if (!reactor6.IsActive)
                {
                    availableSabs.Add(SystemTypes.Reactor);
                }

                var oxygen6 = ShipStatus.Instance.Systems[SystemTypes.LifeSupp].Cast<LifeSuppSystemType>();
                if (!oxygen6.IsActive)
                {
                    availableSabs.Add(SystemTypes.LifeSupp);
                }

                var lights6 = ShipStatus.Instance.Systems[SystemTypes.Electrical].Cast<SwitchSystem>();
                if (!lights6.IsActive)
                {
                    availableSabs.Add(SystemTypes.Electrical);
                }

                if (ShipStatus.Instance.Systems.TryGetValue(SystemTypes.Laboratory, out var seismic1) &&
                    !seismic1.Cast<IActivatable>().IsActive)
                {
                    availableSabs.Add(SystemTypes.Laboratory);
                }

                break;
    }

    return availableSabs;
}

    [HideFromIl2Cpp]
    public static void SabRandom()
    {
        var availableSabs = SabotagableSabs();
        if (availableSabs.Count <= 0)
        {
            return;
        }
        foreach (var sab in availableSabs)
        {
            Info(sab);
        }
        var chosenSab = availableSabs[UnityEngine.Random.Range(0, availableSabs.Count)];
        switch (chosenSab)
        {
            case SystemTypes.Comms:
                ShipStatus.Instance.RpcUpdateSystem(SystemTypes.Comms, 128);
                break;
            case SystemTypes.Reactor:
                ShipStatus.Instance.RpcUpdateSystem(SystemTypes.Reactor, 128);
                break;
            case SystemTypes.LifeSupp:
                ShipStatus.Instance.RpcUpdateSystem(SystemTypes.LifeSupp, 128);
                break;
            case SystemTypes.Laboratory:
                ShipStatus.Instance.RpcUpdateSystem(SystemTypes.Laboratory, 128);
                break;
            case SystemTypes.HeliSabotage:
                ShipStatus.Instance.RpcUpdateSystem(SystemTypes.HeliSabotage, 128);
                break;
            case SystemTypes.MushroomMixupSabotage:
                ShipStatus.Instance.RpcUpdateSystem(SystemTypes.MushroomMixupSabotage, 1);
                break;
            case SystemTypes.Electrical:
                byte num = 4;

                for (var index = 0; index < 5; ++index)
                {
                    if (BoolRange.Next())
                        num |= (byte)(1 << index);
                }

                ShipStatus.Instance.RpcUpdateSystem(SystemTypes.Electrical, (byte)(num | 128U));
                break;
        }
    }

    public void FixedUpdate()
    {
        if (!Player || Player.Data.Role is not DisruptorRole || Player.HasDied() || !Player.AmOwner ||
            MeetingHud.Instance || !VigilanteUtils.IsAnySabotageActive())
        {
            return;
        }
        
        var value = OptionGroupSingleton<DisruptorOptions>.Instance.DisruptorSaboCooldownMultiplier - 1;
        
        foreach (var ability in CustomButtonManager.Buttons)
        {
            if (ability.EffectActive) continue;
            if (ability.TimerPaused) continue;

            ability.DecreaseTimer(Time.deltaTime * value);
        }

        Player.killTimer -= Time.deltaTime * value;
    }
}
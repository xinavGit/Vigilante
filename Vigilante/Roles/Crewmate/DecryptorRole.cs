using AmongUs.GameOptions;
using System.Linq;
using Vigilante.Assets;
using Vigilante.Modifiers.Logic.Crewmate;
using Vigilante.Options;
using Il2CppInterop.Runtime.Attributes;
using Il2CppSystem.Runtime.InteropServices;
using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using MiraAPI.Patches.Stubs;
using MiraAPI.Roles;
using MiraAPI.Utilities;
using Reactor.Networking.Attributes;
using TownOfUs;
using TownOfUs.Extensions;
using TownOfUs.Modules;
using TownOfUs.Modules.Components;
using TownOfUs.Modules.Wiki;
using TownOfUs.Roles;
using TownOfUs.Roles.Crewmate;
using TownOfUs.Utilities;
using UnityEngine;
using TownOfUs.Modifiers;
using System.Text;
using MiraAPI.Translation;
using MiraAPI.Utilities.Assets;
using TownOfUs.Modifiers.Crewmate;
using TownOfUs.Assets;
using MiraAPI.Hud;
using TownOfUs.Buttons;
using Reactor.Utilities;
using System.Globalization;
using Vigilante.Interfaces;
using TownOfUs.Modifiers.Game.Alliance;

namespace Vigilante.Roles.Crewmate;

public sealed class DecryptorRole(IntPtr cppPtr) : CrewmateRole(cppPtr), IVigilanteRole, IWikiDiscoverable, IDoomable
{
    public string IdPart => "Decryptor";
    public Color RoleColor => VigilanteColors.Decryptor;
    public ModdedRoleTeams Team => ModdedRoleTeams.Crewmate;
    public RoleAlignment RoleAlignment => RoleAlignment.CrewmateInvestigative;

    public string RoleName => MiraLocaleManager.Get($"Vigilante.Role.{IdPart}");
    public string RoleDescription => MiraLocaleManager.Get($"Vigilante.Role.{IdPart}.IntroBlurb");
    public string RoleLongDescription => MiraLocaleManager.Get($"Vigilante.Role.{IdPart}.TabDescription");

    public DoomableType DoomHintType => DoomableType.Insight;

    public string GetAdvancedDescription()
    {
        return
            MiraLocaleManager.Get($"Vigilante.Role.{IdPart}.WikiDescription") +
            MiscUtils.AppendOptionsText(GetType());
    }

    public CustomRoleConfiguration Configuration => new(this)
    {
        IconTmp = TmpSpriteUtils.CreateSpriteAsset(VigilanteAssets.DecryptorIcon.LoadAsset(), "Vigilante.Role.Crewmate.Decryptor", 1.45f),
        Icon = VigilanteAssets.DecryptorIcon,
        IntroSound = TouAudio.GlitchSound,
        MaxRoleCount = 1
    };


    public static Dictionary<byte, List<char>> KnownCharacters {get; private set;} = [];
    private static int LettersRevealed;

    public static int TasksTowardCompletion {get; set;}

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        var stringB = ITownOfUsRole.SetNewTabText(this);

        //reveals left till evils alerted (if option on)

        //tasks left to next reveal

        return stringB;
    }

    public override void Initialize(PlayerControl player)
    {
        RoleBehaviourStubs.Initialize(this, player);

        KnownCharacters.Clear();
        LettersRevealed = 0;
        TasksTowardCompletion = 0;
    }

    //rpc for reveal letter? maybe... RpcDecryptorRevealLetter
    //rpc for alerting evils RpcDecryptorAlert

    public static Dictionary<byte, string> SpawnRoleNames { get; } = [];

    public static void SnapshotSpawnRoles()
    {
        SpawnRoleNames.Clear();
        KnownCharacters.Clear();
        LettersRevealed = 0;
        TasksTowardCompletion = 0;

        foreach (var player in PlayerControl.AllPlayerControls)
        {
            if (player == null || player.Data?.Role == null) continue;
            SpawnRoleNames[player.PlayerId] = player.Data.Role.GetRoleName().ToUpperInvariant();
        }
    }

    [MethodRpc((uint)VigilanteRpcs.DecryptorAlert)]
    public static void RpcDecryptorAlert(PlayerControl decryptor)
    {
        if (decryptor.AmOwner && (OptionGroupSingleton<DecryptorOptions>.Instance.AlertImpostors || OptionGroupSingleton<DecryptorOptions>.Instance.AlertNeutrals))
        {
            var notif = Helpers.CreateAndShowNotification(
                $"<b>{MiraLocaleManager.Get("Vigilante.Feedback.Decryptor.ExposedAlert")}</b>",
                Color.white, new Vector3(0f, 1f, -20f), spr: VigilanteAssets.DecryptorIcon.LoadAsset());

                notif.AdjustNotification();
                
            Coroutines.Start(MiscUtils.CoFlash(VigilanteColors.Decryptor));
        }

        if (PlayerControl.LocalPlayer.IsImpostorAligned() && OptionGroupSingleton<DecryptorOptions>.Instance.AlertImpostors ||
            PlayerControl.LocalPlayer.IsNeutral() && OptionGroupSingleton<DecryptorOptions>.Instance.AlertNeutrals)
        {
            if (!decryptor.HasModifier<EgotistModifier>())
            {
                var notif = Helpers.CreateAndShowNotification(
                    $"<b>{MiraLocaleManager.Get("Vigilante.Feedback.Decryptor.EvilsAlert")}</b>",
                    Color.white, new Vector3(0f, 1f, -20f), spr: VigilanteAssets.DecryptorIcon.LoadAsset());

                    notif.AdjustNotification();
                
                Coroutines.Start(MiscUtils.CoFlash(VigilanteColors.Decryptor));
            }
            else
            {
                var notif = Helpers.CreateAndShowNotification(
                    $"<b>{MiraLocaleManager.Get("Vigilante.Feedback.Decryptor.EvilsAlertEgo")}</b>",
                    Color.white, new Vector3(0f, 1f, -20f), spr: VigilanteAssets.DecryptorIcon.LoadAsset());

                    notif.AdjustNotification();
                
                Coroutines.Start(MiscUtils.CoFlash(TownOfUsColors.Egotist));
            }
        }

        if (PlayerControl.LocalPlayer.IsCrewmate() && decryptor.HasModifier<EgotistModifier>())
        {
            var notif = Helpers.CreateAndShowNotification(
                    $"<b>{MiraLocaleManager.Get("Vigilante.Feedback.Decryptor.CrewAlertEgo")}</b>",
                    Color.white, new Vector3(0f, 1f, -20f), spr: VigilanteAssets.DecryptorIcon.LoadAsset());

                    notif.AdjustNotification();
                
            Coroutines.Start(MiscUtils.CoFlash(TownOfUsColors.Egotist));
        }
    }

    public static void RevealLetter()
    {
        foreach (var player in PlayerControl.AllPlayerControls)
        {
            if (player == null || player.Data.IsDead) continue;

            if (!SpawnRoleNames.TryGetValue(player.PlayerId, out var roleName)) continue;

            if (!KnownCharacters.TryGetValue(player.PlayerId, out var known))
            {
                known = [];
                KnownCharacters[player.PlayerId] = known;
            }

            var pool = GetRemainingLetterPool(roleName, known);
            if (pool.Count == 0) continue;

            known.Add(pool[UnityEngine.Random.Range(0, pool.Count)]);
        }

        LettersRevealed++;

        if (LettersRevealed == (int)OptionGroupSingleton<DecryptorOptions>.Instance.LettersForAlert.Value && (OptionGroupSingleton<DecryptorOptions>.Instance.AlertImpostors || OptionGroupSingleton<DecryptorOptions>.Instance.AlertNeutrals))
        {
            RpcDecryptorAlert(PlayerControl.LocalPlayer);
        }
    }

    private static List<char> GetRemainingLetterPool(string roleName, List<char> alreadyRevealed)
    {
        var remaining = roleName.Where(char.IsLetter).ToList();

        foreach (var known in alreadyRevealed)
            remaining.Remove(known); // removes just one copy, not all matching letters

        return remaining;
    }
}
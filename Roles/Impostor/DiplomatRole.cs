using AmongUs.GameOptions;
using Il2CppInterop.Runtime.Attributes;
using MiraAPI.Modifiers;
using MiraAPI.Patches.Stubs;
using MiraAPI.Roles;
using MiraAPI.Translation;
using MiraAPI.Utilities;
using MiraAPI.Utilities.Assets;
using MiraAPI.Voting;
using Reactor.Networking.Attributes;
using Reactor.Utilities;
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
using Vigilante.Modifiers.Logic.Impostor;

namespace Vigilante.Roles.Impostor;

public sealed class DiplomatRole(IntPtr cppPtr) : ImpostorRole(cppPtr), IVigilanteRole, IWikiDiscoverable, IDoomable
{
    private MeetingMenu? manipulationMenu;
    public DoomableType DoomHintType => DoomableType.Trickster;
    public string IdPart => "Diplomat";
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

    [HideFromIl2Cpp] public List<PlayerControl>? manipulatedChosen { get; set; } = new List<PlayerControl>();
    public bool isRevealed;
    

    public CustomRoleConfiguration Configuration => new(this)
    {
        IconTmp = TmpSpriteUtils.CreateSpriteAsset(VigilanteAssets.DiplomatIcon.LoadAsset(), "Vigilante.Role.Impostor.Diplomat", 1.45f),
        OptionsScreenshot = TouBanners.ImpostorRoleBanner,
        Icon = VigilanteAssets.DiplomatIcon,
        MaxRoleCount = 1
    };

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities
    {
        get
        {
            return
            [
                new(MiraLocaleManager.Get($"Vigilante.Role.{IdPart}.Manipulate", "Manipulate"),
                    MiraLocaleManager.Get($"Vigilante.Role.{IdPart}.Manipulate.WikiDescription"),
                    TouImpAssets.ControlSprite),
                new(MiraLocaleManager.Get($"Vigilante.Role.{IdPart}.Reveal", "Reveal"),
                    MiraLocaleManager.Get($"Vigilante.Role.{IdPart}.Reveal.WikiDescription"),
                    TouAssets.RevealButtonSprite)
            ];
        }
    }

    public override void Initialize(PlayerControl player)
    {
        RoleBehaviourStubs.Initialize(this, player);
        // clear chosen
        isRevealed = false;
        if (manipulatedChosen != null) manipulatedChosen.Clear();

        manipulationMenu = new MeetingMenu(
                this,
                Click, // on click
                MeetingAbilityType.Toggle,
                VigilanteAssets.DiplomatManipulate,
                VigilanteAssets.MagistrateJudgement,
                IsExempt, // who NOT applies to
                position: new Vector3(-0.40f, 0f, -3f)
                );//will break next toum update
    }

    public void Click(PlayerVoteArea voteArea, MeetingHud __)
    {
        var player = GameData.Instance.GetPlayerById(voteArea.PlayerId)?.Object;
        if (player == null) return;
        int limit = isRevealed ? 2 : 1;

        if (manipulatedChosen.Count > (limit-1))
        {
            manipulationMenu!.Actives[manipulatedChosen[0].PlayerId] = false;
            manipulatedChosen.RemoveAt(0);
        }
        manipulatedChosen.Add(player);

        manipulationMenu!.Actives[voteArea.PlayerId] = true;
    }

    public bool IsExempt(PlayerVoteArea voteArea)
    {
        var player = GameData.Instance.GetPlayerById(voteArea.PlayerId)?.Object;
        if (player.HasDied() || player == null || player.IsImpostor() || player.IsRole<DiplomatRole>()) 
        {
            Info("PLAYER EXEMPT");
            return true;
        }
        Info("NOT EXEMPT");
        return false;
    }

    public override void OnMeetingStart()
    {
        RoleBehaviourStubs.OnMeetingStart(this);
        manipulatedChosen.Clear();

        var meeting = MeetingHud.Instance;
        if (Player.AmOwner && meeting != null && !Player.HasDied())
        {
            manipulationMenu?.GenButtons(meeting, true);
            Info("PRINTING MENUUU");
        }
    }

    [MethodRpc((uint)VigilanteRpcs.DiplomatReveal)]
    public static void RpcReveal(PlayerControl player)
    {
        if (LobbyBehaviour.Instance)
        {
            MiscUtils.RunAnticheatWarning(player);
            return;
        }
        if (player.Data.Role is not DiplomatRole diplomat)
        {
            return;
        }

        if (PlayerControl.LocalPlayer.IsHost())
        {
            foreach (var plr in Helpers.GetAlivePlayers())
            {
                var voteData = plr.GetVoteData();

                if (!voteData.VotedFor(player.PlayerId))
                {
                    continue;
                }

                voteData.Votes.RemoveAll(x=>x.Suspect==player.PlayerId);
                voteData.VotesRemaining += 1;

                VotingUtils.RpcRemoveVote(PlayerControl.LocalPlayer, plr.PlayerId, player.PlayerId);
            }
        }

        player.AddModifier<DiplomatRevealModifier>(RoleManager.Instance.GetRole((RoleTypes)RoleId.Get<DiplomatRole>()));
        player.AddModifier<DiplomatImmunityModifier>();
        diplomat.isRevealed = true;

        TouAudio.PlaySound(TouAudio.ScreamIntro);
        Coroutines.Start(MiscUtils.CoFlash(TownOfUsColors.Impostor));

        var notif = Helpers.CreateAndShowNotification("The Diplomat has revealed themself!!!", // replace with proper locale later
            Color.white, new Vector3(0f, 1f, -20f), spr: TouRoleIcons.Impostor.LoadAsset());
        notif.AdjustNotification();
    }
}
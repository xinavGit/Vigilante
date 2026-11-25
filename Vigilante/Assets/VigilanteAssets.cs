using System;
using Vigilante;
using MiraAPI.Utilities.Assets;
using Reactor.Utilities;
using UnityEngine;

namespace Vigilante.Assets;

public static class VigilanteAssets
{
    private const string ButtonPath = "Vigilante.Resources.Buttons";
    private const string IconPath = "Vigilante.Resources.Icons";
    private const string SoundPath = "Vigilante.Resources.Sound";
    private const string BannerPath = "Vigilante.Resources.Banners";

    //icons
    public static LoadableAsset<Sprite> ChameleonIcon { get; } = new LoadableResourceAsset($"{IconPath}.Chameleon.png", 200);
    public static LoadableAsset<Sprite> DecryptorIcon { get; } = new LoadableResourceAsset($"{IconPath}.Decryptor.png", 200);
    public static LoadableAsset<Sprite> InspectorIcon { get; } = new LoadableResourceAsset($"{IconPath}.Inspector.png", 200);
    public static LoadableAsset<Sprite> JudgeIcon { get; } = new LoadableResourceAsset($"{IconPath}.Judge.png", 200);
    public static LoadableAsset<Sprite> PacifistIcon { get; } = new LoadableResourceAsset($"{IconPath}.Pacifist.png", 200);
    public static LoadableAsset<Sprite> CrusaderIcon { get; } = new LoadableResourceAsset($"{IconPath}.Crusader.png", 200);
    public static LoadableAsset<Sprite> DiplomatIcon { get; } = new LoadableResourceAsset($"{IconPath}.Diplomat.png", 200);
    public static LoadableAsset<Sprite> DisruptorIcon { get; } = new LoadableResourceAsset($"{IconPath}.Disruptor.png", 200);
    public static LoadableAsset<Sprite> KidnapperIcon { get; } = new LoadableResourceAsset($"{IconPath}.Kidnapper.png", 200);
    public static LoadableAsset<Sprite> JinxIxon { get; } = new LoadableResourceAsset($"{IconPath}.Jinx.png", 200);
    public static LoadableAsset<Sprite> RitualistIcon { get; } = new LoadableResourceAsset($"{IconPath}.Ritualist.png", 200);
    public static LoadableAsset<Sprite> BansheeIcon { get; } = new LoadableResourceAsset($"{IconPath}.Banshee.png", 200);
    public static LoadableAsset<Sprite> AccumulatorIcon { get; } = new LoadableResourceAsset($"{IconPath}.Accumulator.png", 200);
    public static LoadableAsset<Sprite> StarspawnIcon { get; } = new LoadableResourceAsset($"{IconPath}.Starspawn.png", 200);
    // modifier icons
    public static LoadableAsset<Sprite> BreakdownIcon { get; } = new LoadableResourceAsset($"{IconPath}.Breakdown.png", 200);

    //buttons
    public static LoadableAsset<Sprite> ChameleonVent { get; } = new LoadableResourceAsset($"{ButtonPath}.ChameleonVent.png");
    public static LoadableAsset<Sprite> CrusaderSanctify { get; } = new LoadableResourceAsset($"{ButtonPath}.Sanctify.png");
    public static LoadableAsset<Sprite> RitualistRitual { get; } = new LoadableResourceAsset($"{ButtonPath}.Ritual.png");

    // Audio clips (16000hz)
    public static LoadableAsset<AudioClip> InspectorPublish { get; } = new LoadableAudioResourceAsset($"{SoundPath}.InspectorPublish.wav");
    public static LoadableAsset<AudioClip> RitualistRit { get; } = new LoadableAudioResourceAsset($"{SoundPath}.Ritted.wav");
    public static LoadableAsset<AudioClip> CourtGavel { get; } = new LoadableAudioResourceAsset($"{SoundPath}.CourtGavel.wav");
    public static LoadableAsset<AudioClip> DiplomatIntro { get; } = new LoadableAudioResourceAsset($"{SoundPath}.DiplomatIntro.wav");

    // Meeting nameplate toggles:
    public static LoadableAsset<Sprite> InspectorInspect { get; } = new LoadableResourceAsset($"{ButtonPath}.Inspect.png", 300f);
    public static LoadableAsset<Sprite> MagistrateJudgement { get; } = new LoadableResourceAsset($"{ButtonPath}.Judgement.png", 300f); // button from BTOS2
    public static LoadableAsset<Sprite> DiplomatManipulate { get; } = new LoadableResourceAsset($"{ButtonPath}.Manipulate.png", 300f);
    public static LoadableAsset<Sprite> RitualistEliminate { get; } = new LoadableResourceAsset($"{ButtonPath}.Eliminate.png", 300f);
    public static LoadableAsset<Sprite> InspectorPublishButton { get; } = new LoadableResourceAsset($"{ButtonPath}.Publish.png", 300f);
    public static LoadableAsset<Sprite> InspectorForgeButton { get; } = new LoadableResourceAsset($"{ButtonPath}.Forge.png", 300f);

    // Option Banners:
    public static LoadableAsset<Sprite> ChameleonBanner { get; } = new LoadableResourceAsset($"{BannerPath}.ChameleonBanner.png");
    public static LoadableAsset<Sprite> DecryptorBanner { get; } = new LoadableResourceAsset($"{BannerPath}.DecryptorBanner.png");
    public static LoadableAsset<Sprite> InspectorBanner { get; } = new LoadableResourceAsset($"{BannerPath}.InspectorBanner.png");
    public static LoadableAsset<Sprite> DisruptorBanner { get; } = new LoadableResourceAsset($"{BannerPath}.DisruptorBanner.png");
    public static LoadableAsset<Sprite> DiplomatBanner { get; } = new LoadableResourceAsset($"{BannerPath}.DiplomatBanner.png");
    // i forgot to make one for ritualist


    public static LoadableAsset<Sprite> CourtOverlay { get; } = new LoadableResourceAsset($"Vigilante.Resources.CourtOverlay.png");
}

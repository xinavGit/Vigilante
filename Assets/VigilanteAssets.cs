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

    // Audio clips (16000hz)
    public static LoadableAsset<AudioClip> InspectorPublish { get; } = new LoadableAudioResourceAsset($"{SoundPath}.InspectorPublish.wav");

    // Meeting nameplate toggles:
    public static LoadableAsset<Sprite> InspectorInspect { get; } = new LoadableResourceAsset($"{ButtonPath}.Inspect.png", 300f);
    public static LoadableAsset<Sprite> MagistrateJudgement { get; } = new LoadableResourceAsset($"{ButtonPath}.Judgement.png", 300f); // button from BTOS2
}

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
    public static LoadableAsset<Sprite> DreamerIcon { get; } = new LoadableResourceAsset($"{IconPath}.Dreamer.png", 200);
    public static LoadableAsset<Sprite> ChameleonIcon { get; } = new LoadableResourceAsset($"{IconPath}.Chameleon.png", 200);

    //buttons
    public static LoadableAsset<Sprite> ChameleonVent { get; } = new LoadableResourceAsset($"{ButtonPath}.ChameleonVent.png");

    // Audio clips (16000hz)
    public static LoadableAsset<AudioClip> DreamerIntro { get; } = new LoadableAudioResourceAsset($"{SoundPath}.DreamerIntro.wav");

    // Meeting nameplate toggles:
    public static LoadableAsset<Sprite> DreamerMeetingDream { get; } = new LoadableResourceAsset($"{ButtonPath}.DreamerMeetingDream.png", 440f);
}

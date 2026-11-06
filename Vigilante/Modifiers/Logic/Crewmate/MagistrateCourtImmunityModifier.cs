using System.Collections;
using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using MiraAPI.Utilities;
using Reactor.Utilities;
using TownOfUs.Options.Roles.Impostor;
using TownOfUs.Assets;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;
using MiraAPI.Translation;
using TownOfUs.Utilities;
using Vigilante.Assets;

namespace Vigilante.Modifiers.Logic.Crewmate;

public sealed class MagistrateCourtImmunityModifier() : BaseModifier
{
    public SpriteRenderer BmOverlay;
    public PlayerVoteArea VoteArea;
    public override string ModifierName => "Magistrate Immunity";
    public override bool HideOnUi => true;
    public override bool ShowInFreeplay => true;

    public override void OnActivate()
    {
        base.OnActivate();
        SetUpOverlay();
    }

    public override void OnMeetingStart()
    {
        base.OnMeetingStart();
        SetUpOverlay();
    }

    public void SetUpOverlay()
    {
        var meetingInstance = MeetingHud.Instance;
        if (!meetingInstance)
        {
            return;
        }

        VoteArea = meetingInstance.playerStates.FirstOrDefault(x => x.PlayerId == Player.PlayerId)!;

        BmOverlay = Object.Instantiate(VoteArea.XMark, VoteArea.XMark.transform.parent);
        BmOverlay.transform.localPosition = new Vector3(0, 0, -2);
        BmOverlay.transform.localScale = new Vector3(0.769f, 1, 1);
        BmOverlay.sprite = VigilanteAssets.CourtOverlay;
        BmOverlay.gameObject.SetActive(true);
        VoteArea.ColorBlindName.gameObject.SetActive(false);
    }
    public override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    public override void OnDeath(DeathReason reason)
    {
        RemoveSelf();
    }
}
/*using MiraAPI.Events;
using MiraAPI.Modifiers;
using MiraAPI.Roles;
using MiraAPI.Translation;
using MiraAPI.Utilities;
using Reactor.Utilities.Extensions;
using TownOfUs;
using TownOfUs.Assets;
using TownOfUs.Events.TouEvents;
using TownOfUs.Roles.Crewmate;
using TownOfUs.Utilities;
using UnityEngine;
using UnityEngine.UI;
using Vigilante.Assets;
using Vigilante.Roles.Impostor;
using Object = UnityEngine.Object;

namespace Vigilante.Modifiers.Logic.Impostor;

public sealed class KidnapperKidnappedModifier(byte kidnapperId) : BaseModifier
{
    private GameObject jailCell;
    public override string ModifierName => "Kidnapped";
    public override bool HideOnUi => true;
    public byte KidnapperId { get; } = kidnapperId;

    public bool IsKidnapperValid => !GameData.Instance.GetPlayerById(KidnapperId).Object.HasDied() &&
                                    GameData.Instance.GetPlayerById(KidnapperId).Object.Data.Role is KidnapperRole;

    public override void OnActivate()
    {
        base.OnActivate();
    }

    public override void OnMeetingStart()
    {
        Clear();
        var meeting = MeetingHud.Instance;
        if (GameData.Instance.GetPlayerById(KidnapperId).Object.HasDied() ||
            GameData.Instance.GetPlayerById(KidnapperId).Object.Data.Role is not KidnapperRole || Player.HasDied() ||
            meeting == null)
        {
            RemoveSelf();
            return;
        }

        if (Player.Data.Role is ProsecutorRole pros)
        {
            pros.HideProsButton = true;
        }

        if (Player.AmOwner)
        {
            var title =
                $"<color=#{TownOfUsColors.ImpSoft.ToHtmlStringRGBA()}>{MiraLocaleManager.Get("TownOfUsMira.Modifier.JailedFeedbackTitle")}</color>";

            var text = MiraLocaleManager.Get("TownOfUsMira.Modifier.JailedNonCrewFeedback");

            if (PlayerControl.LocalPlayer.Is(ModdedRoleTeams.Crewmate))
            {
                text = MiraLocaleManager.Get("TownOfUsMira.Modifier.JailedCrewFeedback");
            }

            MiscUtils.AddFakeChat(PlayerControl.LocalPlayer.Data, title, text, false, true);

            var notif1 = Helpers.CreateAndShowNotification(
                $"<b>{TownOfUsColors.ImpSoft.ToTextColor()}{text}</color></b>", Color.white,
                new Vector3(0f, 1f, -20f), spr: VigilanteAssets.KidnapperIcon);

            notif1.AdjustNotification();
        }

        foreach (var voteArea in meeting.playerStates)
        {
            if (Player.PlayerId == voteArea.PlayerId)
            {
                GenCell(voteArea);
            }
        }
    }

    public void Clear()
    {
        if (jailCell)
        {
            jailCell.Destroy();
        }
    }

    private void GenCell(PlayerVoteArea voteArea)
    {
        var confirmButton = voteArea.Buttons.transform.GetChild(0).gameObject;
        var parent = confirmButton.transform.parent.parent;

        var jailCellObj = Object.Instantiate(confirmButton, voteArea.transform);

        var cellRenderer = jailCellObj.GetComponent<SpriteRenderer>();
        cellRenderer.sprite = TouAssets.InJailSprite;

        jailCellObj.transform.localPosition = new Vector3(-0.95f, 0f, -2f);
        jailCellObj.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
        jailCellObj.layer = 5;
        jailCellObj.transform.parent = parent;
        jailCellObj.transform.GetChild(0).gameObject.DeepDestroy();

        var passive = jailCellObj.GetComponent<PassiveButton>();
        passive.OnClick = new Button.ButtonClickedEvent();

        jailCell = jailCellObj;
    }

    public override void OnDeath(DeathReason reason)
    {
        RemoveSelf();
    }
}*/
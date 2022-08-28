using TMPro;
using UnityEngine;

public class PlayerIconManager : MonoBehaviour
{
    public TMP_Text PlayerName;
    public GameObject PlayingIndicator;
    public GameObject DeadIndicator;
    public GameObject ConfirmButton;
    public GameObject ViewButton;

    public PlayerManager CurrentManager;
    public OtherPlayerSpaceManager OtherPlayerSpace;

    public void SetName(string name)
    {
        PlayerName.text = name;
    }

    public void SetPlaying()
    {
        PlayingIndicator.SetActive(true);
    }

    public void UnsetPlaying()
    {
        PlayingIndicator.SetActive(false);
    }

    public void SetDead()
    {
        DeadIndicator.SetActive(true);
    }

    public void Selected()
    {
        if (this.CurrentManager != OtherPlayerSpace.Manager.LogicManager.CurrentPlayer &&
            (OtherPlayerSpace.Manager.LogicManager.IsStealing ||
             OtherPlayerSpace.Manager.LogicManager.IsFavor) &&
             OtherPlayerSpace.LocalPlayerNum == OtherPlayerSpace.Manager.LogicManager.CurrentPlayer.GetPosition())
        {
            this.ConfirmButton.SetActive(true);
        }

        OtherPlayerSpace.HandleClicks(this);
        ViewButton.SetActive(true);
    }

    public void ViewPlayer()
    {
        ViewButton.SetActive(false);
        ConfirmButton.SetActive(false);
        OtherPlayerSpace.ViewPlayerCards(this.CurrentManager);
    }

    public void ConfirmPlayer()
    {
        ViewButton.SetActive(false);
        ConfirmButton.SetActive(false);
        OtherPlayerSpace.PlayerSelected(this.CurrentManager);
    }

    public void InitializePlayerIcon(OtherPlayerSpaceManager otherPlayerSpaceManager, PlayerManager playerManager)
    {
        this.OtherPlayerSpace = otherPlayerSpaceManager;
        this.CurrentManager = playerManager;
    }

    public void DeactivateButtons()
    {
        ConfirmButton.SetActive(false);
        ViewButton.SetActive(false);
    }

    void Start()
    {
        PlayingIndicator.SetActive(false);
        DeadIndicator.SetActive(false);

        ConfirmButton.SetActive(false);
        ViewButton.SetActive(false);
    }
}

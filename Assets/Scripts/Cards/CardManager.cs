using TMPro;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    public TMP_Text CardName;
    public PlayerManager Player;
    public PlayerSpaceManager SpaceManager;

    public void ChangeCardName(string newName)
    {
        CardName.text = newName;
    }

    public void Selected()
    {
        SpaceManager.CardSelected(this);
    }

    public void InitializeCard(PlayerSpaceManager spaceManager, PlayerManager player)
    {
        this.SpaceManager = spaceManager;
        this.Player = player;
    }
}

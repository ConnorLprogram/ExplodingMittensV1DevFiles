using TMPro;
using UnityEngine;

public class CardInfoManager : MonoBehaviour
{
    public GameObject PopUpWindow;
    public TMP_Text CardInfo;
    public TMP_Dropdown CardDropdown;

    public void InfoButtonPressed()
    {
        if (PopUpWindow.activeSelf)
        {
            PopUpWindow.SetActive(false);
        }
        else
        {
            PopUpWindow.SetActive(true);
        }
    }

    public void ShowInfo()
    {
        int infoIndex = CardDropdown.value;

        switch (infoIndex)
        {
            case 0:
                CardInfo.text = "Defuse: If you happen to draw a bomb instead of dying a defuse will be automatically taken from your hand.\nIf you have no more defuses you will be removed from the game.";
                break;
            case 1:
                CardInfo.text = "Bomb: You won't see a bomb in this version of the game but an announcement will be made if you drew one, it will be randomly put back into the deck. If you don't have a defuse you will be removed from the game.";
                break;
            case 2:
                CardInfo.text = "Attack: This card allows you to attack a player and steal any of their cards";
                break;
            case 3:
                CardInfo.text = "Favor: Like Attack, this card allows you to attack a player but they get to choose a card to give you";
                break;
            case 4:
                CardInfo.text = "No: If someone uses Attack or Favor on you and you have this, you can use this to not give them any cards. Use it quickly if attacked or else they can take a card.";
                break;
            case 5:
                CardInfo.text = "Shuffle: Shuffles the deck of cards";
                break;
            case 6:
                CardInfo.text = "Future: Allows you to see 3 cards ahead.";
                break;
            case 7:
                CardInfo.text = "Skip: This card allows you to skip drawing a card at the end of your turn";
                break;
            case 8:
                CardInfo.text = "Mittnes: If you have a pair of mittens, must have the same number, you can use them to steal from another player, essentially an Attack. In this version you are only allowed to use pairs";
                break;
        }
    }

    void Start()
    {
        PopUpWindow.SetActive(false);
        ShowInfo();
    }
}

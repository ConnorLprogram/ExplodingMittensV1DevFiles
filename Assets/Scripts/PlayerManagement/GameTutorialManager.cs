using TMPro;
using UnityEngine;

public class GameTutorialManager : MonoBehaviour
{
    public GameObject TutorialPopUpWindow;
    public TMP_Text GameInfo;
    public TMP_Dropdown TutorialDropdown;


    public void InfoButtonPressed()
    {
        if (TutorialPopUpWindow.activeSelf)
        {
            TutorialPopUpWindow.SetActive(false);
        }
        else
        {
            TutorialPopUpWindow.SetActive(true);
        }
    }
    public void ShowInfo()
    {
        int infoIndex = TutorialDropdown.value;

        switch (infoIndex)
        {
            case 0:
                GameInfo.text = "Cards will show in the yellow section of the screen. If it is your turn you can click a valid action or pair card and use it. If you are stealing another persons card you can view their cards and click on the one you want to steal, see Players on how to do this.";
                break;
            case 1:
                GameInfo.text = "Players are shown in the white section of the game screen. You can view any player's cards whenever you want to, although it won't show the names if it is not yours. If you want to attack or use a favor and it is your turn, you will be able to click on the player you want to steal from and click confirm.";
                break;
            case 2:
                GameInfo.text = "Making a move: When it is your turn, you can make a move by clicking a valid card on your screen. The only card you can play when it is not your turn is a \"No\" card if you are being asked a favor or attacked. Once you use a card, you cannot end turn while it is being played, such as an Attack, Pair of Mittens, or Favor.";
                break;
            case 3:
                GameInfo.text = "Ending your turn: There are 2 ways to end your turn, either perform 3 actions or click the end turn button that will show on the middle left of the screen when it is your turn.";
                break;
            case 4:
                GameInfo.text = "Using Pairs: When you want to use a pair, click on both of them that you want to use. If you click on one and choose not to use it, a button to cancel will pop up on the right side of the game space.";
                break;
        }
    }

    void Start()
    {
        TutorialPopUpWindow.SetActive(false);
        ShowInfo();
    }
}

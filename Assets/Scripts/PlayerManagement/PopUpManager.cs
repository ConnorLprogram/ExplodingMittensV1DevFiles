using TMPro;
using UnityEngine;

public class PopUpManager : MonoBehaviour
{
    public TMP_Text PopUpText;
    public GameObject PopUpSpace;

    public void ChangePopUpText(string text)
    {
        PopUpText.text = text;
    }

    public void Hide()
    {
        PopUpSpace.SetActive(false);
    }
    public void Show()
    {
        PopUpSpace.SetActive(true);
    }

    void Start()
    {
        PopUpSpace.SetActive(false);
    }
}

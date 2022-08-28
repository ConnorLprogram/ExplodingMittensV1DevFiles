using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpaceManager : MonoBehaviourPunCallbacks
{
    private float CurTime = 0.0f;
    private readonly float TimeInterval = 1f;
    private bool SetUp = false;
    public int LocalPlayerNum;

    public GameObject EndButton;
    public GameObject CancelButton;
    public PlayerManager CurrentPlayerManager;
    public List<GameObject> CardObjects = new List<GameObject>();
    public List<CardManager> CardManagers = new List<CardManager>();

    public GameObject CardTemplate;

    public GameLogicManager LogicManager;
    public PopUpManager PopUpWindow;

    public void CardSelected(CardManager selectedCardManager)
    {
        int cardIndex = CardManagers.IndexOf(selectedCardManager);
        PlayerManager manager = selectedCardManager.Player;
        Card selectedCard = manager.GetHand().GetCards()[cardIndex];
        string type = selectedCard.GetCardType();

        if (!(LogicManager.IsGivingFavor || LogicManager.IsStealingCard) &&
            !(manager.IsPlaying || manager == LogicManager.Victim) &&
            !(LogicManager.CurrentPlayer.GetPosition() == LocalPlayerNum && manager.GetPosition() == LocalPlayerNum))
        {
            return;
        }

        if ((LogicManager.IsStealingCard || LogicManager.IsGivingFavor) &&
            manager == LogicManager.Victim && selectedCard.GetName() == "No" &&
            manager.GetPosition() == LocalPlayerNum)
        {
            LogicManager.NoUsed(selectedCard);
            return;
        }

        if ((LogicManager.IsGivingFavor && manager.IsPlaying) ||
            (LogicManager.IsStealingCard && manager.IsPlaying) ||
            (LogicManager.IsStealingCard && manager != LogicManager.Victim) ||
            (LogicManager.IsGivingFavor && manager.GetPosition() != LocalPlayerNum)||
            (LogicManager.IsStealingCard && LogicManager.CurrentPlayer.GetPosition() != LocalPlayerNum)) 
        {
            return;
        }

        if (LogicManager.IsStealingCard) 
        {
            LogicManager.StealCard(LogicManager.CurrentPlayer, LogicManager.Victim, cardIndex);
            return;
        }
        if (LogicManager.IsGivingFavor)
        {
            LogicManager.StealCard(LogicManager.CurrentPlayer, LogicManager.Victim, cardIndex);
            return;
        }

        if (manager != LogicManager.CurrentPlayer || 
            CurrentPlayerManager.GetPosition() != LocalPlayerNum || 
            !(type == "Action" || type == "Pair") || 
            LogicManager.GameOver)
        {
            return;
        }

        LogicManager.PlayCard(cardIndex);
    }

    public void UpdatePlayerSpaceHand(PlayerManager curPlayer)
    {
        CurrentPlayerManager = curPlayer;

        foreach (var cardManager in this.CardObjects)
        {
            Destroy(cardManager);
        }
        CardObjects.Clear();
        CardManagers.Clear();

        int indexCounter = 1;
        foreach (var card in curPlayer.GetHand().GetCards())
        {
            GameObject newCard = Instantiate(CardTemplate, new Vector3(0, 0, 0), Quaternion.identity);
            newCard.transform.SetParent(this.transform, false);
            CardManager cardManager = newCard.GetComponent<CardManager>();
            CardObjects.Add(newCard);
            CardManagers.Add(cardManager);
            cardManager.InitializeCard(this, curPlayer);

            if (curPlayer.GetPosition() == LocalPlayerNum)
            {
                cardManager.ChangeCardName(card.GetName());
            }
            else
            {
                cardManager.ChangeCardName(indexCounter.ToString());
            }
            indexCounter++;
        }
    }

    public void UpdateButtons(PlayerManager curPlayer)
    {
        if (!(LogicManager.IsStealing ||
            LogicManager.IsFavor ||
            LogicManager.IsStealingCard ||
            LogicManager.IsGivingFavor ||
            LogicManager.GameOver ||
            LogicManager.CurrentPlayer.GetPosition() != LocalPlayerNum))
        {
            EndButton.SetActive(true);
        }
        else
        {
            EndButton.SetActive(false);
        }

        if (curPlayer.IsPlaying &&
            curPlayer.GetPosition() == LocalPlayerNum &&
            !LogicManager.GameOver &&
            LogicManager.IsPairing)
        {
            CancelButton.SetActive(true);
        }
        else
        {
            CancelButton.SetActive(false);
        }
    }

    public void EndButtonPressed()
    {
        EndButton.SetActive(false);
        LogicManager.EndCurrentTurn();
    }

    public void CancelButtonPressed()
    {
        LogicManager.CancelPlay();
    }

    public void ShowPopUp(string popUpText)
    {
        PopUpWindow.ChangePopUpText(popUpText);
        PopUpWindow.Show();
    }

    public void HidePopUp()
    {
        PopUpWindow.Hide();
    }

    public void InitializePlayerSpace(PlayerManager newPlayerManager) 
    {
        CurrentPlayerManager = newPlayerManager;
        CurrentPlayerManager.UpdatePlayerSpaceManager(this);
        SetUp = true;

        GameObject numObject = GameObject.Find("PlayerNumberHolder(Clone)");
        PlayerNumberHolder numHolder = numObject.GetComponent<PlayerNumberHolder>();
        LocalPlayerNum = numHolder.PlayerPosition;
    }

    void Start()
    {
        EndButton.SetActive(false);
        CancelButton.SetActive(false);
    }

    void Update()
    {
        if (SetUp && Time.time > CurTime)
        {
            CurTime += TimeInterval;
            CurrentPlayerManager.UpdatePlayerSpaceManager(this);
            CurrentPlayerManager.UpdateButtons(this);
        }
    }
}

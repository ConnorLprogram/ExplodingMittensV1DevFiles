using Photon.Pun;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameLogicManager : MonoBehaviourPunCallbacks
{
    public int NumPlayers = 0;
    public int NumRounds = 0;
    private int LocalPlayerNum;
    public bool GameOver = false;
    public Deck CurDeck; 

    public bool IsStealing = false;
    public bool IsStealingCard = false;
    public bool IsFavor = false;
    public bool IsGivingFavor = false;
    public bool IsPairing = false;

    public Queue<PlayerManager> Players = new Queue<PlayerManager>();
    public List<PlayerManager> PlayerList = new List<PlayerManager>();
    public PlayerManager Victim;
    public PlayerManager CurrentPlayer;
    private Card FirstPair;

    public TMP_Text CurrentPlayerText;
    public TMP_Text GameAnnouncementText;
    public TMP_Text LocalPlayerAnnouncment;
    public TMP_Text DebugText;
    public GameObject RestartButton;

    public PlayerManager PlayerTemplate;
    public GameManager CurrentGameManager;
    public PlayerSpaceManager LocalPlayerSpace;
    public OtherPlayerSpaceManager OtherPlayerSpace;


    public void NextPlayer(bool curPlayerAlive = true)
    {
        IsPairing = false;
        IsStealing = false;
        IsStealingCard = false;
        IsFavor = false;
        IsGivingFavor = false;
        if (!curPlayerAlive)
        {
            this.photonView.RPC("RemoveCurrentPlayer", RpcTarget.All, CurrentPlayer.GetPosition());
        }
        else
        {
            this.Players.Enqueue(this.CurrentPlayer);
            this.CurrentPlayer = Players.Dequeue();
        }
        ChangePlaying();
        CurrentGameManager.EndPlayerTurn();
        CurrentPlayerText.text = "Player " + CurrentPlayer.GetPosition() + " is now playing";
        PhotonNetwork.SendAllOutgoingCommands();
    }

    private void ChangePlaying()
    {
        foreach (var player in this.PlayerList)
        {
            if (player != CurrentPlayer)
            {
                player.IsPlaying = false;
            }
            else
            {
                player.IsPlaying = true;
                
                if (LocalPlayerNum == player.GetPosition())
                {
                    LocalPlayerAnnouncment.text = "It is your turn";
                }
            }
        }

        OtherPlayerSpace.UpdatePlaying();
    }

    public void PlayCard(int cardIndex)
    {
        this.photonView.RPC("EnactPlayCard", RpcTarget.All, cardIndex);
    }

    [PunRPC]
    public void EnactPlayCard(int playedCardIndex)
    {
        if (CurrentGameManager.TurnEnded())
        {
            return;
        }

        Card playedCard = CurrentPlayer.GetHand().RetrieveCardAt(playedCardIndex);

        Debug.Log("Card played: " + playedCard.GetName() + "; Player position: " + CurrentPlayer.GetPosition() + "; num action: " + CurrentGameManager.GetCurPlayerActions());

        string cardType = playedCard.GetCardType();
        string cardName = playedCard.GetName();

        bool skipped = false;

        if (cardType == "Action")
        {
            if (cardName == "Attack")
            {
                if (CurrentPlayer.GetPosition() == LocalPlayerSpace.LocalPlayerNum)
                {
                    LocalPlayerAnnouncment.text = "Choose a player to steal from";
                }

                IsStealing = true;
                CurrentPlayer.RemoveCard(playedCard);
                return;
            }
            else if (cardName == "Favor")
            {
                if (CurrentPlayer.GetPosition() == LocalPlayerSpace.LocalPlayerNum)
                {
                    LocalPlayerAnnouncment.text = "Choose a player to ask a favor from";
                }
                IsFavor = true;
                CurrentPlayer.RemoveCard(playedCard);
                return;
            }
            else if (cardName == "Skip") 
            {
                skipped = true;
            }
            else if (cardName == "Shuffle")
            {
                if (CurrentPlayer.GetPosition() == LocalPlayerNum)
                {
                    this.CurDeck.ShuffleDeck();
                    //CurrentPlayer.RemoveCard(playedCard);
                    //CurrentGameManager.IncreaseAction();
                    this.photonView.RPC("SetDeck", RpcTarget.Others, CurDeck.ConvertDeckToNums());
                }
                //else
                //{
                //    CurrentPlayer.RemoveCard(playedCard);
                //    CurrentGameManager.IncreaseAction();
                //}

                //return;
            }
            else if (cardName == "Future")
            {
                if (this.CurrentPlayer.GetPosition() == LocalPlayerSpace.LocalPlayerNum)
                {
                    List<Card> top3Cards = this.CurDeck.GetTop3Cards();
                    LocalPlayerSpace.ShowPopUp("Next 3 cards are: " + top3Cards[0].GetName() + ", " + top3Cards[1].GetName() + ", " + top3Cards[2].GetName());
                }
            }
            else return;
        }
        else if (cardType == "Pair")
        {
            if (!IsPairing) 
            {
                if (CurrentPlayer.GetPosition() == LocalPlayerSpace.LocalPlayerNum)
                {
                    LocalPlayerAnnouncment.text = "Choose a second pair";
                }
                FirstPair = playedCard;
                IsPairing = true;
                return;
            }
            else
            {
                if (!IsPair(FirstPair, playedCard) || FirstPair == playedCard)
                {
                    if (CurrentPlayer.GetPosition() == LocalPlayerSpace.LocalPlayerNum)
                    {
                        LocalPlayerAnnouncment.text = "Not a pair, choose a " + FirstPair.GetName();
                    }
                    return;
                }
                else
                {
                    if (CurrentPlayer.GetPosition() == LocalPlayerSpace.LocalPlayerNum)
                    {
                        LocalPlayerAnnouncment.text = "Choose a player to steal from";
                    }
                    IsStealing = true;
                    IsPairing = false;
                    CurrentPlayer.RemoveCard(playedCard);
                    CurrentPlayer.RemoveCard(FirstPair);
                }
                return;
            }
        }
        else return;

        if (cardType == "Action" || cardType == "Pair" && cardName != "No")
        {
            CurrentPlayerText.text = "Player " + CurrentPlayer.GetPosition() + " has played: " + cardName;
        }

        CurrentPlayer.RemoveCard(playedCard);

        if (skipped)
        {
            Debug.Log("Skipped");
            NextPlayer();
            return;
        }

        CurrentGameManager.IncreaseAction();
        PhotonNetwork.SendAllOutgoingCommands();
    }

    public void StealingFrom(PlayerManager victim)
    {
        this.photonView.RPC("EnactStealingFrom", RpcTarget.All, victim.GetPosition());
    }

    [PunRPC]
    public void EnactStealingFrom(int playerNum)
    {
        IsStealing = false;
        IsStealingCard = true;
        Victim = PlayerList[playerNum - 1];

        GameAnnouncementText.text = "Player " + CurrentPlayer.GetPosition() + " is stealing from Player " + playerNum;

        if (playerNum == LocalPlayerSpace.LocalPlayerNum)
        {
            LocalPlayerAnnouncment.text = "Player " + CurrentPlayer.GetPosition() + " is going to take a card from you, play No now if you have one before they steal a card";
        }
    }

    public void AskingFavorFrom(PlayerManager victim)
    {
        this.photonView.RPC("EnactFavorFrom", RpcTarget.All, victim.GetPosition());
    }

    [PunRPC]
    public void EnactFavorFrom(int playerNum)
    {
        IsFavor = false;
        IsGivingFavor = true;
        Victim = PlayerList[playerNum - 1];

        GameAnnouncementText.text = "Player " + CurrentPlayer.GetPosition() + " is asking a favor from Player " + playerNum;

        if (playerNum == LocalPlayerSpace.LocalPlayerNum)
        {
            LocalPlayerAnnouncment.text = "Player " + CurrentPlayer.GetPosition() + " is asking a favor from you, give them a card or play No if you have one";
        }
    }

    public void StealCard(PlayerManager stealer, PlayerManager victim, int index)
    {
        this.photonView.RPC("EnactStealCard", RpcTarget.All, stealer.GetPosition(), victim.GetPosition(), index);
    }

    [PunRPC]
    public void EnactStealCard(int stealerNum, int victimNum, int index)
    {
        GameAnnouncementText.text = "Player " + CurrentPlayer.GetPosition() + " has taken from Player " + victimNum;
        IsPairing = false;
        IsStealing = false;
        IsStealingCard = false;
        IsFavor = false;
        IsGivingFavor = false;

        PlayerManager stealer = this.PlayerList[stealerNum - 1];
        PlayerManager victim = this.PlayerList[victimNum - 1];

        Card stolenCard = victim.GetHand().RemoveCardAtIndex(index);

        if (stealerNum == LocalPlayerSpace.LocalPlayerNum)
        {
            LocalPlayerAnnouncment.text = "You have received: " + stolenCard.GetName();
        }

        if (victimNum == LocalPlayerSpace.LocalPlayerNum)
        {
            LocalPlayerAnnouncment.text = "You lost: " + stolenCard.GetName();
        }

        stealer.AddCard(stolenCard);
        Debug.Log(stealer.GetHand().GetCards()[stealer.GetHand().GetCards().Count - 1].GetName());

        CurrentGameManager.IncreaseAction();
    }

    public void NoUsed(Card usedNoCard)
    {
        this.photonView.RPC("EnactNoUsed", RpcTarget.All, Victim.GetHand().GetCards().IndexOf(usedNoCard));
    }

    [PunRPC]
    public void EnactNoUsed(int usedNoCardIndex)
    {
        IsPairing = false;
        IsStealing = false;
        IsStealingCard = false;
        IsFavor = false;
        IsGivingFavor = false;
        GameAnnouncementText.text = "Player " + Victim.GetPosition() + " has used a No";

        Victim.RemoveCard(Victim.GetHand().GetCards()[usedNoCardIndex]);
        CurrentGameManager.IncreaseAction();
    }

    public void EndCurrentTurn()
    {
        LocalPlayerAnnouncment.text = "";
        Debug.Log("Locally ended turn");
        this.photonView.RPC("EnactEndCurrentTurn", RpcTarget.All);
    }

    [PunRPC]
    public void EnactEndCurrentTurn()
    {
        //List<Card> top3Cards = this.CurDeck.GetTop3Cards(); //Debug code for current and next 2 cards
        //Debug.Log("Next 3 cards are: " + top3Cards[0].GetName() + ", " + top3Cards[1].GetName() + ", " + top3Cards[2].GetName());
        //DebugText.text = "top3Cards[0].GetName() + ", " + top3Cards[1].GetName() + ", " + top3Cards[2].GetName();

        bool playerAlive = true;

        Card drawnCard = DrawCard();
        this.CurrentPlayer.AddCard(drawnCard);
        
        if (drawnCard.GetName() == "Bomb")
        {
            GameAnnouncementText.text = "Player " + CurrentPlayer.GetPosition() + " drew a bomb";
            this.CurrentPlayer.RemoveCard(drawnCard);
            playerAlive = BombDrawn(drawnCard);
        }

        NextPlayer(playerAlive);
    }

    public bool BombDrawn(Card bombCard)
    {
        bool alive = true;

        if (CurrentPlayer.HasCardType("Defuse"))
        {
            if (CurrentPlayer.GetPosition() == LocalPlayerSpace.LocalPlayerNum)
            {
                LocalPlayerAnnouncment.text = "Bomb drawn, removing defuse";
            }

            Card removedDefuse = CurrentPlayer.GetHand().FindOneOfCard("Defuse");
            CurrentPlayer.RemoveCard(removedDefuse);
            Debug.Log("Current player lost a defuse");
        }
        else
        {
            alive = false;
        }

        System.Random random = new System.Random();
        this.ReturnCard(random.Next(0, this.GetNumCards()), bombCard);

        return alive;
    }

    public void ReturnCard(int index, Card card)
    {
        this.CurDeck.AddPlayedCard(index, card);
    }

    [PunRPC] //Should not need to be an rpc but for some reason player deaths will not sync
    public void RemoveCurrentPlayer(int playerIndex)
    {
        GameAnnouncementText.text = "Player " + playerIndex + " has been removed";
        PlayerManager playerToRemove = PlayerList[playerIndex - 1];
        playerToRemove.IsAlive = false;
        OtherPlayerSpace.UpdateDeath(playerToRemove);
        if (playerIndex == CurrentPlayer.GetPosition())
        {
            this.CurrentPlayer = Players.Dequeue();
        }

        if (Players.Count == 0) //Would like to put into another method but for some reason does not sync if so
        {
            GameAnnouncementText.text = "Player " + CurrentPlayer.GetPosition() + " is the winner";
            if (CurrentPlayer.GetPosition() == LocalPlayerNum)
            {
                LocalPlayerAnnouncment.text = "You are the winner!";
            }
            GameOver = true;
            CurrentPlayer.IsPlaying = false;
        }
    }

    public Card DrawCard()
    {
        return this.CurDeck.DrawCard();
    }

    public bool IsPair(Card card1, Card card2)
    {
        return card1.GetCardType() == "Pair" && card2.GetCardType() == "Pair" && card1.GetName() == card2.GetName();
    }

    public void CancelPlay() //Only really useful for pairing as of now
    {
        IsPairing = false;
        IsStealing = false;
        IsStealingCard = false;
        IsFavor = false;
        IsGivingFavor = false;

        LocalPlayerAnnouncment.text = "Cancelled pairing";
    }

    public int GetNumCards()
    {
        return this.CurDeck.GetNumCards();
    }

    [PunRPC]
    public void SetDeck(string newDeck)
    {
        CurDeck = new Deck(newDeck);
    }

    //public void EndGame(int winningPlayerNum) //Currently not working
    //{

    //    //if (PhotonNetwork.IsMasterClient)
    //    //{
    //    //    RestartButton.SetActive(true);
    //    //}
    //}

    //public void RestartGame() //Not working
    //{
    //    RestartButton.SetActive(false);
    //    if (PhotonNetwork.IsMasterClient)
    //    {
    //        PhotonNetwork.LoadLevel(1); //shouldn't need to check if master client
    //    }
    //}

    public void InitializeLogicManager(int numPlayers, string newCards, int localPlayerNum)
    {
        LocalPlayerNum = localPlayerNum;
        this.CurDeck = new Deck(newCards);
        this.NumPlayers = numPlayers;
        CreateAllPlayers();
        this.CurrentPlayer = this.Players.Dequeue();
        CurrentPlayer.IsPlaying = true;
        LocalPlayerSpace.InitializePlayerSpace(PlayerList[localPlayerNum - 1]);

        CurrentPlayerText.text = "Current player: Player " + CurrentPlayer.GetPosition();
        GameAnnouncementText.text = "";
        LocalPlayerAnnouncment.text = "";
    }

    private void CreateAllPlayers()
    {
        for (int position = 1; position <= NumPlayers; position++)
        {
            PlayerManager newPlayer = Instantiate(PlayerTemplate);
            newPlayer.transform.SetParent(this.transform, false);
            newPlayer.InitializePlayer(CreateHand(), position);

            this.Players.Enqueue(newPlayer);
            this.PlayerList.Add(newPlayer);
        }
    }

    private Hand CreateHand()
    {
        List<Card> newHand = new List<Card> { new DefuseCard() };
        for (int i = 0; i < 7; i++)
        {
            newHand.Add(this.CurDeck.DrawCard());
        }

        return new Hand(newHand);
    }

    private void Start()
    {
        RestartButton.SetActive(false);
        DebugText.text = "";
    }

    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }
}

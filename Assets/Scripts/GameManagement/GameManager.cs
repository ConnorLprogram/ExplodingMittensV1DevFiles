using Photon.Pun;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviourPunCallbacks
{
    
    private bool EndedTurn = false;
    public TMP_Text ActionsLeft;

    public GameLogicManager LogicManager;
    public OtherPlayerSpaceManager OtherPlayerSpace;
    public int CurPlayerActions = 0;

    public void AutoEndPlayerTurn()
    {
        CurPlayerActions = 0;
        ActionsLeft.text = "Actions left: 3";
        EndedTurn = false;
        LogicManager.EndCurrentTurn();
    }

    public void EndPlayerTurn()
    {
        CurPlayerActions = 0;
        ActionsLeft.text = "Actions left: 3";
        EndedTurn = false;
    }

    public void IncreaseAction()
    {
        CurPlayerActions++;
        ActionsLeft.text = $"Actions left: {3 - CurPlayerActions}";
    }

    public int GetCurPlayerActions()
    {
        return this.CurPlayerActions;
    }

    public bool TurnEnded() 
    {
        return CurPlayerActions >= 3;
    }

    [PunRPC]
    public void SetLogicManager(string cardNums)
    {
        GameObject numObject = GameObject.Find("PlayerNumberHolder(Clone)");
        PlayerNumberHolder numHolder = numObject.GetComponent<PlayerNumberHolder>();

        this.LogicManager.InitializeLogicManager(PhotonNetwork.CurrentRoom.PlayerCount, cardNums, numHolder.PlayerPosition);
        OtherPlayerSpace.ShowPlayers(LogicManager.PlayerList, numHolder.PlayerPosition);
        OtherPlayerSpace.UpdatePlaying();
        ActionsLeft.text = "Actions left: 3";
        //OtherPlayerSpace.IconManagers[0].SetPlaying(); //Hardcoded but still does not work.
    }

    [PunRPC]
    public void SyncDecks(string newDeck)
    {
        LogicManager.SetDeck(newDeck);
        Debug.Log("SyncDecks called");
    }

    // Start is called before the first frame update
    void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Deck startDeck = new Deck();
            string convertedDeckToString = startDeck.ConvertDeckToNums();
            this.photonView.RPC("SetLogicManager", RpcTarget.All, convertedDeckToString);
            
            LogicManager.CurDeck.AddBombs();
            LogicManager.CurDeck.ShuffleDeck();
            this.photonView.RPC("SyncDecks", RpcTarget.All, LogicManager.CurDeck.ConvertDeckToNums());

            PhotonNetwork.SendAllOutgoingCommands();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!EndedTurn && CurPlayerActions == 3 && PhotonNetwork.IsMasterClient)
        {
            EndedTurn = true;
            AutoEndPlayerTurn();
        }
    }
}

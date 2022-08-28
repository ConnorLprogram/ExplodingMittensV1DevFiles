using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    public TMP_Text LobbyText;
    public GameObject StartButton;
    public PlayerNumberHolder NumberHolder;

    private bool Started = false;
    string gameVersion = "1";

    [SerializeField]
    private byte MaxPlayersPerRoom = 5;

    public void Connect()
    {
        if (PhotonNetwork.IsConnected && PhotonNetwork.NetworkClientState == ClientState.ConnectedToMasterServer)
        {
            PhotonNetwork.JoinRandomRoom();
        }
        else
        {
            PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.GameVersion = gameVersion;
        }
    }

    public override void OnConnectedToMaster()
    {
        if (PhotonNetwork.NetworkClientState == ClientState.ConnectedToMasterServer)
        {
            PhotonNetwork.JoinRandomRoom();
        }
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogWarningFormat("OnDisconnected() was called by PUN with reason {0}", cause);
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("OnJoinRandomFailed() was called by PUN. No random room available, so we create one.\nCalling: PhotonNetwork.CreateRoom");

        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = MaxPlayersPerRoom });
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("OnJoinedRoom() called by PUN. Now this client is in a room. ");
    }

    public void StartGame()
    {
        Started = true;
        int playerNum = 1;
        Player[] players = PhotonNetwork.PlayerList;
        List<string> actorToPlayerNum = new();

        foreach (var player in players)
        {
            actorToPlayerNum.Add(player.ActorNumber + "@" + playerNum.ToString());
            playerNum++;
        }

        string actorToPlayerNumString = string.Join(" ", actorToPlayerNum);

        if (PhotonNetwork.IsMasterClient)
        {
            this.photonView.RPC("SetLocalPlayerNum", RpcTarget.All, actorToPlayerNumString);
            PhotonNetwork.SendAllOutgoingCommands();
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.LoadLevel(1);
        }
    }

    [PunRPC]
    public void SetLocalPlayerNum(string actorToPlayerNumString)
    {
        List<string> actorToPlayerNums = actorToPlayerNumString.Split(" ").ToList();
        foreach(var actorToPlayer in actorToPlayerNums)
        {
            string[] actorToPlayerConversion = actorToPlayer.Split("@");
            if (Int32.Parse(actorToPlayerConversion[0]) == PhotonNetwork.LocalPlayer.ActorNumber)
            {
                PlayerNumberHolder numberHolder = Instantiate(NumberHolder);
                numberHolder.PlayerPosition = Int32.Parse(actorToPlayerConversion[1]);
                DontDestroyOnLoad(numberHolder);
                return;
            }
        }
    }

    void Start()
    {
        StartButton.SetActive(false);
        Connect();
    }

    void Update()
    {
        if (PhotonNetwork.InRoom)
        {
            LobbyText.text = "Current players: " + PhotonNetwork.CurrentRoom.PlayerCount;

            if (!Started && PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom.PlayerCount >= 2)
            {
                StartButton.SetActive(true);
            }
            else
            {
                StartButton.SetActive(false);
            }
        }
    }

    void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

}

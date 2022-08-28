using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OtherPlayerSpaceManager : MonoBehaviour
{
    public int LocalPlayerNum = 1;
    public GameManager Manager;
    public PlayerSpaceManager SpaceManager;
    private PlayerIconManager CurrentClick = null;
    public List<PlayerIconManager> IconManagers = new List<PlayerIconManager>();

    public PlayerIconManager PlayerIconTemplate;

    public void ShowPlayers(List<PlayerManager> players, int localPlayerNum)
    {
        LocalPlayerNum = localPlayerNum;
        int playerNum = 1;
        foreach (var player in players)
        {
            PlayerIconManager playerIcon = Instantiate(PlayerIconTemplate, new Vector3(0, 0, 0), Quaternion.identity);
            string playerName = "Player " + player.GetPosition();
            Debug.Log(LocalPlayerNum);
            if (playerNum == localPlayerNum)
            {
                playerName += "\n(You)";
            }

            playerIcon.SetName(playerName);
            playerIcon.InitializePlayerIcon(this, player);
            playerIcon.transform.SetParent(this.transform, false);
            IconManagers.Add(playerIcon);

            playerNum++;
        }
    }
    
    public void UpdatePlaying()
    {
        foreach (var icon in IconManagers)
        {
            if (icon.CurrentManager.IsPlaying)
            {
                icon.SetPlaying(); //Does not show playing bar for player 1 when game starts, but shows after, debug says it is active;
            }
            else
            {
                icon.UnsetPlaying();
            }
        }
    }

    public void UpdateDeath(PlayerManager player)
    {
        PlayerIconManager iconManager = IconManagers.Find(icon => icon.CurrentManager == player);
        iconManager.SetDead();
    }

    public void PlayerSelected(PlayerManager player)
    {
        if (Manager.LogicManager.IsStealing && player.IsAlive)
        {
            Manager.LogicManager.StealingFrom(player);
        }
        else if (Manager.LogicManager.IsFavor && player.IsAlive)
        {
            Manager.LogicManager.AskingFavorFrom(player);
        }
    }

    public void HandleClicks(PlayerIconManager newIconManager)
    {
        if (CurrentClick != null && newIconManager != CurrentClick)
        {
            CurrentClick.DeactivateButtons();
            CurrentClick = newIconManager;
        }
        else if (CurrentClick == null)
        {
            CurrentClick = newIconManager;
        }
    }

    public PlayerIconManager GetCurrentIcon()
    {
        return this.CurrentClick;
    }

    public void ViewPlayerCards(PlayerManager player)
    {
        SpaceManager.UpdatePlayerSpaceHand(player);
    }
}

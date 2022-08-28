using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    private Hand CurHand;
    private int Position;
    public bool IsPlaying = false;
    public bool IsAlive = true;

    public Hand GetHand()
    {
        return this.CurHand;
    }

    public bool HasCardType(string type)
    {
        return this.CurHand.ContainsCardType(type);
    }


    public void AddCard(Card card)
    {
        this.CurHand.AddCard(card);
    }

    public void RemoveCard(Card card)
    {
        this.CurHand.RemoveCard(card);
    }

    public int GetPosition()
    {
        return this.Position;
    }

    public void InitializePlayer(Hand curHand, int pos)
    {
        this.CurHand = curHand;
        this.Position = pos;
    }

    public void UpdatePlayerSpaceManager(PlayerSpaceManager spaceManager)
    {
        spaceManager.UpdatePlayerSpaceHand(this);
    }

    public void UpdateButtons(PlayerSpaceManager spaceManager)
    {
        spaceManager.UpdateButtons(this);
    }
}

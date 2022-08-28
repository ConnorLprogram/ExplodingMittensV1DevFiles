using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombCard : Card
{
    public BombCard()
    {
        SetCardType("Bomb");
    }

    public override string GetName()
    {
        return "Bomb";
    }
}

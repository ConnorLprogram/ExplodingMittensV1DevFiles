using System;
using System.Collections.Generic;
using System.Linq;

public class Hand
{
    private List<Card> CurHand = new List<Card>();

    public Hand(List<Card> startHand)
    {
        this.CurHand = startHand;
    }

    public List<Card> GetCards()
    {
        return this.CurHand;
    }


    public void AddCard(Card card)
    {
        this.CurHand.Add(card);
    }

    public void RemoveCard(Card card)
    {
        if (ContainsCard(card))
        {
            this.CurHand.Remove(card);
        }
        else
        {
            throw new MissingCardException($"{card.GetName()} is not in the hand");
        }
    }

    public Card RemoveCardAtIndex(int index)
    {
        if (0 <= index && index < this.CurHand.Count())
        {
            Card stolen = this.CurHand.ElementAt(index);
            this.CurHand.RemoveAt(index);

            return stolen;
        }
        else
        {
            throw new MissingCardException($"{index} is out of range");
        }
    }

    public Card FindOneOfCard(string type)
    {
        Card card = this.CurHand.First(card => card.GetCardType() == type); //Might change to first or default

        return card;
    }

    public bool ContainsCard(Card possibleCard)
    {
        return this.CurHand.Contains(possibleCard);
    }

    public bool ContainsCardType(string name)
    {
        return this.CurHand.Exists(item => item.GetName() == name);
    }

    public Card RetrieveCardAt(int index)
    {
        return this.CurHand[index];
    }

    public class MissingCardException : Exception
    {
        public MissingCardException(string message) : base(message) { }
    }
}

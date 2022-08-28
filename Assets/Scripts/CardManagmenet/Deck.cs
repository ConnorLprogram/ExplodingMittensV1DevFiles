using System;
using System.Collections.Generic;
using System.Linq;

public class Deck
{
    private readonly int NumOfEachActions = 4;
    private readonly List<string> ActionNames = new List<string> { "Attack", "Skip", "Future", "Shuffle", "Favor", "No" };
    private readonly int NumOfEachPair = 4;
    private readonly List<string> PairNames = new List<string> { "Mitten1", "Mitten2", "Mitten3", "Mitten4" };
    private readonly List<string> CardNameToIndex = new List<string> { "Bomb", "Mitten1", "Mitten2", "Mitten3", "Mitten4", "Attack", "Skip", "Future", "Shuffle", "Favor", "No", "Defuse"};
    private readonly int numDefuses = 2;
    private List<Card> CurCards = new List<Card>();
    public int numCards;

    public Deck()
    {
        for (int i = 0; i < this.ActionNames.Count(); i++)
        {
            for (int j = 0; j < NumOfEachActions; j++)
            {
                this.CurCards.Add(new ActionCard(this.ActionNames[i]));
            }
        }

        for (int i = 0; i < PairNames.Count(); i++)
        {
            for (int j = 0; j < NumOfEachPair; j++)
            {
                this.CurCards.Add(new PairCard(this.PairNames[i]));
            }
        }

        for (int i = 0; i < numDefuses; i++)
        {
            this.CurCards.Add(new DefuseCard());
        }

        var randomizer = new System.Random();
        this.CurCards = CurCards.OrderBy(item => randomizer.Next()).ToList();
        this.numCards = this.CurCards.Count;
    }

    public Deck(string newCardString)
    {
        List<string> newCards = newCardString.Split(" ").ToList();
        foreach (string numString in newCards)
        {
            int num = Int32.Parse(numString);

            if (num == 0)
            {
                CurCards.Add(new BombCard());
            }
            else if (num == 1)
            {
                CurCards.Add(new PairCard(CardNameToIndex[num]));
            }
            else if (num == 2)
            {
                CurCards.Add(new PairCard(CardNameToIndex[num]));
            }
            else if (num == 3)
            {
                CurCards.Add(new PairCard(CardNameToIndex[num]));
            }
            else if (num == 4)
            {
                CurCards.Add(new PairCard(CardNameToIndex[num]));
            }
            else if (num == 5)
            {
                CurCards.Add(new ActionCard(CardNameToIndex[num]));
            }
            else if (num == 6)
            {
                CurCards.Add(new ActionCard(CardNameToIndex[num]));
            }
            else if (num == 7)
            {
                CurCards.Add(new ActionCard(CardNameToIndex[num]));
            }
            else if (num == 8)
            {
                CurCards.Add(new ActionCard(CardNameToIndex[num]));
            }
            else if (num == 9)
            {
                CurCards.Add(new ActionCard(CardNameToIndex[num]));
            }
            else if (num == 10)
            {
                CurCards.Add(new ActionCard(CardNameToIndex[num]));
            }
            else if (num == 11)
            {
                CurCards.Add(new DefuseCard());
            }
        }
        this.numCards = this.CurCards.Count;
    }

    public string ConvertDeckToNums()
    {
        List<int> convertedDeck = new List<int>();
        foreach (var card in CurCards)
        {
            convertedDeck.Add(CardNameToIndex.FindIndex(cardIndex => cardIndex == card.GetName()));
        }

        return string.Join(" ", convertedDeck);
    }

    public void AddPlayedCard(int index, Card card)
    {
        this.CurCards.Insert(index, card);
    }

    public void AddBombs(int num = 9)
    {
        for (int i = 0; i < num; i++)
        {
            this.CurCards.Add(new BombCard());
        }
    }

    public Card DrawCard()
    {
        Card drawnCard = this.CurCards[0];
        this.CurCards.Remove(drawnCard);
        this.numCards--;

        return drawnCard;
    }

    public void ShuffleDeck()
    {
        var randomizer = new System.Random();
        this.CurCards = CurCards.OrderBy(item => randomizer.Next()).ToList();
    }

    public List<Card> GetTop3Cards()
    {
        return new List<Card> { this.CurCards[0], this.CurCards[1], this.CurCards[2] };
    }

    public int GetNumCards()
    {
        return this.CurCards.Count;
    }
}

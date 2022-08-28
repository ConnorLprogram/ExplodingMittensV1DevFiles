public class PairCard : Card
{
    private string PairName;
    public PairCard(string name)
    {
        SetCardType("Pair");
        this.PairName = name;
    }

    public override string GetName()
    {
        return this.PairName;
    }

    public bool IsPair(PairCard possiblePair)
    {
        return this.PairName == possiblePair.GetName();
    }

    public bool IsPair(Card card1, Card card2) //Not working as intended
    {
        return card1.GetCardType() == "Pair" && card2.GetCardType() == "Pair" && card1.GetName() == card2.GetName();
    }
}

public class ActionCard : Card
{
    private string ActionName;
    public ActionCard(string name)
    {
        SetCardType("Action");
        this.ActionName = name;
    }

    public override string GetName()
    {
        return this.ActionName;
    }

    public override bool IsSameType(Card card)
    {
        return (card.GetCardType() == "Action" && card.GetName() == this.ActionName);
    }
}

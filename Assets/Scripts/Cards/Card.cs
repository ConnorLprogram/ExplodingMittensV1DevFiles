using System;

public class Card //: MonoBehaviour
{
    private string type = "";

    public string GetCardType()
    {
        return this.type;
    }

    public void SetCardType(string newName)
    {
        this.type = newName;
    }

    public virtual bool IsSameType(Card card)
    {
        return card.GetCardType() == this.type;
    }

    public virtual string GetName()
    {
        return "NONE";
    }
}

public class InvalidCardException : Exception
{
    public InvalidCardException(string message) : base(message) { }
}

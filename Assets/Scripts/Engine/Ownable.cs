using UnityEngine;
using System.Collections;

public abstract class Ownable : MonoBehaviour
{
    protected Player owner;
    public float RadarRange;

    public void Owned(Player newOwner)
    {
        if (this.owner != null)
            this.owner.Lose(this);
        newOwner.Own(this);
        this.owner = newOwner;
    }

    public void Lose()
    {
        this.owner = null;
    }

    public string GetOwnerName()
    {
        if (this.owner == null)
            return "";
        return this.owner.name;
    }

    public Player GetOwner()
    {
        return this.owner;
    }

    abstract public void SetupNewTurn();
}

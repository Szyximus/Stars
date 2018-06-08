using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

// Base class for every object that can be owned by a player, like spaceship or planet
public abstract class Ownable : NetworkBehaviour
{
    [SyncVar]
    public new string name;

    protected Player owner;
    protected GameController gameController;
    protected GameApp gameApp;

    [SyncVar]
    public float RadarRange;

    public void Awake()
    {
        gameController = GameObject.Find("GameController").GetComponent<GameController>();
        gameApp = GameObject.Find("GameApp").GetComponent<GameApp>();
    }

    protected bool CanMakeAction()
    {
        return gameController.GetCurrentPlayer().name.Equals(owner.name);
    }

    public void Owned(Player newOwner)
    {
        if (this.owner != null)
            this.owner.Lose(this);
        newOwner.Own(this);
        this.owner = newOwner;
    }

    public void Lose()
    {
        if (this.owner != null)
            this.owner.Lose(this);
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

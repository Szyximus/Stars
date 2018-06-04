using UnityEngine;
using UnityEngine.UI;
public class ShipMinimapIcon : MonoBehaviour
{
    private GameController gameController;
    private SpriteRenderer sprite;

    public void Start()
    {
        gameController = GameObject.Find("GameController").GetComponent<GameController>();
        sprite = GetComponent<SpriteRenderer>();
    }

    void LateUpdate()
    {
        if (GetComponentInParent<Ownable>().GetOwner() == gameController.GetCurrentPlayer())
        {
            sprite.color = Color.white;

        } else sprite.color = new Color(0.75f, 0, 0);

        if (GetComponentInParent<Ownable>().GetOwner() == null)
        {
            sprite.color = new Color(0.5f, 0.5f, 0.5f);

        }
    }
}
using UnityEngine;

public class AntidoteController : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(GameTags.Player))
        {
            var game = GameObject.FindObjectOfType<GameController>();
            game.PickupAntidote();

            Destroy(gameObject);
        }
    }
}

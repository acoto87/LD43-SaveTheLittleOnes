using UnityEngine;

public class DissapearLastText : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(GameTags.Player))
        {
            var game = GameObject.FindObjectOfType<GameController>();
            game.infoText.enabled = false;
        }
    }
}

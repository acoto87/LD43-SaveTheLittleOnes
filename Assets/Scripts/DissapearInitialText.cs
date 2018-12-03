using UnityEngine;

public class DissapearInitialText : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(GameTags.Player))
        {
            var game = GameObject.FindObjectOfType<GameController>();
            game.infoText.text = string.Empty;
        }
    }
}

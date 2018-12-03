using UnityEngine;

public class ShallowBlockController : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(GameTags.Player))
        {
            Destroy(gameObject);
            AudioManager.Play(GameAudioClip.Powerup);
        }
    }
}

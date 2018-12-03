using UnityEngine;

public class PowerUpController : MonoBehaviour
{
    public float increaseSpeed;
    public float increaseMaxJumpHeight;
    public float increaseMinJumpHeight;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(GameTags.Player))
        {
            var game = GameObject.FindObjectOfType<GameController>();
            game.PickupPowerUp(increaseSpeed, increaseMaxJumpHeight, increaseMinJumpHeight);

            Destroy(gameObject);
        }
    }
}

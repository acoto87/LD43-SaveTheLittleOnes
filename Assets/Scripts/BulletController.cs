using UnityEngine;

public class BulletController : MonoBehaviour
{
    public Vector2 direction = Vector2.up;
    public float speed = 1;
    public float hurtAmount = 0.5f;

    void Update()
    {
        transform.position += (Vector3)direction * speed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(GameTags.Boundary) || 
            collision.CompareTag(GameTags.BoundaryContainer) || 
            collision.CompareTag(GameTags.Cannon))
        {
            return;
        }

        if (collision.CompareTag(GameTags.Player))
        {
            var game = GameObject.FindObjectOfType<GameController>();
            game.Hurt(hurtAmount);
        }

        Destroy(gameObject);
    }
}

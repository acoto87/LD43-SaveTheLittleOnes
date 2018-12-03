using UnityEngine;

public class SpikeController : MonoBehaviour
{
    public float hurtAmount = 0.5f;
    public bool canFall = false;
    public float speed = 5.0f;
    public float maxFallDistance = 3.0f;

    public LayerMask collisionMask;

    private bool _falling;

    private void Update()
    {
        if (_falling)
        {
            transform.position += Vector3.down * speed * Time.deltaTime;
        }
        else if (canFall)
        {
            var hit = Physics2D.Raycast(transform.position + Vector3.down, Vector2.down, maxFallDistance, collisionMask);
            if (hit)
            {
                var collider = hit.collider;
                if (collider.CompareTag(GameTags.Player))
                {
                    _falling = true;
                    AudioManager.Play(GameAudioClip.Falling);
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(GameTags.Boundary) ||
            collision.CompareTag(GameTags.BoundaryContainer))
        {
            return;
        }
        
        if (collision.CompareTag(GameTags.Player))
        {
            var player = collision.GetComponent<PlayerController2D>();
            var game = GameObject.FindObjectOfType<GameController>();
            game.Hurt(hurtAmount);
        }

        if (_falling)
        {
            Destroy(gameObject);
            AudioManager.Play(GameAudioClip.Crash);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag(GameTags.Player))
        {
            var player = collision.GetComponent<PlayerController2D>();
            var game = GameObject.FindObjectOfType<GameController>();
            game.Hurt(hurtAmount);
        }
    }

    void OnDrawGizmos()
    {
        if (canFall)
            Gizmos.DrawLine(transform.position, transform.position + Vector3.down * maxFallDistance);
    }
}

using UnityEngine;

public class CannonController : MonoBehaviour
{
    public Vector2 direction;
    public float delay;
    public BulletController bulletPrefab;
    public Transform bulletsContainer;
    public float bulletsSpeed;

    public BoxCollider2D boundary;

    private float _nextShootTime;

    private void Start()
    {
        _nextShootTime = Time.time + delay;
    }

    private void Update()
    {
        if (Time.time >= _nextShootTime)
        {
            var bullet = GameObject.Instantiate<BulletController>(bulletPrefab);
            bullet.transform.position = transform.position + (Vector3)direction * 0.5f;
            bullet.transform.parent = bulletsContainer;
            bullet.direction = direction;
            bullet.speed = bulletsSpeed;

            _nextShootTime = Time.time + delay;

            var player = GameObject.FindObjectOfType<PlayerController2D>();
            var bounds = boundary.bounds;
            if (bounds.Contains(player.transform.position))
                AudioManager.Play(GameAudioClip.Bullet);
        }
    }
}

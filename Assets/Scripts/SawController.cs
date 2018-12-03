using System;
using UnityEngine;

public class SawController : MonoBehaviour
{
    public float hurtAmount = 0.5f;

    public float speed = 4.0f;
    public float waitTime = 1.0f;
    public Vector2[] localWaypoints;
    [Range(0, 2)]
    public float easeAmount;

    private Vector3 _velocity;
    private Vector3[] _globalWaypoints;
    private int _fromWaypointIndex;
    private float _percentBetweenWaypoints;
    private float _nextMoveTime;

    void Start()
    {
        _globalWaypoints = new Vector3[localWaypoints.Length];
        for (int i = 0; i < localWaypoints.Length; i++)
        {
            _globalWaypoints[i] = (Vector3)localWaypoints[i] + transform.position;
        }
    }

    void Update()
    {
        _velocity = Vector3.zero;
        CalculateSawMovement();
        transform.Translate(_velocity);
    }

    private void CalculateSawMovement()
    {
        if (_globalWaypoints == null || _globalWaypoints.Length == 0)
        {
            return;
        }

        if (Time.time < _nextMoveTime)
        {
            return;
        }

        if (_percentBetweenWaypoints >= 1)
        {
            _fromWaypointIndex++;
            _percentBetweenWaypoints = 0;
        }

        var toWaypointIndex = _fromWaypointIndex + 1;
        if (toWaypointIndex >= _globalWaypoints.Length)
        {
            Array.Reverse(_globalWaypoints);
            _fromWaypointIndex = 0;
            toWaypointIndex = 1;
        }

        var distanceBetweenWaypoints = Vector3.Distance(_globalWaypoints[_fromWaypointIndex], _globalWaypoints[toWaypointIndex]);
        _percentBetweenWaypoints += Time.deltaTime * speed / distanceBetweenWaypoints;
        _percentBetweenWaypoints = Mathf.Clamp01(_percentBetweenWaypoints);
        var easedPercentBetweenWaypoints = Ease(_percentBetweenWaypoints);

        var newPosition = Vector3.Lerp(_globalWaypoints[_fromWaypointIndex], _globalWaypoints[toWaypointIndex], easedPercentBetweenWaypoints);
        _velocity = newPosition - transform.position;

        if (_percentBetweenWaypoints >= 1)
        {
            _nextMoveTime = Time.time + waitTime;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(GameTags.Player))
        {
            var game = GameObject.FindObjectOfType<GameController>();
            game.Hurt(hurtAmount);
        }
    }

    void OnDrawGizmos()
    {
        if (localWaypoints != null)
        {
            Gizmos.color = Color.red;

            var size = 0.3f;
            for (int i = 0; i < localWaypoints.Length; i++)
            {
                var globalWaypointPosition = Application.isPlaying ? _globalWaypoints[i] : ((Vector3)localWaypoints[i] + transform.position);
                Gizmos.DrawLine(globalWaypointPosition - Vector3.up * size, globalWaypointPosition + Vector3.up * size);
                Gizmos.DrawLine(globalWaypointPosition - Vector3.left * size, globalWaypointPosition + Vector3.left * size);
            }
        }
    }

    private float Ease(float x)
    {
        var a = easeAmount + 1;
        return Mathf.Pow(x, a) / (Mathf.Pow(x, a) + Mathf.Pow(1 - x, a));
    }
}

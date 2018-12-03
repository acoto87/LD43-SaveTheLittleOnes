using UnityEngine;

public static class GizmosHelper
{
    public static void GizmosDrawRect(Rect r)
    {
        Gizmos.color = Color.blue;

        var topLeft = new Vector2(r.xMin, r.yMin);
        var topRight = new Vector2(r.xMin + r.width, r.yMin);
        var bottomLeft = new Vector2(r.xMin, r.yMin + r.height);
        var bottomRight = new Vector2(r.xMin + r.width, r.yMin + r.height);

        Gizmos.DrawLine(topLeft, topRight);
        Gizmos.DrawLine(topRight, bottomRight);
        Gizmos.DrawLine(bottomRight, bottomLeft);
        Gizmos.DrawLine(bottomLeft, topLeft);
    }

    public static void GizmosDrawCircle(Vector3 position, float radius)
    {
        Gizmos.color = Color.red;

        var lastPos = GetPositionInCircle(position, 0.0f, radius);
        for (var theta = 0.1f; theta <= Mathf.PI * 2; theta += 0.1f)
        {
            var newPos = GetPositionInCircle(position, theta, radius);
            Gizmos.DrawLine(lastPos, newPos);
            lastPos = newPos;
        }
        Gizmos.DrawLine(lastPos, GetPositionInCircle(position, 0.0f, radius));
    }

    private static Vector3 GetPositionInCircle(Vector3 position, float theta, float radius)
    {
        var x = radius * Mathf.Cos(theta);
        var y = radius * Mathf.Sin(theta);
        return new Vector3(position.x + x, position.y + y, 0);
    }
}
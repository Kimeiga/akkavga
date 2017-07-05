using UnityEngine;
using System.Collections;

public static class CircleCollider2DExtension
{
    public static Vector3 GetWorldCenter(this CircleCollider2D circle)
    {
        return circle.transform.position + new Vector3(circle.offset.x, circle.offset.y, 0);
    }

    public static void SetWorldCenter(this CircleCollider2D circle, Vector3 position)
    {
        circle.transform.position = position - new Vector3(circle.offset.x, circle.offset.y, 0);
    }
}

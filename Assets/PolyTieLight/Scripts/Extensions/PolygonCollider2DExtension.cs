using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public static class PolygonCollider2DExtension 
{
    public static Vector2[] GetWorldPoints(this PolygonCollider2D polygon)
    {
        var scale = new Vector2(polygon.transform.localScale.x, polygon.transform.localScale.y);
        var trans = new Vector2(polygon.transform.position.x, polygon.transform.position.y);
        var rot = polygon.transform.localRotation;

        return polygon.points
            .Select<Vector2, Vector2>(p => trans + (Vector2)(rot * Vector2.Scale(p, scale))).ToArray<Vector2>();
    }

    public static Vector2 GetWorldPoint(this PolygonCollider2D polygon, Vector2 localPoint)
    {
        var scale = new Vector2(polygon.transform.localScale.x, polygon.transform.localScale.y);
        var trans = new Vector2(polygon.transform.position.x, polygon.transform.position.y);
        var rot = polygon.transform.localRotation;

        return trans + (Vector2)(rot * Vector2.Scale(localPoint, scale));
    }

    public static Vector2 GetLocalPoint(this PolygonCollider2D polygon, Vector2 worldPoint)
    {
        var scale = new Vector2(polygon.transform.localScale.x, polygon.transform.localScale.y);
        var trans = new Vector2(-polygon.transform.position.x, -polygon.transform.position.y);
        var rot = Quaternion.Inverse(polygon.transform.localRotation);
        scale = new Vector2(1 / scale.x, 1 / scale.y);
        return Vector2.Scale((Vector2)(rot * (worldPoint + trans)), scale);
    }
}

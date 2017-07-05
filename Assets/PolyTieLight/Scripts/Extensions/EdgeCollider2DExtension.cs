using UnityEngine;
using System.Collections;
using System.Linq;

public static class EdgeCollider2DExtension
{
    public static Vector2[] GetWorldPoints(this EdgeCollider2D edge)
    {
        var scale = new Vector2(edge.transform.localScale.x, edge.transform.localScale.y);
        var trans = new Vector2(edge.transform.position.x, edge.transform.position.y);
        var rot = edge.transform.localRotation;
        return edge.points
            .Select<Vector2, Vector2>(p => trans + (Vector2)(rot * Vector2.Scale(p, scale))).ToArray<Vector2>();
    }

    public static Vector2 GetWorldPoint(this EdgeCollider2D edge, Vector2 localPoint)
    {
        var scale = new Vector2(edge.transform.localScale.x, edge.transform.localScale.y);
        var trans = new Vector2(edge.transform.position.x, edge.transform.position.y);
        var rot = edge.transform.localRotation;

        return trans + (Vector2)(rot * Vector2.Scale(localPoint, scale));
    }

    public static Vector2 GetLocalPoint(this EdgeCollider2D edge, Vector2 worldPoint)
    {
        var scale = new Vector2(edge.transform.localScale.x, edge.transform.localScale.y);
        var trans = new Vector2(-edge.transform.position.x, -edge.transform.position.y);
        var rot = Quaternion.Inverse(edge.transform.localRotation);
        scale = new Vector2(1 / scale.x, 1 / scale.y);
        return Vector2.Scale((Vector2)(rot * (worldPoint + trans)), scale);
    }
}

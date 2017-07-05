using UnityEngine;
using System.Collections;
using System.Linq;

public class VectorPath2D : MonoBehaviour 
{
    public Vector2[] points;

    public Vector2[] GetWorldPoints()
    {
        if (points == null)
            return null;

        var scale = new Vector2(transform.localScale.x, transform.localScale.y);
        var trans = new Vector2(transform.position.x, transform.position.y);
        var rot = transform.localRotation;
        return points
            .Select<Vector2, Vector2>(p => trans + (Vector2)(rot * Vector2.Scale(p, scale))).ToArray<Vector2>();
    }

    public Vector2 GetWorldPoint(Vector2 localPoint)
    {
        var scale = new Vector2(transform.localScale.x, transform.localScale.y);
        var trans = new Vector2(transform.position.x, transform.position.y);
        var rot = transform.localRotation;

        return trans + (Vector2)(rot * Vector2.Scale(localPoint, scale));
    }

    public Vector2 GetLocalPoint(Vector2 worldPoint)
    {
        var scale = new Vector2(transform.localScale.x, transform.localScale.y);
        var trans = new Vector2(-transform.position.x, -transform.position.y);
        var rot = Quaternion.Inverse(transform.localRotation);
        scale = new Vector2(1 / scale.x, 1 / scale.y);
        return Vector2.Scale((Vector2)(rot * (worldPoint + trans)), scale);
    }
}

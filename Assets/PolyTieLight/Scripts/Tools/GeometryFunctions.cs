using UnityEngine;
using System.Collections;

/// <summary>
/// Class containing various geometry functions.
/// </summary>
public static class GeometryFunctions
{
    /// <summary>
    /// Calculates the distance from a point to the given line
    /// segment. The line segment it defined by two R2 vectors.
    /// It returns the squared distance from point to line segment.
    /// </summary>
    /// <param name="A">First point defining the line segment</param>
    /// <param name="B">Second point defining the line segment</param>
    /// <param name="P">Point from where calculate the distance to line segment</param>
    /// <returns>Squared distance from point to line segment</returns>
    public static float DistnacePointSegmentSquared(Vector2 A, Vector2 B, Vector2 P)
    {
        float d = (B - A).sqrMagnitude;
        if (d == 0.0f)  // Case A == B -> Line segment is a point.
            return (B - P).sqrMagnitude;

        float t = Vector2.Dot((P - A), (B - A)) / d;
        if (t < 0.0f)   // Case point is beyond segment point A
            return (A - P).sqrMagnitude;
        if (t > 1.0f)   // Case point is beyond segment point B
            return (B - P).sqrMagnitude;

        // Projection falls onto segment -> calculate projection vector.
        Vector2 ps = A + t * (B - A);
        return (ps - P).sqrMagnitude;
    }

    /// <summary>
    /// Calculates the distance from a point to the given line
    /// segment. The line segment it defined by two R2 vectors.
    /// It returns the Distance from point to line segment.
    /// </summary>
    /// <param name="A">First point defining the line segment</param>
    /// <param name="B">Second point defining the line segment</param>
    /// <param name="P">Point from where calculate the distance to line segment</param>
    /// <returns>Distance from point to line segment</returns>
    public static float DistnacePointSegment(Vector2 A, Vector2 B, Vector2 P)
    {
        return Mathf.Sqrt(DistnacePointSegmentSquared(A, B, P));
    }
}

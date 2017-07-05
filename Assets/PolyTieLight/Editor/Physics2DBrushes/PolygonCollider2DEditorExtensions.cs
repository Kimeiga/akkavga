using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Linq;

/// <summary>
/// Static class containing extension methods for the Unity intern PolygonCollider2D class that
/// are used within the Editor. The methods handle vertices manipulation.
/// </summary>
public static class PolygonCollider2DEditorExtensions
{
    /// <summary>
    /// Insert new new vertex into the polygon at the given position.
    /// The vertex is inserted after the closest vertex to this position.
    /// </summary>
    /// <param name="position">Position of the inserted vertex</param>
    /// <param name="forceLast">Vertex will be always inserted after the last vertex of the polygon</param>
    /// <returns>Retruns true if insertion was successful</returns>
    public static bool InsertVertex(this PolygonCollider2D polygon, Vector2 position, bool forceLast = false)
    {
        Undo.RecordObject(polygon, string.Format("Insert vertex in {0}", polygon.name));

        var worldPos = new Vector2(polygon.transform.position.x, polygon.transform.position.y);
        position = position - worldPos;
        var vertices = polygon.points;
        // Handle easy case first.
        if (vertices == null || vertices.Length == 0)
        {
            vertices = new Vector2[1];
            vertices[0] = position;
            polygon.points = vertices;
            return true;
        }

        Vector2[] newVertices = new Vector2[vertices.Length + 1];

        int insertIdx = -1;
        if (forceLast == true || vertices.Length < 2)
        {
            insertIdx = vertices.Length;
        }
        else
        {
            // Find closest line segment.
            float minDist = float.PositiveInfinity;
            for (int i = 0; i < vertices.Length; i++)
            {
                int ii = (i + 1) % vertices.Length;
                float dist = GeometryFunctions.DistnacePointSegmentSquared(vertices[i], vertices[ii], position);
                if (dist < minDist)
                {
                    minDist = dist;
                    insertIdx = ii;
                }
            }
        }

        // Insert new vertex.
        if (insertIdx == -1 || insertIdx >= newVertices.Length)
            return false;

        System.Array.Copy(vertices, 0, newVertices, 0, insertIdx);
        System.Array.Copy(vertices, insertIdx, newVertices, insertIdx + 1, (vertices.Length - insertIdx));
        newVertices[insertIdx] = position;
        vertices = newVertices;

        polygon.points = vertices;
        UpdateOrder(polygon);

        return true;
    }

    /// <summary>
    /// Deletes the vertex with the specified id within the given polygon. 
    /// The id corresponds to the array position within the point array of the polygon.
    /// </summary>
    /// <param name="vertexId">ID of the vertex that should be delted</param>
    /// <returns>Retruns true if polygon is still valid</returns>
    public static bool DeleteVertex(this PolygonCollider2D polygon, int vertexId)
    {
        Undo.RecordObjects(new Object[] { polygon, polygon.transform }, string.Format("Delete vertex in {0}", polygon.name));

        // Delete polygon if only one vertex left.
        if (polygon.points.Length <= 3)
        {
            return false;
        }

        var tmp = new Vector2[polygon.points.Length - 1];
        System.Array.Copy(polygon.points, 0, tmp, 0, vertexId);
        System.Array.Copy(polygon.points, vertexId + 1, tmp, vertexId, tmp.Length - vertexId);

        polygon.points = tmp;

        polygon.UpdateCenter();
        return true;
    }

    /// <summary>
    /// Tries to select a vertex at the given position. Takes 
    /// specified selection radius into account. Returns the
    /// index of the vertex within the given polygon. Returns
    /// -1 if no vertex could be selected.
    /// </summary>
    /// <param name="position">The position at which the vertex should be selected</param>
    /// <returns>Returns the index of the selected vertex within the polygon. (-1 if selection failed)</returns>
    public static int TrySelectVertex(this PolygonCollider2D polygon, Vector2 position)
    {
        var vertices = polygon.GetWorldPoints();
        for (int i = 0; i < vertices.Length; i++)
        {
            var vertex = vertices[i];
            if ((vertex - position).magnitude <= BrushSettingsWindow.VertexSize)
            {
                return i;
            }
        }
        return -1;
    }

    public static void UpdateVertexPosition(this PolygonCollider2D polygon, Vector3 newPosition, int selectedVertex)
    {
        Undo.RecordObject(polygon, string.Format("Move vertex {0} in {1}", selectedVertex, polygon.name));
        Vector2[] vertices = polygon.points;
        vertices[selectedVertex] = polygon.GetLocalPoint(newPosition);
        polygon.points = vertices;
    }

    /// <summary>
    /// Updates the center of the polygon so it's the geometric center
    /// of all the vertices of the polygon.
    /// </summary>
    public static void UpdateCenter(this PolygonCollider2D polygon)
    {
        float signedArea = 0.0f;
        Vector2 centroid = Vector2.zero;
        for (int i = 0; i < polygon.points.Length; i++)
        {
            int ii = (i + 1) % polygon.points.Length;
            float x0 = polygon.points[i].x;
            float y0 = polygon.points[i].y;
            float x1 = polygon.points[ii].x;
            float y1 = polygon.points[ii].y;

            float a = x0 * y1 - x1 * y0;
            signedArea += a;
            centroid.x += (x0 + x1) * a;
            centroid.y += (y0 + y1) * a;
        }

        signedArea *= 0.5f;
        centroid.x /= (6.0f * signedArea);
        centroid.y /= (6.0f * signedArea);

        bool reverseVertices = Mathf.Sign(signedArea) != -1;

        Vector2[] updatedVertices = new Vector2[polygon.points.Length];
        for (int i = 0; i < polygon.points.Length; i++)
        {
            if (reverseVertices == true)
                updatedVertices[i] = polygon.points[polygon.points.Length - 1 - i] - centroid;
            else
                updatedVertices[i] = polygon.points[i] - centroid;
        }
        polygon.transform.position += new Vector3(centroid.x, centroid.y, 0.0f);
        polygon.points = updatedVertices;
    }

    public static void UpdateOrder(this PolygonCollider2D polygon)
    {
        float signedArea = 0.0f; 
        for (int i = 0; i < polygon.points.Length; i++)
        {
            int ii = (i + 1) % polygon.points.Length;
            float x0 = polygon.points[i].x;
            float y0 = polygon.points[i].y;
            float x1 = polygon.points[ii].x;
            float y1 = polygon.points[ii].y;

            float a = x0 * y1 - x1 * y0;
            signedArea += a;
        }
        signedArea *= 0.5f;
        bool reverseVertices = Mathf.Sign(signedArea) != -1;
        if (reverseVertices == true)
        {
            Vector2[] updatedVertices = new Vector2[polygon.points.Length];
            for (int i = 0; i < polygon.points.Length; i++)
            {
                updatedVertices[i] = polygon.points[polygon.points.Length - 1 - i];
            }
            polygon.points = updatedVertices;
        }
    }
}

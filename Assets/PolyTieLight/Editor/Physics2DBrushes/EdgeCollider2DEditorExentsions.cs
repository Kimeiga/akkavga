using UnityEngine;
using UnityEditor;
using System.Collections;

public static class EdgeCollider2DEditorExentsions 
{
    /// <summary>
    /// Updates the center of the edge so it's the mean position of all points
    /// </summary>
    public static void UpdateCenter(this EdgeCollider2D edge)
    {
        Vector2 meanPosition = Vector2.zero;

        for (int i = 0; i < edge.points.Length; i++)
        {
            meanPosition += edge.points[i];
        }
        meanPosition = meanPosition / edge.points.Length;

        Vector2[] updatedVertices = new Vector2[edge.points.Length];
        for (int i = 0; i < edge.points.Length; i++)
        {
            updatedVertices[i] = edge.points[i] - meanPosition;
        }
        edge.transform.position += new Vector3(meanPosition.x, meanPosition.y, 0);
        edge.points = updatedVertices;
    }

    /// <summary>
    /// Inserts a new vertex into the edge.
    /// </summary>
    /// <param name="position">The position in world cooridnates where the new vertex should be inserted</param>
    /// <param name="forceLast">Forces the function to insert the vertex at the end of the point array</param>
    /// <returns>Retruns true if insertion was successful</returns>
    public static bool InsertVertex(this EdgeCollider2D edge, Vector2 position, bool forceLast = false)
    {
        Undo.RecordObject(edge, string.Format("Insert vertex in {0}", edge.name));

        var worldPos = new Vector2(edge.transform.position.x, edge.transform.position.y);
        position = position - worldPos;
        var vertices = edge.points;

        // Handle easy case first.
        if (vertices == null || vertices.Length == 0)
        {
            vertices = new Vector2[1];
            vertices[0] = position;
            edge.points = vertices;
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

        edge.points = vertices;

        return true;
    }

    /// <summary>
    /// Deletes the vertex with the specified id within the given edge. 
    /// The id corresponds to the array position within the point array of the edge.
    /// </summary>
    /// <param name="vertexId">ID of the vertex that should be deleted</param>
    /// <returns>Retruns true if path is still valid</returns>
    public static bool DeleteVertex(this EdgeCollider2D edge, int vertexId)
    {
        Undo.RecordObjects(new Object[] { edge, edge.transform }, string.Format("Delete vertex in {0}", edge.name));

        // Delete polygon if only one vertex left.
        if (edge.points.Length <= 2)
        {
            return false;
        }

        var tmp = new Vector2[edge.points.Length - 1];
        System.Array.Copy(edge.points, 0, tmp, 0, vertexId);
        System.Array.Copy(edge.points, vertexId + 1, tmp, vertexId, tmp.Length - vertexId);

        edge.points = tmp;

        edge.UpdateCenter();
        return true;
    }

    /// <summary>
    /// Tries to select a vertex at the given position. Takes 
    /// specified selection radius into account. Returns the
    /// index of the vertex within the given path. Returns
    /// -1 if no vertex could be selected.
    /// </summary>
    /// <param name="position">The position at which the vertex should be selected</param>
    /// <returns>Returns the index of the selected vertex within the edge. (-1 if selection failed)</returns>
    public static int TrySelectVertex(this EdgeCollider2D edge, Vector2 position)
    {
        var vertices = edge.GetWorldPoints();
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

    public static void UpdateVertexPosition(this EdgeCollider2D edge, Vector3 newPosition, int selectedVertex)
    {
        Undo.RecordObject(edge, string.Format("Move vertex {0} in {1}", selectedVertex, edge.name));
        Vector2[] vertices = edge.points;
        vertices[selectedVertex] = edge.GetLocalPoint(newPosition);
        edge.points = vertices;
    }
}

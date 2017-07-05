using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Linq;

/// <summary>
/// Static class containing extension methods for the Unity intern BoxCollider2D class that
/// are used within the Editor. The methods handle manipulation of the collider shape.
/// </summary>
public static class BoxCollider2DEditorExtensions  
{
    /// <summary>
    /// Tries to select a corner at the given positon. Takes specified selection radius into
    /// account. Returns the index of the corner of the given box. Returns -1 if no corner 
    /// could be selected
    /// </summary>
    /// <param name="position">The position at which the corner should be selected</param>
    /// <returns>Returns the index of the selected corner within the box. (-1 if selection failed)</returns>
    public static int TrySelectCorner(this BoxCollider2D box, Vector2 position)
    {
        var corners = box.GetWorldCorners();
        for (int i = 0; i < corners.Length; i++)
        {
            var corner = corners[i];
            if ((corner - position).magnitude <= BrushSettingsWindow.VertexSize)
                return i;
        }

        return -1;
    }

    /// <summary>
    /// Tries to select the edge at the given positon. Takes specified selection radius into
    /// account. Returns the index of the edge of the given box. Returns -1 if no edge 
    /// could be selected
    /// </summary>
    /// <param name="position">The position at which the edge should be selected</param>
    /// <returns>Returns the index of the selected edge within the box. (-1 if selection failed)</returns>
    public static int TrySelectEdge(this BoxCollider2D box, Vector2 position)
    {
        var corners = box.GetWorldCorners();
        for (int i = 0; i < corners.Length; i++)
        {
            int ii = (i + 1) % corners.Length;

            float dist = GeometryFunctions.DistnacePointSegment(corners[i], corners[ii], position);
            if (dist <= BrushSettingsWindow.VertexSize)
                return i;
        }
        return -1;
    }
}

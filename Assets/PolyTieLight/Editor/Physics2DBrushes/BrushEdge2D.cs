using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(EdgeCollider2D))]
public class BrushEdge2D : Editor
{
    private static Dictionary<EdgeCollider2D, ColorStates> _assignedColors = new Dictionary<EdgeCollider2D, ColorStates>();
    private static Vector2 _selectedVertexStartPosition;    // Necessary for axis aligned movement.
    private static KeyCode _lastKey;
    private static bool _isDrawing;
    private static bool _isPressingKey;
    private static int _selectedVertex = -1;

    public bool isManipulatingEdge { get; private set; }

    private int _currentCreateVertex = 0;

    [MenuItem("GameObject/Create 2D Objects/Edges/Edge &e")]
    public static void CreateEdge()
    {
        var go = new GameObject("Edge2D");
        var collider = go.AddComponent<EdgeCollider2D>();
        collider.points = new Vector2[] { Vector2.zero, Vector2.zero };
        _assignedColors.Add(collider, ColorStates.IDLE);

        activateDrawing(go);
    }

    [DrawGizmo(GizmoType.InSelectionHierarchy | GizmoType.NotInSelectionHierarchy | GizmoType.Pickable)]
    static void DrawEdge2D(EdgeCollider2D edge, GizmoType gizmoType)
    {
        BrushSettingsWindow.Initialize();
        var lineColor = ColorPalletReader.GetLineColor(ColorStates.SELECTED);
        if (gizmoType == GizmoType.NotInSelectionHierarchy)
        {
            ColorStates state = ColorStates.IDLE;
            if (_assignedColors.TryGetValue(edge, out state))
            {
                lineColor = ColorPalletReader.GetLineColor(state);
            }
            else
            {
                state = Utilities.DetermineColorState(edge);
                lineColor = ColorPalletReader.GetLineColor(state);
                _assignedColors.Add(edge, state);    // Cache it because GetComponent is slow.
            }
        }

        drawEdge(edge, lineColor);
    }

    private static void drawEdge(EdgeCollider2D edge, Color color)
    {
        Vector2[] vertices = edge.GetWorldPoints();
        if (vertices == null)
            return;

        for (int i = 0; i < vertices.Length; i++)
        {
            int ii = (i + 1) % vertices.Length;
            // Draw edge
            if (ii != 0)
            {
                Gizmos.color = color;
                Gizmos.DrawLine(vertices[i], vertices[ii]);
            }

            // Draw vertex
            if (Selection.activeGameObject == edge.gameObject)
            {
                Gizmos.color = ColorPalletReader.GetVertexColor(ColorStates.PATH);
                if (i == _selectedVertex)
                    Gizmos.color = ColorPalletReader.GetVertexColor(ColorStates.SELECTED);
                Gizmos.DrawSphere(vertices[i], BrushSettingsWindow.VertexSize);
                if (BrushSettingsWindow.ShowVertexInfo == true)
                    Handles.Label(vertices[i] + new Vector2(0.1f, 0.4f), edge.GetLocalPoint(vertices[i]).ToString());
            }
        }
    }


    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        // Update center button.
        if (GUILayout.Button("Update Center") == true)
        {
            var edge = target as EdgeCollider2D;
            if (edge != null)
                edge.UpdateCenter();
        }
    }

    void OnSceneGUI()
    {
        var prefabType = PrefabUtility.GetPrefabType(target);
        if (prefabType == PrefabType.Prefab || prefabType == PrefabType.ModelPrefab)
            return;

        var edge = target as EdgeCollider2D;

        if (edge == null)
            return;
        int controlId = GUIUtility.GetControlID(FocusType.Passive);

        if (_isDrawing == true)
        {
            isManipulatingEdge = true;
            if (Event.current.type == EventType.MouseUp)
            {
                if (Event.current.button == 0)  // Left mouse button adds new vertex to the polygon.
                {
                    Utilities.PerformActionAtMouseWorldPosition((p) =>
                    {
                        if (BrushSettingsWindow.SnapToGrid == true)
                        {
                            p = Utilities.SnapToGrid(p, BrushSettingsWindow.GridSize);
                        }
                        if (_currentCreateVertex <= 1)  // Update first two vertices of edge - they are already created so no insert.
                        {
                            edge.UpdateVertexPosition(p, _currentCreateVertex);
                        }
                        else
                        {
                            edge.InsertVertex(p, true);
                        }
                        _currentCreateVertex++;
                        EditorUtility.SetDirty(target);
                    });
                }
                else if (Event.current.button == 1)  // Right mouse button stops drawing and closes the polygon.
                {
                    _isDrawing = false;
                    isManipulatingEdge = false;
                    edge.UpdateCenter();
                    _currentCreateVertex = 0;
                }
            }
        }

        if (Selection.activeGameObject == edge.gameObject)
        {
            if (Event.current.type == EventType.MouseDown && Event.current.clickCount > 1)
            {
                Utilities.PerformActionAtMouseWorldPosition((p) =>
                {
                    edge.InsertVertex(p);
                    EditorUtility.SetDirty(target);
                });
            }
            if (Event.current.type == EventType.MouseMove && _isDrawing == false && Event.current.type != EventType.MouseDrag)
            {
                // Select vertex
                Utilities.PerformActionAtMouseWorldPosition(p => _selectedVertex = edge.TrySelectVertex(p));
                if (_selectedVertex >= 0)
                {
                    _selectedVertexStartPosition = edge.GetWorldPoint(edge.points[_selectedVertex]);
                    HandleUtility.Repaint();
                }
            }
        }

        if (_selectedVertex != -1 && Event.current.button == 1)
        {
            var deleteOption = new GUIContent();
            deleteOption.text = "Delete Vertex";
            EditorUtility.DisplayCustomMenu(new Rect(Event.current.mousePosition.x, Event.current.mousePosition.y, 0, 0), new GUIContent[] { deleteOption }, -1, (userData, options, selected) =>
            {
                switch (selected)
                {
                    case 0:
                        if (edge.DeleteVertex(_selectedVertex) == false)
                            Undo.DestroyObjectImmediate(edge.gameObject);
                        break;
                    default:
                        break;
                }
            }, null);
        }

        if (Event.current.type == EventType.keyDown && _isPressingKey == false)
        {
            _lastKey = Event.current.keyCode;
            _isPressingKey = true;
        }
        if (Event.current.type == EventType.keyUp)
        {
            _lastKey = KeyCode.None;
            _isPressingKey = false;
        }

        // Manipulate polygon vertices
        if (Event.current.type == EventType.MouseDrag && _selectedVertex != -1 && Event.current.button == 0)
        {
            // Update position of the selected vertex
            Utilities.PerformActionAtMouseWorldPosition((p) =>
            {
                if (BrushSettingsWindow.SnapToGrid == true)
                {
                    p = Utilities.SnapToGrid(p, BrushSettingsWindow.GridSize);
                }
                else if (_lastKey == KeyCode.V)     // Snap to vertex.
                {
                    p = Utilities.SnapToVertex(edge, p, 1.0f);
                }
                else if (Event.current.modifiers == EventModifiers.Shift)    // Align with axis when holding shift.
                {
                    p = Utilities.AlignWithAxis(p, _selectedVertexStartPosition);
                }
                else if (Event.current.modifiers == EventModifiers.Control)
                {
                    float xMove = EditorPrefs.GetFloat("MoveSnapX");
                    float yMove = EditorPrefs.GetFloat("MoveSnapY");
                    p = Utilities.SnapToValues(p, _selectedVertexStartPosition, xMove, yMove);
                }
                edge.UpdateVertexPosition(p, _selectedVertex);
                HandleUtility.Repaint();
            });
        }

        if (Event.current.type == EventType.MouseUp && _selectedVertex != -1)
        {
            _selectedVertex = -1;
        }

        if (_selectedVertex != -1)
        {
            DefaultHandles.Hidden = true;
        }
        else
        {
            DefaultHandles.Hidden = false;
        }

        if ((Event.current.type == EventType.Layout && _isDrawing == true) || _selectedVertex != -1)
        {
            HandleUtility.AddDefaultControl(controlId);
        }
    }

    private static void activateDrawing(GameObject go)
    {
        Selection.activeGameObject = go;
        _isDrawing = true;
    }
}

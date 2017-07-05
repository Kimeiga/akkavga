using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(BoxCollider2D))]
public class BrushRectangle2D : Editor 
{
    private static Dictionary<BoxCollider2D, ColorStates> _assignedColors = new Dictionary<BoxCollider2D, ColorStates>();
    private static Vector3 _startingCorner;
    private static Vector2 _selectStartPosition;
    private static KeyCode _lastKey;
    private static bool _isDrawing;
    private static bool _isPressingKey;
    private static int _selectedCorner = -1;
    private static int _selectedEdge = -1;

    public bool isManipulatingRectangle { get; private set; }

    [MenuItem("GameObject/Create 2D Objects/Rectangles/Rectangle &r")]
    public static void CreateRectangle()
    {
        var go = new GameObject("Rectangle2D");
        Undo.RegisterCreatedObjectUndo(go, "Created Rectangle2D");
        var collider = go.AddComponent<BoxCollider2D>();
        collider.size = Vector2.zero;
        _assignedColors.Add(collider, ColorStates.IDLE);

        activateDrawing(go);
    }

    [DrawGizmo(GizmoType.InSelectionHierarchy | GizmoType.NotInSelectionHierarchy | GizmoType.Pickable)]
    static void DrawRectangleColliders2D(BoxCollider2D box, GizmoType gizmoType)
    {
        BrushSettingsWindow.Initialize();

        drawRectangle(box);
    }

    private static void drawRectangle(BoxCollider2D box)
    {
        var corners = box.GetWorldCorners();
        for (int i = 0; i < corners.Length; i++)
        {
            int ii = (i + 1) % corners.Length;

            ColorStates state = ColorStates.IDLE;
            Color lineColor = ColorPalletReader.GetLineColor(state);
            Color vertexColor = ColorPalletReader.GetLineColor(state);
            if (_assignedColors.TryGetValue(box, out state))
            {
                lineColor = ColorPalletReader.GetLineColor(state);
                vertexColor = ColorPalletReader.GetVertexColor(state);
            }
            else
            {
                state = Utilities.DetermineColorState(box);
                lineColor = ColorPalletReader.GetLineColor(state);
                vertexColor = ColorPalletReader.GetVertexColor(state);
                _assignedColors.Add(box, state);
            }
            if (i == _selectedEdge && Selection.activeGameObject == box.gameObject)
                lineColor = ColorPalletReader.GetLineColor(ColorStates.SELECTED);

            // Draw edges
            Gizmos.color = lineColor;
            Gizmos.DrawLine(corners[i], corners[ii]);

            // Draw corners
            if (Selection.activeGameObject == box.gameObject)
            {
                if (i == _selectedCorner)
                    vertexColor = ColorPalletReader.GetVertexColor(ColorStates.SELECTED);

                Gizmos.color = vertexColor;
                Gizmos.DrawSphere(corners[i], BrushSettingsWindow.VertexSize);
                if (BrushSettingsWindow.ShowVertexInfo == true)
                    Handles.Label(corners[i] + new Vector2(0.1f, 0.4f), (corners[i] - new Vector2(box.transform.position.x, box.transform.position.y)).ToString());
            }
        }
    }

    void OnSceneGUI()
    {
        var prefabType = PrefabUtility.GetPrefabType(target);
        if (prefabType == PrefabType.Prefab || prefabType == PrefabType.ModelPrefab)
            return;

        var box = target as BoxCollider2D;

        if (box == null)
            return;

        int controlId = GUIUtility.GetControlID(FocusType.Passive);

        if (_isDrawing == true)
        {
            isManipulatingRectangle = true;
            if (Event.current.button == 0)
            {
                if (Event.current.type == EventType.MouseDown)
                {
                    // Press left mouse button to determine box corner.
                    Utilities.PerformActionAtMouseWorldPosition((p) => 
                    {
                        if (BrushSettingsWindow.SnapToGrid == true)
                        {
                            p = Utilities.SnapToGrid(p, BrushSettingsWindow.GridSize);
                        }
                        _startingCorner = p;
                    });
                }
                else if (Event.current.type == EventType.MouseDrag)
                {
                    Utilities.PerformActionAtMouseWorldPosition((p) =>
                    {
                        if (BrushSettingsWindow.SnapToGrid == true)
                        {
                            p = Utilities.SnapToGrid(p, BrushSettingsWindow.GridSize);
                        }
                        var size = p - _startingCorner;
                        box.SetWorldCenter(_startingCorner + size * 0.5f);
                        box.size = new Vector2(Mathf.Abs(size.x), Mathf.Abs(size.y));
                    });
                }
                else if (Event.current.type == EventType.MouseUp)
                {
                    // Close off circle when mouse button is released.
                    _isDrawing = false;
                    isManipulatingRectangle = false;
                    EditorUtility.SetDirty(target);
                }
            }
        }
        else if (Selection.activeGameObject == box.gameObject)
        {
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

            if (Event.current.type == EventType.MouseMove)
            {
                Utilities.PerformActionAtMouseWorldPosition((p) => 
                {
                    _selectedCorner = box.TrySelectCorner(p);
                    _selectedEdge = box.TrySelectEdge(p);
                });
                if (_selectedCorner != -1)
                {
                    _selectStartPosition = box.GetWorldCorners()[_selectedCorner];
                    _selectedEdge = -1;
                    HandleUtility.Repaint();
                }
                else if (_selectedEdge != -1)
                {
                    _selectStartPosition = box.GetWorldCorners()[_selectedEdge];
                    _selectedCorner = -1;
                    HandleUtility.Repaint();
                }
            }
            if (Event.current.button == 0)
            {
                if ((_selectedCorner != -1 || _selectedEdge != -1) && Event.current.type == EventType.MouseDown)
                {
                    isManipulatingRectangle = true;
                    if (_selectedCorner != -1)
                    {
                        _startingCorner = box.GetWorldCorners()[((_selectedCorner + 2) % 4)];
                    }
                    else if (_selectedEdge != -1)
                    {
                        _startingCorner = box.GetWorldCorners()[((_selectedEdge + 2) % 4)];
                    }
                }
                else if (Event.current.type == EventType.MouseDrag && (_selectedCorner != -1 || _selectedEdge != -1))
                {
                    Utilities.PerformActionAtMouseWorldPosition((p) =>
                    {
                        if (BrushSettingsWindow.SnapToGrid == true)
                        {
                            p = Utilities.SnapToGrid(p, BrushSettingsWindow.GridSize);
                        }
                        else if (_lastKey == KeyCode.V)     // Snap to vertex.
                        {
                            p = Utilities.SnapToVertex(box, p, 1.0f);
                        }
                        else if (Event.current.modifiers == EventModifiers.Shift)    // Align with axis when holding shift.
                        {
                            p = Utilities.AlignWithAxis(p, _selectStartPosition);
                        }
                        else if (Event.current.modifiers == EventModifiers.Control)
                        {
                            float xMove = EditorPrefs.GetFloat("MoveSnapX");
                            float yMove = EditorPrefs.GetFloat("MoveSnapY");

                            p = Utilities.SnapToValues(p, _selectStartPosition, xMove, yMove);
                        }
                        if (_selectedEdge != -1)    // Align with corresponding axis when edge is selected
                        {
                            if (_selectedEdge == 0 || _selectedEdge == 2) // Top or bottom edge
                            {
                                p.x = _selectStartPosition.x;
                            }
                            else
                            {
                                p.y = _selectStartPosition.y;
                            }
                        }

                        var size = p - _startingCorner;
                        Undo.RecordObjects(new Object[] { box, box.transform }, string.Format("Change size of {0}", box.name));
                        box.SetWorldCenter(_startingCorner + size * 0.5f);
                        box.size = new Vector2(Mathf.Abs(size.x), Mathf.Abs(size.y));
                        EditorUtility.SetDirty(target);
                    });
                }
            }
        }

        DefaultHandles.Hidden = (_selectedCorner != -1) || (_selectedEdge != -1);

        if ((Event.current.type == EventType.Layout && _isDrawing == true) || _selectedCorner != -1 || _selectedEdge != -1)
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

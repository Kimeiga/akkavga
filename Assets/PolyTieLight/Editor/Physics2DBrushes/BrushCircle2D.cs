using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(CircleCollider2D))]
public class BrushCircle2D : Editor
{
    private static Dictionary<CircleCollider2D, ColorStates> _assignedColors = new Dictionary<CircleCollider2D, ColorStates>();
    private static Vector2 _selectStartPosition;
    private static bool _isDrawing;
    private static bool _isAdjusting;

    public bool isManipulatingCircle { get; private set; }

    [MenuItem("GameObject/Create 2D Objects/Circles/Circle &c")]
    public static void CreateCircle()
    {
        var go = new GameObject("Circle2D");
        Undo.RegisterCreatedObjectUndo(go, "Created Circle2D");
        var collider = go.AddComponent<CircleCollider2D>();
        collider.radius = 0f;
        _assignedColors.Add(collider, ColorStates.IDLE);

        activateDrawing(go);
    }

    [DrawGizmo(GizmoType.InSelectionHierarchy | GizmoType.NotInSelectionHierarchy | GizmoType.Pickable)]
    static void DrawCircleColliders2D(CircleCollider2D circle, GizmoType gizmoType)
    {
        BrushSettingsWindow.Initialize();
        drawCircle(circle);
    }

    private static void drawCircle(CircleCollider2D circle)
    {
        var lineColor = ColorPalletReader.GetLineColor(ColorStates.IDLE);
        ColorStates state = ColorStates.IDLE;
        if (_assignedColors.TryGetValue(circle, out state))
        {
            lineColor = ColorPalletReader.GetLineColor(state);
        }
        else
        {
            state = Utilities.DetermineColorState(circle);
            lineColor = ColorPalletReader.GetLineColor(state);
            _assignedColors.Add(circle, state);
        }

        if (circle.gameObject == Selection.activeGameObject && _isAdjusting)
            lineColor = ColorPalletReader.GetLineColor(ColorStates.SELECTED);

        Gizmos.color = lineColor;
        Gizmos.DrawWireSphere(circle.GetWorldCenter(), circle.radius);
    }

    void OnSceneGUI()
    {
        var prefabType = PrefabUtility.GetPrefabType(target);
        if (prefabType == PrefabType.Prefab || prefabType == PrefabType.ModelPrefab)
            return;

        var circle = target as CircleCollider2D;

        if (circle == null)
            return;

        int controlId = GUIUtility.GetControlID(FocusType.Passive);

        if (_isDrawing == true)
        {
            isManipulatingCircle = true;
            if (Event.current.button == 0)
            {
                if (Event.current.type == EventType.MouseDown)
                {
                    // Press left mouse button to determine circle center.
                    Utilities.PerformActionAtMouseWorldPosition((p) =>
                    {
                        if (BrushSettingsWindow.SnapToGrid == true)
                        {
                            p = Utilities.SnapToGrid(p, BrushSettingsWindow.GridSize);
                        }
                        circle.SetWorldCenter(p);
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
                        var radius = (circle.GetWorldCenter() - p).magnitude;
                        circle.radius = radius;
                    });
                }
                else if (Event.current.type == EventType.MouseUp)
                {
                    // Close off circle when mouse button is released.
                    _isDrawing = false;
                    isManipulatingCircle = false;
                    EditorUtility.SetDirty(target);
                }
            }
        }
        else if (Selection.activeGameObject == circle.gameObject)
        {
            if (Event.current.type == EventType.MouseMove)
            {
                Utilities.PerformActionAtMouseWorldPosition((p) =>
                {
                    var radius = (circle.GetWorldCenter() - p).magnitude;
                    var diff = Mathf.Abs(radius - circle.radius);
                    float selectionThreshold = Mathf.Min(circle.radius * 0.2f, BrushSettingsWindow.VertexSize);

                    if (diff <= selectionThreshold)
                    {
                        _isAdjusting = true;
                        _selectStartPosition = p;
                        HandleUtility.Repaint();
                    }
                    else
                    {
                        _isAdjusting = false;
                        HandleUtility.Repaint();
                    }
                });
            }
            if (Event.current.button == 0)
            {
                if (_isAdjusting == true && Event.current.type == EventType.MouseDown)
                {
                    isManipulatingCircle = true;
                }
                else if (_isAdjusting == true && Event.current.type == EventType.MouseDrag)
                {
                    Utilities.PerformActionAtMouseWorldPosition((p) =>
                    {
                        if (BrushSettingsWindow.SnapToGrid == true)
                        {
                            p = Utilities.SnapToGrid(p, BrushSettingsWindow.GridSize);
                        }
                        else if (Event.current.modifiers == EventModifiers.Control)
                        {
                            float xMove = EditorPrefs.GetFloat("MoveSnapX");
                            float yMove = EditorPrefs.GetFloat("MoveSnapY");

                            p = Utilities.SnapToValues(p, _selectStartPosition, xMove, yMove);
                        }
                        var radius = (circle.GetWorldCenter() - p).magnitude;
                        Undo.RecordObject(circle, string.Format("Change radius of {0}", circle.name));
                        circle.radius = radius;
                        EditorUtility.SetDirty(target);
                    });
                }
                else if (_isAdjusting == true && Event.current.type == EventType.MouseUp)
                {
                    _isAdjusting = false;
                    isManipulatingCircle = false;
                    HandleUtility.Repaint();
                }
            }
        }
        
        DefaultHandles.Hidden = _isAdjusting;

        if ((Event.current.type == EventType.Layout && _isDrawing == true) || _isAdjusting == true)
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

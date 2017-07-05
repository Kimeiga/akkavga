using UnityEngine;
using UnityEditor;
using System.Collections;

public class BrushPolygon2DWindow : EditorWindow
{
    private Vector2 _scrollPos;

    [MenuItem("Window/PolyTie/2D Brushes")]
    private static void init()
    {
        var test = EditorWindow.GetWindow<BrushPolygon2DWindow>("2D Brushes");
        Debug.Log(test);
    }


    void OnGUI()
    {
        _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos, GUIStyle.none);
        // Polygons
        GUILayout.Label("Create 2D polygon objects", EditorStyles.boldLabel);

        if (GUILayout.Button("Polygon") == true)
            BrushPolygon2D.CreatePolygon();

        EditorGUILayout.Separator();
        GUILayout.Label("Create 2D circle objects", EditorStyles.boldLabel);

        if (GUILayout.Button("Circle") == true)
            BrushCircle2D.CreateCircle();

        EditorGUILayout.Separator();
        GUILayout.Label("Create 2D rectangle objects", EditorStyles.boldLabel);

        if (GUILayout.Button("Rectangle") == true)
            BrushRectangle2D.CreateRectangle();

        EditorGUILayout.Separator();
        GUILayout.Label("Create 2D edge objects", EditorStyles.boldLabel);

        if (GUILayout.Button("Edge") == true)
            BrushEdge2D.CreateEdge();
        EditorGUILayout.Separator();
        GUILayout.Label("Create 2D path object", EditorStyles.boldLabel);

        if (GUILayout.Button("Path") == true)
            BrushVectorPath2D.CreatePath();
        EditorGUILayout.EndScrollView();
    }
}

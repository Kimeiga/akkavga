using UnityEngine;
using UnityEditor;
using System.Collections;
using System;

[CustomEditor(typeof(ColorPallet))]
public class ColorPalletEditor : Editor 
{
    public override void OnInspectorGUI()
    {
        var colorPallet = target as ColorPallet;

        EditorGUILayout.LabelField("Fill colors: ");
        foreach (var state in Enum.GetValues(typeof(ColorStates)))
        {
            Color oldColor = colorPallet.FillColors[(int)state];
            Color newColor = EditorGUILayout.ColorField(string.Format("{0}: ", state.ToString()), oldColor);
            if (oldColor != newColor)
            {
                colorPallet.FillColors[(int)state] = newColor;
                EditorUtility.SetDirty(colorPallet);
            }
        }

        EditorGUILayout.Separator();
        EditorGUILayout.LabelField("Line colors: ");
        foreach (var state in Enum.GetValues(typeof(ColorStates)))
        {
            Color oldColor = colorPallet.LineColors[(int)state];
            Color newColor = EditorGUILayout.ColorField(string.Format("{0}: ", state.ToString()), oldColor);
            if (oldColor != newColor)
            {
                colorPallet.LineColors[(int)state] = newColor;
                EditorUtility.SetDirty(colorPallet);
            }
        }

        EditorGUILayout.Separator();
        EditorGUILayout.LabelField("Vertex colors: ");
        foreach (var state in Enum.GetValues(typeof(ColorStates)))
        {
            Color oldColor = colorPallet.VertexColors[(int)state];
            Color newColor = EditorGUILayout.ColorField(string.Format("{0}: ", state.ToString()), colorPallet.VertexColors[(int)state]);
            if (oldColor != newColor)
            {
                colorPallet.VertexColors[(int)state] = newColor;
                EditorUtility.SetDirty(colorPallet);
            }
        }
    }
}

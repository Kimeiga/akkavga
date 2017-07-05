using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Linq;

[CustomEditor(typeof(FreezeHandle))]
[CanEditMultipleObjects]
public class FreezeHandleEditor : Editor 
{
    private SerializedProperty freezeProp;

    public override void OnInspectorGUI()
    {
        freezeProp.boolValue = EditorGUILayout.Toggle("Freeze", freezeProp.boolValue);
        serializedObject.ApplyModifiedProperties();
        if (GUI.changed)
        {
            var freezeObjects = targets.Cast<FreezeHandle>();
            foreach (var freezeObj in freezeObjects)
            {
                freezeObj.UpdateFreeze();
            }
        }
    }

    void OnEnable()
    {
        if (SceneView.focusedWindow != null && SceneView.focusedWindow.title == "UnityEditor.SceneView")
        {
            var freezeHandle = target as FreezeHandle;
            if (Selection.activeGameObject == freezeHandle.gameObject && freezeHandle.Freeze == true)
            {
                Selection.activeGameObject = null;
            }
        }
        freezeProp = serializedObject.FindProperty("Freeze");
    }

    void OnSceneGUI()
    {
        if (SceneView.focusedWindow != null && SceneView.focusedWindow.title == "UnityEditor.SceneView")
        {            
            var freezeHandle = target as FreezeHandle;
            if (Selection.activeGameObject == freezeHandle.gameObject && freezeHandle.Freeze == true)
            {
                Selection.activeGameObject = null;
            }
        }
    }
}

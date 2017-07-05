using UnityEngine;
using UnityEditor;
using System.IO;

public static class ScriptableObjectUtility
{
    [MenuItem("Assets/Create/Color Pallet")]
    public static void CreateAsset()
    {
        var pallet = CreateAsset<ColorPallet>();
        for (int i = 0; i < pallet.FillColors.Length; i++)
        {
            pallet.FillColors[i] = Color.green;
        }
        for (int i = 0; i < pallet.LineColors.Length; i++)
        {
            pallet.LineColors[i] = Color.blue;
        }
        for (int i = 0; i < pallet.VertexColors.Length; i++)
        {
            pallet.VertexColors[i] = Color.white;
        }
    }

    public static T CreateAsset<T>() where T : ScriptableObject
    {
        T asset = ScriptableObject.CreateInstance<T>();

        string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        if (path == "")
        {
            path = "Assets";
        }
        else if (Path.GetExtension(path) != "")
        {
            path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
        }

        string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + "/New " + typeof(T).ToString() + ".asset");

        AssetDatabase.CreateAsset(asset, assetPathAndName);

        AssetDatabase.SaveAssets();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;

        return asset;
    }
}

using UnityEngine;
using UnityEditor;
using System.Collections;

public class MenuShortCuts : Editor 
{
    [MenuItem("Edit/PolyTie/Add Debug Camera")]
    private static void addDebugCamera()
    {
        if (Camera.main.gameObject.GetComponent<Physics2DDebugRenderer>() == null)
            Camera.main.gameObject.AddComponent<Physics2DDebugRenderer>();
    }
}

using UnityEngine;
using System.Collections;

public class ColorPallet : ScriptableObject 
{
    [SerializeField]
    public Color[] FillColors = new Color[(int)ColorStates.SELECTED + 1];
    [SerializeField]
    public Color[] LineColors = new Color[(int)ColorStates.SELECTED + 1];
    [SerializeField]
    public Color[] VertexColors = new Color[(int)ColorStates.SELECTED + 1];
}

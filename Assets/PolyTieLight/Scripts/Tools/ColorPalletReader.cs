using UnityEngine;
using System.Collections;

public enum ColorStates
{
    IDLE,
    PATH,
    SELECTED
}

public static class ColorPalletReader  
{
    private static Color[] _fillColors = new Color[(int)ColorStates.SELECTED + 1];
    private static Color[] _lineColors = new Color[(int)ColorStates.SELECTED + 1];
    private static Color[] _vertexColors = new Color[(int)ColorStates.SELECTED + 1];

    public static Color GetFillColor(ColorStates state)
    {
        return _fillColors[(int)state];
    }

    public static Color GetLineColor(ColorStates state)
    {
        return _lineColors[(int)state];
    }

    public static Color GetVertexColor(ColorStates state)
    {
        return _vertexColors[(int)state];
    }

    public static void Read(ColorPallet pallet)
    {
        _fillColors = pallet.FillColors;
        _lineColors = pallet.LineColors;
        _vertexColors = pallet.VertexColors;
    }
}

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Text;
using System.IO;

public class BrushSettingsWindow : EditorWindow
{
    public const string KeyVertexSize = "PolyTie_VertexSize";
    public const string KeyShowVertexInfo = "PolyTie_ShowVertexInfo";
    public const string KeyGridSize = "PolyTie_GridSize";
    public const string KeySnapToGrid = "PolyTie_SnapToGrid";
    public const string KeyShowSnapGrid = "PolyTie_ShowSnapGrid";
    public const string KeySelectedColourPallet = "PolyTie_SelectedColorPallet";

    private static ColorPallet[] _colorPallets;
    private static string[] _colorOptions;
    private static int _selectedColorPallet;
    
    public static float VertexSize = 0.1f;
    public static bool ShowVertexInfo = false;

    public static float GridSize = 0.5f;
    public static bool SnapToGrid = false;
    public static bool ShowSnapGrid = false;

    private static bool _initialized = false;

    [MenuItem("Window/PolyTie/Display Settings")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        EditorWindow.GetWindow(typeof(BrushSettingsWindow));
        readSettings();

        // Get all color pallets
        loadColorPallet();
    }

    void OnGUI()
    {
        GUILayout.Label("Display Settings", EditorStyles.boldLabel);

        if (_colorOptions == null)
            loadColorPallet();

        var oldPalletSelection = _selectedColorPallet;
        _selectedColorPallet = EditorGUILayout.Popup("Select color scheme", _selectedColorPallet, _colorOptions);
        if (oldPalletSelection != _selectedColorPallet)
        {
            ColorPalletReader.Read(_colorPallets[_selectedColorPallet]);
        }

        EditorGUILayout.Separator();

        VertexSize = EditorGUILayout.Slider("Vertex size", VertexSize, 0.0f, 1.0f);
        ShowVertexInfo = EditorGUILayout.Toggle("Show vertex info", ShowVertexInfo);

        EditorGUILayout.Separator();

        GridSize = EditorGUILayout.FloatField("Grid Size", GridSize);
        if (GridSize < 0)
            GridSize = 0;

        SnapToGrid = EditorGUILayout.Toggle("Snap to grid", SnapToGrid);
        ShowSnapGrid = EditorGUILayout.Toggle("Show snap grid", ShowSnapGrid);

        if (GUI.changed == true)
        {
            EditorUtility.SetDirty(this);
        }
    }

    public void OnFocus()
    {
        loadColorPallet();
    }

    public void OnEnable()
    {
        readSettings();
    }

    public void OnDisable()
    {
        writeSettings();
    }

    public void OnLostFocus()
    {
        writeSettings();
    }
    public void OnDestroy()
    {
        writeSettings();
    }

    public static void Initialize()
    {
        if (_initialized == false)
        {
            readSettings();
            loadColorPallet();
        }
    }

    private static void readSettings()
    {
        if (_initialized == false)
        {
            _initialized = true;
            if (EditorPrefs.HasKey(KeySelectedColourPallet))
                _selectedColorPallet = EditorPrefs.GetInt(KeySelectedColourPallet);
            if (EditorPrefs.HasKey(KeyVertexSize))
                VertexSize = EditorPrefs.GetFloat(KeyVertexSize);
            if (EditorPrefs.HasKey(KeyShowVertexInfo))
                ShowVertexInfo = EditorPrefs.GetBool(KeyShowVertexInfo);
            if (EditorPrefs.HasKey(KeyGridSize))
                GridSize = EditorPrefs.GetFloat(KeyGridSize);
            if (EditorPrefs.HasKey(KeySnapToGrid))
                SnapToGrid = EditorPrefs.GetBool(KeySnapToGrid);
            if (EditorPrefs.HasKey(KeyShowSnapGrid))
                ShowSnapGrid = EditorPrefs.GetBool(KeyShowSnapGrid);
        }
    }

    private static void writeSettings()
    {
        EditorPrefs.SetInt(KeySelectedColourPallet, _selectedColorPallet);
        EditorPrefs.SetFloat(KeyVertexSize, VertexSize);
        EditorPrefs.SetBool(KeyShowVertexInfo, ShowVertexInfo);
        EditorPrefs.SetFloat(KeyGridSize, GridSize);
        EditorPrefs.SetBool(KeySnapToGrid, SnapToGrid);
        EditorPrefs.SetBool(KeyShowSnapGrid, ShowSnapGrid);
    }

    private static void loadColorPallet()
    {
        _colorPallets = Resources.LoadAll<ColorPallet>("ColorPallets");
        _colorOptions = new string[_colorPallets.Length];
        for (int i = 0; i < _colorPallets.Length; i++)
        {
            _colorOptions[i] = _colorPallets[i].name;
        }
        ColorPalletReader.Read(_colorPallets[_selectedColorPallet]);
    }
}

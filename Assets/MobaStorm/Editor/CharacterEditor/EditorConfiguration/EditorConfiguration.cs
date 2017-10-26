using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System;

// TODO: Add the possibility to edit Color Schemes through an editor.
public class EditorConfiguration : EditorWindow
{
    public enum EEditorColorSchemes
    {
        Ocean,
        Dark,
        Light
    }

    private static string m_colorSchemeFileName = "ColorSchemesConfiguration.json";
    private static string m_activeColorSchemeFileName = "EditorColorScheme.json";

    private static EditorWindow m_window;
    private static bool m_colorSchemesLoaded;
    private static bool m_colorSchemeConfigurationLoaded;
    private static bool m_colorSchemeConfigurationSaved;
    private static EEditorColorSchemes m_defaultColorSchemeType = EEditorColorSchemes.Ocean;
    private static EEditorColorSchemes m_selectedColorSchemeType = EEditorColorSchemes.Ocean;

    private static EditorColorScheme m_colorSchemeOcean;
    private static EditorColorScheme m_colorSchemeDark;
    private static EditorColorScheme m_colorSchemeLight;

    private static float m_winWidth;
    private static float m_winHeight;

    private static EditorColorScheme m_activeColorScheme;

    public static EditorColorScheme ActiveColorScheme()
    {
        if(m_activeColorScheme == null)
        {
            LoadActiveColorScheme();
            if(m_activeColorScheme == null)
            {
                Debug.Log("Returning default color scheme");
                m_activeColorScheme = GetColorScheme(m_defaultColorSchemeType);
            }
            return m_activeColorScheme;
        }
        return m_activeColorScheme;
    }
    
    private static void Init()
    {
        m_window = GetWindow();
        m_winWidth = m_window.position.width;
        m_winHeight = m_window.position.height;
        m_window.titleContent = new GUIContent("Editor Configuration");
        m_colorSchemesLoaded = false;
        m_colorSchemeConfigurationLoaded = false;
    }

    private static EditorWindow GetWindow()
    {
        return GetWindow(typeof(EditorConfiguration));
    }

    private void OnGUI()
    {
        if (m_window == null)
        {
            m_window = GetWindow();
            return;
        }
        else
        {
            m_winWidth = m_window.position.width;
            m_winHeight = m_window.position.height;
            RenderOptions();
        }

        if (!EditorFileManager.DataFileExist(m_colorSchemeFileName))
        {
            LoadDefaultColorSchemes();
            SaveColorSchemes();
        }
        else if(EditorFileManager.DataFileExist(m_colorSchemeFileName) && !m_colorSchemesLoaded)
        {
            LoadColorSchemes();
        }

        if(!EditorFileManager.DataFileExist(m_activeColorSchemeFileName))
        {
            m_selectedColorSchemeType = m_defaultColorSchemeType;
            SaveActiveColorScheme();
        }
        else if(EditorFileManager.DataFileExist(m_activeColorSchemeFileName) && !m_colorSchemeConfigurationLoaded)
        {
            LoadActiveColorScheme();
        }
    }

    private void RenderOptions()
    {
        GUILayout.BeginArea(new Rect(10, 10, m_winWidth - 20, m_winHeight - 35));
        GUILayout.Label("Editor Configuration");
        GUILayout.Space(15);
        EEditorColorSchemes selectedColorScheme = (EEditorColorSchemes)EditorGUILayout.EnumPopup("Use Color Scheme", m_selectedColorSchemeType);
        if (selectedColorScheme != m_selectedColorSchemeType)
        {
            m_selectedColorSchemeType = selectedColorScheme;
            m_colorSchemeConfigurationSaved = false;
        }
        GUILayout.EndArea();

        GUILayout.BeginArea(new Rect(10, m_winHeight - 35, m_winWidth - 20, 35));

        GUILayout.BeginHorizontal();
        
        if(m_colorSchemeConfigurationSaved)
        {
            GUILayout.Label("Style " + m_selectedColorSchemeType + " saved!");
        }
        else
        {
            GUILayout.Space(m_winWidth - 100);

            if (GUILayout.Button("Save"))
            {
                SaveActiveColorScheme();
                m_colorSchemeConfigurationSaved = true;
            }
        }

        GUILayout.Space(10);

        GUILayout.EndHorizontal();

        GUILayout.EndArea();
    }

    private void LoadColorSchemes()
    {
        if (!EditorFileManager.DataFileExist(m_colorSchemeFileName))
        {
            return;
        }
        string data = EditorFileManager.ReadDataFile(m_colorSchemeFileName);
        Dictionary<string, object> deserializedData = MiniJSON.Json.Deserialize(data) as Dictionary<string, object>;

        foreach (EEditorColorSchemes colorSchemeType in Enum.GetValues(typeof(EEditorColorSchemes)))
        {
            Dictionary<string, object> colorSchemeData = deserializedData["ColorScheme-" + colorSchemeType.ToString()] as Dictionary<string, object>;
            if(colorSchemeType == EEditorColorSchemes.Ocean)
            {
                m_colorSchemeOcean = new EditorColorScheme(colorSchemeData);
            }
            else if(colorSchemeType == EEditorColorSchemes.Dark)
            {
                m_colorSchemeDark = new EditorColorScheme(colorSchemeData);
            }
            else if(colorSchemeType == EEditorColorSchemes.Light)
            {
                m_colorSchemeLight = new EditorColorScheme(colorSchemeData);
            }
        }

        m_colorSchemesLoaded = true;

        Debug.Log("Color Schemes Loaded - Ocean: " + m_colorSchemeOcean + " \n\nDark: " + m_colorSchemeDark + " \n\nLight " + m_colorSchemeLight);
    }

    private static void SaveColorSchemes()
    {
        Dictionary<string, object> colorSchemes = new Dictionary<string, object>();
        foreach(EEditorColorSchemes colorSchemeType in Enum.GetValues(typeof(EEditorColorSchemes)))
        {
            EditorColorScheme colorScheme = GetColorScheme(colorSchemeType);
            colorSchemes.Add("ColorScheme-" + colorSchemeType.ToString(), colorScheme.Serialize());
        }
        string serializedData = MiniJSON.Json.Serialize(colorSchemes);
        EditorFileManager.RemoveDataFile(m_colorSchemeFileName);
        EditorFileManager.WriteJSONDataFile(serializedData, m_colorSchemeFileName);
    }

    private static void LoadActiveColorScheme()
    {
        if(!EditorFileManager.DataFileExist(m_activeColorSchemeFileName))
        {
            return;
        }
        string data = EditorFileManager.ReadDataFile(m_activeColorSchemeFileName);
        Dictionary<string, object> deserializedData = MiniJSON.Json.Deserialize(data) as Dictionary<string, object>;
        Dictionary<string, object> colorSchemeData = deserializedData["ActiveColorScheme"] as Dictionary<string, object>;
        m_activeColorScheme = new EditorColorScheme(colorSchemeData);
        m_selectedColorSchemeType = m_activeColorScheme.Type;
        Debug.Log("Loaded Color Scheme Configuration: " + m_activeColorScheme);
        m_colorSchemeConfigurationLoaded = true;
    }

    private static void SaveActiveColorScheme()
    {
        Dictionary<string, object> activeColorScheme = new Dictionary<string, object>();
        EditorColorScheme colorScheme = GetColorScheme(m_selectedColorSchemeType);
        activeColorScheme.Add("ActiveColorScheme", colorScheme.Serialize());
        string serializedData = MiniJSON.Json.Serialize(activeColorScheme);
        EditorFileManager.RemoveDataFile(m_activeColorSchemeFileName);
        EditorFileManager.WriteJSONDataFile(serializedData, m_activeColorSchemeFileName);
        Debug.Log("Active Color Scheme Configuration " + m_selectedColorSchemeType + " saved!");
    }

    private static EditorColorScheme GetColorScheme(EEditorColorSchemes colorSchemeIdentifier)
    {
        switch(colorSchemeIdentifier)
        {
            case EEditorColorSchemes.Ocean:
                return m_colorSchemeOcean;
            case EEditorColorSchemes.Dark:
                return m_colorSchemeDark;
            case EEditorColorSchemes.Light:
                return m_colorSchemeLight;
        }
        return null;
    }

    // TODO: Configure proper colors for dark and light schemes
    public static void LoadDefaultColorSchemes()
    {
        m_colorSchemeOcean = new EditorColorScheme();
        m_colorSchemeOcean.Type = EEditorColorSchemes.Ocean;
        m_colorSchemeOcean.DefaultTextColor = Color.white;
        m_colorSchemeOcean.LinkTextColor = Color.yellow;
        m_colorSchemeOcean.ButtonTextColor = Color.black;
        m_colorSchemeOcean.ImportantTextColor = Color.yellow;
        m_colorSchemeOcean.ErrorTextColor = Color.white;
        m_colorSchemeOcean.HeaderBackgroundColor = new Color32(26, 73, 105, 255);
        m_colorSchemeOcean.BodyBackgroundColor = new Color32(194, 194, 194, 255);
        m_colorSchemeOcean.BodyBackgroundColor2 = new Color32(138, 138, 138, 255);
        m_colorSchemeOcean.ListBackgroundColor = new Color32(21, 122, 140, 255);
        m_colorSchemeOcean.ListHeaderBackgroundColor = new Color32(33, 173, 148, 255);
        m_colorSchemeOcean.BoxBackgroundColor = new Color32(145, 200, 255, 255);
        m_colorSchemeOcean.ErrorBoxBackgroundColor = Color.red;

        m_colorSchemeDark = new EditorColorScheme();
        m_colorSchemeDark.Type = EEditorColorSchemes.Dark;
        m_colorSchemeDark.DefaultTextColor = Color.white;
        m_colorSchemeDark.LinkTextColor = Color.yellow;
        m_colorSchemeDark.ButtonTextColor = Color.green;
        m_colorSchemeDark.ImportantTextColor = Color.yellow;
        m_colorSchemeDark.ErrorTextColor = Color.white;
        m_colorSchemeDark.HeaderBackgroundColor = new Color32(48, 48, 48, 255);
        m_colorSchemeDark.BodyBackgroundColor = new Color32(198, 198, 198, 255);
        m_colorSchemeDark.BodyBackgroundColor2 = new Color32(154, 154, 154, 255);
        m_colorSchemeDark.ListBackgroundColor = new Color32(219, 219, 219, 255);
        m_colorSchemeDark.ListHeaderBackgroundColor = new Color32(78, 78, 78, 255);
        m_colorSchemeDark.BoxBackgroundColor = new Color32(160, 185, 162, 255);
        m_colorSchemeDark.ErrorBoxBackgroundColor = Color.red;

        m_colorSchemeLight = new EditorColorScheme();
        m_colorSchemeLight.Type = EEditorColorSchemes.Light;
        m_colorSchemeLight.DefaultTextColor = Color.white;
        m_colorSchemeLight.LinkTextColor = Color.yellow;
        m_colorSchemeLight.ButtonTextColor = Color.black;
        m_colorSchemeLight.ImportantTextColor = Color.yellow;
        m_colorSchemeLight.ErrorTextColor = Color.white;
        m_colorSchemeLight.HeaderBackgroundColor = new Color32(26, 73, 105, 255);
        m_colorSchemeLight.BodyBackgroundColor = new Color32(194, 194, 194, 255);
        m_colorSchemeLight.BodyBackgroundColor2 = new Color32(138, 138, 138, 255);
        m_colorSchemeLight.ListBackgroundColor = new Color32(21, 122, 140, 255);
        m_colorSchemeLight.ListHeaderBackgroundColor = new Color32(33, 173, 148, 255);
        m_colorSchemeLight.BoxBackgroundColor = new Color32(145, 200, 255, 255);
        m_colorSchemeLight.ErrorBoxBackgroundColor = Color.red;
    }
}

using UnityEngine;
using System.Collections;
using UnityEditor;
using MiniJSON;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

[CustomEditor(typeof(GameDataManager))]
public class GameDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawGlobal();
        
    }

    private void DrawGlobal()
    {
        GameDataManager mytarget = (GameDataManager)target;
        //Draw Globals
        mytarget.GlobalConfig.DrawEditor();
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Save Global Data"))
        {
            
            string savePath;
            if (Utils.SaveFileDialog(GameDataManager.m_globalExtension, "Load Global Data", out savePath))
            {
                Utils.Save(mytarget.GlobalConfig, savePath);
            }
        }
        if (GUILayout.Button("Load Global Data"))
        {
            string path;
            if (Utils.OpenFileDialog(GameDataManager.m_globalExtension, "Load Global Data", out path))
            {
                GlobalConfig config = Utils.Load<GlobalConfig>(path);
                mytarget.GlobalConfig = config;
            }
        }
        EditorGUILayout.EndHorizontal();
    }  

    public static Texture2D MakeTex(int width, int height, Color col)
    {
        Color[] pix = new Color[width * height];

        for (int i = 0; i < pix.Length; i++)
            pix[i] = col;

        Texture2D result = new Texture2D(width, height);
        result.SetPixels(pix);
        result.Apply();
        return result;
    }
}

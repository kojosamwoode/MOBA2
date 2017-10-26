using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(SpawnManager))]
public class SpawnManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        GUIStyle style = new GUIStyle(GUI.skin.button);
        style.normal.background = MakeTex(600, 1, new Color(0f, 0.5f, 0.1f, 0.5f));
        SpawnManager mytarget = (SpawnManager)target;
        EditorGUILayout.LabelField("Spawn manager Editor");
        for (int i = 0; i < mytarget.m_holderList.Count; i++)
        {
            EditorGUILayout.BeginVertical(style);
            EditorGUILayout.Separator();
            mytarget.m_holderList[i] = EditorGUILayout.ObjectField(mytarget.m_holderList[i], typeof(PrefabHolder), true) as PrefabHolder;
            if (GUILayout.Button("Remove This"))
            {
                mytarget.m_holderList.RemoveAt(i);
                return;
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();
        }
        if (GUILayout.Button("Add New Holder"))
        {
            mytarget.m_holderList.Add(new PrefabHolder());
        }
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

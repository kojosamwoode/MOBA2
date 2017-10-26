using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(PrefabHolder))]
public class PrefabHolderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        GUIStyle style = new GUIStyle(GUI.skin.button);
        style.normal.background = MakeTex(600, 1, new Color(0f, 0.5f, 0.1f, 0.5f));
        PrefabHolder mytarget = (PrefabHolder)target;

        EditorGUILayout.LabelField("Prefab Holder Editor");
        for (int i = 0; i < mytarget.m_poolDataList.Count; i++)
        {
            string name = "Not Assigned";
            if (mytarget.m_poolDataList[i].m_obj)
            {
                name = mytarget.m_poolDataList[i].m_obj.name;
            }

            mytarget.m_poolDataList[i].m_foldOut = EditorGUILayout.Foldout(mytarget.m_poolDataList[i].m_foldOut, name);

            if (mytarget.m_poolDataList[i].m_foldOut)
            {

                EditorGUILayout.BeginVertical(style);
                EditorGUILayout.Separator();
                mytarget.m_poolDataList[i].m_quantity = EditorGUILayout.IntField("Quantity", mytarget.m_poolDataList[i].m_quantity);
                mytarget.m_poolDataList[i].m_register = EditorGUILayout.Toggle("Register Prefab UNET", mytarget.m_poolDataList[i].m_register);
                mytarget.m_poolDataList[i].m_obj = EditorGUILayout.ObjectField("GameObject", mytarget.m_poolDataList[i].m_obj, typeof(GameObject), true) as GameObject;

                if (GUILayout.Button("Remove This"))
                {
                    mytarget.m_poolDataList.RemoveAt(i);
                    return;
                }
                EditorGUILayout.EndVertical();
                EditorGUILayout.Space();
            }

        }
        if (GUILayout.Button("Add New Object"))
        {
            mytarget.m_poolDataList.Add(new PoolData());
        }
        EditorUtility.SetDirty(mytarget);
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

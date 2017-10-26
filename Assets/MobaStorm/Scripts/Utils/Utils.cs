using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.IO;

public class Utils {

    public static Vector3[] GenerateCirclePoints(Vector3 fromPosition, float radius, int segments)
    {
        float x = 0f;
        float y = 0.2f;
        float z = 0f;
        Vector3[] circlePoints = new Vector3[segments + 1];
        float angle = 20f;

        for (int i = 0; i < (segments + 1); i++)
        {
            x = Mathf.Sin(Mathf.Deg2Rad * angle) * radius;
            z = Mathf.Cos(Mathf.Deg2Rad * angle) * radius;

            circlePoints[i] = new Vector3(x, y, z); //+ fromPosition;
            angle += (360f / segments);
        }

        return circlePoints;
    }

    public static object CreateGenericInstance(string FullyQualifiedNameOfClass, MobaEntity entity)
    {
        object[] args = new object[] { entity };
        Type t = Type.GetType(FullyQualifiedNameOfClass);
        return Activator.CreateInstance(t, args);
    }
    public static LineRenderer CreateLineRendererObject(MobaEntity entity, bool useWorldSpace)
    {
        GameObject obj = new GameObject("Debug Renderer");
        LineRenderer lineRenderer = obj.AddComponent<LineRenderer>();
        lineRenderer.startWidth = 0.01f;
        lineRenderer.endWidth = 0.01f;
        lineRenderer.useWorldSpace = useWorldSpace;
        obj.transform.position = entity.transform.position;
        obj.transform.SetParent(entity.transform);
        return lineRenderer;
    }

    public static Vector3 Vector2ToVector3(Vector2 position)
    {
        return new Vector3(position.x, 0, position.y);
    }
    public static Vector2 Vector3ToVector2(Vector3 position)
    {
        return new Vector2(position.x, position.z);
    }

    /// <summary>
    /// Checks if two positions on the grid are in range
    /// </summary>
    /// <param name="currentPos">Current Position</param>
    /// <param name="targetPos">Current Target Position</param>
    /// <param name="range">Range</param>
    /// <returns></returns>
    public static bool IsOnRange(Vector2 currentPos, Vector2 targetPos, float range)
    {
        float distance = Vector2.Distance(currentPos, targetPos);
        if (distance < range)
        {
            return true;
        }
        return false;
    }
    /// <summary>
    /// Finds a target on a range
    /// </summary>
    /// <param name="abilityPos">Current Position</param>
    /// <param name="entities">Entities to calculate distance</param>
    /// <param name="range">Range to search</param>
    /// <param name="targetEntity">Target found</param>
    /// <returns></returns>
    public static bool IsOnRange(Vector2 abilityPos, HashSet<MobaEntity> entities, float range, out MobaEntity targetEntity)
    {
        targetEntity = null;
        foreach (MobaEntity current in entities)
        {
            if (current.Dead)
                continue;

            float distance = Vector2.Distance(abilityPos, current.Position);
            if (distance < range)
            {
                targetEntity = current;
                return true;
            }
        }

        return false;
    }
    /// <summary>
    /// Finds a target on a range with a prioritizing any entity type
    /// </summary>
    /// <param name="currentPos">Current Position</param>
    /// <param name="entities">Entities to calculate distance</param>
    /// <param name="entityClass">Entity Class to prioritize {IAEntity, CharacterEntity}</param>
    /// <param name="range">Range to search</param>
    /// <param name="targetEntity">Target found</param>
    /// <returns></returns>
    public static bool IsOnRangeWithPriority(Vector2 currentPos, HashSet<MobaEntity> entities, string entityClass, float range, out MobaEntity targetEntity)
    {
        targetEntity = null;
        foreach (MobaEntity current in entities)
        {
            if (current.Dead)
                continue;

            float distance = Vector2.Distance(currentPos, current.Position);
            if (distance < range)
            {
                if (current.GetType().ToString() == entityClass)
                {
                    targetEntity = current;
                    return true;
                }
                targetEntity = current;
            }
        }

        if (targetEntity != null)
        {
            return true;
        }
        return false;
    }


#if UNITY_EDITOR
    public static bool OpenFileDialog(string extension, string windowMessage, out string path)
    {
        path = EditorUtility.OpenFilePanel(windowMessage, Application.dataPath + GameDataManager.m_gameDataPath, extension);
        if (path.Length != 0)
        {
            return true;
        }

        return false;
    }


    public static bool SaveFileDialog(string extension, string windowMessage, out string path)
    {
        path = EditorUtility.SaveFilePanel(windowMessage, Application.dataPath + GameDataManager.m_gameDataPath, "", extension);
        if (path.Length != 0)
        {
            return true;
        }

        return false;
    }

    public static T Load<T>(string filePath)
    {
        T obj = default(T);
        string text = "";
        if (File.Exists(filePath))
        {
            text = File.ReadAllText(filePath);
            obj = JsonUtility.FromJson<T>(text);
            return obj;
        }
        return obj;
    }

    public static void Save(object obj, string path)
    {
        string text = JsonUtility.ToJson(obj, true);
        File.WriteAllText(path, text);
    }

#endif
}

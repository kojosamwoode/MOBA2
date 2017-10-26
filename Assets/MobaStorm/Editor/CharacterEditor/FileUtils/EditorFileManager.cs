using UnityEngine;
using System.Collections;
using System.IO;
using UnityEditor;

public class EditorFileManager
{
    public static string BaseDataPathString = Application.dataPath + "/Editor/CharacterEditor/EditorConfiguration/";

    private static string m_emptyJSONObject = "{}";

    public static string ReadDataFile(string fileName)
    {
        string fullPath = BaseDataPathString + fileName;
        if (!DataFileExist(fileName))
        {
            Debug.Log("<color=green>File doesn't exist on path: " + fullPath + "</color>");
            return m_emptyJSONObject;
        }
        return ReadDataFile(BaseDataPathString, fileName);
    }

    private static string ReadDataFile(string basePath, string fileName)
    {
        string line = string.Empty;
        line = File.ReadAllText(basePath + fileName);
        Debug.Log("<color=green>Data loaded from: " + basePath + fileName + "</color>");
        return line;
    }

    public static void WriteJSONDataFile(string data, string fileName)
    {
        string fullPath = BaseDataPathString + fileName;
        RemoveDataFile(fileName);
        var sr = File.CreateText(fullPath);
        sr.WriteLine(data);
        sr.Close();
        Debug.Log("Saved file at: " + fullPath + " data " + data);
    }

    public static bool DataFileExist(string fileName)
    {
        string fullPath = BaseDataPathString + fileName;
        return File.Exists(fullPath);
    }

    public static void RemoveDataFile(string fileName)
    {
        if (DataFileExist(fileName))
        {
            string fullPath = BaseDataPathString + fileName;
            FileUtil.DeleteFileOrDirectory(fullPath);
        }
    }
}

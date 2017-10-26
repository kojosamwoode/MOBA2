using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

using System.IO;

public static class ScriptableObjectUtility
{
    /// <summary>
    //	This makes it easy to create, name and place unique new ScriptableObject asset files.
    /// </summary>
    public static T CreateAsset<T>(string id, string path) where T : ScriptableObject
    {
        T asset = ScriptableObject.CreateInstance<T>();

        //string path = "Assets/MobaStorm/Scripts/Abilities/AbilityAssets/";

        string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + id + ".asset");

        AssetDatabase.CreateAsset(asset, assetPathAndName);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        return asset;
    }

    public static SideEffect CreateAsset(string effectClass, string abilityClass) 
    {
        SideEffect asset = ScriptableObject.CreateInstance(effectClass) as SideEffect;
        string path = "Assets/MobaStorm/Scripts/Abilities/AbilityAssets/EffectAssets/";

        string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + abilityClass + "-" + effectClass + ".asset");

        AssetDatabase.CreateAsset(asset, assetPathAndName);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        return asset;
    }

}
#endif
using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System;
using System.Reflection;

public class CharacterEditorWindow : BaseView<CharacterEditorWindow>
{
    public enum ECharacterEditorState
    {
        Default,
        CreatingHolder,
        CreatingCharacter
    }

    public static string m_resourcesPath = "Assets/MobaStorm/Editor/Resources";
    public static string m_holdersPath = "Assets/MobaStorm/Prefabs/Holders";
    public static PrefabHolder m_entityHolderPrefab;
    public static ECharacterEditorState m_currentState;

    private static HeaderSection m_header;
    private static CharacterListSection m_characterList;
    private static CreateHolderSection m_createHolder;
    private static BackgroundSection m_background;
    private static CharacterCreationSection m_characterCreation;
    public CharacterCreationSection CharacterCreation { get { return m_characterCreation; } }
    private static DefaultSection m_defaultSection;

    public static GameObject m_serverEntityPrefab;
    public static GameObject m_clientEntityPrefab;

    public static GUISkin m_skin;

    public static MobaEntityData m_activeEntityData;
    public Camera m_characterCamera;
    public GameObject m_cameraHolder;
    private string m_cameraHolderPath = "Assets/MobaStorm/Editor/CharacterEditor/CharacterEditorCameraHolder.prefab";
    public static GameDataManager m_gameDataManager;

    private string m_dataManagerPath = "Assets/MobaStorm/Prefabs/Managers/DatabaseManager.prefab";

    [MenuItem("MobaStorm/Entity Editor")]
    private static void Init()
    {
        Setup();
        GUIContent windowTitle = new GUIContent();
        windowTitle.text = "Entity Editor";
        m_currentState = ECharacterEditorState.Default;
    }

    void OnEnable()
    {
        m_currentState = ECharacterEditorState.Default;
        RefreshHoldersList();
        m_skin = (GUISkin)AssetDatabase.LoadAssetAtPath("Assets/MobaStorm/Resources/EditorSkin.guiskin", typeof(GUISkin));
        SetupCharacterCamera();
        m_gameDataManager = (GameDataManager)AssetDatabase.LoadAssetAtPath(m_dataManagerPath, typeof(GameDataManager));
        if (m_gameDataManager != null)
        {
            m_gameDataManager.LoadAllData();
            Debug.Log("DataManager loaded!");
        }
        else
        {
            Debug.LogError("Cant find GameDataManager on path: " + m_dataManagerPath);
        }
    }

    private void SetupCharacterCamera()
    {
        if(m_cameraHolder != null)
        {
            return;
        }

        GameObject prefabAsset = AssetDatabase.LoadAssetAtPath<GameObject>(m_cameraHolderPath);
        m_cameraHolder = (GameObject)PrefabUtility.InstantiatePrefab(prefabAsset);
        m_characterCamera = m_cameraHolder.GetComponentInChildren<Camera>();
        m_characterCamera.orthographic = false;
        m_characterCamera.gameObject.transform.localPosition = new Vector3(0, 0, -5);
    }

    private void DestroyCharacterCamera()
    {
        CharacterEditorWindow.DestroyImmediate(m_cameraHolder);
        m_cameraHolder = null;
        m_characterCamera = null;
    }

    public void LoadCharacterView(MobaEntityData entityData)
    {
        m_activeEntityData = entityData;
        m_characterCreation.Initialize();
        CharacterEditorWindow.m_currentState = CharacterEditorWindow.ECharacterEditorState.CreatingCharacter;
    }

    // ask for Logic class
    public static string[] GetEffectClasses<T>()
    {
        List<string> classes = new List<string>();
        foreach (var a in AppDomain.CurrentDomain.GetAssemblies())
        {
            foreach (var t in a.GetTypes())
            {
                if (t.IsSubclassOf(typeof(T)))
                {
                    classes.Add(t.ToString());
                }
            }
        }
        return classes.ToArray();
    }

    public void RefreshHoldersList()
    {
        //if (m_characterHolder == null)
        //{
        //    m_characterHolder = new List<GameObject>();
        //}
        //m_characterHolder.Clear();
        m_entityHolderPrefab = GetEntityHolderAsset();
    }

    protected override void OnGUI()
    {
        base.OnGUI();
    }

    protected override void CreateViews()
    {
        if (m_background == null)
        {
            m_background = new BackgroundSection();
            m_background.Initialize();
        }

        if (m_header == null)
        {
            m_header = new HeaderSection();
            m_header.Initialize();
        }

        if(m_characterList == null)
        {
            m_characterList = new CharacterListSection();
            m_characterList.Initialize();
        }

        if(m_createHolder == null)
        {
            m_createHolder = new CreateHolderSection();
            m_createHolder.Initialize();
        }

        if(m_characterCreation == null)
        {
            m_characterCreation = new CharacterCreationSection();
            m_characterCreation.Initialize();
        }

        if(m_defaultSection == null)
        {
            m_defaultSection = new DefaultSection();
            m_defaultSection.Initialize();
        }
    }

    protected override void InitializeViews()
    {
        if (m_background != null && !m_background.Initialized)
        {
            m_background.Initialize();
        }

        if (m_header != null && !m_header.Initialized)
        {
            m_header.Initialize();
        }

        if (m_characterList != null && !m_characterList.Initialized)
        {
            m_characterList.Initialize();
        }

        if(m_defaultSection != null && !m_defaultSection.Initialized)
        {
            m_defaultSection.Initialize();
        }
    }

    protected override void UpdateTemplateParameters()
    {
        m_header.UpdateViewParameters(m_winWidth, m_winHeight, this);
        m_characterList.UpdateViewParameters(m_winWidth, m_winHeight, this);
        m_createHolder.UpdateViewParameters(m_winWidth, m_winHeight, this);
        m_background.UpdateViewParameters(m_winWidth, m_winHeight, this);
        m_characterCreation.UpdateViewParameters(m_winWidth, m_winHeight, this);
        m_defaultSection.UpdateViewParameters(m_winWidth, m_winHeight, this);
    }

    protected override void RenderTemplates()
    {
        m_background.Render();
        m_header.Render();
        m_characterList.Render();

        if(m_currentState == ECharacterEditorState.Default)
        {
            m_defaultSection.Render();
        }
        else if(m_currentState == ECharacterEditorState.CreatingHolder)
        {
            if(!m_createHolder.Initialized)
            {
                m_createHolder.Initialize();
            }

            m_createHolder.Render();
        }
        else if(m_currentState == ECharacterEditorState.CreatingCharacter)
        {
            m_characterCreation.Render();
        }
    }

    void Update()
    {
        if(m_characterCreation != null && m_characterCreation.Initialized)
        {
            m_characterCreation.Update();
        }
    }

    private void OnDestroy()
    {
        Debug.Log("Entity editor window destroyed");
        m_background.Destroy();
        m_header.Destroy();
        m_characterList.Destroy();
        m_createHolder.Destroy();
        m_characterCreation.Destroy();
        m_defaultSection.Destroy();

        DestroyCharacterCamera();
    }

    private PrefabHolder GetEntityHolderAsset()
    {
        AssetDatabase.Refresh();
        PrefabHolder holderGameObject = AssetDatabase.LoadAssetAtPath<PrefabHolder>(m_holdersPath + "/Entity_Holder.prefab");
        if (holderGameObject == null)
        {
            Debug.LogError("Cannot find Entity_Holder in: " + m_holdersPath + "/Entity_Holder.prefab");
        }
        return holderGameObject;
    }

    public static Type GetType(string TypeName)
    {
        var type = Type.GetType(TypeName);

        if (type != null)
        {
            return type;
        }

        if (TypeName.Contains("."))
        {
            var assemblyName = TypeName.Substring(0, TypeName.IndexOf('.'));
            var assembly = Assembly.Load(assemblyName);
            if (assembly == null)
            {
                return null;
            }

            type = assembly.GetType(TypeName);
            if (type != null)
            {
                return type;
            }
        }

        var currentAssembly = Assembly.GetExecutingAssembly();
        var referencedAssemblies = currentAssembly.GetReferencedAssemblies();
        foreach (var assemblyName in referencedAssemblies)
        {
            var assembly = Assembly.Load(assemblyName);
            if (assembly != null)
            {
                type = assembly.GetType(TypeName);
                if (type != null)
                {
                    return type;
                }
                    
            }
        }
        return null;
    }
}

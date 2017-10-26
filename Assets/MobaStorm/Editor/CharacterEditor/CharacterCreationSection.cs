using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

public class CharacterCreationSection : BaseTemplate
{
    private float m_sectionWidth;
    private GameObject m_characterInstance;

    private Animation m_animation;
    private Animator m_animator;
    private Dictionary<string, AnimationClip> m_clips;
    private string[] m_animationClipOptions;
    private int m_animationOptionSelectedIndex;

    private double m_deltaTime;
    private float m_currentTime;
    private bool m_playingAnimation;
    private string m_activeAnimationName;
    private float m_animationLength;

    private bool m_prefabsLoaded = false;

    public override void Initialize()
    {
        base.Initialize();
        if (m_characterInstance != null)
        {
            CharacterEditorWindow.DestroyImmediate(m_characterInstance);
        }
        m_characterInstance = null;
        if((m_parent as CharacterEditorWindow) == null || CharacterEditorWindow.m_activeEntityData == null)
        {
            // Prevent null reference exception when the item list is selected by default or when the editor is closing but still
            // executes a render call.
            return;
        }

        if (LoadEntityClientAssets())
        {
            m_characterInstance = CharacterEditorWindow.Instantiate(CharacterEditorWindow.m_clientEntityPrefab);
            GameObject cameraHolder = (m_parent as CharacterEditorWindow).m_cameraHolder;
            m_characterInstance.transform.SetParent(cameraHolder.transform);
            m_characterInstance.transform.position = new Vector3(0, 0, 0);
            m_animation = m_characterInstance.GetComponentInChildren<Animation>();
            if (m_animation == null)
            {
                m_animator = m_characterInstance.GetComponentInChildren<Animator>();
            }

            m_characterInstance.transform.rotation = Quaternion.Euler(0, 180, 0);
            InitializeClips();

            (m_parent as CharacterEditorWindow).m_characterCamera.targetTexture = new RenderTexture(512, 512, 24, RenderTextureFormat.ARGB32);


            ResetCharacterPosition();
        }

        m_playingAnimation = false;
        m_activeAnimationName = string.Empty;
        
    }

    public override void Destroy()
    {
        AssetDatabase.SaveAssets();
        if (CharacterEditorWindow.m_clientEntityPrefab && CharacterEditorWindow.m_serverEntityPrefab && m_characterInstance)
        {
            SaveCharacter();
        }
        base.Destroy();
    }

    private void InitializeClips()
    {
        if(m_animation == null && m_animator == null)
        {
            Debug.LogWarning("Can't initialize animations in this character, No (Animation or Animator) component found!");
            CharacterEditorWindow.m_currentState = CharacterEditorWindow.ECharacterEditorState.Default;
            return;
        }

        if (m_animation == null)
        {
            return;
        }
        m_animationClipOptions = Enum.GetNames(typeof(EEntityState));
        m_clips = new Dictionary<string, AnimationClip>();      

        foreach (AnimationState clipState in m_animation)
        {
            AnimationClip clip = clipState.clip;
            foreach (EEntityState animation in Enum.GetValues(typeof(EEntityState)))
            {
                if (clip.name == animation.ToString())
                {
                    m_clips.Add(clip.name, clip);
                    if (!CharacterEditorWindow.m_activeEntityData.AnimationDataExist(animation))
                    {
                        AnimationData def = new AnimationData(animation, 1);
                        CharacterEditorWindow.m_activeEntityData.AddAnimationData(def);
                    }
                }
            }        
        }
    }

    private bool LoadEntityClientAssets()
    {
        MobaEntityData entityentityData = CharacterEditorWindow.m_activeEntityData;
        CharacterEditorWindow.m_serverEntityPrefab = CharacterEditorWindow.m_entityHolderPrefab.GetPrefabFromHolder(entityentityData.m_prefab);
        CharacterEditorWindow.m_clientEntityPrefab = CharacterEditorWindow.m_entityHolderPrefab.GetPrefabFromHolder(entityentityData.m_prefab + "Client");

        m_prefabsLoaded = CharacterEditorWindow.m_serverEntityPrefab != null && CharacterEditorWindow.m_clientEntityPrefab != null;
        
        return m_prefabsLoaded;
    }

    public override void Render()
    {
        base.Render();
        m_sectionWidth = m_windowWidth - CharacterListSection.m_listWidth;
        float sectionHeight = m_windowHeight - 150;
        DrawScrolleableRegionWithDefaultContent(CharacterListSection.m_listWidth + 15, 150, m_sectionWidth, sectionHeight, m_defaultColor, m_sectionWidth - 15, sectionHeight, true);
    }

    protected override void DefaultRegionContent()
    {
        base.DefaultRegionContent();


        if (m_prefabsLoaded)
        {
            GUILayout.BeginVertical();

            GUILayout.BeginHorizontal();

            GUILayout.Label("Entity: " + CharacterEditorWindow.m_activeEntityData, CharacterEditorWindow.m_skin.customStyles[4]);

            GUILayout.Space(20);

            if (GUILayout.Button(new GUIContent("Back"), CharacterEditorWindow.m_skin.button, GUILayout.Width(120)))
            {
                Destroy();
                CharacterEditorWindow.m_currentState = CharacterEditorWindow.ECharacterEditorState.Default;
            }

            if (GUILayout.Button(new GUIContent("Remove Entity"), CharacterEditorWindow.m_skin.button, GUILayout.Width(120)))
            {
                CharacterEditorWindow.m_entityHolderPrefab.RemovePrefab(CharacterEditorWindow.m_clientEntityPrefab.name);
                CharacterEditorWindow.m_entityHolderPrefab.RemovePrefab(CharacterEditorWindow.m_serverEntityPrefab.name);
                EditorUtility.SetDirty(CharacterEditorWindow.m_entityHolderPrefab);
                CharacterEditorWindow.m_gameDataManager.RemoveEntityData(CharacterEditorWindow.m_activeEntityData);
                EditorUtility.SetDirty(CharacterEditorWindow.m_gameDataManager);
                //GameObject.DestroyImmediate(CharacterEditorWindow.m_clientEntityPrefab, true);
                //GameObject.DestroyImmediate(CharacterEditorWindow.m_serverEntityPrefab, true);
                AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(CharacterEditorWindow.m_clientEntityPrefab));
                AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(CharacterEditorWindow.m_serverEntityPrefab));
                string absoluteAssetPath = Application.dataPath;
                string relativePath = "Assets/MobaStorm/Prefabs/Entities/" + CharacterEditorWindow.m_activeEntityData.m_prefab;
                FileUtil.DeleteFileOrDirectory(relativePath);
                AssetDatabase.SaveAssets();

                Destroy();
                CharacterEditorWindow.m_currentState = CharacterEditorWindow.ECharacterEditorState.Default;
            }

            GUILayout.FlexibleSpace();

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();

            GUILayout.Label("Movement ", CharacterEditorWindow.m_skin.label);

            if (GUILayout.Button(new GUIContent("ML"), CharacterEditorWindow.m_skin.button, GUILayout.Width(50)))
            {
                m_characterInstance.transform.position = m_characterInstance.transform.position + new Vector3(-0.3f, 0, 0);
            }

            if (GUILayout.Button(new GUIContent("MR"), CharacterEditorWindow.m_skin.button, GUILayout.Width(50)))
            {
                m_characterInstance.transform.position = m_characterInstance.transform.position + new Vector3(0.3f, 0, 0);
            }

            if (GUILayout.Button(new GUIContent("MT"), CharacterEditorWindow.m_skin.button, GUILayout.Width(50)))
            {
                m_characterInstance.transform.position = m_characterInstance.transform.position + new Vector3(0, 0.3f, 0);
            }

            if (GUILayout.Button(new GUIContent("MB"), CharacterEditorWindow.m_skin.button, GUILayout.Width(50)))
            {
                m_characterInstance.transform.position = m_characterInstance.transform.position + new Vector3(0, -0.3f, 0);
            }

            GUILayout.Space(5);

            GUILayout.Label("Rotation ", CharacterEditorWindow.m_skin.label);

            if (GUILayout.Button(new GUIContent("RL"), CharacterEditorWindow.m_skin.button, GUILayout.Width(50)))
            {
                m_characterInstance.transform.Rotate(0, -30, 0);
            }

            if (GUILayout.Button(new GUIContent("RR"), CharacterEditorWindow.m_skin.button, GUILayout.Width(50)))
            {
                m_characterInstance.transform.Rotate(0, 30, 0);
            }

            if (GUILayout.Button(new GUIContent("RT"), CharacterEditorWindow.m_skin.button, GUILayout.Width(50)))
            {
                m_characterInstance.transform.Rotate(30, 0, 0);
            }

            if (GUILayout.Button(new GUIContent("RB"), CharacterEditorWindow.m_skin.button, GUILayout.Width(50)))
            {
                m_characterInstance.transform.Rotate(-30, 0, 0);
            }

            GUILayout.Space(5);

            if (GUILayout.Button(new GUIContent("Zoom In"), CharacterEditorWindow.m_skin.button, GUILayout.Width(80)))
            {
                Vector3 currentPos = (m_parent as CharacterEditorWindow).m_characterCamera.gameObject.transform.localPosition;
                (m_parent as CharacterEditorWindow).m_characterCamera.gameObject.transform.localPosition = currentPos + new Vector3(0, 0, 1);
                //float newOrthoSize = (m_parent as CharacterEditorWindow).m_characterCamera.orthographicSize - 0.2f;
                //(m_parent as CharacterEditorWindow).m_characterCamera.orthographicSize = Math.Max(0.6f, newOrthoSize);
            }

            if (GUILayout.Button(new GUIContent("Zoom Out"), CharacterEditorWindow.m_skin.button, GUILayout.Width(80)))
            {
                Vector3 currentPos = (m_parent as CharacterEditorWindow).m_characterCamera.gameObject.transform.localPosition;
                (m_parent as CharacterEditorWindow).m_characterCamera.gameObject.transform.localPosition = currentPos + new Vector3(0, 0, -1);
                //float newOrthoSize = (m_parent as CharacterEditorWindow).m_characterCamera.orthographicSize + 0.2f;
                //(m_parent as CharacterEditorWindow).m_characterCamera.orthographicSize = Math.Min(8, newOrthoSize);
            }

            GUILayout.Space(5);

            if (GUILayout.Button(new GUIContent("Reset"), CharacterEditorWindow.m_skin.button, GUILayout.Width(80)))
            {
                ResetCharacterPosition();
            }

            GUILayout.FlexibleSpace();

            GUILayout.EndHorizontal();

            // CHARACTER MODEL REGION
            CharacterModelRegion();

            GUILayout.Space(50);

            GUILayout.Space(20);

            GUILayout.FlexibleSpace();

            GUILayout.EndVertical();
        }
        else
        {
            GUILayout.BeginVertical();

            GUILayout.BeginHorizontal();

            GUILayout.Label("Entity: " + CharacterEditorWindow.m_activeEntityData, CharacterEditorWindow.m_skin.customStyles[4]);

            if (GUILayout.Button(new GUIContent("Back"), CharacterEditorWindow.m_skin.button, GUILayout.Width(120)))
            {
                Destroy();
                CharacterEditorWindow.m_currentState = CharacterEditorWindow.ECharacterEditorState.Default;
            }

            if (GUILayout.Button(new GUIContent("Remove Entity Data"), CharacterEditorWindow.m_skin.button, GUILayout.Width(120)))
            {
                CharacterEditorWindow.m_gameDataManager.RemoveEntityData(CharacterEditorWindow.m_activeEntityData);
                EditorUtility.SetDirty(CharacterEditorWindow.m_gameDataManager);
                AssetDatabase.SaveAssets();
                Destroy();
                CharacterEditorWindow.m_currentState = CharacterEditorWindow.ECharacterEditorState.Default;
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();

            if (GUILayout.Button(new GUIContent("Back"), CharacterEditorWindow.m_skin.button, GUILayout.Width(120)))
            {
                Destroy();
                CharacterEditorWindow.m_currentState = CharacterEditorWindow.ECharacterEditorState.Default;
            }

            if (GUILayout.Button(new GUIContent("Remove Entity Data"), CharacterEditorWindow.m_skin.button, GUILayout.Width(120)))
            {
                CharacterEditorWindow.m_gameDataManager.RemoveEntityData(CharacterEditorWindow.m_activeEntityData);
                EditorUtility.SetDirty(CharacterEditorWindow.m_gameDataManager);
                AssetDatabase.SaveAssets();
                Destroy();
                CharacterEditorWindow.m_currentState = CharacterEditorWindow.ECharacterEditorState.Default;
            }
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();


            GUILayout.Space(20);

            GUILayout.BeginHorizontal();
            GUILayout.Space(50);
            GUILayout.FlexibleSpace();

            CharacterDataRegion();
            GUILayout.Space(700);
            GUILayout.EndHorizontal();
        }
        
    }

    private void SaveCharacter()
    {
        MobaEntityData entityData = CharacterEditorWindow.m_activeEntityData;
        if (CharacterEditorWindow.m_entityHolderPrefab.RemovePrefab(entityData.m_prefab) && CharacterEditorWindow.m_entityHolderPrefab.RemovePrefab(entityData.m_prefab + "Client"))
        {
            GameObject newClientPrefab =  PrefabUtility.ReplacePrefab(m_characterInstance, CharacterEditorWindow.m_clientEntityPrefab, ReplacePrefabOptions.Default);
            GameObject newServerPrefab = PrefabUtility.ReplacePrefab(m_characterInstance, CharacterEditorWindow.m_serverEntityPrefab, ReplacePrefabOptions.Default);
            CharacterEditorWindow.m_entityHolderPrefab.AddPrefab(newServerPrefab, true);
            CharacterEditorWindow.m_entityHolderPrefab.AddPrefab(newClientPrefab);
            EditorUtility.SetDirty(CharacterEditorWindow.m_entityHolderPrefab);
            AssetDatabase.SaveAssets();
        }
    }

    private void ResetCharacterPosition()
    {
        m_characterInstance.transform.position = new Vector3(0, 0, 0);
        m_characterInstance.transform.rotation = Quaternion.Euler(0, 180, 0);
        (m_parent as CharacterEditorWindow).m_characterCamera.orthographic = false;
        (m_parent as CharacterEditorWindow).m_characterCamera.transform.localPosition = new Vector3(0, 0, -5);
    }

    private void CharacterDataRegion()
    {
        GUILayout.BeginVertical();

        GUILayout.Label("Entity Data ", CharacterEditorWindow.m_skin.label);

        GUILayout.Space(10);

        CharacterEditorWindow.m_activeEntityData.DrawEditor(CharacterEditorWindow.m_skin);
        DrawLoadSave();

        EditorUtility.SetDirty(CharacterEditorWindow.m_gameDataManager);

        GUILayout.EndVertical();
    }

    private void DrawLoadSave()
    {
        //Draw Globals
        EditorGUILayout.BeginHorizontal();
        
        if (GUILayout.Button("Save Entity Data"))
        {

            string savePath;
            if (Utils.SaveFileDialog(CharacterEditorWindow.m_activeEntityData.DataFileExtension(), "Save Entity Data", out savePath))
            {
                Utils.Save(CharacterEditorWindow.m_activeEntityData, savePath);
            }
        }
        if (GUILayout.Button("Load Entity Data"))
        {
            string path;
            if (Utils.OpenFileDialog(CharacterEditorWindow.m_activeEntityData.DataFileExtension(), "Load Entity Data", out path))
            {                          
                MobaEntityData newEntityData = CharacterEditorWindow.m_activeEntityData.LoadEntityData(path);

                if (CharacterEditorWindow.m_gameDataManager.ReplaceEntityData(CharacterEditorWindow.m_activeEntityData, newEntityData))
                {
                    CharacterEditorWindow.m_activeEntityData = newEntityData;
                    EditorUtility.SetDirty(CharacterEditorWindow.m_gameDataManager);
                    AssetDatabase.SaveAssets();
                    Debug.Log("Entity Data Replaced");
                }
                else
                {
                    Debug.LogError("Error: Cant replace entity data");
                }
              
            }
        }
        EditorGUILayout.EndHorizontal();
    }
    private void CharacterAnimationRegion()
    {
        GUILayout.BeginVertical();

        GUILayout.BeginHorizontal();

        GUILayout.BeginVertical();

        GUILayout.Space(5);

        GUILayout.Label("Add Animation ", CharacterEditorWindow.m_skin.label);

        GUILayout.EndVertical();

        GUILayout.Space(5);

        GUILayout.BeginVertical();

        GUILayout.Space(8);

        m_animationOptionSelectedIndex = EditorGUILayout.Popup(m_animationOptionSelectedIndex, m_animationClipOptions);

        GUILayout.EndVertical();

        GUILayout.Space(5);

        if (GUILayout.Button(new GUIContent(""), CharacterEditorWindow.m_skin.customStyles[1], GUILayout.Width(36), GUILayout.Height(36)))
        {
            string clipName = m_animationClipOptions[m_animationOptionSelectedIndex];
            if (!m_clips.ContainsKey(clipName))
            {
                m_clips.Add(clipName, null);
                EEntityState animation = (EEntityState)Enum.Parse(typeof(EEntityState), clipName);
                if (!CharacterEditorWindow.m_activeEntityData.AnimationDataExist(animation))
                {
                    AnimationData def = new AnimationData(animation, 1);
                    CharacterEditorWindow.m_activeEntityData.AddAnimationData(def);
                }
            }
            else
            {
                Debug.LogWarning("Error trying to add an already existent clip");
            }
        }

        GUILayout.FlexibleSpace();

        GUILayout.EndHorizontal();

        List<string> clipKeys = new List<string>(m_clips.Keys);

        foreach (string activeClipName in clipKeys)
        {
            DrawAnimationClipField(activeClipName);
        }

        GUILayout.EndVertical();
    }

    private void CharacterModelRegion()
    {
        GUILayout.BeginVertical();

        if(m_sectionWidth > 100)
        {
            GUILayout.Space(20);

            GUILayout.BeginHorizontal();

            if((m_parent as CharacterEditorWindow).m_characterCamera.targetTexture != null)
            {
                GUI.DrawTexture(new Rect(0, 75, 512, 512), (m_parent as CharacterEditorWindow).m_characterCamera.targetTexture);
            }

            GUILayout.Space(520);

            CharacterDataRegion();

            GUILayout.Space(10);

            if (m_animation != null)
            {
                CharacterAnimationRegion();
            }


            GUILayout.Space(50);

            GUILayout.FlexibleSpace();

            GUILayout.EndHorizontal();
        }

        GUILayout.EndVertical();
    }

    private void DrawAnimationClipField(string clipName)
    {
        GUILayout.BeginHorizontal();

        GUILayout.Label(clipName, CharacterEditorWindow.m_skin.label);

        GUILayout.Space(5);
        
        AnimationClip clipObject = (AnimationClip)EditorGUILayout.ObjectField(m_clips[clipName], typeof(AnimationClip), false);
        if(clipObject != null && clipObject != m_clips[clipName])
        {
            m_clips[clipName] = clipObject;
        }

        GUILayout.Space(5);

        if (m_clips[clipName] != null && !ClipExist(clipName))
        {
            if (GUILayout.Button("Add", CharacterEditorWindow.m_skin.button))
            {
                m_animation.AddClip(m_clips[clipName], clipName);
            }
        }
        else if (m_clips[clipName] != null && ClipExist(clipName) && !m_playingAnimation)
        {
            if (GUILayout.Button("Play", CharacterEditorWindow.m_skin.button))
            {
                m_clips[clipName] = m_animation.GetClip(clipName);
                m_animationLength = m_clips[clipName].length;
                m_currentTime = 0;
                m_activeAnimationName = clipName;
                m_playingAnimation = true;
                m_deltaTime = EditorApplication.timeSinceStartup;
            }

            GUILayout.Space(5);

            if(GUILayout.Button("Remove", CharacterEditorWindow.m_skin.button))
            {
                m_animation.RemoveClip(m_animation.GetClip(clipName));
                m_clips.Remove(clipName);
            }
        }

        GUILayout.Space(5);

        EEntityState animation = (EEntityState)Enum.Parse(typeof(EEntityState), clipName);

        if (CharacterEditorWindow.m_activeEntityData.AnimationDataExist(animation))
        {
            AnimationData def = CharacterEditorWindow.m_activeEntityData.GetAnimationData(animation);
            GUILayout.Label(new GUIContent("Animation Speed: ", "Normal Speed = 1"), CharacterEditorWindow.m_skin.label);
            def.m_animationSpeed = EditorGUILayout.FloatField(def.m_animationSpeed, CharacterEditorWindow.m_skin.textField, GUILayout.Width(25), GUILayout.Height(20));
        }


        GUILayout.FlexibleSpace();

        GUILayout.EndHorizontal();
    }

    private bool ClipExist(string clipName)
    {
        return m_animation.GetClip(clipName) != null;
    }

    public void Update()
    {
        if(!Initialized)
        {
            return;
        }

        if ((m_parent as CharacterEditorWindow).m_characterCamera != null)
        {
            (m_parent as CharacterEditorWindow).m_characterCamera.Render();
        }

        if (m_playingAnimation)
        {
            // Force editor repaint to prevent frame drops
            (m_parent as CharacterEditorWindow).Repaint();
            m_clips[m_activeAnimationName].SampleAnimation(m_animation.gameObject, m_currentTime);
            AnimationData animationDef = CharacterEditorWindow.m_activeEntityData.GetAnimationData((EEntityState)Enum.Parse(typeof(EEntityState), m_activeAnimationName));

            m_currentTime += (float)(EditorApplication.timeSinceStartup - m_deltaTime) * animationDef.m_animationSpeed;
            m_deltaTime = EditorApplication.timeSinceStartup;
            if (m_currentTime > m_animationLength)
            {
                m_playingAnimation = false;
            }
        }
    }
}

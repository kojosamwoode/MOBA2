using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEngine.Networking;
using System;

public class CreateHolderSection : BaseTemplate
{
    private string m_characterName;
    private bool m_showHolderWarningAlert;
    private int m_entityLogicChoice;
    private string[] m_entityTypesLogics;
    private GameObject m_characterModel;
    private GUIStyle m_textFieldStyle;

    private bool m_errorOnModel;
    private bool m_errorOnName;
    private bool m_holderCreated;

    private float m_sectionWidth;

    public override void Initialize()
    {
        base.Initialize();
        m_entityTypesLogics = CharacterEditorWindow.GetEffectClasses<Logic>();
        m_entityLogicChoice = 0;
        m_characterName = string.Empty;
        m_textFieldStyle = new GUIStyle(CharacterEditorWindow.m_skin.textField);
        m_textFieldStyle.fixedWidth = 200;
        m_errorOnModel = false;
        m_errorOnName = false;
        m_holderCreated = false;
        m_characterModel = null;
    }

    public override void Destroy()
    {
        base.Destroy();
    }

    public override void Render()
    {
        base.Render();

        m_sectionWidth = m_windowWidth - CharacterListSection.m_listWidth;

        if (m_holderCreated)
        {
            DrawRegion(CharacterListSection.m_listWidth + 15, 150, m_sectionWidth, 60, () => {

                GUILayout.Label("Entity Holder has been created! refresh the character list and select a character to edit!", CharacterEditorWindow.m_skin.label);

                if (GUILayout.Button(new GUIContent("Back"), CharacterEditorWindow.m_skin.button, GUILayout.Width(120)))
                {
                    Destroy();
                    CharacterEditorWindow.m_currentState = CharacterEditorWindow.ECharacterEditorState.Default;
                }

            }, m_defaultColor, true);
        }
        else
        {
            RenderHolderCreation();
        }
    }

    private void RenderHolderCreation()
    {
        DrawRegionWithDefaultContent(CharacterListSection.m_listWidth + 15, 150, m_sectionWidth, 60, m_defaultColor, true);

        if (m_showHolderWarningAlert)
        {
            GUIStyle labelWarningStyle = CreateTextStyle(GUI.skin.label, TextAnchor.MiddleLeft, FontStyle.Bold, 11, Color.white, 360, 35);
            labelWarningStyle.padding = new RectOffset(10, 0, 0, 0);
            labelWarningStyle.wordWrap = true;

            string errorMsg = string.Empty;

            if (m_errorOnModel)
            {
                errorMsg = "(!!!) Can't create a entityu without a entity model";
            }

            if (m_errorOnName)
            {
                errorMsg = "(!!!) Invalid Entity Name (Must be unique and not empty)";
            }

            if (m_errorOnModel && m_errorOnName)
            {
                errorMsg = "(!!!) You must select a model and a name to create a character!";
            }

            EditorGUI.LabelField(new Rect(m_windowWidth - 360, 0, 150, 25), errorMsg, labelWarningStyle);
        }

        DrawRegion(CharacterListSection.m_listWidth + 15, 220, m_sectionWidth, m_windowHeight - 280, ModelDropRegionContent, m_defaultColor, true);
    }

    protected override void DefaultRegionContent()
    {
        base.DefaultRegionContent();

        GUILayout.BeginHorizontal();

        GUILayout.Label("Entity Name", CharacterEditorWindow.m_skin.label);

        m_characterName = GUILayout.TextField(m_characterName, m_textFieldStyle);

        GUILayout.Space(5);

        GUILayout.Label("Entity Logic", CharacterEditorWindow.m_skin.label);

        GUILayout.BeginVertical();

        GUILayout.Space(5);

        m_entityLogicChoice = EditorGUILayout.Popup(m_entityLogicChoice, m_entityTypesLogics);

        GUILayout.EndVertical();

        GUILayout.Space(40);

        if (GUILayout.Button(new GUIContent("Create\nEntity Holder", "A character holder is required to create a character"), CharacterEditorWindow.m_skin.button, GUILayout.Width(100), GUILayout.Height(35)))
        {
            CheckCharacterName();
            CheckIfModelExists();

            if (CanCreateEntity())
            {
                CreateEntity();
            }
            else
            {
                Debug.LogWarning("Can't create a Entity: " + m_characterName + " empty? or " + m_characterModel + " null?");
            }
        }

        GUILayout.Space(5);

        if (GUILayout.Button(new GUIContent("Back"), CharacterEditorWindow.m_skin.button, GUILayout.Width(100), GUILayout.Height(35)))
        {
            Destroy();
            CharacterEditorWindow.m_currentState = CharacterEditorWindow.ECharacterEditorState.Default;
        }

        GUILayout.FlexibleSpace();

        GUILayout.Space(10);

        GUILayout.EndHorizontal();
    }

    private bool CanCreateEntity()
    {
        m_showHolderWarningAlert = m_errorOnName;
        return !m_showHolderWarningAlert;
    }

    private void ModelDropRegionContent()
    {
        GUILayout.Label("Drop Entity Model Here", CharacterEditorWindow.m_skin.label);
        m_characterModel = (GameObject) EditorGUILayout.ObjectField(m_characterModel, typeof(GameObject), false, GUILayout.Width(400));
    }

    private bool CheckCharacterName()
    {
        if (!CharacterEditorWindow.m_gameDataManager.EntityData.ContainsKey(m_characterName))
        {
            return true;
        }
        else
        {
            m_errorOnName = true;
            return false;
        }
    }

    private void CheckIfModelExists()
    {
        m_errorOnModel = m_characterModel == null;
    }

    private void CreateEntity()
    {
        Debug.Log("Creating entity...");

        m_showHolderWarningAlert = false;
             
        if (!m_errorOnModel)
        {
            //If there is no error with the model. Create the entity prefab and the entity data
            string absoluteAssetPath = Application.dataPath;
            string relativePath = "/MobaStorm/Prefabs/Entities/" + m_characterName;
            string characterFolderPath = absoluteAssetPath + relativePath;
            Debug.Log("Creating Entity folder at " + characterFolderPath);
            System.IO.Directory.CreateDirectory(characterFolderPath);
            CreateCharacter(relativePath, CharacterEditorWindow.m_entityHolderPrefab);
        }
        else
        {
            //Generate only the entity data
            var selectedLogicType = m_entityTypesLogics[m_entityLogicChoice];
            System.Type logicType = CharacterEditorWindow.GetType(selectedLogicType);
            GameObject dummyModel = new GameObject();
            Logic logic = dummyModel.AddComponent(logicType) as Logic;
            CreateCharacterData(logic);
            GameObject.DestroyImmediate(dummyModel);
        }
        m_holderCreated = true;
    }
    
    private void CreateCharacterData(Logic characterLogic)
    {
        var entityData = characterLogic.GetMobaEntityData();
        entityData.m_dataIdentifier = "Entity_" + m_characterName;
        entityData.m_prefab = "Entity_" + m_characterName;

        CharacterEditorWindow.m_gameDataManager.AddEntityData(entityData);
        CharacterEditorWindow.m_gameDataManager.LoadAllData();
    }

    private void CreateCharacter(string folderPath, PrefabHolder holder)
    {
        folderPath = "Assets" + folderPath;

        // CLIENT PREFAB

        GameObject clientTemp = new GameObject();
        GameObject modelClient = CharacterEditorWindow.Instantiate(m_characterModel);
        modelClient.name = m_characterName + " - Idle";
        modelClient.transform.SetParent(clientTemp.transform);
        clientTemp.name = "Entity_" + m_characterName + "Client";
        clientTemp.layer = LayerMask.NameToLayer("Entity");


        clientTemp.AddComponent<EntityCanvas>();
        clientTemp.AddComponent<EntityBehaviour>();
        NetworkIdentity networkIdentity = clientTemp.GetComponent<NetworkIdentity>();
        

        BoxCollider boxColliderClient = clientTemp.AddComponent<BoxCollider>();
        boxColliderClient.center = new Vector3(0, 0.5f, 0);
        boxColliderClient.size = new Vector3(0.5f, 1, 0.5f);
        boxColliderClient.isTrigger = true;

        var selectedLogicType = m_entityTypesLogics[m_entityLogicChoice];
        System.Type logicType = CharacterEditorWindow.GetType(selectedLogicType);
        Logic logic = clientTemp.AddComponent(logicType) as Logic;
        logic.SetMobaEntity(clientTemp);
        networkIdentity.localPlayerAuthority = logic.IsLocalPlayerAuthority();

        EntityTransform modelTransform = modelClient.AddComponent<EntityTransform>();
        modelTransform.EntityTransformType = EEntityTransform.Model;

        GameObject transformLeftHandGOClient = new GameObject();
        transformLeftHandGOClient.name = "Transform_" + EEntityTransform.LeftHand;
        EntityTransform leftHandTransformClient = transformLeftHandGOClient.AddComponent<EntityTransform>();
        leftHandTransformClient.EntityTransformType = EEntityTransform.LeftHand;

        GameObject transformRightHandGOClient = new GameObject();
        transformRightHandGOClient.name = "Transform_" + EEntityTransform.RightHand;
        EntityTransform rightHandTransformClient = transformRightHandGOClient.AddComponent<EntityTransform>();
        rightHandTransformClient.EntityTransformType = EEntityTransform.RightHand;

        GameObject transformCenterGOClient = new GameObject();
        transformCenterGOClient.name = "Transform_" + EEntityTransform.Center;
        EntityTransform centerTransformClient = transformCenterGOClient.AddComponent<EntityTransform>();
        centerTransformClient.EntityTransformType = EEntityTransform.Center;

        GameObject transformCanvasGOClient = new GameObject();
        transformCanvasGOClient.name = "Transform_" + EEntityTransform.Head;
        EntityTransform canvasTransformClient = transformCanvasGOClient.AddComponent<EntityTransform>();
        canvasTransformClient.EntityTransformType = EEntityTransform.Head;

        GameObject transformFloorGOClient = new GameObject();
        transformFloorGOClient.name = "Transform_" + EEntityTransform.Floor;
        EntityTransform floorTransformClient = transformFloorGOClient.AddComponent<EntityTransform>();
        floorTransformClient.EntityTransformType = EEntityTransform.Floor;

        GameObject transformSkyGOClient = new GameObject();
        transformSkyGOClient.name = "Transform_" + EEntityTransform.Sky;
        EntityTransform skyTransformClient = transformSkyGOClient.AddComponent<EntityTransform>();
        skyTransformClient.EntityTransformType = EEntityTransform.Sky;

        

        transformRightHandGOClient.transform.SetParent(clientTemp.transform);
        transformRightHandGOClient.transform.localPosition = new Vector3(0, 0.5f, 0.25f);
        transformLeftHandGOClient.transform.SetParent(clientTemp.transform);
        transformLeftHandGOClient.transform.localPosition = new Vector3(0, 0.5f, 0.25f);

        transformCenterGOClient.transform.SetParent(clientTemp.transform);
        transformCenterGOClient.transform.localPosition = new Vector3(0, 0.5f, 0);

        transformCanvasGOClient.transform.SetParent(clientTemp.transform);
        transformCanvasGOClient.transform.localPosition = new Vector3(0, 1.3f, 0);

        transformFloorGOClient.transform.SetParent(clientTemp.transform);
        transformSkyGOClient.transform.SetParent(clientTemp.transform);
        transformSkyGOClient.transform.localPosition = new Vector3(0, 5f, 0);


        Debug.Log("Create Client prefab at " + folderPath + "/" + clientTemp.name + ".prefab");
        UnityEngine.Object emptyPrefab = PrefabUtility.CreateEmptyPrefab(folderPath + "/" + clientTemp.name + ".prefab");
        GameObject clientPrefab = PrefabUtility.ReplacePrefab(clientTemp, emptyPrefab, ReplacePrefabOptions.ConnectToPrefab);

        CreateCharacterData(clientTemp.GetComponent<Logic>());

        GameObject.DestroyImmediate(GameObject.Find(clientTemp.name));

        // SERVER PREFAB

        GameObject serverTemp = new GameObject();
        GameObject modelServer = CharacterEditorWindow.Instantiate(m_characterModel);
        modelServer.name = m_characterName + " - Idle";
        modelServer.transform.SetParent(serverTemp.transform);
        serverTemp.name = "Entity_" + m_characterName;
        serverTemp.layer = LayerMask.NameToLayer("Entity");

        serverTemp.AddComponent<EntityCanvas>();
        serverTemp.AddComponent<EntityBehaviour>();
        NetworkIdentity networkIdentityServer = serverTemp.GetComponent<NetworkIdentity>();
        networkIdentityServer.localPlayerAuthority = true;

        BoxCollider boxColliderServer = serverTemp.AddComponent<BoxCollider>();
        boxColliderServer.center = new Vector3(0, 0.5f, 0);
        boxColliderServer.size = new Vector3(0.5f, 1, 0.5f);
        boxColliderServer.isTrigger = true;

        Logic serverLogic = serverTemp.AddComponent(logicType) as Logic;
        serverLogic.SetMobaEntity(serverTemp);
        networkIdentityServer.localPlayerAuthority = serverLogic.IsLocalPlayerAuthority();

        EntityTransform modelServerTransform = modelServer.AddComponent<EntityTransform>();
        modelServerTransform.EntityTransformType = EEntityTransform.Model;

        GameObject transformLeftHandGOServer = new GameObject();
        transformLeftHandGOServer.name = "Transform_" + EEntityTransform.LeftHand;
        EntityTransform leftHandTransformServer = transformLeftHandGOServer.AddComponent<EntityTransform>();
        leftHandTransformServer.EntityTransformType = EEntityTransform.LeftHand;

        GameObject transformRightHandGOServer = new GameObject();
        transformRightHandGOServer.name = "Transform_" + EEntityTransform.RightHand;
        EntityTransform rightHandTransformServer = transformRightHandGOServer.AddComponent<EntityTransform>();
        rightHandTransformServer.EntityTransformType = EEntityTransform.RightHand;

        GameObject transformCenterGOServer = new GameObject();
        transformCenterGOServer.name = "Transform_" + EEntityTransform.Center;
        EntityTransform centerTransformServer = transformCenterGOServer.AddComponent<EntityTransform>();
        centerTransformServer.EntityTransformType = EEntityTransform.Center;

        GameObject transformCanvasGOServer = new GameObject();
        transformCanvasGOServer.name = "Transform_" + EEntityTransform.Head;
        EntityTransform canvasTransformServer = transformCanvasGOServer.AddComponent<EntityTransform>();
        canvasTransformServer.EntityTransformType = EEntityTransform.Head;

        GameObject transformFloorGOServer = new GameObject();
        transformFloorGOServer.name = "Transform_" + EEntityTransform.Floor;
        EntityTransform floorTransformServer = transformFloorGOServer.AddComponent<EntityTransform>();
        floorTransformServer.EntityTransformType = EEntityTransform.Floor;

        GameObject transformSkyGOServer = new GameObject();
        transformSkyGOServer.name = "Transform_" + EEntityTransform.Sky;
        EntityTransform skyTransformServer = transformSkyGOServer.AddComponent<EntityTransform>();
        skyTransformServer.EntityTransformType = EEntityTransform.Sky;

        transformRightHandGOServer.transform.SetParent(serverTemp.transform);
        transformRightHandGOServer.transform.localPosition = new Vector3(0, 0.5f, 0.25f);
        transformLeftHandGOServer.transform.SetParent(serverTemp.transform);
        transformLeftHandGOServer.transform.localPosition = new Vector3(0, 0.5f, 0.25f);

        transformCenterGOServer.transform.SetParent(serverTemp.transform);
        transformCenterGOServer.transform.localPosition = new Vector3(0, 0.5f, 0);

        transformCanvasGOServer.transform.SetParent(serverTemp.transform);
        transformCanvasGOServer.transform.localPosition = new Vector3(0, 1.3f, 0);

        transformFloorGOServer.transform.SetParent(serverTemp.transform);
        transformSkyGOServer.transform.SetParent(serverTemp.transform);
        transformSkyGOServer.transform.localPosition = new Vector3(0, 5f, 0);


        UnityEngine.Object emptyPrefabServer = PrefabUtility.CreateEmptyPrefab(folderPath + "/" + serverTemp.name + ".prefab");
        GameObject serverPrefab = PrefabUtility.ReplacePrefab(serverTemp, emptyPrefabServer, ReplacePrefabOptions.ConnectToPrefab);
        GameObject.DestroyImmediate(GameObject.Find(serverTemp.name));

        PoolData clientCharacterPoolData = new PoolData();
        clientCharacterPoolData.m_quantity = 0;
        clientCharacterPoolData.m_register = false;
        clientCharacterPoolData.m_obj = clientPrefab;

        PoolData serverCharacterPoolData = new PoolData();
        serverCharacterPoolData.m_quantity = 0;
        serverCharacterPoolData.m_register = true;
        serverCharacterPoolData.m_obj = serverPrefab;

        holder.m_poolDataList.Add(clientCharacterPoolData);
        holder.m_poolDataList.Add(serverCharacterPoolData);

        PrefabUtility.ReplacePrefab(SpawnManager.instance.gameObject, PrefabUtility.GetPrefabParent(SpawnManager.instance), ReplacePrefabOptions.ConnectToPrefab);
    }
}

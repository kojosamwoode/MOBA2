using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Networking;
/// <summary>
/// This script is attached to the player and it draws the healthbar of players above them.
/// This script accesses the PlayerStats script for 
/// determining the healthbar length.

public class EntityCanvas : NetworkBehaviour {

    CharacterCanvas m_characterCanvas;
    public CharacterCanvas CharacterCanvas
    {
        get { return m_characterCanvas; }
    }
    MobaEntity m_entity;
	public void Initialize (MobaEntity entity)
	{
        if (m_entity == null)
        {
            m_entity = entity;
            if (!string.IsNullOrEmpty(entity.EntityData.m_canvasPrefab))
            {
                GameObject canvasObj = SpawnManager.instance.InstantiatePool(entity.EntityData.m_canvasPrefab, Vector3.zero, Quaternion.identity);
                EntityTransform canvasTrasnform = m_entity.GetTransformPosition(EEntityTransform.Head);
                canvasObj.transform.SetParent(canvasTrasnform.transform);
                canvasObj.transform.localPosition = Vector3.zero;
                m_characterCanvas = canvasObj.GetComponent<CharacterCanvas>();
                m_characterCanvas.Initialize(m_entity);
            }
        }
        else
        {
            m_characterCanvas.Initialize(m_entity);
        }
         
    }   

    public void ShowFloatingText(FloatingText.FloatingTextType type , string text)
    {
        if (m_characterCanvas == null)
            return;
        GameObject floatingObj =  SpawnManager.instance.InstantiatePool(type.ToString(), Vector3.zero, Quaternion.identity);
        FloatingText floatingText = floatingObj.GetComponent<FloatingText>();
        floatingText.ShowFloatingText(type, text);
        floatingText.transform.SetParent(m_characterCanvas.transform);
        floatingText.transform.localPosition = Vector3.zero;
        floatingText.transform.rotation = m_characterCanvas.transform.rotation;
        RpcShowFloatingText((int)type, text);
    }
    [ClientRpc]
    public void RpcShowFloatingText(int type, string text)
    {
        if (m_characterCanvas == null)
            return;
        FloatingText.FloatingTextType textType = (FloatingText.FloatingTextType)type;
        GameObject floatingObj = SpawnManager.instance.InstantiatePool(textType.ToString(), Vector3.zero, Quaternion.identity);
        FloatingText floatingText = floatingObj.GetComponent<FloatingText>();
        floatingText.ShowFloatingText(textType, text);
        floatingText.transform.SetParent(m_characterCanvas.transform);
        floatingText.transform.localPosition = Vector3.zero;
        floatingText.transform.rotation = m_characterCanvas.transform.rotation;
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class InventoryManager : NetworkBehaviour {

    private static InventoryManager m_instance;
    public static InventoryManager Instance
    {
        get { return m_instance; }
    }

    public List<BaseItemContainer> m_items = new List<BaseItemContainer>();

    private Dictionary<string, BaseItemContainer> m_itemsDict = new Dictionary<string, BaseItemContainer>();

    private void Awake()
    {
        m_instance = this;
    }

    void Start()
    {
        foreach (BaseItemContainer item in m_items)
        {
            m_itemsDict.Add(item.name, item);       
        }

    }

    /// <summary>
    /// Get an Item from its Identifier
    /// </summary>
    /// <param name="identifier">Item Identifier</param>
    /// <returns>AbilityBase</returns>
    public BaseItemContainer GetItem(string identifier)
    {
        if (m_itemsDict.ContainsKey(identifier))
        {
            return m_itemsDict[identifier];
        }

        return null;
    }



    


}

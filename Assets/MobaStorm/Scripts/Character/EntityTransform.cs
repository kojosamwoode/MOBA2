using UnityEngine;
using System.Collections;


public class EntityTransform : MonoBehaviour {

    [SerializeField]
    private EEntityTransform m_entityTransform;
    public EEntityTransform EntityTransformType
    {
        get { return m_entityTransform; }
        set { m_entityTransform = value; }
    }

}

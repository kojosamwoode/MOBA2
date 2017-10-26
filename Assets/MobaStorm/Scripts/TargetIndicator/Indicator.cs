using UnityEngine;
using System.Collections;

public abstract class Indicator : MonoBehaviour {

    protected MobaEntity m_entity;
    public MobaEntity Entity
    {
        get { return m_entity; }
        set { m_entity = value; }
    }

    protected Ability m_ability;
    public Ability Ability
    {
        get { return m_ability; }
        set { m_ability = value; }
    }

    public virtual void Initialize(MobaEntity entity, Ability ability, EAttackType attackType)
    {
        m_entity = entity;
        m_ability = ability;
        m_attackType = attackType;
    }

    private EAttackType m_attackType;
    public EAttackType AttackType
    {
        get { return m_attackType; }
        set { m_attackType = value; }
    }



    public abstract void OnFinish();

    // Update is called once per frame
    public virtual void Update()
    {
    }

    public virtual ImageIndicator CreateImageIndicator(string indicatorPrefab, Vector3 imageSize)
    {
        GameObject rangueIndicatorObj = SpawnManager.instance.InstantiatePool(indicatorPrefab, transform.position, transform.rotation);
        ImageIndicator imageIndicator = rangueIndicatorObj.GetComponent<ImageIndicator>();
        imageIndicator.transform.SetParent(this.transform);
        imageIndicator.gameObject.transform.localScale = imageSize;
        return imageIndicator;
    }

    //public virtual void OnFinish()
    //{
    //
    //}
    //public virtual void OnStart()
    //{
    //
    //}
    // Use this for initialization

}

using UnityEngine;
using System.Collections;
using System;

public class BasicIndicator : Indicator ,IPooled{
    private ImageIndicator m_rangueImageIndicator;
    private ImageIndicator m_targetImageIndicator;
    [SerializeField]
    private LayerMask floorMask;
    public override void Initialize(MobaEntity entity, Ability ability, EAttackType attackType)
    {
        base.Initialize(entity, ability, attackType);
        //Ability holder = m_entity.EntityAbilities.GetAbility(ability.AttackType);
        switch (m_ability.AbilityBase.TargetType)
        {
            case ETargetType.EntityTarget:
                m_rangueImageIndicator = CreateImageIndicator(m_ability.AbilityBase.RangeImageIndicator, new Vector3(ability.Range * 2, 1, ability.Range * 2));
                break;
            case ETargetType.PositionTarget:
                m_rangueImageIndicator = CreateImageIndicator(m_ability.AbilityBase.RangeImageIndicator, new Vector3(ability.Range * 2, 1, ability.Range * 2));
                if (m_ability.AbilityBase.SkillShotType == ESkillShotType.FloorSkill)
                {
                    m_targetImageIndicator = CreateImageIndicator(m_ability.AbilityBase.TargetImageIndicator ,new Vector3(ability.AbilityBase.AoeRange, 1, ability.AbilityBase.AoeRange));
                }
                else
                {
                    m_targetImageIndicator = CreateImageIndicator(m_ability.AbilityBase.TargetImageIndicator, Vector3.one);
                }
                break;
            case ETargetType.SelfTarget:
                break;
            case ETargetType.RandomTarget:
                break;
            case ETargetType.AllTarget:
                break;
            default:
                break;
        }

    }
  
	// Update is called once per frame
	public override void Update () {
        base.Update();
        if (m_entity)
        {
            transform.position = new Vector3(m_entity.transform.position.x, 0.015f, m_entity.transform.position.z);
            if (m_ability.AbilityBase.TargetType == ETargetType.PositionTarget)
            {
                if (m_ability.AbilityBase.SkillShotType == ESkillShotType.FloorSkill)
                {
                    RaycastHit hit;
                    if (Physics.Raycast(CameraController.instance.sources.currentCamera.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, floorMask))
                    {
                        m_targetImageIndicator.transform.position = new Vector3(hit.point.x, 0.015f, hit.point.z);
                    }
                }
                else
                {
                    RaycastHit hit;
                    if (Physics.Raycast(CameraController.instance.sources.currentCamera.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, floorMask))
                    {
                        Vector3 planePos = hit.point;
                        planePos.y = transform.position.y;
                        m_targetImageIndicator.transform.LookAt(planePos);
                        m_targetImageIndicator.transform.position = new Vector3(m_entity.transform.position.x, 0.015f, m_entity.transform.position.z);
                    }
                }
            }
        }
	
	}

    public override void OnFinish()
    {
        if (m_rangueImageIndicator)
        {
            SpawnManager.instance.DestroyPool(m_rangueImageIndicator.gameObject);
        }
        if (m_targetImageIndicator)
        {
            SpawnManager.instance.DestroyPool(m_targetImageIndicator.gameObject);
        }
    }

    public void OnInstantiate()
    {
    }

    public void OnUnSpawn()
    {
        OnFinish();
    }
}

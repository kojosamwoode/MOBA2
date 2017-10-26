using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class SideEffect : ScriptableObject {
    protected MobaEntity m_attacker;
    protected MobaEntity m_reciever;
    protected Ability m_ability;
    protected SideEffectPrefab m_prefabGameobject;


    public enum EUseType
    {
        Timed,
        Ticks,
        Permanent,
        ItemPassive,
    }

    [SerializeField]
    protected EUseType m_useType;
    public EUseType UseType
    {
        get { return m_useType; }
        set { m_useType = value; }
    }

    [SerializeField]
    private float m_duration;
    public float Duration
    {
        get { return m_duration; }
        set { m_duration = value; }
    }

    [SerializeField]
    protected int m_ticks;
    public int Ticks
    {
        get { return m_ticks; }
        set { m_ticks = value; }
    }

    [SerializeField]
    private string m_effectIdentifier = "";
    public string EffectIdentifier
    {
        get { return m_effectIdentifier; }
        set { m_effectIdentifier = value; }
    }
    [SerializeField]
    private string m_prefab = "";
    public string Prefab
    {
        get { return m_prefab; }
        set { m_prefab = value; }
    }

    [SerializeField]
    private string m_effectSound = "";
    public string EffectSound
    {
        get { return m_effectSound; }
        set { m_effectSound = value; }
    }
    [SerializeField]
    private int m_volume;
    public int Volume
    {
        get { return m_volume; }
        set { m_volume = value; }
    }

    [SerializeField]
    private bool m_loopSound = false;
    public bool LoopSound
    {
        get { return m_loopSound; }
        set { m_loopSound = value; }
    }

    [SerializeField]
    private EEntityTransform m_effectPos;
    public EEntityTransform EffectPos
    {
        get { return m_effectPos; }
        set { m_effectPos = value; }
    }


    [SerializeField]
    private bool m_isPermanent;
    public bool IsPermanent
    {
        get { return m_isPermanent; }
        set { m_isPermanent = value; }
    }

    protected float m_endTime = 0;
    protected float m_intervalTime = 0;

    public virtual void ApplyEffect(Ability ability, MobaEntity attacker, MobaEntity reciever)
    {
        m_ability = ability;
        m_attacker = attacker;
        m_reciever = reciever;
        m_endTime = Time.time + m_duration;
        m_intervalTime = m_duration / (float)m_ticks;

        if (!string.IsNullOrEmpty(m_prefab))
        {
            EntityTransform entityTransform = reciever.GetTransformPosition(m_effectPos);
            m_prefabGameobject = reciever.EntityAbilities.SpawnSideEffectPrefab(this, entityTransform);
            m_prefabGameobject.transform.SetParent(entityTransform.transform);
        }
    }


    public virtual void ProcessEffect()
    {
        switch (m_useType)
        {
            case EUseType.Timed:
                if (Time.time > m_endTime)
                {
                    Isfinish = true;
                }
                break;
            case EUseType.Ticks:
                m_intervalTime -= Time.fixedDeltaTime;
                if (m_intervalTime <= 0)
                {
                    ApplyTick();
                    m_intervalTime = m_duration / (float)m_ticks;
                }
                if (Time.fixedTime > m_endTime)
                {
                    Isfinish = true;
                }
                break;
            case EUseType.Permanent:
                m_finished = true;
                break;
            case EUseType.ItemPassive:
                break;
            default:
                break;
        }      
    }

    public virtual void ApplyTick()
    {
        
    }

    public virtual void FinishEffect()
    {
        if (m_useType != EUseType.Permanent)
        {
            RemoveEffect();
        }       
    }


    public virtual void RemoveEffect()
    {
        if (m_prefabGameobject)
        {
            m_reciever.EntityAbilities.UnSpawnSideEffectPrefab(m_prefabGameobject);
        }
    }


    public virtual int DrawEffect(int xPos ,int yPos, GUISkin skin)
    {
        //int initPos = yPos;
#if UNITY_EDITOR
        EditorGUI.LabelField(new Rect(xPos + 150, yPos += 20, 150, 20), m_effectIdentifier, skin.label);
        EditorGUI.LabelField((new Rect(xPos, yPos, 180, 20)), new GUIContent("SideEffect Identifier: "), skin.label);

        m_prefab = EditorGUI.TextField(new Rect(xPos + 150, yPos += 20, 150, 20), m_prefab, skin.textField);
        EditorGUI.LabelField((new Rect(xPos, yPos, 180, 20)), new GUIContent("Effect Prefab Name: "), skin.label);

        m_effectSound = EditorGUI.TextField(new Rect(xPos + 150, yPos += 20, 150, 20), m_effectSound, skin.textField);
        EditorGUI.LabelField((new Rect(xPos, yPos, 180, 20)), new GUIContent("Side Effect Sound: "), skin.label);

        m_volume = EditorGUI.IntSlider(new Rect(xPos + 150, yPos += 20, 150, 20), m_volume, 0, 100);
        EditorGUI.LabelField((new Rect(xPos, yPos, 180, 20)), new GUIContent("Effect Sound Volume"), skin.label);

        m_loopSound = EditorGUI.Toggle(new Rect(xPos + 150, yPos += 20, 150, 20), m_loopSound);
        EditorGUI.LabelField((new Rect(xPos, yPos, 180, 20)), new GUIContent("Effect Sound Volume"), skin.label);

        m_effectPos = (EEntityTransform)EditorGUI.EnumPopup(new Rect(xPos + 150, yPos += 20, 150, 20), m_effectPos);
        EditorGUI.LabelField((new Rect(xPos, yPos, 180, 20)), new GUIContent("Launch Effect Pos: "), skin.label);

        m_useType = (EUseType)EditorGUI.EnumPopup(new Rect(xPos + 150, yPos += 20, 150, 20), m_useType);
        EditorGUI.LabelField((new Rect(xPos, yPos, 180, 20)), new GUIContent("Use Type: "), skin.label);

        switch (m_useType)
        {
            case EUseType.Timed:
                m_duration = EditorGUI.FloatField(new Rect(xPos + 150, yPos += 20, 200, 20), m_duration);
                EditorGUI.LabelField((new Rect(xPos, yPos, 180, 20)), new GUIContent("Effect Duration: "), skin.label);
                break;
            case EUseType.Ticks:
                m_ticks = EditorGUI.IntField(new Rect(xPos + 150, yPos += 20, 200, 20), m_ticks);
                EditorGUI.LabelField((new Rect(xPos, yPos, 180, 20)), new GUIContent("Effect Ticks: "), skin.label);
                break;
            case EUseType.Permanent:
                break;
            default:
                break;
        }

        EditorUtility.SetDirty(this);
#endif
        return yPos;
    }

    public SideEffect(SideEffect effect)
    {
        m_effectIdentifier = effect.m_effectIdentifier;
        m_prefab = effect.m_prefab;
        m_effectPos = effect.m_effectPos;
        m_effectSound = effect.m_effectSound;
        m_volume = effect.m_volume;
        m_loopSound = effect.m_loopSound;
        m_useType = effect.m_useType;
        m_isPermanent = effect.m_isPermanent;
        m_duration = effect.m_duration;
        m_ticks = effect.m_ticks;

    }
    public virtual SideEffect Clone()
    {
        return new SideEffect(this);
    }
    public bool CanApplyEffect()
    {
        return true;
    }

    private bool m_finished;
    public bool Isfinish
    {
        get { return m_finished; }
        set { m_finished = value; }
    }

}

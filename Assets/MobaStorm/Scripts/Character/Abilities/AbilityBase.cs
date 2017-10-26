using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif


public enum EAttackType
{
    Basic,
    Q,
    W,
    E,
    R,
    Passive,
    Special1,
    Special2,
    ItemSlot0,
    ItemSlot1,
    ItemSlot2,
    ItemSlot3,
    ItemSlot4,
    ItemSlot5,
    None,
}

public enum ETargetType
{
    EntityTarget,
    PositionTarget,
    SelfTarget,
    RandomTarget,
    AllTarget,
}

public enum ESkillShotType
{
    FrontSkill,
    FloorSkill,
}

public enum ELaunchingState
{
    CastingState,
    AnimationState,
}

public enum AbilityProcessorType 
{
    DamageImpact,
    LaunchAbility,
    CastSideEffectsOnly,
}

public enum EDamageLogic
{
    None,
    DamageTargetLogic,
    DamageAoeLogic,
    DamageCustomLogic,
}

public enum EDescriptionParameter
{
    Entity_BaseDamage,
    Entity_BaseAbility,
    Ability_AttackDamage,
    Ability_AbilityPower,
}

public class AbilityBase : ScriptableObject {


    [SerializeField]
    private bool m_requiresTarget;
    public bool RequiresTarget
    {
        get { return m_requiresTarget; }
        set { m_requiresTarget = value; }
    }

    [SerializeField]
    private int m_maxTargets;
    public int MaxTargets
    {
        get { return m_maxTargets; }
        set { m_maxTargets = value; }
    }

    [SerializeField]
    private EAllegiance m_allegiance;
    public EAllegiance Allegiance
    {
        get { return m_allegiance; }
        set { m_allegiance = value; }
    }

    [SerializeField]
    private bool m_canDamageTowers;
    public bool CanDamageTowers
    {
        get { return m_canDamageTowers; }
        set { m_canDamageTowers = value; }
    }

    [SerializeField]
    private ESkillShotType m_skillShotType;
    public ESkillShotType SkillShotType
    {
        get { return m_skillShotType; }
        set { m_skillShotType = value; }
    }


    [SerializeField]
    private EAttackType m_attackType;
    public EAttackType AttackType
    {
        get { return m_attackType; }
        set { m_attackType = value; }
    }

    [SerializeField]
    private ETargetType m_targetType;
    public ETargetType TargetType
    {
        get { return m_targetType; }
        set { m_targetType = value; }
    }
    [SerializeField]
    private string m_indicatorPrefab = "SimpleIndicator";
    public string IndicatorPrefab
    {
        get { return m_indicatorPrefab; }
        set { m_indicatorPrefab = value; }
    }

    [SerializeField]
    private string m_rangeImageIndicator = "SimpleRangueImage";
    public string RangeImageIndicator
    {
        get { return m_rangeImageIndicator; }
        set { m_rangeImageIndicator = value; }
    }
    [SerializeField]
    private string m_targetImageIndicator = "SimpleFloorImage";
    public string TargetImageIndicator
    {
        get { return m_targetImageIndicator; }
        set { m_targetImageIndicator = value; }
    }
    

    [SerializeField]
    private float m_detectionRange = 0.3f;
    public float DetectionRange
    {
        get { return m_detectionRange; }
        set { m_detectionRange = value; }
    }

    [SerializeField]
    private string m_identifier;
    public string Identifier
    {
        get { return m_identifier; }
        set { m_identifier = value; }
    }

    [SerializeField]
    private string m_abilityName;
    public string AbilityName
    {
        get { return m_abilityName; }
        set { m_abilityName = value; }
    }

    [SerializeField]
    private Sprite m_icon;
    public Sprite Icon
    {
        get { return m_icon; }
        set { m_icon = value; }
    }

    [SerializeField]
    private List<SideEffect> m_sideEffects = new List<SideEffect>();
    public List<SideEffect> SideEffects
    {
        get { return m_sideEffects; }
        set { m_sideEffects = value; }
    }

    [SerializeField]
    private bool m_faceTarget;
    public bool FaceTarget
    {
        get { return m_faceTarget; }
        set { m_faceTarget = value; }
    }

    public bool PlaysAnimation
    {
        get { return m_playAbilityAnimation || m_playCastAnimation; }
    }

    //Animation Parameters
    [SerializeField]
    private bool m_playAbilityAnimation;
    public bool PlayAbilityAnimation
    {
        get { return m_playAbilityAnimation; }
        set { m_playAbilityAnimation = value; }
    }
    [SerializeField]
    private EEntityState m_animation = EEntityState.AbilityQ;
    public EEntityState Animation
    {
        get { return m_animation; }
        set { m_animation = value; }
    }

    [SerializeField]
    private ELaunchingState m_launchingType = ELaunchingState.AnimationState;
    public ELaunchingState LaunchingType
    {
        get { return m_launchingType; }
        set { m_launchingType = value; }
    }

    [SerializeField]
    private int m_animationPercentLaunch = 0;
    public int AnimationPercentLaunch
    {
        get { return m_animationPercentLaunch; }
        set { m_animationPercentLaunch = value; }
    }

    [SerializeField]
    private bool m_playCastAnimation;
    public bool PlaysCastAnimation
    {
        get { return m_playCastAnimation; }
        set { m_playCastAnimation = value; }
    }

    [SerializeField]
    private float m_castTime = 0;
    public float CastTime
    {
        get { return m_castTime; }
        set { m_castTime = value; }
    }
    [SerializeField]
    private EEntityState m_castingAnimation = EEntityState.CastingQ;
    public EEntityState CastingAnimation
    {
        get { return m_castingAnimation; }
        set { m_castingAnimation = value; }
    }

    [SerializeField]
    private string m_castParticleIdentifier = "";
    public string CastParticleIdentifier
    {
        get { return m_castParticleIdentifier; }
        set { m_castParticleIdentifier = value; }
    }
    [SerializeField]
    private AbilityProcessorType m_processor;
    public AbilityProcessorType Processor
    {
        get { return m_processor; }
    }

    [SerializeField]
    private int m_projectileQuantity;
    public int ProjectileQuantity
    {
        get { return m_projectileQuantity; }
        set { m_projectileQuantity = value; }
    }

    [SerializeField]
    private float m_projectileSpeed;
    public float ProjectileSpeed
    {
        get { return m_projectileSpeed; }
        set { m_projectileSpeed = value; }
    }

    [SerializeField]
    private string m_projectileIdentifier;
    public string ProjectileIdentifier
    {
        get { return m_projectileIdentifier; }
        set { m_projectileIdentifier = value; }
    }

    [SerializeField]
    private EEntityTransform m_launchPos;
    public EEntityTransform LaunchPos
    {
        get { return m_launchPos; }
        set { m_launchPos = value; }
    }
    [SerializeField]
    private string m_launchSoundIdentifier = "";
    public string LaunchSoundIdentifier {  get { return m_launchSoundIdentifier; } }
    [SerializeField]
    private int m_launchSoundVolume = 100;
    public int LaunchSoundVolume { get { return m_launchSoundVolume; } }

    [SerializeField]
    private string m_launchParticleIdentifier = "";
    public string LaunchParticleIdentifier  { get { return m_launchParticleIdentifier; } }

    [SerializeField]
    private int m_launchParticlePercentLaunch = 0;
    public int LaunchParticlePercentLaunch
    {
        get { return m_launchParticlePercentLaunch; }
        set { m_launchParticlePercentLaunch = value; }
    }

    [SerializeField]
    private string m_impactParticleIdentifier = "";
    public string ImpactParticleIdentifier
    {
        get { return m_impactParticleIdentifier; }
        set { m_impactParticleIdentifier = value; }
    }
    [SerializeField]
    private string m_impactSoundIdentifier = "";
    public string ImpactSoundIdentifier
    {
        get { return m_impactSoundIdentifier; }
        set { m_impactSoundIdentifier = value; }
    }
    [SerializeField]
    private int m_impactSoundVolume = 100;
    public int ImpactSoundVolume { get { return m_impactSoundVolume; } }

    [SerializeField]
    private EDamageLogic m_damageLogic;
    public EDamageLogic DamageLogic
    {
        get { return m_damageLogic; }
        set { m_damageLogic = value; }
    }

    [SerializeField]
    private float m_aoeRange;
    public float AoeRange
    {
        get { return m_aoeRange; }
        set { m_aoeRange = value; }
    }
    [System.Serializable]
    public class AbilityDamageData
    {
        [SerializeField]
        private float m_baseAdDamage;
        public float BaseAdDamage
        {
            get { return m_baseAdDamage; }
            set { m_baseAdDamage = value; }
        }

        [SerializeField]
        private float m_baseApDamage;
        public float BaseApDamage
        {
            get { return m_baseApDamage; }
            set { m_baseApDamage = value; }
        }

        [SerializeField]
        private float m_range = 1;
        public float Range
        {
            get { return m_range; }
            set { m_range = value; }
        }

        [SerializeField]
        private float m_coldDownSeconds;
        public float ColdDownSeconds
        {
            get { return m_coldDownSeconds; }
            set { m_coldDownSeconds = value; }
        }

        [SerializeField]
        private float m_manaCost;
        public float ManaCost
        {
            get { return m_manaCost; }
            set { m_manaCost = value; }
        }
    }

    [SerializeField]
    private bool m_canLevelUp = true;
    public bool CanLevelUp { get { return m_canLevelUp; } }

    [SerializeField]
    private List<AbilityDamageData> m_abilityDamageData = new List<AbilityDamageData>();
    public List<AbilityDamageData> AbilityDamage
    {
        get { return m_abilityDamageData; }
    }


    [SerializeField]
    private string m_abilityDescription;

   

    [SerializeField]
    private List<EDescriptionParameter> m_descriptionParamList = new List<EDescriptionParameter>();
    public List<EDescriptionParameter> DescriptionParamList
    {
        get { return m_descriptionParamList; }
        set { m_descriptionParamList = value; }
    }
    private string[] m_effectClasses;
    private int m_effectClassIndex;

    public string GetAbilityDescription()
    {
        object[] args = new object[m_descriptionParamList.Count];
        for (int i = 0; i < m_descriptionParamList.Count; i++)
        {
            if (m_descriptionParamList[i].ToString().Contains("Entity"))
            {
                string property = m_descriptionParamList[i].ToString().Replace("Entity_", "");
                args[i] = GetPropValue(GameManager.instance.LocalPlayer.CharacterEntity, property);
            }
        }
        string test = string.Format(m_abilityDescription, args);
        return test;
    }

    public static object GetPropValue(object src, string propName)
    {
        return src.GetType().GetProperty(propName).GetValue(src, null);
    }

    public static IEnumerable<Type> GetAllSubclassOf(Type parent)
    {
        foreach (var a in AppDomain.CurrentDomain.GetAssemblies())
            foreach (var t in a.GetTypes())
                if (t.IsSubclassOf(parent)) yield return t;
    }

    public static string[] GetEffectClasses()
    {
        List<string> classes = new List<string>();
        foreach (var a in AppDomain.CurrentDomain.GetAssemblies())
        {
            foreach (var t in a.GetTypes())
            {
                if (t.IsSubclassOf(typeof(SideEffect)))
                {
                    classes.Add(t.ToString());
                }
            }
        }
        return classes.ToArray();
    }

    public AbilityDamageData GetAbilityDamageData(int level)
    {
        while (m_abilityDamageData.Count < (level - 1))
        {
            m_abilityDamageData.Add(new AbilityDamageData());
            Debug.LogError("Error: Ability " + m_identifier + " doesnt have damage data set. Creating new damage data for level: " + level);
        }
        if (m_abilityDamageData.Count > (level -1))
        {
            return m_abilityDamageData[level - 1];
        }
        return m_abilityDamageData[m_abilityDamageData.Count - 1];
        
    }


    public int DrawAbility(int posX, int yPos, int maxLevel, GUISkin skin)
    {

       
        int initPos = yPos;
#if UNITY_EDITOR
        //EditorStyles.textField.fontStyle = FontStyle.Bold;

        m_icon = (Sprite)EditorGUI.ObjectField(new Rect(posX, yPos, 70, 70), m_icon, typeof(Sprite), false);

        yPos += 70;

        m_identifier = EditorGUI.TextField(new Rect(posX + 150, yPos += 20, 150, 20), m_identifier, skin.textField);
        EditorGUI.LabelField(new Rect(posX, yPos, 150, 20), new GUIContent("Ability Identifier", "Ability identifier used to identity and load abilities"), skin.label);

        m_abilityName = EditorGUI.TextField(new Rect(posX + 150, yPos += 20, 150, 20), m_abilityName, skin.textField);
        EditorGUI.LabelField(new Rect(posX, yPos, 150, 20), new GUIContent("Ability Description Name: ", "Ability name to show"), skin.label);

        m_attackType = (EAttackType)EditorGUI.EnumPopup(new Rect(posX + 150, yPos += 20, 150, 20), m_attackType);
        EditorGUI.LabelField(new Rect(posX, yPos, 150, 20), new GUIContent("Ability Type: ", "Ability type"), skin.label);

        m_requiresTarget = EditorGUI.Toggle(new Rect(posX + 150, yPos += 20, 150, 20), m_requiresTarget);
        EditorGUI.LabelField(new Rect(posX, yPos, 150, 20), new GUIContent("Requires Target?: ", "Select this if the current ability requires targetting. The target could be a Position or Entity"), skin.label);

        m_faceTarget = EditorGUI.Toggle(new Rect(posX + 150, yPos += 20, 150, 20), m_faceTarget);
        EditorGUI.LabelField(new Rect(posX, yPos, 150, 20), new GUIContent("Look Target?: ", "Select this if you want the entity to face the target while casting the ability"), skin.label);

        if (m_requiresTarget)
        {
            posX += 40;
            
            m_targetType = (ETargetType)EditorGUI.EnumPopup(new Rect(posX + 150, yPos += 20, 150, 20), m_targetType);
            EditorGUI.LabelField(new Rect(posX, yPos, 150, 20), new GUIContent("Target Type: ", "Select the type of the target that you want the ability to make effect."), skin.label);
            if (m_targetType != ETargetType.SelfTarget)
            {
                m_allegiance = (EAllegiance)EditorGUI.EnumPopup(new Rect(posX + 150, yPos += 20, 150, 20), m_allegiance);
                EditorGUI.LabelField(new Rect(posX, yPos, 150, 20), new GUIContent("Target Allegiance: ", "Select the allegiance to be used by the ability to filter targets or detect collisions."), skin.label);
            }
            if (m_targetType != ETargetType.SelfTarget && m_targetType != ETargetType.AllTarget)
            {
                m_maxTargets = EditorGUI.IntSlider(new Rect(posX + 150, yPos += 20, 150, 20), m_maxTargets, 1, 10);
                EditorGUI.LabelField(new Rect(posX, yPos, 150, 20), new GUIContent("Max Targets: ", "Max targets the ability will try go cast the ability on."), skin.label);
            }
            posX -= 40;
            if (m_targetType == ETargetType.PositionTarget)
            {
                m_skillShotType = (ESkillShotType)EditorGUI.EnumPopup(new Rect(posX + 150, yPos += 20, 150, 20), m_skillShotType);
                EditorGUI.LabelField(new Rect(posX, yPos, 150, 20), new GUIContent("SkillShot Type: ", "If the target type is a Position Target. \nFrontSkill: The ability will be casted from the entity to the target direction. \nFloorSkill: The ability will be casted from the floor position."), skin.label);
                if (m_skillShotType == ESkillShotType.FrontSkill)
                {
                    m_detectionRange = EditorGUI.FloatField(new Rect(posX + 150, yPos += 20, 150, 20), m_detectionRange, skin.textField);
                    EditorGUI.LabelField(new Rect(posX, yPos, 150, 20), new GUIContent("Collision Detection Range: ", "This is the distance the ability will be using to detect collisions. \nRecommended Setting: 0.3f"), skin.label);
                }
            }
            m_indicatorPrefab = EditorGUI.TextField(new Rect(posX + 150, yPos += 20, 150, 20), m_indicatorPrefab, skin.textField);
            EditorGUI.LabelField(new Rect(posX, yPos, 150, 20), new GUIContent("Indicator Prefab: ", "This is the Indicator used to handle the gizmo logic to show the ability range, position or direction. \nDefault Values: \n-SimpleIndicator"), skin.label);
            if (m_targetType == ETargetType.PositionTarget)
            {
                m_targetImageIndicator = EditorGUI.TextField(new Rect(posX + 150, yPos += 20, 150, 20), m_targetImageIndicator, skin.textField);
                EditorGUI.LabelField(new Rect(posX, yPos, 150, 20), new GUIContent("Target Indicator Image: ", "This is the Target Image loaded by the Indicator. \nDefault Values: \n-SimpleFloorImage  \n-SimpleFrontImage"), skin.label);
            }
            m_rangeImageIndicator = EditorGUI.TextField(new Rect(posX + 150, yPos += 20, 150, 20), m_rangeImageIndicator, skin.textField);
            EditorGUI.LabelField(new Rect(posX, yPos, 150, 20), new GUIContent("Range Indicator Image: ", "This is the Range Image loaded by the Indicator to show the ability Range to the player. \nDefault Values: \n-SimpleRangueImage"), skin.label);

        }
        else
        {
            
        }

        GUI.color = new Color(0f, .8f, .8f, 0.15f);
        //GUI.Box(new Rect(posX, initPos, 350, (yPos - initPos) + 20), "");
        GUI.color = Color.white;

        yPos += 30;


        //Casting Parameters
        m_playCastAnimation = EditorGUI.Toggle(new Rect(posX + 150, yPos += 20, 150, 20), m_playCastAnimation);
        EditorGUI.LabelField(new Rect(posX, yPos, 150, 20), new GUIContent("Play Cast Animation? "), skin.label);

        if (m_playCastAnimation)
        {
            posX += 40;
            m_castingAnimation = (EEntityState)EditorGUI.EnumPopup(new Rect(posX + 150, yPos += 20, 150, 20), m_castingAnimation);
            EditorGUI.LabelField(new Rect(posX, yPos, 150, 20), new GUIContent("Cast Animation: "), skin.label);

            m_castTime = EditorGUI.FloatField(new Rect(posX + 150, yPos += 20, 180, 20), m_castTime, skin.textField);
            EditorGUI.LabelField(new Rect(posX, yPos, 150, 20), new GUIContent("Casting Time"), skin.label);

            m_castParticleIdentifier = EditorGUI.TextField(new Rect(posX + 150, yPos += 20, 150, 20), m_castParticleIdentifier, skin.textField);
            EditorGUI.LabelField(new Rect(posX, yPos, 150, 20), new GUIContent("Cast Particle Identifier"), skin.label);
            posX -= 40;
        }

        //Animation Parameters
        m_playAbilityAnimation = EditorGUI.Toggle(new Rect(posX + 150, yPos += 20, 150, 20), m_playAbilityAnimation);
        EditorGUI.LabelField(new Rect(posX, yPos, 150, 20), new GUIContent("Plays Ability Anim? ", "Mark this if the ability runs animations."), skin.label);

        if (m_playAbilityAnimation)
        {
            posX += 40;
            m_animation = (EEntityState)EditorGUI.EnumPopup(new Rect(posX + 150, yPos += 20, 150, 20), m_animation);
            EditorGUI.LabelField(new Rect(posX, yPos, 150, 20), new GUIContent("Ability Animation: "), skin.label);
            posX -= 40;
        }

        if (m_playCastAnimation || m_playAbilityAnimation)
        {
            m_launchingType = (ELaunchingState)EditorGUI.EnumPopup(new Rect(posX + 150, yPos += 20, 150, 20), m_launchingType);
            EditorGUI.LabelField(new Rect(posX, yPos, 150, 20), new GUIContent("Launch State: "), skin.label);

            m_animationPercentLaunch = EditorGUI.IntSlider(new Rect(posX + 150, yPos += 20, 150, 20), m_animationPercentLaunch, 0, 100);
            EditorGUI.LabelField(new Rect(posX, yPos, 150, 20), new GUIContent("Anim Percent Launch"), skin.label);
        }
       


        m_processor = (AbilityProcessorType)EditorGUI.EnumPopup(new Rect(posX + 150, yPos += 20, 150, 20), m_processor);
        EditorGUI.LabelField(new Rect(posX, yPos, 150, 20), new GUIContent("Ability processor type"), skin.label);


        switch (m_processor)
        {
            case AbilityProcessorType.DamageImpact:

                break;
            case AbilityProcessorType.LaunchAbility:
                posX += 40;
                m_projectileIdentifier = EditorGUI.TextField(new Rect(posX + 150, yPos += 20, 180, 20), m_projectileIdentifier, skin.textField);
                EditorGUI.LabelField(new Rect(posX, yPos, 150, 20), new GUIContent("Projectile Prefab Identifier"), skin.label);
                m_projectileQuantity = EditorGUI.IntField(new Rect(posX + 150, yPos += 20, 180, 20), m_projectileQuantity, skin.textField);
                EditorGUI.LabelField(new Rect(posX, yPos, 150, 20), new GUIContent("Projectile Quantity"), skin.label);
                m_projectileSpeed = EditorGUI.FloatField(new Rect(posX + 150, yPos += 20, 180, 20), m_projectileSpeed, skin.textField);
                EditorGUI.LabelField(new Rect(posX, yPos, 150, 20), new GUIContent("Projectile Speed"), skin.label);

                posX -= 40;
                break;
            case AbilityProcessorType.CastSideEffectsOnly:
                m_damageLogic = EDamageLogic.None;
                break;
            default:
                break;
        }
        if (m_processor != AbilityProcessorType.CastSideEffectsOnly)
        {
            m_launchPos = (EEntityTransform)EditorGUI.EnumPopup(new Rect(posX + 150, yPos += 20, 150, 20), m_launchPos);
            EditorGUI.LabelField(new Rect(posX, yPos, 150, 20), new GUIContent("Ability launch Position"), skin.label);
            m_launchParticleIdentifier = EditorGUI.TextField(new Rect(posX + 150, yPos += 20, 150, 20), m_launchParticleIdentifier, skin.textField);
            EditorGUI.LabelField(new Rect(posX, yPos, 150, 20), new GUIContent("Launch Particle Prefab Identifier"), skin.label);
            m_launchParticlePercentLaunch = EditorGUI.IntSlider(new Rect(posX + 150, yPos += 20, 150, 20), m_launchParticlePercentLaunch, 0, 100);
            EditorGUI.LabelField(new Rect(posX, yPos, 150, 20), new GUIContent("Launch Particle Animation Percent"), skin.label);
            m_impactParticleIdentifier = EditorGUI.TextField(new Rect(posX + 150, yPos += 20, 150, 20), m_impactParticleIdentifier, skin.textField);
            EditorGUI.LabelField(new Rect(posX, yPos, 150, 20), new GUIContent("Impact Particle Prefab Identifier"), skin.label);
            m_launchSoundIdentifier = EditorGUI.TextField(new Rect(posX + 150, yPos += 20, 150, 20), m_launchSoundIdentifier, skin.textField);
            EditorGUI.LabelField(new Rect(posX, yPos, 150, 20), new GUIContent("Launch Sound Identifier"), skin.label);
            m_launchSoundVolume = EditorGUI.IntSlider(new Rect(posX + 150, yPos += 20, 150, 20), m_launchSoundVolume, 0, 100);
            EditorGUI.LabelField(new Rect(posX, yPos, 150, 20), new GUIContent("Launch Sound Volume"), skin.label);
            m_impactSoundIdentifier = EditorGUI.TextField(new Rect(posX + 150, yPos += 20, 150, 20), m_impactSoundIdentifier, skin.textField);
            EditorGUI.LabelField(new Rect(posX, yPos, 150, 20), new GUIContent("Impact Sound Identifier"), skin.label);
            m_impactSoundVolume = EditorGUI.IntSlider(new Rect(posX + 150, yPos += 20, 150, 20), m_impactSoundVolume, 0, 100);
            EditorGUI.LabelField(new Rect(posX, yPos, 150, 20), new GUIContent("Impact Sound Volume"), skin.label);
            posX += 400;
            yPos = 150;
            m_damageLogic = (EDamageLogic)EditorGUI.EnumPopup(new Rect(posX + 150, yPos += 20, 150, 20), m_damageLogic);
            EditorGUI.LabelField(new Rect(posX, yPos, 150, 20), new GUIContent("Damage Logic"), skin.label);

        }




        if (m_damageLogic != EDamageLogic.None)
        {
            m_canDamageTowers = EditorGUI.Toggle(new Rect(posX + 150, yPos += 20, 150, 20), m_canDamageTowers);
            EditorGUI.LabelField(new Rect(posX, yPos, 150, 20), new GUIContent("Can Damage towers?: "), skin.label);

            if (m_damageLogic == EDamageLogic.DamageAoeLogic)
            {
                m_aoeRange = EditorGUI.FloatField(new Rect(posX + 150, yPos += 20, 180, 20), m_aoeRange);
                EditorGUI.LabelField(new Rect(posX, yPos, 150, 20), new GUIContent("Damage Aoe Range"), skin.label);
            }
        }
        else
        {
            //Damage logic == none

        }

        int prevPosX = posX;
            m_canLevelUp = EditorGUI.Toggle(new Rect(posX + 150, yPos += 20, 150, 20), m_canLevelUp);
            EditorGUI.LabelField(new Rect(posX, yPos, 150, 20), new GUIContent("Can level up?: "), skin.label);
            if (m_canLevelUp)
            {
                while (m_abilityDamageData.Count < maxLevel)
                {
                    m_abilityDamageData.Add(new AbilityDamageData());
                }
            }
            else
            {
                if (m_abilityDamageData.Count == 0)
                {
                    m_abilityDamageData.Add(new AbilityDamageData());
                }
                if (m_abilityDamageData.Count > 1)
                {
                    m_abilityDamageData.RemoveRange(1, m_abilityDamageData.Count - 1);
                }
            }
            yPos += 20;
            for (int i = 0; i < m_abilityDamageData.Count; i++)
            {
                if (i == 0)
                {
                    EditorGUI.LabelField((new Rect(posX, yPos + 20, 180, 20)), new GUIContent("Base Damage"), skin.label);
                    EditorGUI.LabelField((new Rect(posX, yPos + 40, 180, 20)), new GUIContent("Base Ability"), skin.label);
                    EditorGUI.LabelField((new Rect(posX, yPos + 60, 180, 20)), new GUIContent("Range"), skin.label);
                    EditorGUI.LabelField((new Rect(posX, yPos + 80, 180, 20)), new GUIContent("ColdDown"), skin.label);
                    EditorGUI.LabelField((new Rect(posX, yPos + 100, 180, 20)), new GUIContent("ManaCost"), skin.label);
                    posX += 100;
                }
                EditorGUI.LabelField((new Rect(posX + (50 * i), yPos, 180, 20)), new GUIContent("Lvl: " + (i + 1)), skin.label);
                m_abilityDamageData[i].BaseAdDamage = EditorGUI.FloatField(new Rect(posX + (50 * i), yPos += 20, 35, 20), m_abilityDamageData[i].BaseAdDamage);
                m_abilityDamageData[i].BaseApDamage = EditorGUI.FloatField(new Rect(posX + (50 * i), yPos += 20, 35, 20), m_abilityDamageData[i].BaseApDamage);
                m_abilityDamageData[i].Range = EditorGUI.FloatField(new Rect(posX + (50 * i), yPos += 20, 35, 20), m_abilityDamageData[i].Range);
                if (m_abilityDamageData[i].Range < 1)
                {
                    m_abilityDamageData[i].Range = 1;
                }
                m_abilityDamageData[i].ColdDownSeconds = EditorGUI.FloatField(new Rect(posX + (50 * i), yPos += 20, 35, 20), m_abilityDamageData[i].ColdDownSeconds);
                m_abilityDamageData[i].ManaCost = EditorGUI.FloatField(new Rect(posX + (50 * i), yPos += 20, 35, 20), m_abilityDamageData[i].ManaCost);

                yPos -= 5 * 20;

            }
            posX -= 400;
            yPos += 5 * 20;
            posX = prevPosX;
            yPos += 30;

            m_abilityDescription = EditorGUI.TextArea(new Rect(posX, yPos += 20, 300, 20 + 60), m_abilityDescription);
            yPos += 60;
            if (GUI.Button(new Rect(posX, yPos += 20, 300, 20), "Add Description Param"))
            {
                m_descriptionParamList.Add(new EDescriptionParameter());
                return 0;
            }
            for (int i = 0; i < m_descriptionParamList.Count; i++)
            {
                m_descriptionParamList[i] = (EDescriptionParameter)EditorGUI.EnumPopup(new Rect(posX + 150, yPos += 20, 150, 20), m_descriptionParamList[i]);
                EditorGUI.LabelField((new Rect(posX, yPos, 180, 20)), new GUIContent("Param Number: " + i), skin.label);

            }

       

        //Effects Parameters

        yPos = initPos;

        GUIStyle m_boldStyle = new GUIStyle();
        m_boldStyle.fontSize = 18;
        m_boldStyle.fontStyle = FontStyle.Bold;
        EditorGUI.LabelField(new Rect(posX + 400, yPos - 40, 300, 20), new GUIContent("Side Effects Configuration"), skin.GetStyle("BoldText"));

        m_effectClasses = GetEffectClasses();

        m_effectClassIndex = EditorGUI.Popup(new Rect(posX + 400, yPos += 20, 300, 20), m_effectClassIndex, m_effectClasses);
        GUI.color = Color.green;
        if (GUI.Button(new Rect(posX + 400, yPos += 20, 300, 20), "Add Side Effect"))
        {
            SideEffect effect = ScriptableObjectUtility.CreateAsset(m_effectClasses[m_effectClassIndex], Identifier);
            effect.EffectIdentifier = Identifier + "-" + m_effectClasses[m_effectClassIndex];
            m_sideEffects.Add(effect);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            return 0;
        }
        GUI.color = Color.white;
        yPos += 30;
        for (int i = 0; i < m_sideEffects.Count; i++)
        {
            //GUI.contentColor = Color.cyan;

            EditorGUI.LabelField((new Rect(posX + 400, yPos += 20, 300, 20)), new GUIContent("Effect Type: " + m_sideEffects[i].GetType().ToString()), skin.label);


            int boxPosY = yPos;
            yPos = m_sideEffects[i].DrawEffect(posX + 400, yPos, skin);
            //GUI.color = Color.cyan;
            GUI.Box(new Rect(posX + 390, boxPosY, 8, (yPos - boxPosY) + 20), "");
            //GUI.color = Color.red;
            EditorGUI.LabelField((new Rect(posX + 400, yPos += 20, 300, 20)), new GUIContent("Remove this effect "), skin.GetStyle("RedText"));
            if (GUI.Button(new Rect(posX + 550, yPos , 20, 20), "", skin.GetStyle("RemoveButton")))
            {
                string path = AssetDatabase.GetAssetPath(m_sideEffects[i]);
                AssetDatabase.DeleteAsset(path);
                m_sideEffects.RemoveAt(i);
                break;
            }
            //    if (GUI.Button(new Rect(posX + 400, yPos += 20, 300, 20), "Remove This Effect"))
            //{
            //   
            //}
            //GUI.color = Color.white;
            yPos += 20;
        }
       

        EditorUtility.SetDirty(this);
#endif
        return yPos - initPos;
    }

   
 
}

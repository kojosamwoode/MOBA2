using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
[System.Serializable]
public class AnimationData
{
    public AnimationData(EEntityState animation, float animationSpeed)
    {
        m_animation = animation;
        m_animationSpeed = animationSpeed;
    }
    public EEntityState m_animation = EEntityState.Basic;
    public float m_animationSpeed;
}

[System.Serializable]
public class MobaEntityData : ICloneable
{
    //GENERAL VARIABLES
    public string m_dataIdentifier = "";
    public string m_dataDisplayName = "";
    public string m_prefab = "";
    public string m_canvasPrefab = "";
    public string m_spawnParticle = "";
    public EEntityTransform m_spawnParticlePosition = EEntityTransform.Floor;
    public string m_deathPrefabParicle = "";
    public EEntityTransform m_deathParticlePosition = EEntityTransform.Floor;
    public EEntityRespawnType m_respawnType = EEntityRespawnType.NoRespawn;
    public float m_respawnTime = 10;
    public string m_icon = "";
    public bool m_usesPathFinfing = true;
    public ETeam m_team;
    public int m_goldBonus = 5;

    //OFFENSIVE VARIABLES
    public float m_baseAttackDamage = 50;
    public float m_adPerLevel = 3;
    public float m_baseMagicDamage = 50;

    //DEFENSIVE VARIABLES
    public float m_armor = 20;
    public float m_armorPerLevel = 3;
    public float m_magicRes;

    //HEALTH VARIABLES
    public float m_healthPerLevel = 85;
    public float m_healthMax = 260;
    public float m_healthRegenerate = 1.3f;

    //MANA VARIABLES
    public float m_manaMax = 150;
    public float m_manaRegeneration = 5f;

    public float m_speed = 1.4f;

    public float m_expToGive = 20;

    public int m_goldToGive = 15;

    public List<string> m_characterAbilities = new List<string>();

    public List<AnimationData> m_animationDataList = new List<AnimationData>();

    public void AddAnimationData(AnimationData animationData)
    {
        m_animationDataList.Add(animationData);
    }

    public bool AnimationDataExist(EEntityState animation)
    {
        foreach (AnimationData animationDef in m_animationDataList)
        {
            if (animationDef.m_animation == animation)
            {
                return true;
            }
        }
        return false;
    }

    public AnimationData GetAnimationData(EEntityState animation)
    {
        foreach (AnimationData animationDef in m_animationDataList)
        {
            if (animationDef.m_animation == animation)
            {
                return animationDef;
            }
        }
        return null;
    }

    public bool RemoveAnimationData(EEntityState animation)
    {
        for (int i = 0; i< m_animationDataList.Count; i++)
        {
            if (m_animationDataList[i].m_animation == animation)
            {
                m_animationDataList.RemoveAt(i);
                return true;
            }
        }
        return false;
    }

    public object Clone()
    {
        return this.MemberwiseClone();
    }

#if UNITY_EDITOR

    public virtual string DataFileExtension()
    {
        return "entity";
    }

    public virtual MobaEntityData LoadEntityData(string path)
    {
        return Utils.Load<MobaEntityData>(path);
    }
    public virtual MobaEntityData DrawEditor(GUISkin skin)
    {
        GUILayout.BeginVertical();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Data Identifier", skin.label);
        GUILayout.Label(m_dataIdentifier, skin.label, GUILayout.Width(200));
        m_dataIdentifier = GUILayout.TextField(m_dataIdentifier, skin.textField, GUILayout.Width(200));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Display Name", skin.label);
        m_dataDisplayName = GUILayout.TextField(m_dataDisplayName, skin.textField, GUILayout.Width(200));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Prefab Name", skin.label);
        m_prefab = GUILayout.TextField(m_prefab, skin.textField, GUILayout.Width(200));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Canvas", skin.label);
        m_canvasPrefab = GUILayout.TextField(m_canvasPrefab, skin.textField, GUILayout.Width(200));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Spawn Particle", skin.label);
        m_spawnParticle = GUILayout.TextField(m_spawnParticle, skin.textField, GUILayout.Width(200));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Spawn Particle Position", skin.label);
        EEntityTransform entityTransform = (EEntityTransform) EditorGUILayout.EnumPopup(m_spawnParticlePosition, GUILayout.Width(200));
        m_spawnParticlePosition = entityTransform;
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Death Prefab Particle", skin.label);
        m_deathPrefabParicle = GUILayout.TextField(m_deathPrefabParicle, skin.textField, GUILayout.Width(200));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Death Particle Position", skin.label);
        EEntityTransform deathParticlePosition = (EEntityTransform)EditorGUILayout.EnumPopup(m_deathParticlePosition, GUILayout.Width(200));
        m_deathParticlePosition = deathParticlePosition;
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Respawn Type", skin.label);
        EEntityRespawnType respawnType = (EEntityRespawnType)EditorGUILayout.EnumPopup(m_respawnType, GUILayout.Width(200));
        m_respawnType = respawnType;
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Respawn Time", skin.label);
        m_respawnTime = EditorGUILayout.FloatField(m_respawnTime, skin.textField, GUILayout.Width(200), GUILayout.Height(20));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Icon", skin.label);
        m_icon = GUILayout.TextField(m_icon, skin.textField, GUILayout.Width(200));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Uses Path Finfing", skin.label);
        m_usesPathFinfing = EditorGUILayout.Toggle(m_usesPathFinfing, skin.toggle, GUILayout.Width(200));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Team", skin.label);
        ETeam team = (ETeam)EditorGUILayout.EnumPopup(m_team, GUILayout.Width(200));
        m_team = team;
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Gold Bonus", skin.label);
        m_goldBonus = EditorGUILayout.IntField(m_goldBonus, skin.textField, GUILayout.Width(200), GUILayout.Height(20));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Base Attack Damage", skin.label);
        m_baseAttackDamage = EditorGUILayout.FloatField(m_baseAttackDamage, skin.textField, GUILayout.Width(200), GUILayout.Height(20));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Ad Increase Per Level", skin.label);
        m_adPerLevel = EditorGUILayout.FloatField(m_adPerLevel, skin.textField, GUILayout.Width(200), GUILayout.Height(20));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Base Magic Damage", skin.label);
        m_baseMagicDamage = EditorGUILayout.FloatField(m_baseMagicDamage, skin.textField, GUILayout.Width(200), GUILayout.Height(20));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Base Armor", skin.label);
        m_armor = EditorGUILayout.FloatField(m_armor, skin.textField, GUILayout.Width(200), GUILayout.Height(20));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Armor Increase Per Level", skin.label);
        m_armorPerLevel = EditorGUILayout.FloatField(m_armorPerLevel, skin.textField, GUILayout.Width(200), GUILayout.Height(20));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Base Magic Resistance", skin.label);
        m_magicRes = EditorGUILayout.FloatField(m_magicRes, skin.textField, GUILayout.Width(200), GUILayout.Height(20));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Health", skin.label);
        m_healthMax = EditorGUILayout.FloatField(m_healthMax, skin.textField, GUILayout.Width(200), GUILayout.Height(20));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Health Increase Per Level", skin.label);
        m_healthPerLevel = EditorGUILayout.FloatField(m_healthPerLevel, skin.textField, GUILayout.Width(200), GUILayout.Height(20));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Health Base Regeneration", skin.label);
        m_healthRegenerate = EditorGUILayout.FloatField(m_healthRegenerate, skin.textField, GUILayout.Width(200), GUILayout.Height(20));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Mana", skin.label);
        m_manaMax = EditorGUILayout.FloatField(m_manaMax, skin.textField, GUILayout.Width(200), GUILayout.Height(20));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Mana Base Regeneration", skin.label);
        m_manaRegeneration = EditorGUILayout.FloatField(m_manaRegeneration, skin.textField, GUILayout.Width(200), GUILayout.Height(20));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Movement Base Speed", skin.label);
        m_speed = EditorGUILayout.FloatField(m_speed, skin.textField, GUILayout.Width(200), GUILayout.Height(20));
        GUILayout.EndHorizontal();

        for (int i = 0; i < m_characterAbilities.Count; i++)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label((i +1) + " - Ability Identifier: ", skin.label);
            m_characterAbilities[i] = GUILayout.TextField(m_characterAbilities[i], skin.textField, GUILayout.Width(200));
            if (GUILayout.Button(new GUIContent(""), skin.customStyles[2], GUILayout.Width(20), GUILayout.Height(20)))
            {
                m_characterAbilities.RemoveAt(i);
                return this;

            }
            GUILayout.EndHorizontal();
        }

        GUILayout.BeginHorizontal();
        GUILayout.Label("Add Ability", skin.label);
        if (GUILayout.Button(new GUIContent(""), skin.customStyles[1], GUILayout.Width(30), GUILayout.Height(30)))
        {
            m_characterAbilities.Add("");
        }
        GUILayout.EndHorizontal();

        GUILayout.EndVertical();

        return this;
    }
#endif
}

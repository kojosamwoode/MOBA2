using UnityEngine;
using System.Collections;

[System.Serializable]
public class MobaItemSpecs  {

	//public string specName;
	public int specValue;
	public SpecName specName;

	public enum SpecName
	{
		AttackDamage,
		AbilityPower,
		Health,
		Mana,
		Armor,
		MagicResist,
		ColdDownReduction,
		ManaRegeneration,
		HealthRegeneration,
		AttackSpeed,
		LifeSteal,
		CritStrike,
		MovementSpeed

	}

	public MobaItemSpecs (SpecName name, int value)
	{
		specName = name;
		specValue = value;
	}

	public MobaItemSpecs() {}
}

[System.Serializable]
public class MobaItemStats  {

	//public string specName;
	public string statDescription;
	public StatName statName;
	public int methodIndex;
	public string methodName;

	public enum StatName
	{
		Passive,
		Active
	}



	public MobaItemStats (StatName name, string description, int index, string method)
	{
		statName = name;
		statDescription = description;
		methodIndex = index;
		methodName = method;
	}

	public MobaItemStats() {}
}

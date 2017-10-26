using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;


public class ItemDataBase : MonoBehaviour {
	public List<Items> items = new List<Items>();
	public static ItemDataBase data;

}

[System.Serializable]
public class ItemDataClass {
	
	public string name;
	public string desc;
	public int cost;
	public Sprite itemSprite;
	public int stacks;
	public enum ItemType
	{
		HealthPotMini,
		//+250 heal
		HealthPotBig,
		//+400 heal
		ManaPotMini,
		ManaPotBig,
		SwordMini,
		//+20 attack
		SwordBig,
		//+80 attack  +25 speed attack
		ShieldMini,
		//+35 armor
		ShieldBig,
		//+95 armor  +900 health
		MagicMini,
		//+40 spell
		MagicBig,
		//+95 spell +800 mana
		CloakMini,
		//+40 magic resist
		CloakBig,
		//+90 mr +800 health
		BootsMedium,
		//+10% speed
		BootsFast,
		//+25% speed
		Empty

	}
	public ItemType itemType;

	
	
}
[System.Serializable]
public class Items : ItemDataClass {


}


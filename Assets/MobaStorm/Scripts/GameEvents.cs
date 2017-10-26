using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Networking;

public class GameEvents : NetworkBehaviour {


	public static GameEvents Instance;
	private Queue<SlainEvent> slainEventList = new Queue<SlainEvent>();

	[System.Serializable]
	public class SlainEvent {
		public MobaEntity m_killerEntity;
		public MobaEntity m_deadEntity;

		public SlainEvent(MobaEntity killerEntity, MobaEntity deadEntity)
		{
            m_killerEntity = killerEntity;
            m_deadEntity = deadEntity;
		}

	}
	
	public CanvasGroup slainCanvas;
	public CanvasGroup spellUpCanvas;
	public Text killerText;
	public Text slainText;
	public AudioClip slainSound;
	public AudioClip greetingsClip;
	public Image killerImage;
	public Image slainImage;

	public bool showSlainUi = false;

	public bool showItemStore;
	public bool useItemSlot;
	public int slotNumberUsed;
	//Variables Assigned from the ItemManager Script attached to the players
	[HideInInspector] public CanvasGroup itemCanvas;
	private GameObject tooltipObj;
	[HideInInspector] public CanvasGroup tooltipCanvas;
	[HideInInspector] public RectTransform tooltipRect;

	private Text tooltipTextVisual;


	public void PlayGreetingsAudio () {
		StartCoroutine(GreetingsAudio());
		
	}

	public IEnumerator GreetingsAudio () {
		yield return new WaitForSeconds(4);
        AudioManager.instance.Play2dSound("MobaStorm_Greetings", 100);
		
	}
	public void ButtonClick1Audio () {
		//audioSource.PlayOneShot(buttonClick1);
		                      
	}
	public void ButtonClick2Audio () {
		//audioSource.PlayOneShot(buttonClick2);

	}
	public void WindowSlideAudio () {
		//audioSource.PlayOneShot(windowSlide);
	}
	public void ReadyButtonAudio () {
		//audioSource.PlayOneShot(readyButton);
	}

	public void ShowItemStore () {
		itemCanvas.alpha = 1;
		itemCanvas.interactable = true;
		showItemStore = false;
	}
	public void ShowSpellUpButtons () {
		spellUpCanvas.alpha = 1;
		spellUpCanvas.interactable = true;
	}

	public void UseItemSlot (int value) {
		useItemSlot = true;
		slotNumberUsed = value;
	}


	public void ShowToolTip (Transform parent , string name, string desc, int cost, float x, float y) {
		//tooltipObj.transform.SetParent(parent);
		tooltipCanvas.alpha = 1;
		string textfinal = string.Format("<color=white>" + name + "</color> \n <color=lightblue>"+ desc + " </color> \n ");
        if (cost >0)
        {
            textfinal += "\n <color=white>Cost: </color><color=yellow> " + cost + " </color>";
        }
		textfinal = textfinal.Replace("NEWL","\n");
		tooltipTextVisual.text = textfinal;
		tooltipRect.transform.position = new Vector3(parent.position.x, parent.position.y + 60, parent.position.z);

	}

	public void HideToolTip () {
		tooltipCanvas.alpha = 0;
	}


    void Start () {
		Instance = this;
		tooltipObj = GameObject.Find("Tooltip");
        tooltipRect = tooltipObj.GetComponent<RectTransform>();
		tooltipTextVisual = GameObject.Find("TooltipTextVisual").GetComponent<Text>();
		tooltipCanvas = tooltipObj.GetComponent<CanvasGroup>();
	}
	
	// Update is called once per frame
	void Update () {
		if (showSlainUi == true)
		{
			if (slainCanvas.alpha<1)
			slainCanvas.alpha +=  0.1f;
		}
		else
		{
			if (slainCanvas.alpha >0)
			slainCanvas.alpha -= 0.1f;
		}
		if (slainEventList.Count >0 && showSlainUi == false)
		{
			StartCoroutine(ShowPlayerSlain(slainEventList.Dequeue()));
		}
	}
		
    public void ServerSendSlainEvent(DamageProcess damageProcess)
    {
        RpcEnqueueSlainEvent(damageProcess.Attacker.InstanceId, damageProcess.Target.InstanceId);
    }

    [ClientRpc]
    public void RpcEnqueueSlainEvent(string killerEntity, string deadEntity)
    {
        MobaEntity killer = GameManager.instance.GetMobaEntity(killerEntity);
        MobaEntity dead = GameManager.instance.GetMobaEntity(deadEntity);
        SlainEvent capture = new SlainEvent(killer, dead);
        slainEventList.Enqueue(capture);
    }

    private IEnumerator ShowPlayerSlain (SlainEvent slainEvent) {

		//GetComponent<AudioSource>().PlayOneShot(slainSound);

		showSlainUi = true;

		killerImage.sprite = slainEvent.m_killerEntity.Icon;
		
		slainImage.sprite = slainEvent.m_deadEntity.Icon;
			
		killerText.text = slainEvent.m_killerEntity.DisplayName;
			
		slainText.text = slainEvent.m_deadEntity.DisplayName;

		yield return new WaitForSeconds(8f);

		showSlainUi = false;
	}
}

using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Networking;

public class Menu : NetworkBehaviour
{

	private Animator animator;

	private CanvasGroup canvasGroup;

	public GameObject MainMenu;

	public Text errorDialogText;

	public bool IsOpen
	{
		get {return animator.GetBool("IsOpen"); }
	}

    public virtual void OpenMenu()
    {      
        //gameObject.SetActive(true);
        animator.SetBool("IsOpen", true);
    }
    public virtual void CloseMenu()
    {
        animator.SetBool("IsOpen", false);
        //gameObject.SetActive(false);
    }

    // Update is called once per frame
    public void Awake () {
		animator = GetComponent<Animator>();
		canvasGroup = GetComponent<CanvasGroup>();

	
	}
	public void Update()
	{
		if (animator.GetCurrentAnimatorStateInfo(0).IsName("Open") && MenuManager.instance.loadingScreenObj.activeSelf == false)
		{
			canvasGroup.blocksRaycasts = canvasGroup.interactable = true;
		}
		else
		{
			canvasGroup.blocksRaycasts = canvasGroup.interactable = false;
		}
	}

}

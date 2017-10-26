using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SceneFadeInOut : MonoBehaviour
{
	public float fadeSpeed = 1f;          // Speed that the screen fades to and from black.

	private RawImage image;
	
	public bool fadeToBlack = true;      // Whether or not the scene is still fading in.


	void Awake ()
	{
		image = GetComponent<RawImage>();
		if (fadeToBlack)
		{
			image.color = Color.clear;
		}
		else
		{
			image.color = Color.black;
		}

	}
	
	
	void Update ()
	{
		if (fadeToBlack)
		{
			FadeToBlack();
		}
		else
		{
			FadeToClear();
		}

	}
	
	
	void FadeToClear()
	{


		image.color = Color.Lerp(image.color, Color.clear, fadeSpeed * Time.deltaTime);
		//Debug.Log(image.color.a);
		if(image.color.a <= 0.1f)
		{
			Destroy(this.gameObject);
		}
		
	}
	
	
	void FadeToBlack ()
	{

		image.color = Color.Lerp(image.color, Color.black, fadeSpeed * Time.deltaTime);
		if(image.color.a >= 0.99f)
		{
			fadeToBlack = false;
		}
	}
	
	



}
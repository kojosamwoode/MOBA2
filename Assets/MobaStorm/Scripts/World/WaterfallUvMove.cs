using UnityEngine;
using System.Collections;

public class WaterfallUvMove : MonoBehaviour {
	public float speed_X = 0;
	public float speed_Y = 0.3f;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

			float offsetX = Time.time * speed_X;
			float offsetY = Time.time * speed_Y;
			GetComponent<Renderer>().material.mainTextureOffset = new Vector2 (offsetX,offsetY);

	}
}

using UnityEngine;
using System.Collections;

//This script is used to rotate and destroy the mouse pointer after the expire time
public class MovePointerScript : MonoBehaviour {
	public float expireTime = 1.5f;
	public MeshRenderer mesh;
	//public bool destroyThis;
	// Use this for initialization
	void Start () {
		
		//if (destroyThis)
		//StartCoroutine(DestroyMyselfAfterSomeTime());
	}
	
	// Update is called once per frame
	void Update () {
		transform.RotateAround(transform.position, transform.up, Time.deltaTime * 90f);
	}

	public void EnablePointer(Vector3 position )
	{
		transform.position = new Vector3(position.x, position.y + 0.2f, position.z);
		StopAllCoroutines();
		StartCoroutine(EnableMesh());
	}

	IEnumerator EnableMesh()
	{
		mesh.enabled = true;
		yield return new WaitForSeconds (expireTime);
		mesh.enabled = false;
		
	}
}

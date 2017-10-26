using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Camera_Sources 
{
	public Transform pivot;
	public Transform camera;
	public Transform target = null;
    public Camera currentCamera;

	
}
[System.Serializable]
public class Camera_Inputs
{
	public KeyCode FollowChar	= KeyCode.Space;
	public KeyCode MoveLeft		= KeyCode.LeftArrow;
	public KeyCode MoveRight	= KeyCode.RightArrow;
	public KeyCode MoveForward	= KeyCode.UpArrow;
	public KeyCode MoveBackward	= KeyCode.DownArrow;
	public string ScrollWheel		= "Mouse ScrollWheel";
}


[System.Serializable]
public class Camera_Config
{	
	public bool cameraActive = false;
	public bool cameraLocked = false;
	public Camera_Config_Movement movement = new Camera_Config_Movement();
	public Camera_Config_Zoom zoom = new Camera_Config_Zoom();
}

[System.Serializable]
public class Camera_Config_Movement {

	
	// How fast the camera moves
	public float cameraMovementRate	= 1.0f;

	
	// The Distance from the edge of the screen 
	public float edgeHoverOffset 	= 20.0f;

}



[System.Serializable]
public class Camera_Config_Zoom {
	
	// Minimum and Maximum zoom values
	public float minZoom 	= 0.0f;
	public float maxZoom 	= 5.0f;
	
	// How fast the camera zooms in and out
	public float zoomRate 	= 3.0f;
	

}



public class CameraController : MonoSingleton<CameraController> {

	public Camera_Sources sources = new Camera_Sources();
	public Camera_Inputs inputs = new Camera_Inputs();
	public Camera_Config config = new Camera_Config();


	private float actualZoom = 0.0f;


	void Start () {

		actualZoom = config.zoom.maxZoom;

	}
	
	// Update is called once per frame
	void Update () {
	

		ZoomUpdate();

		if (config.cameraActive)
		MoveUpdate();

	}


	void ZoomUpdate() {

		float zoomValue = 0.0f;

		
		float mouseW = Input.GetAxis(inputs.ScrollWheel);
		if(mouseW != 0.0f) 
		{
		
			zoomValue = mouseW;	

		}

		actualZoom += zoomValue * config.zoom.zoomRate  * Time.deltaTime;



		if (actualZoom > config.zoom.minZoom)
			actualZoom = config.zoom.minZoom;
		if (actualZoom < config.zoom.maxZoom)
			actualZoom = config.zoom.maxZoom;
	
		Vector3 _cameraPos = Vector3.zero;
		_cameraPos.z = actualZoom;
		sources.camera.localPosition = _cameraPos;

	}
	void MoveUpdate() {
	
		if (Input.GetKeyDown(inputs.FollowChar) && !ClientChat.instance.IsInputFieldOnFocus)
		{
			config.cameraLocked = !config.cameraLocked;
		}


		if (config.cameraLocked && sources.target != null)
		{
			sources.pivot.position = Vector3.Lerp(sources.pivot.position, sources.target.position, config.movement.cameraMovementRate);
		}
		else

		{
			Vector3 mouseVector = new Vector3(0,0,0);

			if(Input.mousePosition.x < config.movement.edgeHoverOffset) 
			{
				mouseVector -= sources.pivot.transform.right;
			}
			if(Input.mousePosition.x > Screen.width - config.movement.edgeHoverOffset)

			{
				mouseVector += sources.pivot.transform.right;
			}
			if(Input.mousePosition.y < config.movement.edgeHoverOffset)
			{
				mouseVector -= sources.pivot.transform.forward;
			}
			if(Input.mousePosition.y > Screen.height - config.movement.edgeHoverOffset)
			{
				mouseVector += sources.pivot.transform.forward;
			}
			
			sources.pivot.position += mouseVector.normalized * config.movement.cameraMovementRate * Time.deltaTime;
		}


	}
}

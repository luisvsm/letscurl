using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : BootableMonoBehaviour {

	private static InputController _instance;
	public static InputController Instance {
		get{
			if(_instance == null){
				_instance = GameObject.Find("InputController").GetComponent<InputController>();
			}
			return _instance;
		}
	}
	// Use this for initialization
	void Start () {
		
	}
	public Plane groundPlane = new Plane(new Vector3(-50f,0f,-100f), new Vector3(-50f,0f,100f), new Vector3(200f,0f,0f));
	public GameObject currentObj;
	public GameObject prevObj;

	Vector3 previousPosition = new Vector3();
	Vector3 currentPosition = new Vector3();
	Vector3 inputPos;
	bool resetPrevious;
	// Update is called once per frame
	void Update () {

		if(Input.touches.Length > 0){
			
			Touch t = Input.touches[0];
        	inputPos = t.position;
		}else if(Input.GetMouseButton(0)){
        	inputPos = Input.mousePosition;
		}else{
			resetPrevious = true;
			return;
		}
		
		//Update currentPosition to the current world position of the player's finger
		updateCurrentInputPosition();
			
		

		//currentPosition = Camera.main.ScreenToWorldPoint(new Vector3(pos.x, pos.y, Camera.main.nearClipPlane));
		if((previousPosition - currentPosition).magnitude>1){
			Debug.Log("currentPosition: " + currentPosition);
			Debug.Log("previousPosition: " + previousPosition);
			Debug.DrawRay(currentPosition, previousPosition - currentPosition, Color.red);
			Debug.DrawLine(currentPosition, previousPosition, Color.blue);
			previousPosition = currentPosition;
		}
		if(prevObj != null && currentObj != null){
			prevObj.transform.position = previousPosition;
			currentObj.transform.position = currentPosition;
		}
	}
	void updateCurrentInputPosition(){
		Ray ray = Camera.main.ScreenPointToRay(inputPos);
		float rayDistance;
		if (groundPlane.Raycast(ray, out rayDistance)){
			//Hit
			currentPosition = ray.GetPoint(rayDistance);
			if(resetPrevious){
				previousPosition = currentPosition;
				resetPrevious = false;
			}
		}else{
			//Miss
		}
		
	}
	public override void Boot(){
		
	}


}

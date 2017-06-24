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

	List<Vector3> previousPosition = new List<Vector3>();
	Vector3 currentPosition = new Vector3();
	Vector3 inputPos;
	bool fingerIsDown;
	public int positionPoolLength = 4;
	public float updateTime = 0.01f;
	float timeUntilNextUpdate;
	public GameObject stone;
	// Update is called once per frame
	void Update () {

		if(Input.touches.Length > 0){
			Touch t = Input.touches[0];
        	inputPos = t.position;
		}else if(Input.GetMouseButton(0)){
        	inputPos = Input.mousePosition;
		}else{
			if(fingerIsDown){
				Instantiate(stone, currentPosition, Quaternion.identity);
				fingerIsDown = false;
			}
			previousPosition.Clear();
			return;
		}
		fingerIsDown = true; // By now we have input
		//Update currentPosition to the current world position of the player's finger
		updateCurrentInputPosition();
			
		
		timeUntilNextUpdate -= Time.deltaTime;
		//currentPosition = Camera.main.ScreenToWorldPoint(new Vector3(pos.x, pos.y, Camera.main.nearClipPlane));
		if(timeUntilNextUpdate<0){
			timeUntilNextUpdate = updateTime;
			previousPosition.Add(currentPosition);
			if(previousPosition.Count > positionPoolLength)
				previousPosition.RemoveAt(0);
		}
		if(prevObj != null && currentObj != null){
			prevObj.transform.position = previousPosition[0];
			currentObj.transform.position = currentPosition;
		}
	}
	void updateCurrentInputPosition(){
		Ray ray = Camera.main.ScreenPointToRay(inputPos);
		float rayDistance;
		if (groundPlane.Raycast(ray, out rayDistance)){
			//Hit
			currentPosition = ray.GetPoint(rayDistance);
		}else{
			//Miss
		}
		
	}
	public override void Boot(){
		
	}


}

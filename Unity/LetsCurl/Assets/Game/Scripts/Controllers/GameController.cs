using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : BootableMonoBehaviour {

	public GameObject stonePrefab;
	private GameObject _currentStone;
	List<GameObject> StonePool = new List<GameObject>();
	public Vector3 followCameraPosition = new Vector3(0f, 38.3f, -15f);
	public Vector3 followRotation = new Vector3(56.3f, 0f, 0f);
	public Vector3 restingCameraPosition =  new Vector3(0f, 20f, -71.1f);
	public Vector3 restingRotation = new Vector3(42.7f, 0f, 0f);
	GameObject CurrentStone{
		get{
			if(_currentStone == null){
				for (int i = 0; i < StonePool.Count; i++)
				{
					if(!StonePool[i].activeSelf){
						_currentStone = StonePool[i];
						_currentStone.SetActive(true);
						return _currentStone;
					}
				}
				_currentStone = Instantiate(stonePrefab, Vector3.zero, Quaternion.identity);
				StonePool.Add(_currentStone);
			}
			return _currentStone;
		}
		set{
			_currentStone = value;
		}
	}
	bool followStone;
	public float forceMuliplyer = 1;
	public void SetThrowingStone(Vector3 position){
		CurrentStone.transform.position = position;
		CurrentStone.transform.eulerAngles = new Vector3(0f,0f,0f);
	}
	public void ThrowStone(Vector3 position, Vector3 forceVector){
		if(
			position.z > -36 ||
			(Camera.main.transform.position - restingCameraPosition).magnitude > 0.1f
		){
			CurrentStone.SetActive(false);
			CurrentStone = null;
			return;
		}
		CurrentStone.transform.position = position;
		CurrentStone.transform.eulerAngles = new Vector3(0f,0f,0f);
		CurrentStone.GetComponent<Rigidbody>().AddForce((forceVector*forceMuliplyer) - CurrentStone.GetComponent<Rigidbody>().velocity, ForceMode.VelocityChange);
		InputController.Instance.TurnOffInput();
		followStone = true;
	}

	private static GameController _instance;
	public static GameController Instance {
		get{
			if(_instance == null){
				_instance = GameObject.Find("GameController").GetComponent<GameController>();
			}
			return _instance;
		}
	}

	// Use this for initialization
	void Start () {
		InputController.Instance.TurnOnInput();
	}
	// Update is called once per frame
	void Update () {
	}

	int atRestCount;
	void FixedUpdate(){
		if(followStone){
			Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, new Vector3(followCameraPosition.x, followCameraPosition.y, CurrentStone.transform.position.z + followCameraPosition.z), 0.3f);
			Camera.main.transform.eulerAngles = Vector3.Lerp(Camera.main.transform.eulerAngles, followRotation, 0.2f);
		}else{
			Camera.main.transform.position = Vector3.MoveTowards(Camera.main.transform.position, restingCameraPosition, 1);
			Camera.main.transform.eulerAngles = Vector3.Lerp(Camera.main.transform.eulerAngles, restingRotation, 0.2f);
		}
		CleanUpFallenStones();

		if(StonesAreAtRest()){
			Debug.Log("Stones are resting");
			if(atRestCount < 10)
				atRestCount++;
		}else{
			atRestCount = 0;
		}

		if(!InputController.Instance.InputIsOn() && atRestCount == 10){
			CleanUpOutOfBounds();
			resetShot();
		}
	}

	private void resetShot(){
		CurrentStone = null;
		followStone = false;
		InputController.Instance.TurnOnInput();
	}

	public void CleanUpFallenStones(){
		for (int i = 0; i < StonePool.Count; i++)
		{
			if(StonePool[i].activeSelf && StonePool[i].transform.position.y < -10){
				StonePool[i].SetActive(false);
			}
		}
	}
	public void CleanUpOutOfBounds(){
		for (int i = 0; i < StonePool.Count; i++)
		{	
			if(StonePool[i].activeSelf){
				if(StonePool[i].transform.position.z > 69 || StonePool[i].transform.position.z < 36){
					StonePool[i].SetActive(false); // Clean up rouge stones
				}
			}
		}
	}
	public bool StonesAreAtRest(){
		for (int i = 0; i < StonePool.Count; i++)
		{	
			if(StonePool[i].activeSelf && StonePool[i].GetComponent<Rigidbody>().velocity.magnitude > 0.001){
				return false;
			}
		}
		return true;
	}

	public override void Boot(){
		NetworkController.Instance.RegisterReady(NetworkControllerIsReady);
		NetworkController.Instance.Init();
	}

	private void NetworkControllerIsReady(){
		Debug.Log("Wooooo lets do this");
	}
}

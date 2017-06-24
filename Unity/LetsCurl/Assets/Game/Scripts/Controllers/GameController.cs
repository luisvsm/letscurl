using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : BootableMonoBehaviour {

	public GameObject stonePrefab;
	private GameObject _currentStone;
	List<GameObject> StonePool = new List<GameObject>();
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
	}
	bool followStone;
	public float forceMuliplyer = 1;
	public void SetThrowingStone(Vector3 position){
		CurrentStone.transform.position = position;
	}
	Vector3 originalCameraPosition;
	public void ThrowStone(Vector3 position, Vector3 forceVector){
		CurrentStone.GetComponent<Rigidbody>().AddForce(forceVector*forceMuliplyer, ForceMode.VelocityChange);
		InputController.Instance.TurnOffInput();
		followStone = true;
		originalCameraPosition = Camera.main.transform.position;
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
		originalCameraPosition = Camera.main.transform.position;
	}
	// Update is called once per frame
	void Update () {
		if(followStone){
			Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, CurrentStone.transform.position.z - 15), 0.3f);
		}else{
			Camera.main.transform.position = Vector3.MoveTowards(Camera.main.transform.position, originalCameraPosition, 1);
		}
	}

	int frame = 0;
	int atRestCount;
	void FixedUpdate(){
		CleanUpFallenStones();

		if(StonesAreAtRest()){
			Debug.Log("Stones are resting");
			if(atRestCount < 10)
				atRestCount++;
		}else{
			atRestCount = 0;
		}

		if(!InputController.Instance.InputIsOn() && atRestCount == 10){
			resetShot();
		}
	}

	private void resetShot(){
		_currentStone = null;
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

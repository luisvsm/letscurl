using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : BootableMonoBehaviour {

	int numberOfPlayers = 4;
	int turn = 0;
	public List<Player> players;
	
	public Animator outOfBoundsAnimation;
	public PhysicMaterial floorMaterial;
	public GameObject distanceMeter;
	public Text distanceMeterText;
	public GameObject stonePrefab;
	private Stone _currentStone;
	public float originalFriction = 0.08f;
	List<Stone> StonePool = new List<Stone>();
	public Vector3 followCameraPosition = new Vector3(0f, 38.3f, -15f);
	public Vector3 followRotation = new Vector3(56.3f, 0f, 0f);
	public Vector3 restingCameraPosition =  new Vector3(0f, 20f, -71.1f);
	public Vector3 restingRotation = new Vector3(42.7f, 0f, 0f);
	Stone CurrentStone{
		get{
			if(_currentStone == null){
				for (int i = 0; i < StonePool.Count; i++)
				{
					if(!StonePool[i].gameObject.activeSelf){
						_currentStone = StonePool[i];
						_currentStone.gameObject.SetActive(true);
						return _currentStone;
					}
				}
				_currentStone = Instantiate(stonePrefab, Vector3.zero, Quaternion.identity).GetComponent<Stone>();
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
	public void NextTurn(){
		turn = (turn+1) % players.Count;
	}
	public void LowerFriction(float positionX, float positionY){
		if(Mathf.Abs(positionX - CurrentStone.transform.position.x) > 3 || CurrentStone.transform.position.y > positionY){
			return;
		}
		CurrentStone.body.AddForce(new Vector3(1.5f * Time.deltaTime * (positionX - CurrentStone.transform.position.x), 0f, 0f), ForceMode.VelocityChange);
		floorMaterial.dynamicFriction = Mathf.Lerp(floorMaterial.dynamicFriction, 0f, 0.15f);
	}
	public void RestoreFriction(){
		floorMaterial.dynamicFriction = Mathf.Lerp(floorMaterial.dynamicFriction, originalFriction, 0.05f);
	}
	public void SetThrowingStone(Vector3 position){
		CurrentStone.setOwner(players[turn]);
		if(
			position.z > -36 ||
			(Camera.main.transform.position - restingCameraPosition).magnitude > 0.1f
		){
			CurrentStone.gameObject.SetActive(false);
			CurrentStone = null;
			return;
		}
		CurrentStone.gameObject.SetActive(true);
		CurrentStone.ClearTrail();
		CurrentStone.transform.position = position;
		CurrentStone.transform.eulerAngles = new Vector3(0f,0f,0f);
		CurrentStone.GetComponent<Rigidbody>().AddForce(-CurrentStone.body.velocity, ForceMode.VelocityChange);
	}
	public void ThrowStone(Vector3 position, Vector3 forceVector){
		if(
			position.z > -36 ||
			(Camera.main.transform.position - restingCameraPosition).magnitude > 0.1f
		){
			CurrentStone.gameObject.SetActive(false);
			CurrentStone = null;
			return;
		}
		distanceMeter.SetActive(true);
		CurrentStone.setOwner(players[turn]);
		CurrentStone.ClearTrail();
		CurrentStone.gameObject.SetActive(true);
		CurrentStone.GetComponent<Rigidbody>().AddForce(-CurrentStone.body.velocity, ForceMode.VelocityChange);
		CurrentStone.transform.position = position;
		CurrentStone.transform.eulerAngles = new Vector3(0f,0f,0f);
		CurrentStone.GetComponent<Rigidbody>().AddForce((forceVector*forceMuliplyer), ForceMode.VelocityChange);
		InputController.Instance.throwing = false;
		InputController.Instance.sweeping = true;
		followStone = true;
		NextTurn();
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

	}
	// Update is called once per frame
	void Update () {

	}

	int atRestCount;
	void FixedUpdate(){
		RestoreFriction();

		distanceMeterText.text = Math.Round((CurrentStone.transform.position - new Vector3(0f, -0.05f, 57f)).magnitude, 2).ToString();
		if(followStone){
			Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, new Vector3(followCameraPosition.x, followCameraPosition.y, CurrentStone.transform.position.z + followCameraPosition.z), 0.3f);
			Camera.main.transform.eulerAngles = Vector3.Lerp(Camera.main.transform.eulerAngles, followRotation, 0.2f);
		}else{
			Camera.main.transform.position = Vector3.MoveTowards(Camera.main.transform.position, restingCameraPosition, 1);
			Camera.main.transform.eulerAngles = Vector3.Lerp(Camera.main.transform.eulerAngles, restingRotation, 0.2f);
		}
		CleanUpFallenStones();

		if(StonesAreAtRest()){
			
			if(atRestCount < 10)
				atRestCount++;
		}else{
			atRestCount = 0;
		}

		if(!InputController.Instance.throwing && atRestCount == 10){
			Debug.Log("Stones are at rest, resetting shot");
			CleanUpOutOfBounds();
			resetShot();
		}
	}

	private void resetShot(){
		distanceMeter.SetActive(false);
		CurrentStone = null;
		CurrentStone.gameObject.SetActive(false);
		followStone = false;
		InputController.Instance.throwing = true;
		InputController.Instance.sweeping = false;
	}

	public void CleanUpFallenStones(){
		for (int i = 0; i < StonePool.Count; i++)
		{
			if(StonePool[i].gameObject.activeSelf && StonePool[i].transform.position.y < -1){
				StonePool[i].gameObject.SetActive(false);
				outOfBoundsAnimation.SetTrigger("FlashNow");
			}
		}
	}
	public void CleanUpOutOfBounds(){
		for (int i = 0; i < StonePool.Count; i++)
		{	
			if(StonePool[i].gameObject.activeSelf){
				if(StonePool[i].transform.position.z > 63 || StonePool[i].transform.position.z < 36){
					StonePool[i].gameObject.SetActive(false); // Clean up rouge stones
					outOfBoundsAnimation.SetTrigger("FlashNow");
				}
			}
		}
	}
	public bool StonesAreAtRest(){
		for (int i = 0; i < StonePool.Count; i++)
		{	
			if(StonePool[i].gameObject.activeSelf && StonePool[i].body.velocity.magnitude > 0.001){
				return false;
			}
		}
		return true;
	}

	public override void Boot(){
		InputController.Instance.throwing = true;
		InputController.Instance.sweeping = false;
		NetworkController.Instance.RegisterReady(NetworkControllerIsReady);
		NetworkController.Instance.Init();
		players = new List<Player>();
		players.Add(new Player(Player.Team.Team1, 1));
		players.Add(new Player(Player.Team.Team2, 2));
	}

	private void NetworkControllerIsReady(){
		Debug.Log("Wooooo lets do this");
	}
}

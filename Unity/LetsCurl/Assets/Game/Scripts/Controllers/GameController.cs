﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : BootableMonoBehaviour {

	int team1Stones;
	int team2Stones;
	
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
	public GameObject Team1Banner;
	public GameObject Team2Banner;
	public GameObject curlingStartInfo;
	public GameObject sweepingStartInfo;
	public float tutorialWaitTime = 0;
	public void resetTutorialWaitTime(){
		tutorialWaitTime=Time.time+4f;
	}
	public List<GameObject> StonesLeftIndicatorTeam1;
	public List<GameObject> StonesLeftIndicatorTeam2;
	public GameObject stonesLeftTeam1;
	public GameObject stonesLeftTeam2;
	public void SetShotsLeft(){
		for (int i = 0; i < StonesLeftIndicatorTeam1.Count; i++)
		{
			if(team1Stones>i){
				StonesLeftIndicatorTeam1[i].SetActive(true);
			}else{
				StonesLeftIndicatorTeam1[i].SetActive(false);
			}
		}

		for (int i = 0; i < StonesLeftIndicatorTeam2.Count; i++)
		{
			if(team2Stones>i){
				StonesLeftIndicatorTeam2[i].SetActive(true);
			}else{
				StonesLeftIndicatorTeam2[i].SetActive(false);
			}
		}
	}
	float gameOverTimer;
	public bool IsGameOver(){
		return team1Stones <= 0 && team2Stones <= 0;
	}
	int team1Score;
	int team2Score;
	public void CalculateScore(){
		float team1Distance=999;
		float team2Distance=999;
		Vector3 centre = new Vector3(0f, -0.05f, 57f);
		for (int i = 0; i < StonePool.Count; i++)
		{
			if(StonePool[i].team == Player.Team.Team1){
				team1Distance = Mathf.Min(Vector3.Distance(StonePool[i].transform.position,centre), team1Distance);
			}else if(StonePool[i].team == Player.Team.Team2){
				team2Distance = Mathf.Min(Vector3.Distance(StonePool[i].transform.position,centre), team2Distance);
			}
		}
		if(team1Distance<team2Distance){
			for (int i = 0; i < StonePool.Count; i++)
			{
				if(StonePool[i].team == Player.Team.Team1 && Vector3.Distance(StonePool[i].transform.position,centre)<team2Distance){
					team1Score++;
				}
			}
		}else{
			for (int i = 0; i < StonePool.Count; i++)
			{
				if(StonePool[i].team == Player.Team.Team2 && Vector3.Distance(StonePool[i].transform.position,centre)<team1Distance){
					team2Score++;
				}
			}
		}

		if(IsGameOver()){

		}
	}
	public void NextTurn(){
		resetShot();
		turn = (turn+1) % players.Count;
		InputController.Instance.TurnOffInput();
		if(players[turn].team == Player.Team.Team1){
			Team1Banner.SetActive(true);
		}
		else
		{
			Team2Banner.SetActive(true);
		}
	}
	public void LowerFriction(float positionX, float positionY){
		resetTutorialWaitTime();
		
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
		resetTutorialWaitTime();
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
		resetTutorialWaitTime();
		if(
			position.z > -36 ||
			(Camera.main.transform.position - restingCameraPosition).magnitude > 0.1f
		){
			CurrentStone.gameObject.SetActive(false);
			CurrentStone = null;
			return;
		}
		distanceMeter.SetActive(true);
		stonesLeftTeam1.SetActive(false);
		stonesLeftTeam2.SetActive(false);
		CurrentStone.setOwner(players[turn]);
		if(players[turn].team == Player.Team.Team1){
			team1Stones--;
		}else if(players[turn].team == Player.Team.Team2){
			team2Stones--;
		}
		SetShotsLeft();
		CurrentStone.ClearTrail();
		CurrentStone.gameObject.SetActive(true);
		CurrentStone.GetComponent<Rigidbody>().AddForce(-CurrentStone.body.velocity, ForceMode.VelocityChange);
		CurrentStone.transform.position = position;
		CurrentStone.transform.eulerAngles = new Vector3(0f,0f,0f);
		CurrentStone.GetComponent<Rigidbody>().AddForce((forceVector*forceMuliplyer), ForceMode.VelocityChange);
		InputController.Instance.throwing = false;
		InputController.Instance.sweeping = true;
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

	}
	public Text score;
	// Update is called once per frame
	void Update () {
		score.text = "Team1: " + team1Score +  "Team2: " + team2Score;
		if(InputController.Instance.throwing && tutorialWaitTime<Time.time){
			curlingStartInfo.SetActive(true);
		}else if (curlingStartInfo.activeSelf){
			curlingStartInfo.SetActive(false);
		}
		if(InputController.Instance.sweeping && tutorialWaitTime<Time.time){
			sweepingStartInfo.SetActive(true);
		}else if (sweepingStartInfo.activeSelf){
			sweepingStartInfo.SetActive(false);
		}

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
			if(IsGameOver()){
				CalculateScore();
				StartGame2P();
			}else{
				Debug.Log("Stones are at rest, resetting shot");
				CleanUpOutOfBounds();
				NextTurn();
			}
		}
	}

	private void resetShot(){
		distanceMeter.SetActive(false);
		stonesLeftTeam1.SetActive(true);
		stonesLeftTeam2.SetActive(true);
		CurrentStone = null;
		CurrentStone.gameObject.SetActive(false);
		followStone = false;
		InputController.Instance.throwing = true;
		InputController.Instance.sweeping = false;
	}

	public void CleanUpFallenStones(){
		if(StonePool.Count == 0) return;
		for (int i = 0; i < StonePool.Count; i++)
		{
			if(StonePool[i].gameObject.activeSelf && StonePool[i].transform.position.y < -1){
				StonePool[i].gameObject.SetActive(false);
				outOfBoundsAnimation.SetTrigger("FlashNow");
			}
		}
	}
	public void CleanUpOutOfBounds(){
		if(StonePool.Count == 0) return;
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
		if(StonePool.Count == 0) return false;
		for (int i = 0; i < StonePool.Count; i++)
		{	
			if(StonePool[i].gameObject.activeSelf && StonePool[i].body.velocity.magnitude > 0.001){
				return false;
			}
		}
		return true;
	}
	public void StartGame2P(){
		
		for (int i = StonePool.Count-1; i >= 0; i--)
		{	
			Destroy(StonePool[i].gameObject);
		}
		team1Stones = 8;
		team2Stones = 8;
		SetShotsLeft();
		StonePool.Clear();
		curlingStartInfo.SetActive(false); 
		sweepingStartInfo.SetActive(false);
		Team1Banner.SetActive(false); 
		Team2Banner.SetActive(false);
		resetShot();
		resetTutorialWaitTime();
		players = new List<Player>();
		players.Add(new Player(Player.Team.Team1, 1));
		players.Add(new Player(Player.Team.Team2, 2));
		team1Stones = 8;
		team2Stones = 8;
		turn = -1;
		NextTurn();
	}
	
	public override void Boot(){
		StartGame2P();
		//NetworkController.Instance.RegisterReady(NetworkControllerIsReady);
		//NetworkController.Instance.Init();
	}

	private void NetworkControllerIsReady(){
		Debug.Log("Wooooo lets do this");
	}
}

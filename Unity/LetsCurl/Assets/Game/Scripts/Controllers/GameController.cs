﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : BootableMonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public override void Boot(){
		NetworkController.Instance.RegisterReady(NetworkControllerIsReady);
		NetworkController.Instance.Init();
	}

	private void NetworkControllerIsReady(){
		Debug.Log("Wooooo lets do this");
	}
}

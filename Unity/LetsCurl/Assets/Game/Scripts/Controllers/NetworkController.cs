using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkController : MonoBehaviour {
	public string host = "";
	private WebSocket ws;
	private IEnumerator wsCoroutine;

	private static NetworkController _instance;
	public static NetworkController Instance {
		get{
			if(_instance == null){
				_instance = GameObject.Find("NetworkController").GetComponent<NetworkController>();
			}
			return _instance;
		}
	}

	bool ready;

	private delegate void ActionDelegate();
	private ActionDelegate ReadyAction;
	
	public void RegisterReady(Action readyCallback){
		
		//Check for a null value
		if(readyCallback == null){
			Debug.LogError("[NetworkController] Expected ready to not be null");
			return;
		}

		//If the network is already ready, just fire off the ready action right now :P
		if(ready){
			readyCallback();
		}else{
			ReadyAction += new ActionDelegate(readyCallback);
		}
	}
	
	public void Init(){
		
		ws = new WebSocket(new Uri(host));
		wsCoroutine = SocketLoop();
        StartCoroutine(wsCoroutine);
	}

	IEnumerator SocketLoop(){
		yield return StartCoroutine(ws.Connect());

		ready = true;
		Debug.Log("Sending ready");
		ReadyAction();

		ws.SendString("Hi there");

		int i=0;
		while (true)
		{
			string reply = ws.RecvString();
			if (reply != null)
			{
				Debug.Log ("Received: "+reply);
				ws.SendString("Hi there"+i++);
			}
			if (ws.error != null)
			{
				Debug.LogError ("Error: "+ws.error);
				break;
			}
			yield return 0;
		}
		ws.Close();
	}
}

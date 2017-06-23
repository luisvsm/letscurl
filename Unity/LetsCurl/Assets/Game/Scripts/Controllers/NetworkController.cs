using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkController:MonoBehaviour {
	public string host = "ws://localhost";
	private WebSocket ws;
	private IEnumerator wsCoroutine;

	private static NetworkController _instance;
	public static NetworkController Instance {
		get{
			if(_instance == null){
				_instance = GameObject.Find("NetworkController").GetComponent<NetworkController>();
				_instance.Init();
			}
			return _instance;
		}
	}

	public void Init(){
		ws = new WebSocket(new Uri(host));
		wsCoroutine = SocketLoop();
        StartCoroutine(wsCoroutine);
	}

	IEnumerator SocketLoop(){
		yield return StartCoroutine(ws.Connect());
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

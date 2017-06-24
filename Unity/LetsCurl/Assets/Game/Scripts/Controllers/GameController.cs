using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : BootableMonoBehaviour {

	public GameObject stonePrefab;
	private GameObject currentStone;
	List<GameObject> StonePool = new List<GameObject>();
	GameObject CurrentStone{
		get{
			if(currentStone == null){
				for (int i = 0; i < StonePool.Count; i++)
				{
					if(!StonePool[i].activeSelf){
						currentStone = StonePool[i];
						currentStone.SetActive(true);
						return currentStone;
					}
				}
				currentStone = Instantiate(stonePrefab, Vector3.zero, Quaternion.identity);
				StonePool.Add(currentStone);
			}
			return currentStone;
		}
	}
	public float forceMuliplyer = 1;
	public void SetThrowingStone(Vector3 position){
		CurrentStone.transform.position = position;
	}

	public void ThrowStone(Vector3 position, Vector3 forceVector){
		CurrentStone.GetComponent<Rigidbody>().AddForce(forceVector*forceMuliplyer, ForceMode.VelocityChange);
		currentStone = null;
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

	public override void Boot(){
		NetworkController.Instance.RegisterReady(NetworkControllerIsReady);
		NetworkController.Instance.Init();
	}

	private void NetworkControllerIsReady(){
		Debug.Log("Wooooo lets do this");
	}
}

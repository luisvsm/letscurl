using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stone : MonoBehaviour {
	public TrailRenderer trail;
	public Rigidbody _rigidbody;
	public Rigidbody body{
		get{
			if(_rigidbody == null)
				_rigidbody = gameObject.GetComponent<Rigidbody>();
			return _rigidbody;
		}
	}
	public GameController.Player owner = GameController.Player.Annon;
	public void ClearTrail(){
		if (trail == null){
			trail = gameObject.GetComponent<TrailRenderer>();
		}
		trail.Clear();
	}
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stone : MonoBehaviour {

	Rigidbody _rigidbody;
	public Rigidbody body{
		get{
			if(_rigidbody == null)
				_rigidbody = gameObject.GetComponent<Rigidbody>();
			return _rigidbody;
		}
	}

	enum Owner
	{
		Player1 = 1,
		Player2 = 2,
		Player3 = 3,
		Player4 = 4
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}

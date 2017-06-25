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
	public Player player;
	public Player.Team team {
		get{
			return player.team;
		}
	}
	public Color Team1Colour;
	public Color Team2Colour;
	public SpriteRenderer sprite;
	public void ClearTrail(){
		if (trail == null){
			trail = gameObject.GetComponent<TrailRenderer>();
		}
		trail.Clear();
	}
	public void setOwner(Player thePlayer){
		player = thePlayer;
		if(team == Player.Team.Team1)
			sprite.color = Team1Colour;
		if(team == Player.Team.Team2)
			sprite.color = Team2Colour;
	}
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}

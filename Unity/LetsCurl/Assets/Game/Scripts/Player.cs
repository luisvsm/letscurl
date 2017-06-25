using System;
using UnityEngine;
public class Player
{
	public enum Team
	{
		Annon = 0, // Something's not right
		Team1 = 1,
		Team2 = 2
	}
	public int playerID;
	public Team team;
	public Player(Team theTeam, int thePlayerID){
		playerID = thePlayerID;
		team = theTeam;
	}
}
	
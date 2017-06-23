using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BootController : MonoBehaviour {
	
	[SerializeField]
	public List<BootableMonoBehaviour> BootList; // List of things that we need to boot
	
	void Start () {
		for (int i = 0; i < BootList.Count; i++)
		{
			BootList[i].Boot();
		}
	}
}

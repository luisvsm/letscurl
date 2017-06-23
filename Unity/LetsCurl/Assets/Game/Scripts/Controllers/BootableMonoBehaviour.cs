using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BootableMonoBehaviour : MonoBehaviour {

	public virtual void Boot(){
		Debug.Log("[BootableMonoBehaviour] Booting " + this.GetType());
	}
}

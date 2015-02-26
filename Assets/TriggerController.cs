using UnityEngine;
using System.Collections;

public class TriggerController : MonoBehaviour {
	
	public GameObject[] objectsToActivate;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void Activate() {
	
		if(objectsToActivate.Length > 0) {
		
			foreach(GameObject _obj in objectsToActivate) {
			
				_obj.SendMessage("Activate");
		
			
			}
		
		}
		gameObject.SetActive(false);
		enabled = false;
	
	}
}

using UnityEngine;
using System.Collections;

public class InteractiveElement : MonoBehaviour {
	
	
	public InteractionController.InteractionType type;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public void InRange() {
	
	
	}
	
	public void Interacted() {
	
		Destroy(gameObject);
	
	}
	
	public void OutOfRange() {
		
		
	}
	

	
	
}

using UnityEngine;
using System.Collections;

public class FlashlightController : MonoBehaviour {
	
	private GameObject fpsCamera;
	
	// Use this for initialization
	void Start () {
	
		
		
	}
	
	void Awake() {
	
		fpsCamera = transform.parent.FindChild("FPSCamera").gameObject; 
	
	}
	
	// Update is called once per frame
	void Update () {
	
		transform.eulerAngles = new Vector3(fpsCamera.transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z);
		
	}
}

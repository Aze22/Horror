using UnityEngine;
using System.Collections;

public class FollowCamera : MonoBehaviour {
	
	private GameObject fpsCamera;
	
	// Use this for initialization
	void Start () {
	
	}
	
	void Awake() {
	
		fpsCamera = transform.parent.parent.FindChild("FPSCamera").gameObject; 
	
	}
	
	// Update is called once per frame
	void LateUpdate () {
	
		transform.eulerAngles = new Vector3(fpsCamera.transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z);
		transform.position = fpsCamera.transform.position;
	}
}

using UnityEngine;
using System.Collections;

public class CorpsAppearController : MonoBehaviour {
	
	private CharacterMotor motor;
	private GameObject flashlight;
	private MouseLook cameraMovement;
	private GameObject player;
	public GameObject[] corpses;
	private vp_FPSController fpsController;
	private vp_FPSCamera fpsCamera;
	// Use this for initialization
	void Start () {
		
		player = GameObject.FindGameObjectWithTag("Player");
		
		fpsController = player.GetComponent<vp_FPSController>();
		fpsCamera = player.transform.FindChild("FPSCamera").GetComponent<vp_FPSCamera>();
		flashlight = player.transform.FindChild("Flashlight").gameObject;
		
		
		foreach (GameObject current in corpses) {
		
			current.SetActive(false);
		
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void Activate() {
	
		fpsController.SetState("Freeze", true);
		fpsController.SetState("Crouch", false);
		flashlight.animation.Play("BridgeCorpseAppear");
		Invoke("LookAtCorpses", 4.5f);
		Invoke("ShowCorpses", 4.5f);
		Invoke("BackToGame", flashlight.animation["BridgeCorpseAppear"].length + 1.3f);
		
	
	}
	
	void LookAtCorpses() {
	
		fpsCamera.SetState("Freeze", true);
		fpsController.SetPosition(transform.position);
		fpsCamera.SetAngle(-10,15);
	
	}
	
	void BackToGame() {
	
		fpsCamera.SetState("Freeze", false);
		fpsController.SetState("Freeze", false);
	
	}
	
	void ShowCorpses() {
	
		foreach (GameObject current in corpses) {
		
			current.SetActive(true);
		
		}
	
	}
}

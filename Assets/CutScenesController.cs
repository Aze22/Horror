using UnityEngine;
using System.Collections;

public class CutScenesController : MonoBehaviour {
	
	private vp_FPSController fpsController;
	private vp_FPSCamera fpsCamera;
	
	public enum CutScenes {
	
		None,
		Intro,
		PickupFlashLight
	
	}
	public CutScenes currentCutScene;
	// Use this for initialization
	void Start () {
		if(currentCutScene != CutScenes.None) {
			fpsCamera.transform.FindChild("1Pistol").gameObject.SetActive(false);
			PlayCutScene(currentCutScene);
		}
	}
	
	void Awake() {
		
		fpsController = GameObject.Find("Character").GetComponent<vp_FPSController>();
		fpsCamera = GameObject.Find("Character/FPSCamera").GetComponent<vp_FPSCamera>();
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public void PlayCutScene(CutScenes cutSceneToPlay){
	
		if(cutSceneToPlay != CutScenes.None) {
		
			AnimationState animationToPlay = fpsController.gameObject.animation[cutSceneToPlay.ToString()];
			
			
			fpsController.SetState("Freeze", true);
			
			animation.Play(cutSceneToPlay.ToString());
			Invoke("SetInGame", animationToPlay.length);
			
		}
	
	}
	
	public void SetInGame() {
	
		fpsController.SetState("Freeze", false);
		
		if(currentCutScene == CutScenes.Intro) {
		
			
		
		}
		
		currentCutScene = CutScenes.None;
	}
}

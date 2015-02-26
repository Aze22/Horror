using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TouchControls : MonoBehaviour {

	public static bool touchControlsEnabled = false;
	public static GameObject player;
	
	public enum FingerTypes {
	
		None,
		Movement,
		Camera,
		GUI,
		Jump,
		Heal
	
	}
	
	[HideInInspector]
	public vp_FPSCamera fpsCamera;
	
	[HideInInspector]
	public InteractionController interactionController;
	
	public static bool shootTouchAsked = false;
	
	
	[System.Serializable]
	public class Finger {
	
		public TouchControls touchControls;
		[HideInInspector]
		public string name = "Finger";
		public FingerTypes type = FingerTypes.None;
		public Vector2 position = Vector2.zero;
		public Vector2 deltaPosition = Vector2.zero;
		public Vector2 startInputPosition = Vector2.zero;
		public Vector2 endInputPosition = Vector2.zero;
		public int fingerId = -1;
		public TouchPhase currentPhase = TouchPhase.Canceled;
		public bool deleteAsked = false;
		public Touch associatedTouch;
		public Vector2 offsetFromStart;
		public float duration = 0;
		public GameObject objectTouchedAtStart = null;
		
		
		
		public void Initialize() {
		
			touchControls = TouchControls.player.GetComponent<TouchControls>();
			type = FingerTypes.None;
			duration = 0;
		
		}
		
		public void Delete() {
		
			//Debug.Log("Delete " + fingerId);
			if(touchControls != null && touchControls.fingers.Count > 0) {
				CheckTap();
				touchControls.fingers.Remove(this);
			}
			//touchControls.fingers.Sort();
		
		}
		
		public void AskToDelete() {
			
			deleteAsked = true;
		}
		
		public void CheckTap() {
		
			
		
		}
	
	}
	
	[System.Serializable]
	public class TouchZone {
	
		public Rect rectangle;
		public FingerTypes type;
	
	}
	

	[HideInInspector]
	public List<TouchZone> allRectangles;
	public List<Finger> fingers;
	public Touch[] touches;
	
	public float tapDistance = 10;
	public LayerMask tapMask;
	
	
	
	public TouchZone cameraRectangle;
	public TouchZone movementRectangle;
	public TouchZone healRectangle;
	
	private vp_FPSPlayer fpsPlayer;
	public static bool touchReloadAsked = false;
	private Finger weaponFinger;
	private Finger healFinger;
	
	public static bool healTouchAsked = false;
	
	// Use this for initialization
	void Start () {
		player = GameObject.Find("CharacterContainer/Character");
		
		
		cameraRectangle.rectangle.width = Screen.width *0.5f;
		cameraRectangle.rectangle.height = Screen.height;
		cameraRectangle.rectangle.x = Screen.width - cameraRectangle.rectangle.width;
		
		movementRectangle.rectangle.width = Screen.width *0.5f;
		movementRectangle.rectangle.height = Screen.height;
		movementRectangle.rectangle.x = 0;
		
		healRectangle.rectangle.width = Screen.width *0.35f;
		healRectangle.rectangle.height = Screen.height * 0.25f;
		healRectangle.rectangle.x = Screen.width - healRectangle.rectangle.width;
		healRectangle.rectangle.y = Screen.height - healRectangle.rectangle.height;
		
		allRectangles.Add(cameraRectangle);
		allRectangles.Add(movementRectangle);
		allRectangles.Add(healRectangle);
		
		fpsCamera = GameObject.Find("CharacterContainer/Character/FPSCamera").GetComponent<vp_FPSCamera>();
		fpsPlayer = GameObject.Find("CharacterContainer/Character").GetComponent<vp_FPSPlayer>();
		interactionController = player.GetComponent<InteractionController>();
		shootTouchAsked = false;
		touchReloadAsked = false;
		weaponFinger = null;
		healFinger = null;
		healTouchAsked = false;
		
		
	}
	
	// Update is called once per frame
	void Update () {
		
		if(touchControlsEnabled) {
		
			UpdateFingers();
			UpdateDeletesFingers();
			UpdateCameraRotation();
			UpdateMovement();
		
		}
	
	}
	
	void UpdateCameraRotation() {
	
		if(touchControlsEnabled) {
			if(GetFinger(FingerTypes.Camera) != null)
				fpsCamera.SetCameraRotationInput(GetFinger(FingerTypes.Camera).deltaPosition.x, GetFinger(FingerTypes.Camera).deltaPosition.y);
			else fpsCamera.SetCameraRotationInput(0,0);
		}
	
	}
	
	void UpdateMovement() {
	
		if(touchControlsEnabled) {
			if(GetFinger(FingerTypes.Movement) != null)
				fpsPlayer.SetMovementInput(GetFinger(FingerTypes.Movement).offsetFromStart);
			else fpsPlayer.SetMovementInput(Vector2.zero);
		}
	
	}
	
	void UpdateDeletesFingers() {
	
		List<Finger> listToRemove = new List<Finger>();
		
		foreach(Finger finger in fingers) {
		
		
			if(finger.deleteAsked) listToRemove.Add(finger);
		
		}
		
		foreach(Finger finger in listToRemove) {
		
		
			
			finger.Delete();
			
		
		}
	
	}
	
	void UpdateFingers() {
	
		touches = Input.touches;
		
		if(Input.touchCount > 0) {
	
			int currentTouchIndex = 0;
			
			foreach (Touch touch in touches) {
		
				switch(touch.phase) {			
					case TouchPhase.Began:
					if(GetFinger(touch) == null) {
						//Debug.Log("Start");
						
						TouchControls.Finger newFinger;
						newFinger = new TouchControls.Finger();
						newFinger.Initialize();
						newFinger.startInputPosition = new Vector2(touch.position.x, Screen.height - touch.position.y);;
						newFinger.associatedTouch = touch;
						newFinger.objectTouchedAtStart = GetTouchedObject(newFinger);
						
						if(newFinger.objectTouchedAtStart != null && newFinger.objectTouchedAtStart.tag == "PlayerWeapon") {
							weaponFinger = newFinger;
							Invoke("ReloadWeapon", 1);
							GUIController.ShowMags(true);
						}
						
						CheckRectangleCollision(touch, newFinger);
						
						if(IsInRectangle(touch, FingerTypes.Heal)) {
						
							healFinger = newFinger;
							Invoke("Heal", 0.5f);
							GUIController.ShowHealthBar(true);
							//Debug.Log("HEAL");
						
						}
						
						fingers.Add(newFinger);
						
					
					}
					break;
					
					case TouchPhase.Ended:
					if(GetFinger(touch) != null) {
					
						GameObject touchedObj = GetTouchedObject(GetFinger(touch));
						Finger finger = GetFinger(touch);
			
						finger.endInputPosition = finger.position;
			
						if(touchedObj != null && finger.objectTouchedAtStart != null && touchedObj == finger.objectTouchedAtStart){
								
							if(touchedObj.tag == "PlayerWeapon") {
								if(finger.duration < 0.5f) {
									CancelInvoke("ReloadWeapon");
									weaponFinger = null;
									shootTouchAsked = true;
								}
								
							}
							else if (touchedObj == interactionController.GetInteractionObj() && finger.duration < 0.5f){
								//Interaction
								interactionController.ActivateInteraction(interactionController.GetInteractionType());
							}
						}
			
						if(finger.objectTouchedAtStart != null && finger.objectTouchedAtStart.tag == "PlayerWeapon") {
							CancelInvoke("ReloadWeapon");
							weaponFinger = null;
						
						}
						
						if(finger == healFinger) {
						
							CancelInvoke("Heal");
						
						}
						
						
						GetFinger(touch).Delete();
					}
					break;
					
					case TouchPhase.Canceled:
					if(GetFinger(touch) != null) 
					GetFinger(touch).Delete();
					break;
					
					
					default:	
					// Movement
				
					if(GetFinger(touch) != null)
						GetFinger(touch).associatedTouch = touch;
					
					break;
				}
				
					
					
			
			
			}
			
			foreach(Finger finger in fingers) {
			
				finger.currentPhase = finger.associatedTouch.phase;
				finger.deltaPosition = finger.associatedTouch.deltaPosition;
				finger.fingerId = finger.associatedTouch.fingerId;
				
				finger.position = new Vector2(finger.associatedTouch.position.x, Screen.height - finger.associatedTouch.position.y);
				//Debug.Log("Position: "+ finger.position.ToString());
				
				finger.offsetFromStart = new Vector2(finger.position.x - finger.startInputPosition.x, (Screen.height - finger.position.y) - (Screen.height - finger.startInputPosition.y));
				//Debug.Log("Offset: "+ finger.offsetFromStart.ToString());
				finger.duration += Time.deltaTime;
				
				/*finger.offsetFromStart.x = Vector2.Distance(finger.position, finger.startInputPosition);
				finger.offsetFromStart.y = Vector2.Distance(finger.position, finger.startInputPosition);*/
				
				/*if((finger.position.y - finger.startInputPosition.y) > 0)
					finger.position.y = -finger.position.y;
				
				if((finger.position.x - finger.startInputPosition.x) < 0)
					finger.position.x = -finger.position.x;*/
				
				//finger.type = FingerTypes.Camera;
	
			
			}
		
		}
	
	}
	
	void CheckRectangleCollision(Touch touch, Finger newFinger) {
	
		foreach(TouchZone rectangle in allRectangles) {
			
			if(rectangle.rectangle != null) {
				
				if(rectangle.rectangle.Contains(touch.position))
					newFinger.type = rectangle.type;
			}
			
		}
	
	}
	
	bool IsInRectangle(Touch touch, FingerTypes rectangleType) {
	
		foreach(TouchZone rectangle in allRectangles) {
			
			if(rectangle.rectangle != null && rectangle.type == rectangleType) {
				
				if(rectangle.rectangle.Contains(touch.position))
					return true;
				else return false;
			}
			
			
		}
		return false;
	
	}
	
	void OnEnable() {
	
		touchControlsEnabled = true;
	
	}
	
	void OnDisable() {
	
		touchControlsEnabled = false;
	
	}
	
	void ReloadWeapon() {
	
		GameObject currentTouchedObj = GetTouchedObject(weaponFinger);
		if(weaponFinger != null && currentTouchedObj != null && currentTouchedObj.tag == "PlayerWeapon")
			touchReloadAsked = true;
		else touchReloadAsked = false;
		
		weaponFinger = null;
	
	}
	
	void Heal() {
	
		healTouchAsked = true;
		healFinger = null;
	
	}
	
	public GameObject GetTouchedObject(Finger finger) {
	
		//Ray ray = new Ray(fpsCamera.camera.ViewportToWorldPoint(finger.position), fpsCamera.camera.transform.forward);
		RaycastHit hitInfo;
		
		if(Physics.Raycast(fpsCamera.camera.ScreenPointToRay(finger.associatedTouch.position), out hitInfo)) {
		
			//Debug.Log(hitInfo.collider.gameObject.name);
			return hitInfo.collider.gameObject;
		
		}
		else {
			//Debug.Log("Touched nothing");
			return null;
		}
	
	}
	
	public Vector2 GetFingerOffset(FingerTypes type) {
	
		
		return GetFinger(type).deltaPosition;
	
	}
	
	public Finger GetFinger(FingerTypes type) {
	
		foreach (Finger finger in fingers) {
		
			if(type == finger.type) return finger;
		
		}
		
		return null;
	
	}
	
	public Finger GetFinger(Touch touchRef) {
	
		foreach (Finger finger in fingers) {
		
			if(touchRef.fingerId == finger.associatedTouch.fingerId) return finger;
		
		}
		
		return null;
	
	}
	
	public void OnGUI() {
	
		//GUI.Label(new Rect( 20,20,100,100), new GUIContent(fingers.Count.ToString()));
	
	}
}

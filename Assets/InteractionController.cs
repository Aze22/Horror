using UnityEngine;
using System.Collections;

public class InteractionController : MonoBehaviour {
	
	public enum InteractionType {
		
		None,
		Ammo,
		SmallKit,
		LargeKit
		
	}
	
	
	private GameObject currentInteractionObj;
	private InteractionType currentInteractionType;

	
	// Use this for initialization
	void Start () {
		
	}
	
	public GameObject GetInteractionObj() {
	
		return currentInteractionObj;
	
	}
	
	public InteractionType GetInteractionType() {
	
		return currentInteractionType;
	
	}
	
	void Awake() {
	
		InitializeAwakeVariables();
		
	
	}
	
	void InitializeAwakeVariables() {
		
		
		
	}
	
	void OnTriggerEnter(Collider other) {
	
		//Debug.Log("FUCK");
		if(other.tag == "Trigger") {
		
			other.SendMessage("Activate");
		
		}
		else if(other.tag == "Collectible") {
		
			if(other.transform.parent != null && other.transform.parent.GetComponent<InteractiveElement>() != null) {
				
				InteractionType interactiveElementType = other.transform.parent.GetComponent<InteractiveElement>().type;
				
				if(other.collider.transform.parent.gameObject != null)
					currentInteractionObj = other.collider.transform.parent.gameObject;
				
				EnableInteraction(interactiveElementType);
				
			}
		
		}
	
	}
	
	void OnTriggerExit(Collider other) {
	
		
		if(other.tag == "Collectible") {
		
			if(other.transform.parent != null && other.transform.parent.GetComponent<InteractiveElement>() != null && currentInteractionObj != null && currentInteractionType != InteractionType.None) {
				
				InteractionType interactiveElementType = other.transform.parent.GetComponent<InteractiveElement>().type;
				
				if(currentInteractionObj == other.transform.parent.gameObject && currentInteractionType == interactiveElementType)
					DisableInteraction();
				
			}
		
		}
	
	}
	
	void EnableInteraction(InteractionType interactionType) {
		
		currentInteractionType = interactionType;
		
		currentInteractionObj.SendMessage("InRange", SendMessageOptions.DontRequireReceiver);
		GUIController.ShowInteraction(interactionType);
			
	}
	
	public void ActivateInteraction(InteractionType interactionType) {
		
		if(interactionType == InteractionType.SmallKit || interactionType == InteractionType.LargeKit ||interactionType == InteractionType.Ammo) {
		
			if(interactionType == InteractionType.SmallKit || CanInteract(interactionType)) {
				if((interactionType == InteractionType.SmallKit && StatsController._hp < 100) || interactionType != InteractionType.SmallKit) {
					StatsController.PickupObject(interactionType);
				
					currentInteractionObj.SendMessage("Interacted", SendMessageOptions.DontRequireReceiver);
					currentInteractionType = InteractionType.None;
					currentInteractionObj = null;
					GUIController.HideInteraction();
				}
			}
		}
		
	}
	
	public static bool CanInteract(InteractionType type) {
	
		if(StatsController.GetFromInventory(type) < StatsController.GetMaxFromInventory(type)) {
			return true;
		}
		else return false;
	
	}
	
	void DisableInteraction() {
	
		currentInteractionObj.SendMessage("OutOfRange", SendMessageOptions.DontRequireReceiver);
		currentInteractionType = InteractionType.None;
		currentInteractionObj = null;
		GUIController.HideInteraction();
	
	}
	
	void Update() {
	
		if(Input.GetButtonDown("Interaction")) {
		
			if(currentInteractionType != InteractionType.None) {
			
				ActivateInteraction(currentInteractionType);
			
			}
		
		}
		
		if((Input.GetButtonDown("Heal") && !TouchControls.touchControlsEnabled) || (TouchControls.touchControlsEnabled && TouchControls.healTouchAsked )) {
		
			TouchControls.healTouchAsked = false;
			if(StatsController._numberOfKits > 0 && StatsController._hp < 100) {
			
				StatsController.Heal(InteractionType.LargeKit);
			
			}
		
		}
	
	}
}

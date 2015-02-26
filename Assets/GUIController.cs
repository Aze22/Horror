using UnityEngine;
using System.Collections;

public class GUIController : MonoBehaviour {

	private GameObject character;
	private StatsController statsController;
	private static UISlider guiHealthSlider;
	private static UILabel ammoLabel;
	private static UILabel kitCountLabel;
	private static GameObject interactionHolder;
	private static UILabel interactionLabel;
	private static string interactionStringKey;
	private static bool changeHP;
	private static float guiHP;
	private static UILabel magCountLabel;
	
	private static UIPanelAlpha healthFadingGUI;
	private static float healthGuiAlpha;
	public float healthGuiDuration;
	private static float _healthGuiDuration;
	private static float healthGuiDurationOriginal;
	
	private static UIPanelAlpha magsFadingGUI;
	private static float magsGuiAlpha;
	public float magsGuiDuration;
	private static float _magsGuiDuration;
	private static float magsGuiDurationOriginal;

	
	// Use this for initialization
	void Start () {
		
		guiHealthSlider = transform.FindChild("Health/Bar").GetComponent<UISlider>();
		kitCountLabel = transform.FindChild("Health/Kits/Count").GetComponent<UILabel>();
		ammoLabel = transform.FindChild("Ammo/Count").GetComponent<UILabel>();
		magCountLabel= transform.FindChild("Ammo/Count").GetComponent<UILabel>();
		
		interactionHolder = transform.FindChild("Interaction/Holder").gameObject;
		interactionLabel = interactionHolder.transform.FindChild("Text").GetComponent<UILabel>();
		SetLabel(interactionLabel, interactionStringKey);
		interactionHolder.transform.FindChild("BG").gameObject.SetActive(false);
		interactionHolder.transform.FindChild("BG").gameObject.SetActive(true);
		guiHP = StatsController._hp;
	}
	
	void Awake() {
	
		InitializeAwakeVariables();
		
	
	}
	
	void InitializeAwakeVariables() {
			
		character = GameObject.Find("Character");
		statsController = character.GetComponent<StatsController>();
		changeHP = false;
		
		healthFadingGUI = transform.FindChild("Health").GetComponent<UIPanelAlpha>();
		healthGuiAlpha = 1;
		_healthGuiDuration = healthGuiDuration;
		healthGuiDurationOriginal = healthGuiDuration;
		
		magsFadingGUI = transform.FindChild("Ammo").GetComponent<UIPanelAlpha>();
		magsGuiAlpha = 1;
		_magsGuiDuration = magsGuiDuration;
		magsGuiDurationOriginal = magsGuiDuration;
	
	}
	
		
	
	public static void ChangeHPValue(float hp, float armor) {
	
		if(guiHealthSlider != null) {
			changeHP = true;
			ShowHealthBar(true);
			
		}
	
	}
	
	public static void ShowHealthBar(bool state) {
	
		if(state) 
			_healthGuiDuration = healthGuiDurationOriginal;
		else _healthGuiDuration = 0;
		
	
	}
	
	public static void ShowMags(bool state) {
	
		if(state) 
			_magsGuiDuration = magsGuiDurationOriginal;
		else _magsGuiDuration = 0;
		
	
	}
	
	public static void ShowInteraction(InteractionController.InteractionType interactionType) {
		
	
		
		interactionHolder.transform.parent.animation["ShowInteraction"].speed = 1;
		
		if(!interactionHolder.transform.parent.animation.IsPlaying("ShowInteraction")) {
		
			interactionHolder.transform.parent.animation["ShowInteraction"].time = 0;
			interactionHolder.transform.parent.animation.Play("ShowInteraction");
			
		}
		
		// Show Pikcup sign
		
		if(interactionType == InteractionController.InteractionType.Ammo) {
			
			if(InteractionController.CanInteract(InteractionController.InteractionType.Ammo))
				interactionStringKey = "AmmoPickup";
			else interactionStringKey = "AmmoPickupFull";
			
			GUIController.ShowMags(true);
		}
		
		else if(interactionType == InteractionController.InteractionType.LargeKit) {
			if(InteractionController.CanInteract(InteractionController.InteractionType.LargeKit))
				interactionStringKey = "LargeKitPickup";
			else interactionStringKey = "LargeKitPickupFull";
			ShowHealthBar(true);
		}
		
		else if(interactionType == InteractionController.InteractionType.SmallKit) {
			if(StatsController._hp < 100) 
				interactionStringKey = "SmallKitPickup";
			else interactionStringKey = "SmallKitPickupFull";
			ShowHealthBar(true);
		}
		
		if(TouchControls.touchControlsEnabled) interactionStringKey = "Touch_" + interactionStringKey;
		
		SetLabel(interactionLabel, interactionStringKey);
	}


	public static void HideInteraction() {
		
		
		
		interactionHolder.transform.parent.animation["ShowInteraction"].speed = -1;
		
		if(!interactionHolder.transform.parent.animation.IsPlaying("ShowInteraction")) {
		
			interactionHolder.transform.parent.animation["ShowInteraction"].time = interactionHolder.transform.parent.animation["ShowInteraction"].length;
			interactionHolder.transform.parent.animation.Play("ShowInteraction");
		}
		
	}
	
	public static void SetLabel(UILabel label, string newText) {
	
		if(label.GetComponent<UILocalize>() != null) {
			label.GetComponent<UILocalize>().key = newText;
			label.gameObject.SetActive(false);
			label.gameObject.SetActive(true);
		}
		else {
		
			label.text = newText;
			
		}
	
	}
	
	public static void ChangeKits() {
		
		string colorString = "[FFFFFF]";
		string whiteString = "[FFFFFF]";
		string normalString = "[FF99AA]";
		string greenColor = "[CCFFCC]";
		string redColor = "[FF3333]";
	
		if(StatsController._numberOfKits <= 0)
			colorString = redColor;
		else if(StatsController._numberOfKits > 0 && StatsController._numberOfKits <StatsController._numberOfKitsMax)
			colorString = normalString;
		else colorString = greenColor;
	
		SetLabel(kitCountLabel, colorString + StatsController._numberOfKits.ToString() + "x");
		ShowHealthBar(true);
	}
	
	public static void ChangeMagsAvailable() {
	
		string colorString = "[FFFFFF]";
		string whiteString = "[FFFFFF]";
		string normalString = "[FF99AA]";
		string greenColor = "[CCFFCC]";
		string redColor = "[FF3333]";
	
		if(StatsController._magsAvailable <= 0)
			colorString = redColor;
		else if(StatsController._magsAvailable > 0 && StatsController._magsAvailable <StatsController._numberOfKitsMax)
			colorString = normalString;
		else colorString = greenColor;
	
		SetLabel(magCountLabel, colorString + StatsController._magsAvailable.ToString() + "x");
	
	}
	
	void Update() {
		
		//ammoLabel.text = StatsController._ammo + "/" + StatsController._availableAmmo;
		
		if(changeHP) {
		
			if(guiHP > StatsController._hp) {
			
				guiHP -= (Time.deltaTime * 270);
				if(guiHP <= StatsController._hp) {
				
					guiHP = StatsController._hp;
					changeHP = false;
				
				}
			
			}
			else if(guiHP < StatsController._hp) {
				guiHP += (Time.deltaTime * 50);
				if(guiHP >= StatsController._hp) {
				
					guiHP = StatsController._hp;
					changeHP = false;
				
				}
			
			}
			
			guiHealthSlider.sliderValue = guiHP / 100;
		
		}
		
		
		if(_healthGuiDuration > 0) {
		
			if(healthGuiAlpha < 1) healthGuiAlpha += Time.deltaTime *5;
			else healthGuiAlpha = 1;
			
			_healthGuiDuration -= Time.deltaTime;
			
		
		}
		else {
		
			_healthGuiDuration = 0;
			
			if(healthGuiAlpha > 0) 
				healthGuiAlpha -= Time.deltaTime * 0.5f;
			else healthGuiAlpha = 0;
		
		}
		
		healthFadingGUI.alpha = healthGuiAlpha;
		
		
		
		if(_magsGuiDuration > 0) {
		
			if(magsGuiAlpha < 1) magsGuiAlpha += Time.deltaTime *5;
			else magsGuiAlpha = 1;
			
			_magsGuiDuration -= Time.deltaTime;
			
		
		}
		else {
		
			_magsGuiDuration = 0;
			
			if(magsGuiAlpha > 0) 
				magsGuiAlpha -= Time.deltaTime * 0.5f;
			else magsGuiAlpha = 0;
		
		}
		
		magsFadingGUI.alpha = magsGuiAlpha;
	}
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StatsController : MonoBehaviour {
	
	[System.Serializable]
	public class Life {
		
		public float HP = 100;
		public float armor = 100;
		public float smallKitHeals = 15;
		public float largeKitHeals = 45;
		public int numberOfKits = 0;
		public int numberOfKitsMax = 3;
	
	}
	
	public static float _smallKitHealValue;
	public static float _largeKitHealValue;
	
	public Life life;
	
	private bool canBeDamaged = true;
	public static float _hp;
	public static float _armor;
	
	[System.Serializable]
	public class Weapon {
	
		public int ammoInCurrentMag;
		public int magCapacity;
		public int magsAvailable;
		public int magsAvailableMax;
		public int ammoPackAds;
		public LayerMask shootLayerMask;
		public float damage;
		
		
		[HideInInspector] public GameObject weaponObject;
		[HideInInspector] public vp_FPSShooter controller;
	
	}
	
	public Weapon gun;
	public static Weapon currentWeapon;
	
	public static int _ammoInCurrentMag;
	public static int _magCapacity;
	public static int _magsAvailable;
	public static int _magsAvailableMax;
	
	public static int _numberOfKits = 0;
	public static int _numberOfKitsMax = 3;
	
	private GUIController guiController;
	public static List<InteractionController.InteractionType> inventory;
	
	// Use this for initialization
	void Start () {
	
		InitializeVariablesAtStart();
		
	}
	
	void Awake() {
	
		InitializeVariables();
		
	}
	
	void InitializeVariables() {
	
		_hp = life.HP;
		_armor = life.armor;
		_numberOfKits = life.numberOfKits;
		_numberOfKitsMax = life.numberOfKitsMax;
		
		guiController = transform.FindChild("GUI2D/Camera/Controller").GetComponent<GUIController>();
		currentWeapon = gun;
		
		_ammoInCurrentMag = currentWeapon.ammoInCurrentMag;
		_magCapacity = currentWeapon.magCapacity;
		_magsAvailable = currentWeapon.magsAvailable;
		_magsAvailableMax = currentWeapon.magsAvailableMax;
		
		gun.weaponObject = transform.FindChild("FPSCamera/1Pistol").gameObject;
		gun.controller = gun.weaponObject.GetComponent<vp_FPSShooter>();
		gun.controller.AmmoCount = gun.ammoInCurrentMag;
		gun.controller.AmmoMaxCount = gun.magCapacity;	
		
		_smallKitHealValue = life.smallKitHeals;
		_largeKitHealValue = life.largeKitHeals;
		inventory = new List<InteractionController.InteractionType>();
		
	
	}
	
	void InitializeVariablesAtStart() {
		GUIController.ChangeHPValue(_hp, _armor);
	}
	
	public static void Damage(float hp) {
	
		if(_armor > 0) 
			_armor -= hp;
		else _hp -= hp;
		
		
		if(_armor < 0) _armor = 0;
		
		if(_hp <= 0) {
		
			_hp = 0;
			Die();
		
		}
		
		GUIController.ChangeHPValue(_hp, _armor);
		Debug.Log("Damage! Armor: " + _armor + " | HP: " + _hp);
	
	}
	
	public static void Die() {
	
		Application.LoadLevel(Application.loadedLevel);
	
	}
	
	public static void PickupObject(InteractionController.InteractionType objectType) {
		if(objectType == InteractionController.InteractionType.SmallKit) {
		
			Heal(objectType);
		
		}
		else if(objectType == InteractionController.InteractionType.LargeKit) {
		
			if(_numberOfKits < _numberOfKitsMax) {
				AddToInventory(objectType, 1);
				GUIController.ChangeKits();
			}
			else InventoryFull(objectType);
		
		}
		else if(objectType == InteractionController.InteractionType.Ammo) {
			if(_magsAvailable < _magsAvailableMax) {
				AddToInventory(objectType, currentWeapon.ammoPackAds);
				GUIController.ChangeMagsAvailable();
			}
			else InventoryFull(objectType);
		}
	}
	
	public static void InventoryFull(InteractionController.InteractionType objectFullType) {
	
		Debug.Log("Inventory Full of " + objectFullType.ToString());
	
	}
	
	public static void AddToInventory(InteractionController.InteractionType objectTypeToAdd, int numberToAdd) {
	
		
		if(numberToAdd > 0 && objectTypeToAdd != InteractionController.InteractionType.None) {
			 
			for(int i = 0; i < numberToAdd ; i++) {
				inventory.Add(objectTypeToAdd);
					
			}
			
			_numberOfKits = GetFromInventory(InteractionController.InteractionType.LargeKit);
			_magsAvailable = GetFromInventory(InteractionController.InteractionType.Ammo);
			
			
			Debug.Log("Added " + numberToAdd + " " + objectTypeToAdd.ToString() + " to inventory");
			
		}
		
	
	}
	
	public static void RemoveFromInventory(InteractionController.InteractionType objectTypeToRemove, int numberToRemove) {
	
		
		if(numberToRemove > 0 && objectTypeToRemove != InteractionController.InteractionType.None) {
			 
			for(int i = 0; i < numberToRemove ; i++) {
				inventory.Remove(objectTypeToRemove);
					
			}
			
			_numberOfKits = GetFromInventory(InteractionController.InteractionType.LargeKit);
			_magsAvailable = GetFromInventory(InteractionController.InteractionType.Ammo);
		
			
			Debug.Log("Added " + numberToRemove + " " + objectTypeToRemove.ToString() + " to inventory");
			
		}
		
	
	}
	
	public static int GetFromInventory(InteractionController.InteractionType typeToGet) {
		
		int numberToReturn = 0;
		
		inventory.ForEach(type => {
			if(type == typeToGet)
				numberToReturn++;
		}
		);
		
		return numberToReturn;
		
	}
	
	public static int GetMaxFromInventory(InteractionController.InteractionType typeToGet) {
		
		switch(typeToGet) {
		
		case InteractionController.InteractionType.LargeKit:
			return _numberOfKitsMax;
			break;
		case InteractionController.InteractionType.Ammo:
			return _magsAvailableMax;
			break;
		default:
			return 0;
			break;
		
		}
	}
	
	public static void Heal(InteractionController.InteractionType kitType) {
	
		float valueToHeal = _smallKitHealValue;
		
		if(kitType == InteractionController.InteractionType.LargeKit) {
			valueToHeal = _largeKitHealValue;
			RemoveFromInventory(InteractionController.InteractionType.LargeKit, 1);
			GUIController.ChangeKits();
		}
		
		Debug.Log("Healed " + valueToHeal + " HP");
		
		_hp += valueToHeal;
		if(_hp > 100) _hp = 100;
		
		GUIController.ChangeHPValue(_hp, _armor);
		
		
	
	}
	
	
	// Update is called once per frame
	void Update () {
	
	}
}

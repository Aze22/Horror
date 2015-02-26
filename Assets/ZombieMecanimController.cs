using UnityEngine;
using System.Collections;

public class ZombieMecanimController : MonoBehaviour {
	
	private Animator animator;
	
	private NavMeshAgent nma;
	private GameObject player = null;
	private bool justAttacked = false;
	private vp_FPSController fpsController;
	private vp_FPSPlayer fpsPlayer;
	private float _hp;

	
	public enum State {
	
		Idle,
		Patrol,
		Follow,
		Flee,
		Attack,
		Dead
	
	}
	[System.Serializable]
	public class Movement {
		
		public float patrolSpeed = 1.8f;
		public float runSpeed = 3.5f;
		public bool followAfterHurt = true;
		
		
	}
	
	
	
	[System.Serializable]
	public class Vision {
	
		public bool canFollowPlayer = true;
		public float range = 15;
		public float angle = 45;
		public float delay = 2;
		public LayerMask obstruction;
		[HideInInspector] public float currentDelay = 1;
	
	}
	
	[System.Serializable]
	public class Life {
		
		public bool invincible;
		public float HP;
		
	}
	
	[System.Serializable]
	public class Attack {
		
		public bool canAttack = true;
		public float range = 2;
		public float rate = 1;
		public float damage = 30;
		
	}
	
	public Movement movement;
	public Life life;
	public Vision vision;
	public Attack attack;
	public State currentState = State.Idle;
	
	private Vector3 playerPosition;
	private Vector3 enemyPosition;
	private State previousState = State.Idle;
	private float distanceToPlayer;
	private GameObject waypointsHolder;
	private GameObject[] waypoints;
	private int numberOfWaypoints;
	private GameObject currentWaypoint;
	private int currentWaypointIndex;
	private bool pausePatrol;
	private bool firstWaypoint;
	private GameObject eyes;
	
	
	
	
	// Use this for initialization
	void Start () {
		
	}
	
	void Awake() {
		
		InitializeVariables();
		
	}
	
	void InitializeVariables() {
		
		player = GameObject.FindGameObjectWithTag("Player");
		playerPosition = player.transform.position;
		enemyPosition = transform.position;
		animator = GetComponent<Animator>();
		nma = GetComponent<NavMeshAgent>();
		distanceToPlayer = 0;
		vision.currentDelay = vision.delay;
		fpsController = player.GetComponent<vp_FPSController>();
		fpsPlayer = player.GetComponent<vp_FPSPlayer>();
		waypointsHolder = null;
		numberOfWaypoints = 0;
		_hp = life.HP;
		currentWaypoint = null;
		currentWaypointIndex = 0;
		pausePatrol = false;
		firstWaypoint = true;
		if(transform.parent.parent.name == "Zombies") eyes = transform.FindChild("Armature/hips/spine/ribs/neck/head/Eyes").gameObject;
			else eyes = this.gameObject;
		GetWaypoints();
		
		if(currentState == State.Patrol) SetNextPatrolWaypoint();
			
		
		
		
	}
	
	void GetWaypoints() {
	
		waypointsHolder = GameObject.Find("LevelDesign/Enemies/" + transform.parent.parent.name + "/" + "Waypoints/" + name);
		
		if(waypointsHolder != null && waypointsHolder.transform.childCount > 0) {
			
			numberOfWaypoints = waypointsHolder.transform.GetChildCount();
			waypoints = new GameObject[numberOfWaypoints];
			for(int i = 0 ; i <  numberOfWaypoints ; i++) {
				waypoints[i] = waypointsHolder.transform.GetChild(i).gameObject;
			}
			
			currentWaypoint = waypoints[0];
			currentWaypointIndex = -1;
		}
		else {
			if(currentState == State.Patrol) currentState = State.Idle;
			numberOfWaypoints = 0;
			currentWaypoint = null;
		}
	}
	
	// Update is called once per frame
	void Update () {
		
		//if(_hp > 0) {
			
			enemyPosition = transform.position;
			playerPosition = player.transform.position;
			distanceToPlayer = Vector3.Distance(enemyPosition, playerPosition);
			
			
			switch(currentState) {
			case State.Dead:
				DeadUpdate();
				break;
			case State.Idle:
				IdleUpdate();
				break;
			case State.Patrol:
				PatrolUpdate();
				break;
			case State.Follow:
				FollowPlayerUpdate();
				break;
			case State.Flee:
				break;
			case State.Attack:
				AttackUpdate();
				break;
				
			default:
				break;
			
			
			}
		//}
		
	//	else {
		
			// Deactivate the enemy if something went wrong and he's dead, but animation did not play
		//	if(nma.enabled) {
			
		//		Die();
			
	//		}
		
		//	}
	
	}
	
	private void DeadUpdate () {
	
		
			//animation.enabled = true;
			
		
	
	}
	
	private void IdleUpdate() {
		
		if(vision.canFollowPlayer && DetectPlayer()) currentState = State.Follow;
		
		if(!vision.canFollowPlayer && attack.canAttack	) {
		
			if(distanceToPlayer < attack.range) {
			
				currentState = State.Attack;
			
			}
		
		}
				
	}
	
	private void PatrolUpdate() {
	
		if(!animator.GetBool("Walk") && !pausePatrol)
			animator.SetBool("Walk", true);
		
		nma.speed = movement.patrolSpeed;

		if(nma.remainingDistance <= 0.1f && !pausePatrol){
			
			pausePatrol = true;
			animator.SetBool("Walk", false);
	
			Invoke ("SetNextPatrolWaypoint", currentWaypoint.GetComponent<WaypointController>().wait);
		}
		
		if(DetectPlayer()) {
			if(animator.GetBool("Walk"))
				animator.SetBool("Walk", false);
			currentState = State.Follow;
		}
	}
	
	private void SetNextPatrolWaypoint() {
	
		currentWaypointIndex++;
		if(currentWaypointIndex >= numberOfWaypoints) {
			currentWaypointIndex = 0;
		}
		
		currentWaypoint = waypoints[currentWaypointIndex];
		pausePatrol = false;
		if(nma.enabled)
			nma.SetDestination(currentWaypoint.transform.position);
		animator.SetBool("Walk", true);
	
	
	}
	
	private bool DetectPlayer() {
	
		
		if(vision.canFollowPlayer) {
			if(distanceToPlayer < vision.range && PlayerIsVisible()) {
				
			
			
				if(vision.currentDelay > 0) {
					vision.currentDelay -= Time.deltaTime;
					return false;
				}
				else {
					vision.currentDelay = vision.delay;
					animator.SetBool("Idle", false);
					return true;
				}
				
			
			}
			else {
			
				if(vision.currentDelay != vision.delay) vision.currentDelay = vision.delay;
				return false;
			
			}
		}
		else return false;
	
	}
	
	private bool PlayerIsVisible() {
	
		// Check player is in angle	
		if(Vector3.Angle(transform.forward, (playerPosition - enemyPosition)) < vision.angle) {
		
			RaycastHit hit;
			if(Physics.Linecast(eyes.transform.position, player.transform.FindChild("FPSCamera").position, out hit, vision.obstruction)) {
			
				return false;
				
			}
			else return true;
		}
		else return false;
		
	
	}
	
	
	private void FollowPlayerUpdate() {
		
		if(_hp > 0 && nma.enabled) {
			if(distanceToPlayer > attack.range) {
				nma.speed = movement.runSpeed;
				nma.SetDestination(playerPosition);
				if(!animator.GetBool("Run"))
					animator.SetBool("Run", true);
			}
			else {
				StopRunning();
				if(attack.canAttack)
					currentState = State.Attack;
				else 
					currentState = State.Idle;
			}
		}
	}
	
	private void AttackUpdate() {
		
		if(!animator.GetBool("Attack")) {
			animator.SetBool("Attack", true);
			GUIController.ShowHealthBar(true);
		}
		
		if(distanceToPlayer <= attack.range) {
			
			
			if(animator.GetFloat("HitProgression") > 1f && !justAttacked) DamagePlayer();
			
		}
		else {
			animator.SetBool("Attack", false);
			if(vision.canFollowPlayer)
				currentState = State.Follow;
			else currentState = State.Idle;
		}
		
	}
	
	private void StopRunning() {
	
		nma.Stop();
		animator.SetBool("Run", false);
	
	}
	
	private void DamagePlayer() {
	
		justAttacked = true;
		//fpsPlayer.Damage(attack.damage);
		StatsController.Damage(30);
		Invoke ("JustAttackedReset", 0.5f);
		
		
	
	}
	
	private void JustAttackedReset() {
	
		justAttacked = false;
	
	}
	
	public void Hurt() {
	
		if(!life.invincible) {
			_hp -= StatsController.currentWeapon.damage;
			
			
			if(_hp <= 0) {
			
				_hp = 0;
				Die();
			
			}
			else {
				Invoke("StopHurt", 0.1f);
				animator.SetBool("Hurt", true);
			}
		}
	
	}
	
	void StopHurt() {
	
		animator.SetBool("Hurt", false);
		
		if(_hp > 0 && movement.followAfterHurt) {
			currentState = State.Follow;
			vision.canFollowPlayer = true;
		}
	
	}
	
	public void Die() {
	
		Debug.Log("Dead!");
		currentState = State.Dead;
		//animator.enabled =false;
		
		CancelInvoke("StopHurt");
		CancelInvoke("SetNextPatrolWaypoint");
		
		
		animator.SetBool("Idle", false);
		animator.SetBool("Run", false);
		animator.SetBool("Walk", false);
		animator.SetBool("Attack", false);
		animator.SetBool("Hurt", false);
		animator.SetBool("Dead", false);
		animator.SetBool("Die", true);
		
		nma.speed = 0;
		
		//Invoke ("StopDie", 0.34f);
		
		//Invoke("KillObject", 0.18f);
		
		//if(this.animation["dead3"] != null && !this.animation.IsPlaying("dead3")) {
		//	animation.Play("dead3");
		Invoke("StopDie", 0.01f);
		//}
		
	
	}
	
	public void StopDie() {
		
		animator.SetBool("Dead", true);
		animator.SetBool("Die", false);
	
		//animator.SetBool("Die", false);
		
		//animator.SetBool("Dead", true);
		
		//currentState = State.Dead;
		Debug.Log("StopDie");
		nma.enabled = false;
		collider.enabled = false;
		//animation.Stop();
		//animator.enabled = false;
		enabled = false;
		//this.enabled = false;
		//animator.enabled = false;
		
	
	}
	
	public void KillObject() {
	
		Destroy(gameObject);
		
	
	}
	
}

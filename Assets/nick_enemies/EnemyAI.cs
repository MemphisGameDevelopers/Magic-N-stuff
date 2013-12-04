using UnityEngine;
using System.Collections;

[RequireComponent (typeof (SphereCollider))]
public class EnemyAI : MonoBehaviour {
	// Some basic entity variables
	private int health = 100;
	public int Health(){ return health; }
	public void Health(int hp){ health = hp; }
	private float moveSpeed = 0.05f;
	public float MoveSpeed(){ return moveSpeed; }
	public void MoveSpeed(float ms){ moveSpeed = ms; }
	private float attackRange = 2.5f;
	public float AttackRange(){ return attackRange; }
	public void AttackRange(float ar){ attackRange = ar; }


	// Environment/World sensory variables. Populate and expose them for later usage
	private bool playerInBounds; // Aggro Radius Detection
	public bool IsPlayerInBounds(){ return playerInBounds; }
	private float distanceFromPlayer; // Update can calc distance from the player for range logic
	public float DistanceFromPlayer() { return distanceFromPlayer; }
	private bool canSeePlayer; // Raycast to tell if player is in Line of Sight
	public bool CanSeePlayer() { return true; return canSeePlayer; }
	private bool amIAlive; // Don't attack if you're dead, dummy
	public bool IsAlive() { return amIAlive; }
	private bool amAttacking; // Already in an attack
	public bool IsAttacking() { return amAttacking; }
	private float coolDownTimer; // Timer between actions
	public float CoolDownTimer() { return coolDownTimer; }
	public void CoolDownTimer(float cdt){ CancelInvoke ("TimerTick"); coolDownTimer = cdt; Invoke ("TimerTick", 0.1f); }
	public void TimerTick(){ if(coolDownTimer <= 0f) return; coolDownTimer -= 0.1f; Invoke ("TimerTick", 0.1f); }
	// End sensory variables
	
	public GameObject player; // Get reference to player object on startup
	
	public void LookAtPlayer() {
		transform.LookAt ( new Vector3( player.transform.position.x, 0f, player.transform.position.z) );
	}
	public enum StrategyType {skeleton, spider, golem, demon, humanoid};
	public StrategyType _strategyType;
	private EnemyStrategy _strategy;
	
	// Use this for initialization
	void Start () {
		player = GameObject.FindGameObjectWithTag("Player");
		
		// Initialize the enemy type
		switch(_strategyType){
			case StrategyType.skeleton:
				_strategy = new SkeletonStrategy();
				_strategy._setParent(this);
				break;
			case StrategyType.spider:
				_strategy = new SpiderStrategy();
				_strategy._setParent(this);
				break;
			
			case StrategyType.demon:
			
			
			case StrategyType.golem:
			
			default:
				_strategy = new SkeletonStrategy();
				_strategy._setParent(this);
			break;
		}
		
		// Initialize the spherical aggro radius collider/trigger
		((SphereCollider)gameObject.collider).radius = 8;
		((SphereCollider)gameObject.collider).isTrigger = true;
		((SphereCollider)gameObject.collider).center = new Vector3(0f,1.68f,0f);

		
	}
	
	// Update is called once per frame
	void Update () {
	
		_strategy.preupdate();
		_strategy.update ();
		_strategy.postupdate();
	}
	
	void LateUpdate() {
		if(playerInBounds){
			distanceFromPlayer = Vector3.Distance (transform.position, player.transform.position);
		} else {
			distanceFromPlayer = Mathf.Infinity;
		}
	}
	
	void OnTriggerStay(Collider Other){
		// Only if pib isn't already set do we need to check these
		if(!playerInBounds && Other.gameObject.tag == "Player"){
			playerInBounds = true;
		}
	}
	
	void OnTriggerExit(Collider Other){
		if(Other.gameObject.tag == "Player"){
			playerInBounds = false;
		}
	}
}



abstract public class EnemyStrategy {

	protected EnemyAI _parent;
	public void _setParent(EnemyAI caller){
		_parent = caller;
		startup();
	}

	// Some basic state stuff
	public abstract void startup();	// Wakeup and initialization
	public abstract void preupdate(); // Allow for some calculations ahead of time to short-circuit update calls
	public abstract void update(); // Proper update call
	public abstract void postupdate(); // Allow for some calculations post-update to determine if we need to exit or what
	public abstract void exit(); // Ending, deinit, cleanup
	
	// Some more specific actions
	public abstract void spawn(); // Not necessarily "entry" for every enemy, this will be for an enemy's "creation"
	public abstract void attack(); // Pick an attack
	public abstract void isHit();
	public abstract void blocksHit();
	public abstract void dodgeHit();
	public abstract void premove(); // For any calculations to be done ahead of move, 
									// which might be used in a LERP style so should remain light
	public abstract void move(); // Figure out position updates for a given instant
	public abstract void postmove(); // Any calcs need to be done after move attempt? Did we hit something?
	public abstract void die(); // Out of the healths, the enemy is defeated and needs to show its death throes
	
}


public class SkeletonStrategy : EnemyStrategy {
	
	GameObject skeletonModel;

	// --------------------------------------------------
	// HOUSEKEEPING
	public override void startup(){
		// Set up health, skeletons are wimpy
		_parent.Health(20);
		_parent.AttackRange (2.5f);
		
		// Grab reference to model
		skeletonModel = _parent.transform.FindChild ("skeleton").gameObject;
	}
	public override void exit(){
		
	}
	
	public override void spawn(){
		
	}
	// HOUSEKEEPING
	// --------------------------------------------------
	
	// --------------------------------------------------
	// UPDATE EVENTS
	public override void preupdate(){
		
	}
	public override void update(){
		if(_parent.CoolDownTimer () <= 0f && _parent.IsPlayerInBounds() && _parent.CanSeePlayer()){
			_parent.LookAtPlayer();
		
			// If in attack range, can see player, and not attacking, Attack
			if(_parent.DistanceFromPlayer() <= _parent.AttackRange ()){
				attack();
			} else {
				// If not in attack range and can see player, move toward player
				premove ();
				move ();
				postmove ();
			}
		}
	}
	public override void postupdate(){
		
	}
	// UPDATE EVENTS
	// --------------------------------------------------

	// --------------------------------------------------	
	// COMBAT EVENTS
	public override void attack(){
		if(_parent.CoolDownTimer() > 0f) return;
		
		Debug.Log ("Skeleton attacking");
		// Play attack animation
		skeletonModel.animation.Stop ();
		skeletonModel.animation.PlayQueued("attack", QueueMode.PlayNow);
		skeletonModel.animation.PlayQueued("waitingforbattle", QueueMode.CompleteOthers);
		// Play attack sound
		skeletonModel.audio.PlayOneShot ( skeletonModel.audio.clip );
		// Set cooldowntimer
		_parent.CoolDownTimer(5.0f);
	}
	public override void isHit(){
		// Play hit animation and sound
		// Decrement health
	}
	public override void blocksHit(){
		// Play block animation and sound
		// Fire off block particle effect
	}
	public override void dodgeHit(){
		// Play dodge animation and sound
	}	
	public override void die(){
		// Play death animation and sound
		skeletonModel.animation.PlayQueued ("die", QueueMode.PlayNow);
		// Queue exit()
	}
	// COMBAT EVENTS
	// --------------------------------------------------	

	// --------------------------------------------------	
	// MOVEMENT EVENTS
	public override void premove(){
	}
	public override void move(){
		skeletonModel.animation.Play("run");
		_parent.transform.Translate ( new Vector3(0f, 0f, _parent.MoveSpeed()) );
	}
	public override void postmove(){
		
	}
	// MOVEMENT EVENTS
	// --------------------------------------------------	
	

}

public class HumanoidStrategy : EnemyStrategy {
	
	GameObject humanoidModel;
	
	// --------------------------------------------------
	// HOUSEKEEPING
	public override void startup(){
		// Set up health, skeletons are wimpy
		_parent.Health(80);
		_parent.AttackRange (0f);
		
		// Grab reference to model
		humanoidModel = _parent.transform.FindChild ("adventurer").gameObject;
	}
	public override void exit(){
		
	}
	
	public override void spawn(){
		
	}
	// HOUSEKEEPING
	// --------------------------------------------------
	
	// --------------------------------------------------
	// UPDATE EVENTS
	public override void preupdate(){
		
	}
	public override void update(){
		if(_parent.CoolDownTimer () <= 0f && _parent.IsPlayerInBounds() && _parent.CanSeePlayer()){
			_parent.LookAtPlayer();
			
			// If in attack range, can see player, and not attacking, Attack
			if(_parent.DistanceFromPlayer() <= _parent.AttackRange ()){
				attack();
			} else {
				// If not in attack range and can see player, move toward player
				premove ();
				move ();
				postmove ();
			}
		}
	}
	public override void postupdate(){
		
	}
	// UPDATE EVENTS
	// --------------------------------------------------
	
	// --------------------------------------------------	
	// COMBAT EVENTS
	public override void attack(){
		if(_parent.CoolDownTimer() > 0f) return;
		
		// Play attack animation
		// Play attack sound
		// Set cooldowntimer
	}
	public override void isHit(){
		// Play hit animation and sound
		// Decrement health
	}
	public override void blocksHit(){
		// Play block animation and sound
		// Fire off block particle effect
	}
	public override void dodgeHit(){
		// Play dodge animation and sound
	}	
	public override void die(){
		// Play death animation and sound
		// Queue exit()
	}
	// COMBAT EVENTS
	// --------------------------------------------------	
	
	// --------------------------------------------------	
	// MOVEMENT EVENTS
	public override void premove(){
	}
	public override void move(){
	}
	public override void postmove(){
		
	}
	// MOVEMENT EVENTS
	// --------------------------------------------------	
	
	
}

public class SpiderStrategy : EnemyStrategy {
	
	GameObject spiderModel;
	AudioSource[] audios;
	
	// --------------------------------------------------
	// HOUSEKEEPING
	public override void startup(){
		// Set up health, skeletons are wimpy
		_parent.Health(80);
		_parent.AttackRange (2f);
		
		// Grab reference to model
		spiderModel = _parent.transform.FindChild ("spider").gameObject;
		audios = spiderModel.GetComponents<AudioSource>();
	}
	public override void exit(){
		
	}
	
	public override void spawn(){
		
	}
	// HOUSEKEEPING
	// --------------------------------------------------
	
	// --------------------------------------------------
	// UPDATE EVENTS
	public override void preupdate(){
		
	}
	public override void update(){
		if(_parent.CoolDownTimer () <= 0f && _parent.IsPlayerInBounds() && _parent.CanSeePlayer()){
			_parent.LookAtPlayer();
			
			// If in attack range, can see player, and not attacking, Attack
			if(_parent.DistanceFromPlayer() <= _parent.AttackRange ()){
				attack();
			} else {
				// If not in attack range and can see player, move toward player
				premove ();
				move ();
				postmove ();
			}
		}
	}
	public override void postupdate(){
		
	}
	// UPDATE EVENTS
	// --------------------------------------------------
	
	// --------------------------------------------------	
	// COMBAT EVENTS
	public override void attack(){
		if(_parent.CoolDownTimer() > 0f) return;
		
		// Determine which attack to do
		string attackName = "";
		AudioSource attackSound;
		int rnd = Random.Range (1,3);
		if(rnd == 1){
			attackName = "attack1";
			attackSound = audios[0];
		} else {
			attackName = "attack2";
			attackSound = audios[1];
		}
		
		
		
		Debug.Log ("Spider attacking, ");
		// Play attack animation
		spiderModel.animation.Stop ();
		spiderModel.animation.PlayQueued(attackName, QueueMode.PlayNow);
		spiderModel.animation.PlayQueued("idle", QueueMode.CompleteOthers);
		// Play attack sound
		attackSound.Play();
		// Set cooldowntimer
		_parent.CoolDownTimer(5.0f);
	}
	public override void isHit(){
		// Play hit animation and sound
		// Decrement health
	}
	public override void blocksHit(){
		// Play block animation and sound
		// Fire off block particle effect
	}
	public override void dodgeHit(){
		// Play dodge animation and sound
	}	
	public override void die(){
		// Play death animation and sound
		// Queue exit()
	}
	// COMBAT EVENTS
	// --------------------------------------------------	
	
	// --------------------------------------------------	
	// MOVEMENT EVENTS
	public override void premove(){
	}
	public override void move(){
		spiderModel.animation.Play("walk");
		_parent.transform.Translate ( new Vector3(0f, 0f, _parent.MoveSpeed()) );
	}
	public override void postmove(){
		
	}
	// MOVEMENT EVENTS
	// --------------------------------------------------	
	
	
}

public class DemonStrategy : EnemyStrategy {
	
	GameObject demonModel;
	
	// --------------------------------------------------
	// HOUSEKEEPING
	public override void startup(){
		// Set up health, skeletons are wimpy
		_parent.Health(80);
		_parent.AttackRange (0f);
		
		// Grab reference to model
		demonModel = _parent.transform.FindChild ("balrog").gameObject;
	}
	public override void exit(){
		
	}
	
	public override void spawn(){
		
	}
	// HOUSEKEEPING
	// --------------------------------------------------
	
	// --------------------------------------------------
	// UPDATE EVENTS
	public override void preupdate(){
		
	}
	public override void update(){
		if(_parent.CoolDownTimer () <= 0f && _parent.IsPlayerInBounds() && _parent.CanSeePlayer()){
			_parent.LookAtPlayer();
			
			// If in attack range, can see player, and not attacking, Attack
			if(_parent.DistanceFromPlayer() <= _parent.AttackRange ()){
				attack();
			} else {
				// If not in attack range and can see player, move toward player
				premove ();
				move ();
				postmove ();
			}
		}
	}
	public override void postupdate(){
		
	}
	// UPDATE EVENTS
	// --------------------------------------------------
	
	// --------------------------------------------------	
	// COMBAT EVENTS
	public override void attack(){
		if(_parent.CoolDownTimer() > 0f) return;
		
		// Play attack animation
		// Play attack sound
		// Set cooldowntimer
	}
	public override void isHit(){
		// Play hit animation and sound
		// Decrement health
	}
	public override void blocksHit(){
		// Play block animation and sound
		// Fire off block particle effect
	}
	public override void dodgeHit(){
		// Play dodge animation and sound
	}	
	public override void die(){
		// Play death animation and sound
		// Queue exit()
	}
	// COMBAT EVENTS
	// --------------------------------------------------	
	
	// --------------------------------------------------	
	// MOVEMENT EVENTS
	public override void premove(){
	}
	public override void move(){
	}
	public override void postmove(){
		
	}
	// MOVEMENT EVENTS
	// --------------------------------------------------	
	
	
}

public class GolemStrategy : EnemyStrategy {
	
	GameObject golemModel;
	
	// --------------------------------------------------
	// HOUSEKEEPING
	public override void startup(){
		// Set up health, skeletons are wimpy
		_parent.Health(80);
		_parent.AttackRange (0f);
		
		// Grab reference to model
		golemModel = _parent.transform.FindChild ("golem").gameObject;
	}
	public override void exit(){
		
	}
	
	public override void spawn(){
		
	}
	// HOUSEKEEPING
	// --------------------------------------------------
	
	// --------------------------------------------------
	// UPDATE EVENTS
	public override void preupdate(){
		
	}
	public override void update(){
		if(_parent.CoolDownTimer () <= 0f && _parent.IsPlayerInBounds() && _parent.CanSeePlayer()){
			_parent.LookAtPlayer();
			
			// If in attack range, can see player, and not attacking, Attack
			if(_parent.DistanceFromPlayer() <= _parent.AttackRange ()){
				attack();
			} else {
				// If not in attack range and can see player, move toward player
				premove ();
				move ();
				postmove ();
			}
		}
	}
	public override void postupdate(){
		
	}
	// UPDATE EVENTS
	// --------------------------------------------------
	
	// --------------------------------------------------	
	// COMBAT EVENTS
	public override void attack(){
		if(_parent.CoolDownTimer() > 0f) return;
		
		// Play attack animation
		// Play attack sound
		// Set cooldowntimer
	}
	public override void isHit(){
		// Play hit animation and sound
		// Decrement health
	}
	public override void blocksHit(){
		// Play block animation and sound
		// Fire off block particle effect
	}
	public override void dodgeHit(){
		// Play dodge animation and sound
	}	
	public override void die(){
		// Play death animation and sound
		// Queue exit()
	}
	// COMBAT EVENTS
	// --------------------------------------------------	
	
	// --------------------------------------------------	
	// MOVEMENT EVENTS
	public override void premove(){
	}
	public override void move(){
	}
	public override void postmove(){
		
	}
	// MOVEMENT EVENTS
	// --------------------------------------------------	
	
	
}
using UnityEngine;
using System.Collections;

public class Tunneling : MonoBehaviour {
	
	//The player has to satisfy two conditions in order to tunnel, being the right size and being next to a wall
	private bool isSize;//indicates that the player is the right size to tunnel
	private bool inPosition; //indicates that the player is next to a wall, being in position to tunnel
	
	private GameObject collidingWall; //The 'Wall' gameobject the player is touching. Equals null when the player is not touching a wall 
	private float tunnelSpeed = 5f;//Speed at which the player tunnels through the wall, > speed -> faster transition
	private float shiftPosition = 1f;//Total amount the player is shifted from starting point to end point through tunneling
	
	private float reqTunnelingSize = 0.28f; //Transform scale the player needs to be inorder to tunnel

	
	// Use this for initialization
	void Start () {
		
		//Initialize isSize and inPosition
		if(transform.localScale.x >= reqTunnelingSize) {
			isSize = true;
		} else {
			isSize = false;
		}
		
		inPosition = false;
		
	}
	
	// Update is called once per frame
	void Update () {
		
		//Constantly update isSize based on their uncertainity
		if(transform.localScale.x >= reqTunnelingSize) {
			isSize = true;
		} else {
			isSize = false;
		}
		
		if (Input.GetButtonDown("Uncertainty") ) {

			if(isSize && inPosition && (Input.GetAxis ("Uncertainty") < 0) ) {	//If they decrease their uncertainity whilst having the correct size and being in position
				Tunnel();	//Tunnel the player across the wall
			}
			
		}
		
		
		
	}
	
	//As the player stays in a collision state
	void OnCollisionStay2D(Collision2D col) {

		if(col.gameObject.name == "Wall" || col.gameObject.name == "Barrier(Clone)") {	//If they hold a collision with a wall
			inPosition = true;	//Flag that they are in position
			collidingWall = col.gameObject;	//Store the Wall gameobject the player is touching 
			
		}
		
		
	}
	
	//As the player leaves a collision state
	void OnCollisionExit2D(Collision2D col) {
		
		if(col.gameObject.name == "Wall" || col.gameObject.name == "Barrier(Clone)") {	//If they leave a collision with a wall
			inPosition = false; //Flag that they are no longer in position
			collidingWall = null; //Clear the collidingWall variable
			
		}
		
	}
	
	//Tunneling process: Transports the player across the wall
	private void Tunnel() {
		if(collidingWall) { //If they are touching a wall
			
			WallScript wallSc = collidingWall.GetComponent<WallScript>(); // Get the wall script of the wall its touching 
			bool isHorizontal = wallSc.IsHorizontal(); //check if it's horizontal
			Vector3 target = transform.position; //The position the player will go from tunneling
			
			if(!isHorizontal) {
				
				if(transform.position.x < collidingWall.transform.position.x) {
					//Debug.Log ("Tunnel Right");
					target.x = collidingWall.transform.position.x + shiftPosition; //Target position is an increased x value
					
				} else {
					//Debug.Log ("Tunnel Left");
					target.x = collidingWall.transform.position.x - shiftPosition; //Target position is an decreased x value
					
				}
				
			} else {
				
				if(transform.position.y < collidingWall.transform.position.y) {
					//Debug.Log ("Tunnel Up");
					target.y = collidingWall.transform.position.y + shiftPosition; //Target position is an increased y value
				} else {
					//Debug.Log ("Tunnel Down");
					target.y = collidingWall.transform.position.y - shiftPosition; //Target position is an decreased y value
				}
				
			}
			Energy energyScript = GetComponent<Energy>();
			if(energyScript != null) {
				energyScript.DecreaseEnergy(wallSc.energyConsumption);
				
			}
			StartCoroutine("TunnelExec", target); //Transport the player smoothly

		}
		
	}
	
	//Transports the player smoothly by lerping to a target position
	IEnumerator TunnelExec (Vector3 target) {

		
		Controller controller = GetComponent<Controller>();
		controller.DisableInput();
		while(Vector3.Distance(transform.position, target) > 0.17f ) {
			
			if(Vector3.Distance(transform.position, target) > 1.8f) {
				break;
			}
			
			transform.position = Vector3.Lerp (transform.position, target, tunnelSpeed*Time.deltaTime);
			
			yield return null;
			
			
			
		}
		controller.EnableInput ();

		//print ("Finished");
		
	}
	
}

using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour {
	
	public float smoothing = 4f; //Speed the camera adjusts itself 
	
	//private Camera camera;
	
  	Transform targetPlayer; 
	public float dampTime = 0.15f;
	private Vector3 velocity = Vector3.zero;
	
	/*
	//Properties of Target vector. Get and Set functions of the camera's follow target
	public Vector3 Target 
	{
		get{return target;}
		set {
			
			target = value; //New target vector is the value passed in
			target.z = -10;	//Camera always sits at -10 
			
			if(Vector3.Distance(transform.position, target) > 1f) {
				//StopCoroutine("Adjust"); //Stop current camera adjusting coroutine 
				StartCoroutine("Adjust", target); //Start new camera adjusting coroutine with new target vector
			}
			
		}
		
	}
	
	private Vector3 target;
	*/
	
	
	// Use this for initialization
	void Start () {
		UniversalHelperScript.Instance.OnPlayer += OnPlayerCreate;
		
	}
	
	//Called when the player is created
	void OnPlayerCreate(GameObject player) {
		Vector3 playerPos = player.transform.position;
		transform.position = new Vector3(playerPos.x, playerPos.y, -10);
		targetPlayer = player.transform; ///
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		
		if (targetPlayer) {
			Vector3 point = camera.WorldToViewportPoint(targetPlayer.position);
			Vector3 delta = targetPlayer.position - camera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, point.z)); 
			Vector3 destination = transform.position + delta;
			transform.position = Vector3.SmoothDamp(transform.position, destination, ref velocity, dampTime);
		}	
	}
	
	/*
	//Adjust function, adjusts camera gradually to target vector 
	IEnumerator Adjust (Vector3 target) {
		
		
		
		while(Vector3.Distance(transform.position, target) > 0.1f) {	//If target has been reached
			
			transform.position = Vector3.Lerp (transform.position, target, smoothing*Time.smoothDeltaTime);
			//Lerps the current transform.position, to the new target vector
			
			yield return null; //Yeilds execution of the function and return a null IEnumerator object. The coroutine
			//resumes after the next update
			
		}
		
		
		
	}
	*/
	
}

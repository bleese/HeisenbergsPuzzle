using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour {
	
	public float smoothing = 4f; //Speed the camera adjusts itself 
	
	private Camera camera;
	
	//Properties of Target vector. Get and Set functions of the camera's follow target
	public Vector3 Target 
	{
		get{return target;}
		set {
			
			target = value; //New target vector is the value passed in
			target.z = -10;	//Camera always sits at -10 
			
			StopCoroutine("Adjust"); //Stop current camera adjusting coroutine 
			StartCoroutine("Adjust", target); //Start new camera adjusting coroutine with new target vector
			
		}
		
	}
	
	private Vector3 target;
	
	
	void Awake () {
		
		//Vector3 startTarget = GameObject.Find ("player").transform.position; //Initialize camera's starting position
		//this.transform.position = new Vector3(startTarget.x, startTarget.y, -10);
	
	}
	
	// Use this for initialization
	void Start () {
		UniversalHelperScript.Instance.OnPlayer += OnPlayerCreate;
		//Vector3 startTarget = GameObject.FindGameObjectWithTag ("Player").transform.position; //Initialize camera's starting position
		//this.transform.position = new Vector3(startTarget.x, startTarget.y, -10);
		//camera = GetComponent<Camera>(); //Will get back to later (Famous last words);
	}
	
	void OnPlayerCreate(GameObject player) {
		transform.position = player.transform.position;
	}
	
	// Update is called once per frame
	void Update () {
				
	}
	
	//Adjust function, adjusts camera gradually to target vector 
	IEnumerator Adjust (Vector3 target) {
		
		while(Vector3.Distance(transform.position, target) > 0.05f) {	//If target has been reached
			
			transform.position = Vector3.Lerp (transform.position, target, smoothing*Time.deltaTime);
			//Lerps the current transform.position, to the new target vector
			
			yield return null; //Yeilds execution of the function and return a null IEnumerator object. The coroutine
			//resumes after the next update
			
		}
		
	}
	
}

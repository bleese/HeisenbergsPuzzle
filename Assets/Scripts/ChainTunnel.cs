using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ChainTunnel : MonoBehaviour {
	
	private bool chaining; //If the player is setting up a chain
	private bool chainLineUpdate; //If the chain path line should be drawn
	private bool chainMoving;	//If the player is moving along the chain path
	
	private List<Vector2> chain;	//The chain the player is setting

	private Vector2 direction;	//Current direction the chain line is facing
	private Vector2 lastDirection;	//Last direction the chain line is facing
	
	private int chainStage;	//The number of chains set
	private int vertexCount;	//Number of vertices in the chain
	
	private const int LINE_LENGTH = 20;	//Length of a chain line segment
	private const float CHAIN_TUNNEL_SPEED = 16f;	//Speed the player moves in a chain 
	
	LineRenderer lineRender;	
	private Controller controller;
	
	// Use this for initialization
	void Start () {
		EnableInput();
		//Starting bool values
		chaining = false;
		chainLineUpdate = false;
		chainMoving = false;
		
		controller = GetComponent<Controller>();
		
		direction = new Vector2(0,0);
		lastDirection = new Vector2(0,0);
		
		lineRender = transform.Find ("LineDrawer").GetComponent<LineRenderer>();
		
		chainStage = 1;
		vertexCount = 2;
		
	}
	
	// Update is called once per frame
	void Update () {
		
		if(chaining && chainLineUpdate) {	//If the player is laying a chain and if the line should be displayed
			
			Vector2 temp = direction + lastDirection; //placing the current line segment at the last position of the previous line segment
			
			lineRender.SetPosition(chainStage, new Vector3(LINE_LENGTH*temp.x, LINE_LENGTH*temp.y)); //Drawing the line
			
		}
		
		
	}
	
	void OnInputEvent(Vector2 rawValue, ActionType action) {
		if (action == ActionType.Chain) {
				
				if(chainMoving) {
					;	//If the player has set the chain moving process, ignore input
					//Prevents chain cancelling
				} else if(!chaining) {
					ChainStart (); //Start the chain process
				} else {
					if(chainStage < 3) { //3 vertices max in a chain
						ChainProcess();	//Continue laying the chain
					} else {
						ChainFinish();	//Start the chain moving process
						
					}
				}
		} 
		
		if (action == ActionType.ChainCan) { //Cancel a chain
			if(!chainMoving) { //Can't cancel the chain if the player has started moving along the chain
				ChainCancel();
			}
		}
		
		if(action == ActionType.Move) { //Directional Input
			
			if(chaining) { //Only needed when the player is setting directions for a chain line
				DirectionAdust(rawValue);
			}
			
		}
	}
	
	//Start chain ability 
	private void ChainStart() {
		chaining = true; 
		controller.DisableInput(); //Disables the ability for the player to move
		GetComponent<Rigidbody2D>().velocity = new Vector2(0,0); //Cancels any momentum 
		chain = new List<Vector2>();
		chainLineUpdate = true;
		
		//GameObject.Find("Main Camera").GetComponent<Camera>().orthographicSize = 7f;
		
	
	}
	
	//Continue Chain setting ability
	//Called when a player sets a position in a chain 
	private void ChainProcess() {
		
		//Reject chain setting if the player hasn't specified a direction
		if(direction.x == 0 && direction.y == 0) {
			return;
		}
		
		lastDirection = direction + lastDirection; 
		chain.Add (transform.TransformPoint(new Vector2(lastDirection.x*LINE_LENGTH,lastDirection.y*LINE_LENGTH)));
		//Set the position of the line segment
		//transform.TransformPoint -- Converts local to world coordinates. Direction vector is set in local coordinate system
		
		chainStage++; //Increase chain stage
		vertexCount++; //Increase vector count in the line being drawn
		lineRender.SetVertexCount(vertexCount);
	}
	
	//Finish the chain, set the player in motion along the chain
	private void ChainFinish() {
		chainLineUpdate = false;
		lastDirection = direction + lastDirection; 
		chain.Add (transform.TransformPoint(new Vector2(lastDirection.x*LINE_LENGTH,lastDirection.y*LINE_LENGTH)));

		StartCoroutine("ChainExec"); //Start chain movement
	}
	
	//Reads and Sets the direction vector the player is inputting  
	private void DirectionAdust(Vector2 rawValue) {
			
		direction = new Vector2(rawValue.x,rawValue.y);
		direction.Normalize();
	}
	
	//Cancel and/or Clear the chaining process
	//Only effective if the player is not moving along the chain. Cancels the chain laying process 
	private void ChainCancel() {
		
		chaining = false; 
		chainLineUpdate = false;
		controller.EnableInput();
		chain = null;
		direction = new Vector2(0,0);
		lastDirection = new Vector2(0,0);
		chainStage = 1;
		vertexCount = 2;
		
		ChainLineReset();
		
		//GameObject.Find("Main Camera").GetComponent<Camera>().orthographicSize = 4.6f;
		
	}
	
	//Resets the line renderer 
	private void ChainLineReset() {
		
		lineRender.SetVertexCount(2);
		lineRender.SetPosition(1, new Vector3(0, 0));
		
	}
	
	public void EnableInput() {
		InputSystem.Instance.OnInputPlayer += OnInputEvent;
	}
	
	public void DisableInput() {
		InputSystem.Instance.OnInputPlayer -= OnInputEvent;
	}
	
	//Chain moving process
	//Moves the player along the chain set by the player
	IEnumerator ChainExec () {
		/*
		foreach (Vector2 temp in chain) {
			Debug.Log (temp);
		}
		*/
		
		ChainLineReset(); //Clear chain drawing
		
		chainMoving = true; //Player is moving along the chain

		DisableInput(); 
		GetComponent<CircleCollider2D>().enabled = false;
		
		foreach(Vector2 chainTarget in chain) {	//for each point to move to
			
			Vector3 target = new Vector3(chainTarget.x, chainTarget.y);
			
			while(Vector3.Distance(transform.position, target) > 0.05f) { //Lerp to the target point
			
				transform.position = Vector3.Lerp (transform.position, target, CHAIN_TUNNEL_SPEED*Time.deltaTime);
			
				yield return null;
				
			}
			
		}
		
		GetComponent<CircleCollider2D>().enabled = true;
		EnableInput ();
		chainMoving = false; //Chain moving has stopped 
		ChainCancel(); //Clear the chain settings 
		
	}
}

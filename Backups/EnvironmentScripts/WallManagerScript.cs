using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class WallManagerScript : MonoBehaviour {
    public GameObject wall; // Prefab for the barrier
    private GameObject tempWall; // Temporary reference to a wall used in several locations
	private GameObject selectedWall = null; // The wall we currently have selected
	private WallScript selectedScript = null; // The script of the wall we have currently selected
	private List<IEnvironmentObject> walls = new List<GameObject>(); // The list of walls currently on the map
	private Vector3 wallPosition; // The position we want to place walls
	private float cameraZDistance = 10f; // Constant z distance the camera is from the X,Y plane
	private bool create;
	
	// INSTANTIATES wallManager as a singleton
	private static WallManagerScript instance;
	
	public static WallManagerScript Instance
	{
		
		get 
		{
			if(instance == null) {
				instance = GameObject.FindObjectOfType(typeof(WallManagerScript)) as WallManagerScript; //Instantiate class if instance == null
				
			}
			return instance;
			
		}
	}
	// Use this for initialization
	void Start () {
	}
	
	// Once this is called, will select the appropriate event to call
	void OnInputEvent(Vector2 rawValue, ActionType action) {
		if (action == ActionType.Create && create == true) {
			createWall ();
		} else if (selectedWall != null) {
			if (action == ActionType.Flip) {
				FlipWall();
			} else if (action == ActionType.Destroy) {
				DestroyWall();
			} else if (action == ActionType.Resize) {
				ResizeWall(rawValue.x); // Note the y part of this value is arbitary (always 0);
			} else if (action == ActionType.Snap) {
				SnapWall();
		//} else if (action == ActionType.SelectWall) {
		//	selectWall ();
			}
		}
	}
	
	// When we want to flip our selectedWall
	void FlipWall() {
	   selectedScript.changeDirection ();
	}

	// When we want to delete our selected wall	
	void DestroyWall() {
	   Destroy (selectedWall);
	   selectedScript = null;
	   selectedWall = null;
	}
	
	// When we want to resize our selectedwall
	void ResizeWall(float resize) {
	   selectedScript.resizeWall (resize);
	}
	
	// When we want to snap our selected wall to a 1 x 1 grid
	void SnapWall() {
	   selectedScript.snapWall ();
	}
	
	// We create a wall in the space where the mouse is currently at
	// We then add the wall to our list of walls
	public void createWall() {
		wallPosition = Camera.main.ScreenToWorldPoint (new Vector3(Input.mousePosition.x,Input.mousePosition.y, cameraZDistance));
		tempWall = Instantiate (wall,wallPosition, Quaternion.identity) as GameObject;
		walls.Add (tempWall);		
	}
	
	// Update is called once per frame
	void Update () {

	}
	
	

	
	// Makes a given wall a selected wall
	public void selectWall(GameObject wall) {
		selectedWall = wall;
		selectedScript = wall.GetComponent<WallScript>();
	}
	
	// Makes the selected wall null
	public void deselectWall() {
		selectedWall = null;
		selectedScript = null;
	}
	
	
	// Sets whether or not we should allow walls to be movable
	public void SetEditor(bool edit) {
		WallScript tempScript = null;
		for (int counter = 0; counter < walls.Count; counter++) {
			tempScript = walls[counter].GetComponent<WallScript>();
			tempScript.SetEditor (edit);
		}
	}
	
	// Sets whether or not we should create walls when pressing space
	public void SetCreate(bool createWall) {
		create = createWall;
	}
	
	// Subscribes the WallManager to the OnInputEditor events
	public void EnableInput() {
		InputSystem.Instance.OnInputEditor += OnInputEvent;
	}
	
	// Unsubscribes the WallManager to the OnInputEditor events
	public void DisableInput() {
		InputSystem.Instance.OnInputEditor -= OnInputEvent;
	}
}
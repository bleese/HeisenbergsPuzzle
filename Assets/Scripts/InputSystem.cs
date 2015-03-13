using UnityEngine;
using System.Collections;

//Types of actions the player can achieve
public enum ActionType {
	Move = 0,
	Uncertainty = 1,
	Pause = 2,
	Flip = 3,
	Create = 4,
	Resize = 5,
	ResizeDirection = 6,
	Snap = 7,
	Destroy = 8,
	Select = 9,
	ChooseWall = 10,
	ChooseEField = 11,
	ChooseMField = 12,
	DisableEntity = 13,
	ChooseMeasurer = 14,
	ChooseAntiMatter = 15,
	ChooseTeleporter = 16,
	ChooseSpawn = 17,
	ChooseAltSpawn = 18,
	Chain = 19,
	ChainCan = 20,
	ChooseGate = 21,
	ChooseTriggerPoint = 22
}

/*Scans for input from keyboard.
	-If input is recieved it notifies game objects that are subscribed to the input event
*/ 
public class InputSystem : MonoBehaviour {
	
	private static InputSystem instance; //Instance of InputSystem gameobject. InputSystem acts as a singleton class, being independent of other game objects 
	
	public delegate void OnInputEvent(Vector2 rawValue, ActionType action); //reference to the On Input method  
	public event OnInputEvent OnInputPlayer; //Input event which other classes subscribe to, uses OnInputEvent delegate 
	public event OnInputEvent OnInputEditor; //Input event for Manager classes to subscribe to
	
	private InputSystem() {}
	
	public static InputSystem Instance
	{
		
		get 
		{
			if(instance == null) {
				instance = GameObject.FindObjectOfType(typeof(InputSystem)) as InputSystem; //Instantiate class if instance == null
				
			}
			return instance;
			
		}
	}
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		Vector2 rawVal = Vector2.zero;
		if (OnInputPlayer != null) {
			// Always check for velocity input, if no input is made then it'll just be a 0 vector
			rawVal = new Vector2(Input.GetAxis ("Horizontal"), Input.GetAxis("Vertical"));
			OnInputPlayer(rawVal,ActionType.Move);	
			if(Input.GetButtonDown ("Cancel")) {
					OnInputPlayer(new Vector2(0,0),ActionType.Pause);
			
			}
		
			//Check if jump input was recived
			if(Input.GetAxis("Uncertainty") != 0) { // USE for gradual
			//if (Input.GetButtonDown ("Uncertainty")) { // USE for insta-snap
			    // We're only passing one value in so the y component of our vector is always zero
				rawVal = new Vector2 (Input.GetAxis("Uncertainty"), 0);
				OnInputPlayer(rawVal, ActionType.Uncertainty);
			}
		
			// Shift is a special key that does nothing BY ITSELF, but will change certain keys when pressed, thus this behaved differently than other keys
			// Instead of calling another function, this simply enables a boolean in the universalHelper that other objects can check
			if (Input.GetKeyDown (KeyCode.LeftShift)) {
				UniversalHelperScript.Instance.shiftEnabled = true;
			}
		
			// When shift is lifted up the shiftEnabled key gets disabled
			if (Input.GetKeyUp(KeyCode.LeftShift)) {
				UniversalHelperScript.Instance.shiftEnabled = false;
			}
			
			if (Input.GetKeyDown(KeyCode.M)) {
				OnInputPlayer(rawVal,ActionType.Chain);
			}
			
			if (Input.GetKeyDown (KeyCode.N)) {
				OnInputPlayer(rawVal,ActionType.ChainCan);
			}
		}
		// Make sure we have something subscribed to this event...
		if (OnInputEditor != null) {
			// Creating a wall at a location held by the mouse
			// Note rawVal is never used in the following function, unless otherwise specified
			// Just passing it in because apparently "null" is not a valid vector
			if (Input.GetKeyDown (KeyCode.Space)) {
		   		OnInputEditor(rawVal, ActionType.Create);
			}
		
			// Pausing the game
			if (Input.GetButtonDown("Cancel")) {
				OnInputEditor(rawVal,ActionType.Pause);
			}
		
			// Destroying a wall that is selected
			if (Input.GetKeyDown (KeyCode.Delete)) {
				OnInputEditor(rawVal, ActionType.Destroy);
			}
			
			
			// Selecting a wall based on mouse position
			if (Input.GetKeyDown (KeyCode.Mouse0)) {
				OnInputEditor(rawVal, ActionType.Select);
			}
			
			
			// Snapping a wall to a specific location
			if (Input.GetButtonDown ("Snap")) {
				OnInputEditor(rawVal, ActionType.Snap);		   
			}
			
			// Selecting to create Walls
			if (Input.GetKeyDown(KeyCode.Alpha1)) {
				OnInputEditor(rawVal,ActionType.ChooseWall);
			}
			
			// Selecting to create Electric Fields
			if (Input.GetKeyDown(KeyCode.Alpha2)) {
				OnInputEditor(rawVal,ActionType.ChooseEField);
			}
			
			// Selecting to create Magnetic Fields
			if (Input.GetKeyDown(KeyCode.Alpha3)) {
				OnInputEditor(rawVal,ActionType.ChooseMField);
			}
			
			if (Input.GetKeyDown (KeyCode.Alpha4)) {
				OnInputEditor(rawVal,ActionType.ChooseMeasurer);
			}
			
			// Selecting to create Antimatter
			if (Input.GetKeyDown (KeyCode.Alpha5)) {
				OnInputEditor(rawVal,ActionType.ChooseAntiMatter);
			}
			
			// Selecting to create a teleporter
			if (Input.GetKeyDown (KeyCode.Alpha6)) {
				OnInputEditor(rawVal,ActionType.ChooseTeleporter);
			}
			
			// Creates alternate spawn point (for enemies possibly)
			if (Input.GetKeyDown (KeyCode.Alpha7)) {
				OnInputEditor(rawVal,ActionType.ChooseAltSpawn);
			}
			
			// Creates gate for antimatter breaking
			if (Input.GetKeyDown (KeyCode.Alpha8)) {
				Debug.Log ("Alpha8");
				OnInputEditor(rawVal,ActionType.ChooseGate);
			}
			
			//Selecting to create a trigger point
			if (Input.GetKeyDown (KeyCode.Alpha9)) {
				OnInputEditor(rawVal, ActionType.ChooseTriggerPoint);
			}
			
			// Moves the player spawn point to mouse position
			if (Input.GetKeyDown (KeyCode.Alpha0)) {
				OnInputEditor(rawVal,ActionType.ChooseSpawn);
			}
			
			// Flipping a wall that is selected
			if (Input.GetButtonDown ("Flip")) {
				OnInputEditor(rawVal, ActionType.Flip);		   
			}
			
			
			// Changing what AXIS we want to resize with
			if (Input.GetKeyDown (KeyCode.Mouse2)) {
				Debug.Log (true);
				OnInputEditor(rawVal, ActionType.ResizeDirection);
			}
			
			// Disables entity movement, such as whether or not an electric field will push the player
			if (Input.GetKeyDown (KeyCode.Tab)) {
				OnInputEditor(rawVal,ActionType.DisableEntity);
			}
			
			// Resizing a wall (rawVal is used in this function)
			if (Input.GetAxis ("Mouse ScrollWheel") != 0) {
				rawVal = new Vector2(Input.GetAxis ("Mouse ScrollWheel"), 0); // We only need one value, so the y part is always zero
				OnInputEditor(rawVal, ActionType.Resize);
			}
			
		}
	}
}

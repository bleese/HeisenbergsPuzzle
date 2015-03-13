using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

// NOTE: When adding a new IEnvironmentObject to the Editor manager, simply follow these steps
//   1. Add the new environment Objects prefab to the class variables
//   2. Add the ActionType.Choose<YOUR OBJECT> in the Input Systems. As well as a special button for selecting it
//   3. Add a case in the OnInputEvent for your new ActionType, set a new value for the create variable
//   4. Add a case in the "Create function" to instantiate your new object
//   5. Add a case in the "getValidComponent" function for your new object, based on tag

/*Trigger Points are abstracted to provide triggers for more than a single event whenever player move over them
To save the set of events a trigger point contains, when saving it in the toggleState field, enum Flags are used 
Bitwise calculations can be used to determine what flags were set
e.g.
If a trigger triggers end of level, then its flag value is -> 0001
If a trigger triggers Generic1 & Generic2, then its flag value is  0010 | 0100 => 0110
	-To test if the enum has these flag values set
		
		TriggerFlags trigger = TriggerFlags.Generic1 | TriggerFlags.Generic2; //0110
		if((trigger & TriggerFlags.Generic1 & TriggerFlags.Generic2) != 0) {
			blah...
		}
		// Above if statement:  if(0110 & 0010 & 0100) => Bitwise calculation returns 0110 
*/
[Flags]
public enum TriggerFlags {
	
	LevelEnd = 1,
	Generic1 = 2,
	Generic2 = 4,
	Generic3 = 8
	
}

//Basic Environment Object Properties 
// NOTE TEMPORARY CLASS
public class ObjectSet {
	
	public Vector3 position; //transform.position
	public Quaternion rotation; //transform.rotation
	public Vector3 scale; //transfrom.scale
	public string name; //name of game object
	public int toggleState; // Will be different for different objects, but is a simple number that refers to the state of the object
	
	//Initialise Objects properties 
	public ObjectSet (Vector3 pos, Quaternion rot, Vector3 sca, string obName, int toggleState) {
		
		position = pos;
		rotation = rot;
		scale = sca;
		name = obName;
		this.toggleState = toggleState;
	}
	
}

public class EditorManagerScript : MonoBehaviour {
	public GameObject player; // Prefab for the players
	public GameObject efield; // Electric Field prefab;
	public GameObject mfield; // Magnetic Field prefab;
	public GameObject wall; // Barrier prefab
	public GameObject measurer; // Measurer prefab
	public GameObject antiMatter; // prefab for antiMatter
	public GameObject teleporter; // prefab for Teleporter
	public GameObject spawnPoint; // prefab spawnpoint
	public GameObject gate; // Gate prefab
	public GameObject triggerPoint; //trigger point prefab
	private GameObject currentPlayerSpawnPoint; 
	GameObject selectedEnvironment = null;
	IEnvironmentObject selectedScript = null;
	List<GameObject> environments = new List<GameObject>();
	float ResizeExtraPush = 10f;
	float create; // 0 = Wall, 1 = Electric field, 2 = magnetic Field
	// PauseManager is really weird, so disabling input bruteForceStyle;
	bool enableInput = true; // Enables input feed events
	bool editor = false; // Checks whether or not the editor is true
	// Something ONLY for teleporters (may change later)
	TeleporterScript activatedTeleporter = null;
	
	
	// Use this for initialization
	void Start () {
		bool found = false;
		GameObject[] spawns = GameObject.FindGameObjectsWithTag("Spawn");
		foreach(GameObject spaw in spawns) {
			if(spaw.GetComponent<SpawnScript>().playerSpawn) {
				currentPlayerSpawnPoint = GameObject.Find ("SpawnPoint");
				environments.Add (currentPlayerSpawnPoint); 
				found = true;
				break;
			}
		}
		if (!found) {
			GameObject newSpawnPoint = Instantiate (spawnPoint, Vector3.zero, Quaternion.identity) as GameObject;
			environments.Add (newSpawnPoint);
			newSpawnPoint.GetComponent<SpawnScript>().playerSpawn = true;
			newSpawnPoint.GetComponent<SpawnScript>().spawningObject = player;
			currentPlayerSpawnPoint = newSpawnPoint;
		}
	}
	 
	
	// Listens for appropriate Input
	void OnInputEvent(Vector2 rawValue, ActionType action) {
		if (action == ActionType.Pause) {
			enableInput = !enableInput;
			// If the editor is enabled, we have to do some extra things to make sure objects don't get dragged around
			if (enableInput == false && UniversalHelperScript.Instance.editor == true) {
				UniversalHelperScript.Instance.editor = false;
				editor = true;
			} else if (editor == true && enableInput == true) {
				UniversalHelperScript.Instance.editor = true;
				editor = false;
			}
		}
		if (enableInput) {
			if (action == ActionType.Create) {
				Create (create);
			} else if (action == ActionType.Select) {
				Select ();
			} else if (action == ActionType.ChooseWall) {
				create = 0; // Create a wll
			} else if (action == ActionType.ChooseEField) {
				create = 1;
			} else if (action == ActionType.ChooseMField) {
				create = 2;
			} else if (action == ActionType.ChooseMeasurer) {
				create = 3;
			} else if (action == ActionType.ChooseAntiMatter) {
				create = 4;
			} else if (action == ActionType.ChooseTeleporter) {
				create = 5;
			} else if (action == ActionType.ChooseSpawn) {
				create = 6;		
			} else if (action == ActionType.ChooseAltSpawn) {
			create = 7;		
			} else if (action == ActionType.ChooseGate) {
				create = 8;
			} else if (action == ActionType.ChooseTriggerPoint) {
				create = 9;
			} else if (selectedEnvironment != null) {
				if (action == ActionType.Destroy) {
					DestroyEnv ();
				} else if (action == ActionType.Snap) {
					Snap ();
				} else if (action == ActionType.Flip) {
					Flip ();
				} else if (action == ActionType.Resize) {
					Resize (rawValue.x*ResizeExtraPush);
				} else if (action == ActionType.ResizeDirection) {
					ResizeDirection ();
				} else if (action == ActionType.DisableEntity) {
					SetEntityField();
				}
			}
		}
	}
	
	// An overloaded create function for level loading, if we already know where the object will be
	public void Create(ObjectSet obj) {
		GameObject tempEnv = null;
		if (obj.name.Contains ("Barrier")) {
		    // NOTE, if EVEN then vertical, else horizontal. This implies that the final EVEN energy value is TWICe the total energy
		    // Example: Horizontal wall with 100 energy would have a togglestate of 201
			tempEnv = Instantiate (wall,obj.position, obj.rotation) as GameObject;
			tempEnv.transform.localScale = obj.scale;
			if (Mathf.Abs (obj.toggleState) % 2 == 1) { // Absolute value is needed for Antimatter which has negative energy
			   obj.toggleState--;
			   tempEnv.GetComponent<WallScript>().Flip ();
			}
			tempEnv.GetComponent<WallScript>().SetEnergy (obj.toggleState / 2);
		} else if (obj.name.Contains ("ElectricField")) {
			tempEnv = Instantiate (efield,obj.position,Quaternion.identity) as GameObject; // we use default rotation because we handle flips ourselves
			tempEnv.transform.localScale = obj.scale;
			for (int i = 0; i < obj.toggleState;i++)
			 	tempEnv.GetComponent<ElectricFieldScript>().Flip ();
		} else if (obj.name.Contains("Antimatter")) {
			tempEnv = Instantiate (antiMatter,obj.position, obj.rotation) as GameObject;
			tempEnv.transform.localScale = obj.scale;	
			if (obj.toggleState == 1) {
				tempEnv.GetComponent<AntiMatterScript>().Flip();
			}
		}  else if (obj.name.Contains("ElectricField")) {
			tempEnv = Instantiate (efield,obj.position, obj.rotation) as GameObject;
			tempEnv.transform.localScale = obj.scale;		
		}  else if (obj.name.Contains("MagneticField")) {
			tempEnv = Instantiate (mfield,obj.position, obj.rotation) as GameObject;
			tempEnv.transform.localScale = obj.scale;	
			if (obj.toggleState == 1) {
				tempEnv.GetComponent<MagneticFieldScript>().Flip();
			}
		}  else if (obj.name.Contains("Measurer")) {
			tempEnv = Instantiate (measurer,obj.position, obj.rotation) as GameObject;
			tempEnv.transform.localScale = obj.scale;	
			if (obj.toggleState == 1) {
				tempEnv.GetComponent<MeasurerScript>().Flip();
			}
		}  else if (obj.name.Contains("SpawnPoint")) {
			if (obj.toggleState == 1) {
				currentPlayerSpawnPoint.transform.localPosition = obj.position;
			} else {
				tempEnv = Instantiate (spawnPoint,obj.position, obj.rotation) as GameObject;
				tempEnv.transform.localScale = obj.scale;	
			}
		}  else if (obj.name.Contains("Teleporter")) {
			tempEnv = Instantiate (teleporter,obj.position, obj.rotation) as GameObject;
			tempEnv.transform.localScale = obj.scale;
			if (obj.toggleState == 1) {
				tempEnv.GetComponent<TeleporterScript>().Flip ();
			}
		} else if (obj.name.Contains("TriggerPoint")) {
			tempEnv = Instantiate (triggerPoint,obj.position, obj.rotation) as GameObject;
		  	tempEnv.transform.localScale = obj.scale;
		  	TriggerFlags flags = (TriggerFlags)Mathf.CeilToInt(obj.toggleState);
			if((flags & TriggerFlags.LevelEnd) != 0) {
				tempEnv.GetComponent<TriggerPoint>().SetLevelEnd();
		  	}
		}  else if (obj.name.Contains("Gate")) {
			tempEnv = Instantiate (gate,obj.position, obj.rotation) as GameObject;
			tempEnv.transform.localScale = obj.scale;
			if (obj.toggleState == 1) {
				tempEnv.GetComponent<GateScript>().Flip ();
			}
		}  
	}
	
	// We create a wall in the space where the mouse is currently at
	// We then add the wall to our list of walls
	public void Create(float typeOfObject) {
		Vector3 envPosition = Camera.main.ScreenToWorldPoint (new Vector3(Input.mousePosition.x,Input.mousePosition.y, UniversalHelperScript.Instance.cameraZDistance));
		GameObject tempEnv;
		if (create == 0) {
			tempEnv = Instantiate (wall,envPosition, Quaternion.identity) as GameObject;
		} else if (create == 1) {
			tempEnv = Instantiate (efield,envPosition, Quaternion.identity) as GameObject;
		} else if (create == 2) {
			tempEnv = Instantiate (mfield,envPosition, Quaternion.identity) as GameObject;
		} else if (create == 3) {
			tempEnv = Instantiate (measurer,envPosition,Quaternion.identity) as GameObject;
		} else if (create == 4) {
			tempEnv = Instantiate (antiMatter,envPosition,Quaternion.identity) as GameObject;
		} else if (create == 5) {
			tempEnv = Instantiate (teleporter,envPosition,Quaternion.identity) as GameObject;	
		} else if (create == 6) {
			currentPlayerSpawnPoint.transform.localPosition = envPosition;
			return;
		} else if (create == 7) {
			tempEnv = Instantiate (spawnPoint,envPosition,Quaternion.identity) as GameObject;
		} else if (create == 8) {
			tempEnv = Instantiate (gate,envPosition,Quaternion.identity) as GameObject;
		} else if (create == 9) {
			tempEnv = Instantiate (triggerPoint,envPosition, Quaternion.identity) as GameObject;
		}  else {
		   return;
		}
		environments.Add (tempEnv);	
		selectedEnvironment = tempEnv;
		selectedScript = getValidComponent (tempEnv);	
	}
	
	public void DestroyEnv() {
		environments.Remove (selectedEnvironment);
		Destroy (selectedEnvironment);
		selectedScript = null;
		selectedEnvironment = null;
	}
	
	public void DestroyEnv(GameObject toBeDestroyed) {
	   	if (toBeDestroyed == selectedEnvironment) { // This will hold true because we're interested in the REFERENCE of the gameobject, not the object itself
	   		selectedEnvironment = null;
	   		selectedScript = null;
	   	}
	   	environments.Remove (toBeDestroyed);
	   	Destroy (toBeDestroyed);
	}
	
	private void Flip() {
		selectedScript.Flip ();
	}
	
	private void Resize(float resize) {
		selectedScript.Resize (resize);
	}
	
	private void ResizeDirection() {
		selectedScript.ChangeResizeDirection();
	}
	
	private void Snap() {
		selectedScript.Snap();
	}
	
	private void SetEntityField() {
		selectedScript.ToggleEntity();
	}
	
	private void Select() {
		RaycastHit2D hit = Physics2D.Raycast (Camera.main.ScreenToWorldPoint (Input.mousePosition), Vector2.zero);
		if (hit.collider != null) {
			selectedEnvironment = hit.collider.gameObject; // Error with antimatter due to their large trigger colliders (FIXED)
			selectedScript = getValidComponent (selectedEnvironment);
		}
	}
	
	private IEnvironmentObject getValidComponent(GameObject selectedEnvironment) {
		IEnvironmentObject selectedScript = null;
		if (selectedEnvironment.tag == "Walls") {
			selectedScript = selectedEnvironment.GetComponent<WallScript>();
		} else if (selectedEnvironment.tag == "EField") {
			selectedScript = selectedEnvironment.GetComponent<ElectricFieldScript>();
		} else if (selectedEnvironment.tag == "MField") {
			selectedScript = selectedEnvironment.GetComponent<MagneticFieldScript>();
		} else if (selectedEnvironment.tag == "Measurer") {
			selectedScript = selectedEnvironment.GetComponent<MeasurerScript>();
		} else if (selectedEnvironment.tag == "AntiMatter") {
			selectedScript = selectedEnvironment.GetComponent<AntiMatterScript>();	
		} else if (selectedEnvironment.tag == "Teleporter") {
			selectedScript = selectedEnvironment.GetComponent<TeleporterScript>();
		} else if (selectedEnvironment.tag == "Spawn") {
			selectedScript = selectedEnvironment.GetComponent<SpawnScript>();	
		} else if (selectedEnvironment.tag == "Gate") {
			selectedScript = selectedEnvironment.GetComponent<GateScript>();
		} else if (selectedEnvironment.tag == "Trigger") {
			selectedScript = selectedEnvironment.GetComponent<TriggerPoint>();
		}
		return selectedScript;
	}
	
	public void SetTeleporter(TeleporterScript teleporter) {
		if (activatedTeleporter != null) {
			activatedTeleporter.activateTeleporter (false); // Sets the previous active teleporter as inactive
		}
		activatedTeleporter = teleporter; // Sets the new teleporter as active
	}
	
	public TeleporterScript GetTeleport() {
		return activatedTeleporter;
	}
	
	public void EnableInput() {
		InputSystem.Instance.OnInputEditor += OnInputEvent;
	}
	
	public void DisableInput() {
		InputSystem.Instance.OnInputEditor -= OnInputEvent;
	}
	
	public void SetEditor(bool edit) {
		GameObject[] spawns = GameObject.FindGameObjectsWithTag ("Spawn");
		SpawnScript tempScript;
		foreach (GameObject spawn in spawns) {
			tempScript = spawn.GetComponent<SpawnScript>();
			tempScript.SetEditor (edit);
		}
	}
	
	public GameObject GetCurrentSpawnPoint() {
		return currentPlayerSpawnPoint;
	}
	
	private static EditorManagerScript instance; //Instance of InputSystem gameobject. InputSystem acts as a singleton class, being independent of other game objects 
	private EditorManagerScript() {}
	
	public static EditorManagerScript Instance
	{
		
		get 
		{
			if(instance == null) {
				instance = GameObject.FindObjectOfType(typeof(EditorManagerScript)) as EditorManagerScript; //Instantiate class if instance == null
				
			}
			return instance;
			
		}
	}
			
}


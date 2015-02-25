using UnityEngine;
using System.Collections;

// The UniversalHelperScript is a singleton script that has many useful functions that several classes use
// The UniversalHelperScript will contain constants that are true throughout the game and for every object
// As well as functions that many objects will use
public class UniversalHelperScript : MonoBehaviour {
	private static UniversalHelperScript instance;
	private UniversalHelperScript() {}
	public delegate void OnPlayerCreate(GameObject player);
	public event OnPlayerCreate OnPlayer; 
	public static UniversalHelperScript Instance
	{
		
		get 
		{
			if(instance == null) {
				instance = GameObject.FindObjectOfType(typeof(UniversalHelperScript)) as UniversalHelperScript; //Instantiate class if instance == null
				
			}
			return instance;
			
		}
	}
	
	public bool editor = false; // Whether or not the Editor is on. (This avoids every object keeping track of its own editor) (May be LESS efficient, discuss with Adam)
	
	public bool Editor {
		get { return editor;}
		set {
			editor = value;
			EditorManagerScript.Instance.SetEditor (editor);
		}
		
	}
	
	public bool shiftEnabled = false; // Whether or not SHIFT is pressed, this is important for finetuning in the editor
	public float cameraZDistance = 10f; // The distance the camera is away from the other objects (used by every IEnvironmentObject)
	public float playerZDistance = -2f; // The player should be above all other objects
	public float wallZDistance = -1.5f; // The Wall should be on the second layer
	public float antiMatterZDistance = 5f; // Antimatter should be on the bottom layer (to avoid editor problems due to the antimatters massive trigger collider)
	public float defaultGridSize = 0.1f;

	public void InformPlayerCreation(GameObject player) {
		OnPlayer(player);
	}
	
	// We want to SNAP a particular object to a grid. 
	// freeVector = the vector that is currently free, which we want to snap to a grid
	// snapConstant = The grid size we want to snap it too. 0.05 will snap it to a grid of 0.05 units, etc.
	// Returns the newly "snapped" vector
	public Vector3 Snap(Vector3 freeVector, float snapConstant) {
		freeVector *= (1/snapConstant);
		freeVector.x = Mathf.Round (freeVector.x);
		freeVector.y = Mathf.Round (freeVector.y);
		freeVector *= snapConstant;
		return freeVector;
	}
	
	// Overloaded method in-case you want to use the default grid of 0.05
	public Vector3 Snap(Vector3 freeVector) {
		freeVector *= (1/defaultGridSize);
		freeVector.x = Mathf.Round (freeVector.x);
		freeVector.y = Mathf.Round (freeVector.y);
		freeVector *= defaultGridSize;
		return freeVector;
	}
	
}

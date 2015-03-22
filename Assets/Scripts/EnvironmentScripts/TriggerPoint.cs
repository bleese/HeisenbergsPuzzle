using UnityEngine;
using System.Collections;

//Trigger Point : Trigger specific events when the player hovers over the point
public class TriggerPoint : MonoBehaviour, IEnvironmentObject {
	
	private UniversalHelperScript universalHelper;
	
	public bool levelEnd;	//If trigger marks the end of the level
	public bool convertMatter; // If trigger marks swapping the player from matter to AntiMatter;
	
	// Use this for initialization
	void Start () {
		universalHelper = GameObject.FindObjectOfType(typeof(UniversalHelperScript)) as UniversalHelperScript; // Find appropriate universalHelper script to use
	}
	

	//When the player hovers over the trigger point
	void OnTriggerEnter2D(Collider2D col) {
		
		//If it is the player
		if(col.gameObject.tag == "Player") {
			
			if(levelEnd) {	//if it is the end of the level
				
				LevelSwitch(); //Switch level
				
			}
			
			if (convertMatter) {
				ConvertMatter();
				
			}
		}
		
		
	}
	
	private void ConvertMatter() {
		universalHelper.playerScript.SetMatter (!universalHelper.playerScript.GetMatter ());
	}
	
	//Loads next level provided by the LevelManager
	private void LevelSwitch() {
		Debug.Log ("Switch Level");
		string nextMap = LevelManager.Instance.MoveToNextMap(); //Gets the next map/maze
		MazeLoader.Instance.LoadScene(nextMap); //Loads the next maze
		
	}
	
	// Function is called when mouse is held down
	void OnMouseDrag() {
		if (universalHelper.editor == true) {
			// Mouseposition is given in screen coordinates, rather than world coordinates, so we can use this function to convert it relative to a camera
			// In this case we just use Main Camera
			transform.localPosition = Camera.main.ScreenToWorldPoint (new Vector3(Input.mousePosition.x,Input.mousePosition.y, universalHelper.cameraZDistance));
		}
	}
	
	public void ChangeResizeDirection () {
		return;
	}
	
	// resize the wall based on resize direction
	public void Resize(float resize) {
		return;
	}
	
	// Flips the wall;
	public void Flip() {
		return;
	}
	
	// We snap the wall into a grid
	public void Snap() {
		Vector3 tempVector = transform.position;
		tempVector.x = Mathf.Round(tempVector.x);
		tempVector.y = Mathf.Round(tempVector.y);
		transform.position = tempVector;
	}
		
	// Sets whether or not the field should have a force
	public void ToggleEntity () {
		this.collider2D.enabled = !this.collider2D.enabled;
	}
	
	//Set the level end flag to true
	public void SetLevelEnd() {
		levelEnd = true;
	}
	
}

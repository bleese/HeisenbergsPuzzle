using UnityEngine;
using System.Collections;
public class ElectricFieldScript : MonoBehaviour, IEnvironmentObject {
	private Vector2 direction; // Unit vector to specify the direction the electric field should push
	public int flips = 0;
	public float power; // The power the electric field pushes
	private UniversalHelperScript universalHelper; // our universalHelper
	private bool resizeDirection = false; // False = x direction, True = y direction
	// Electric fields are simpler than magnetic field. An electric field will push the player in a specified direction linearly
	private Vector3 offset;
	

	void OnTriggerStay2D(Collider2D col) {
		if (col.gameObject.tag == "Player") {
			col.gameObject.rigidbody2D.AddForce (direction*power);
		}
		//Debug.Log (col.gameObject);
	}
	
	// To stop auto-centering
	void OnMouseDown() {
		RaycastHit2D hit = Physics2D.Raycast (Camera.main.ScreenToWorldPoint (Input.mousePosition), Vector2.zero);
		offset = transform.localPosition - new Vector3(hit.point.x,hit.point.y,0);
	}
	
	// Function is called when mouse is held down
	void OnMouseDrag() {
		if (universalHelper.editor == true) {
			// Mouseposition is given in screen coordinates, rather than world coordinates, so we can use this function to convert it relative to a camera
			// In this case we just use Main Camera
			transform.localPosition = offset + Camera.main.ScreenToWorldPoint (new Vector3(Input.mousePosition.x,Input.mousePosition.y, universalHelper.cameraZDistance));
		}
	}
	
	// Temporary function to see snap objects to their nearest grid (0.5)
	void OnMouseUp() {
		if (universalHelper.editor && !universalHelper.shiftEnabled) {
			transform.localPosition = universalHelper.Snap (transform.localPosition);
		}
	}
	
	public void ChangeResizeDirection () {
		resizeDirection = !resizeDirection; // Thanks adam!
	}
	
	// resize the wall based on resize direction
	public void Resize(float resize) {
		if (universalHelper.shiftEnabled) {
			resize *= 0.1f;
		}
		if (resizeDirection) {
			Vector3 tempVector = transform.localScale;
			tempVector.x += resize;
			if (tempVector.x > 0) {
				transform.localScale = tempVector;
			}
		} else {
			Vector3 tempVector = transform.localScale;
			tempVector.y += resize;
			if (tempVector.y > 0) {
				transform.localScale = tempVector;
			}
		}
	}
	
	// Flips the wall;
	public void Flip() {
		transform.Rotate (0,0,-90); // rotate the object 90 degrees
		// We need to change the resizeDirection to keep it consitent
		// Otherwise rotating the object will in effect rotate the resizeDirection as well;
		ChangeResizeDirection();
		flips++; // For the instantiator to now how many times to flip;
		if (flips > 3) {
		   flips = 0;
		}
		// We also need to change the electricField direction vector
		if (direction == Vector2.up) {
			direction = Vector2.right;
		} else if (direction == Vector2.right) {
			direction = -Vector2.up;
		} else if (direction == -Vector2.up) {
			direction = -Vector2.right;
		} else {
			direction = Vector2.up;
		}
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
		if (power == 0f) {
			power = 600f;
		} else if (power == 600f) {
			power = 0f;
		}
	}
	
	// Use this for initialization
	void Start () {
		universalHelper = GameObject.FindObjectOfType(typeof(UniversalHelperScript)) as UniversalHelperScript; // Find appropriate universalHelper script to use
		direction = Vector2.up;
		power = 600f; // Temporarily 0 for editor reasons, when loading level will be 600f
	}
	
	// Update is called once per frame
	void FixedUpdate () {
	}
	
}

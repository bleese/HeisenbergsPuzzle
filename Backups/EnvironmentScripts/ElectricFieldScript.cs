using UnityEngine;
using System.Collections;
public class ElectricFieldScript : MonoBehaviour, IEnvironmentObject {
	public Vector2 direction; // Unit vector to specify the direction the electric field should push
	public float power; // The power the electric field pushes
	private float cameraZDistance = 10f; // Constant for camera distance
	private bool editor = true;
	private bool resizeDirection = false; // False = x direction, True = y direction
	// Electric fields are simpler than magnetic field. An electric field will push the player in a specified direction linearly
	
	void OnTriggerStay2D(Collider2D col) {
		if (col.gameObject.tag == "Player") {
			col.gameObject.rigidbody2D.AddForce (direction*power);
		}
		//Debug.Log (col.gameObject);
	}
	
	// Function is called when mouse is held down
	void OnMouseDrag() {
		if (editor == true) {
			// Mouseposition is given in screen coordinates, rather than world coordinates, so we can use this function to convert it relative to a camera
			// In this case we just use Main Camera
			transform.localPosition = Camera.main.ScreenToWorldPoint (new Vector3(Input.mousePosition.x,Input.mousePosition.y, cameraZDistance));
		}
	}
	
	public void ChangeResizeDirection () {
		resizeDirection = !resizeDirection; // Thanks adam!
	}
	
	// resize the wall based on resize direction
	public void Resize(float resize) {
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
	
	// Sets the editor
	public void SetEditor(bool edit) {
		editor = edit;
	}
	
	// Sets whether or not the field should have a force
	public void SetForce () {
		this.collider2D.enabled = !this.collider2D.enabled;
	}
	
	// Use this for initialization
	void Start () {
		direction = Vector2.up;
		power = 0f; // Temporarily 0 for editor reasons, when loading level will be 600f
	}
	
	// Update is called once per frame
	void FixedUpdate () {
	}
	
}

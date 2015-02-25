using UnityEngine;
using System.Collections;

public class MagneticFieldScript : MonoBehaviour, IEnvironmentObject {
	// Magnetic field is a bit more complicate than an electric field. There are only two directions, into the screen and out of the screen
	// Into is considered True; out of is considered False
	// Based on the Right Hand Rule (Remember first year physics) the magnetic field will apply a force normal to the direction of velocity
	// This will mean, unlike the electric field, the magnetic field will curve the electron with a force proportional to the speed of the electron
	bool direction; // True = into page, False = out of page
	float power = 40;
	private float cameraZDistance = 10f; // Constant for camera distance
	private bool editor = true;
	private bool resizeDirection = false; // False = x direction, True = y direction
	private bool force = true; // whether or not a force is applied
	public Sprite intoTheScreen; // Sprite for field lines going into page
	public Sprite outOfTheScreen; // Sprite for field lines going out of the page
	private SpriteRenderer spriteRenderer; // Object to actually render the sprites
	
	void OnTriggerStay2D(Collider2D col) {
		if (col.gameObject.gameObject.tag == "Player" && force == true) {
			Vector2 velocity = col.gameObject.rigidbody2D.velocity;
			float temp = velocity.x;
			velocity.x = -velocity.y;
			velocity.y = temp;
			if (!direction) {
		   		velocity *= -1;
			}
			col.gameObject.rigidbody2D.AddForce (velocity*power);
		}
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
			if (tempVector.x != 0) {
				transform.localScale = tempVector;
			}
		} else {
			Vector3 tempVector = transform.localScale;
			tempVector.y += resize;
			if (tempVector.y != 0) {
				transform.localScale = tempVector;
			}
		}
	}
	
	// Flips the wall;
	public void Flip() {
		// Change the direction of the force and the appropriate sprite
		direction = !direction;
		if (spriteRenderer.sprite == intoTheScreen) {
			spriteRenderer.sprite = outOfTheScreen;
		} else {
			spriteRenderer.sprite = intoTheScreen;
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
	public void ToggleEntity () {
		this.collider2D.enabled = !this.collider2D.enabled;
	}
	// Use this for initialization
	void Start () {
		spriteRenderer = GetComponent<SpriteRenderer>();
		if (spriteRenderer.sprite != null) {
			spriteRenderer.sprite = intoTheScreen;
		}
		direction = false;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

using UnityEngine;
using System.Collections;

public class MagneticFieldScript : MonoBehaviour, IEnvironmentObject {
	// Magnetic field is a bit more complicate than an electric field. There are only two directions, into the screen and out of the screen
	// Into is considered True; out of is considered False
	// Based on the Right Hand Rule (Remember first year physics) the magnetic field will apply a force normal to the direction of velocity
	// This will mean, unlike the electric field, the magnetic field will curve the electron with a force proportional to the speed of the electron
	public bool direction; // True = into page, False = out of page
	float power = 40;
	private bool resizeDirection = false; // False = x direction, True = y direction
	private bool force = true; // whether or not a force is applied
	public Sprite intoTheScreen; // Sprite for field lines going into page
	public Sprite outOfTheScreen; // Sprite for field lines going out of the page
	private SpriteRenderer spriteRenderer; // Object to actually render the sprites
	private UniversalHelperScript universalHelper;
	private Vector3 offset;
	
	void OnTriggerStay2D(Collider2D col) {
		if (col.gameObject.gameObject.tag == "Player" && force == true) {
			Vector2 velocity = col.gameObject.rigidbody2D.velocity;
			float temp = velocity.x;
			velocity.x = -velocity.y;
			velocity.y = temp;
			if (!universalHelper.playerScript.GetMatter ()) {
				velocity *= -1;
			}
			if (!direction) {
		   		velocity *= -1;
			}
			col.gameObject.rigidbody2D.AddForce (velocity*power);
		}
	}
	
	// To avoid auto-centering
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
		Debug.Log (spriteRenderer);
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
	//	editor = edit;
	}
	
	// Sets whether or not the field should have a force
	public void ToggleEntity () {
		force = !force;
	}
	// Use this for initialization
	void Awake () {
		intoTheScreen = SpriteKeeperScript.Instance.GetMFieldInto();
		outOfTheScreen = SpriteKeeperScript.Instance.GetMFieldOuto();
		universalHelper = GameObject.FindObjectOfType(typeof(UniversalHelperScript)) as UniversalHelperScript; // Find appropriate universalHelper script to use
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

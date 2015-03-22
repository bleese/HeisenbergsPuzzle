using UnityEngine;
using System.Collections;

public class GateScript : MonoBehaviour, IEnvironmentObject {

	// Magnetic field is a bit more complicate than an electric field. There are only two directions, into the screen and out of the screen
	// Into is considered True; out of is considered False
	// Based on the Right Hand Rule (Remember first year physics) the magnetic field will apply a force normal to the direction of velocity
	// This will mean, unlike the electric field, the magnetic field will curve the electron with a force proportional to the speed of the electro
	private bool resizeDirection = false; // False = x direction, True = y direction
	public bool isMatter = true;
	private UniversalHelperScript universalHelper;
	private Vector3 offset;
	public Sprite gate;
	public Sprite antiGate;
	private SpriteRenderer spriteRenderer;

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
	
	void OnCollisionEnter2D(Collision2D col) {
		if(col.gameObject.tag == "AntiMatter") {
		    AntiMatterScript antiScript = col.gameObject.GetComponent<AntiMatterScript>();
		    if (antiScript.isMatter != this.isMatter) {
			   EditorManagerScript.Instance.DestroyEnv (col.gameObject);
			   EditorManagerScript.Instance.DestroyEnv (this.gameObject);
			}
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
		isMatter = !isMatter;
		if (spriteRenderer.sprite == gate) {
			spriteRenderer.sprite = antiGate;
		} else {
			spriteRenderer.sprite = gate;
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
		this.GetComponent<Collider2D>().enabled = !this.GetComponent<Collider2D>().enabled;
	}
	// Use this for initialization
	void Awake () {
		universalHelper = GameObject.FindObjectOfType(typeof(UniversalHelperScript)) as UniversalHelperScript; // Find appropriate universalHelper script to use
		gate = SpriteKeeperScript.Instance.GetGate ();
		antiGate = SpriteKeeperScript.Instance.GetAntiGate ();
		spriteRenderer = GetComponent<SpriteRenderer>();
		if (spriteRenderer.sprite != null) {
			spriteRenderer.sprite = gate;
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}

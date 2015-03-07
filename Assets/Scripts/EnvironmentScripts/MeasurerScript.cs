using UnityEngine;
using System.Collections;

public class MeasurerScript : MonoBehaviour, IEnvironmentObject {
    private UniversalHelperScript universalHelper;
	private bool resizeDirection = false; // False = x direction, true = y direction
	private bool entityEnabled = true; // Will the measurer measure?
	public bool pORxMeasure = false; // False to measure MOMENTUM, true to measure POSITION
	public Sprite pMeasure; // Sprite for momentum measurer
	public Sprite xMeasure; // Sprite for position measurer
	private SpriteRenderer spriteRenderer; // Object to actually render the sprites
	private Vector3 offset;
	
	void OnTriggerEnter2D(Collider2D col) {
		if (col.gameObject.tag == "Player" && entityEnabled) {
			Controller controllerScript = col.gameObject.GetComponent<Controller>();
			if (pORxMeasure) {
				controllerScript.setSize (controllerScript.minSize);
			} else {
				controllerScript.setSize (controllerScript.maxSize);
			}
			controllerScript.fixSize (true);
		}
	}
	
	
	void OnTriggerExit2D(Collider2D col) {
		if (col.gameObject.tag == "Player") {
			Controller controllerScript = col.gameObject.GetComponent<Controller>();
			controllerScript.fixSize(false);
		}
	}
	
	// Calculating the offset between the mouse and the object to avoid unnecessary centering
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
		pORxMeasure = !pORxMeasure;
		//Debug.Log (spriteRenderer);
		//Debug.Log (pMeasure);
		if (spriteRenderer.sprite == pMeasure) {
			spriteRenderer.sprite = xMeasure;
		} else {
			spriteRenderer.sprite = pMeasure;
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
		entityEnabled = !entityEnabled;
	}
	
	// Use this for initialization
	void Awake () {
		universalHelper = GameObject.FindObjectOfType(typeof(UniversalHelperScript)) as UniversalHelperScript; // Find appropriate universalHelper script to use
		pMeasure = SpriteKeeperScript.Instance.GetPMeasure();
		xMeasure = SpriteKeeperScript.Instance.GetXMeasure();
		spriteRenderer = GetComponent<SpriteRenderer>();
		if (spriteRenderer.sprite != null) {
			spriteRenderer.sprite = pMeasure;
		}
	}
}

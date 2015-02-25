using UnityEngine;
using System.Collections;

public class TeleporterScript : MonoBehaviour, IEnvironmentObject {
	private UniversalHelperScript universalHelper;
	private bool entity = true; // Whether or not the teleporter does its thing
	public bool activated = false; // Whether or not this is the ACTIVE responder teleporter
	private bool alreadyEntered = false; // Whether or not the player only entered this collider due to teleporting to it
	public Sprite unactive; // Sprite for field lines going into page
	public Sprite activeSprite; // Sprite for field lines going out of the page
	private SpriteRenderer spriteRenderer;
	
	// When the player is teleported
	void OnTriggerEnter2D(Collider2D col) {
		if(col.gameObject.tag == "Player" && entity && EditorManagerScript.Instance.GetTeleport() != null && !activated && !alreadyEntered) {
			TeleporterScript targetTele = EditorManagerScript.Instance.GetTeleport ();
			col.gameObject.transform.localPosition = targetTele.transform.localPosition; // Player is teleported to new position
			targetTele.SetEnter (true);
			activateTeleporter (true); // This teleporter is seen as the active teleporter
		}
	}
	
	void OnTriggerExit2D(Collider2D col) {
		alreadyEntered = false;
	}
	
	// Determines to activate or deactivate this teleporter
	public void activateTeleporter(bool activated) {
		if (activated) {
		   EditorManagerScript.Instance.SetTeleporter (this);
		   spriteRenderer.sprite = activeSprite;
		} else {
			spriteRenderer.sprite = unactive;
		}
		this.activated = activated;
	}
	
	
	// Function is called when mouse is held down
	void OnMouseDrag() {
		if (universalHelper.editor == true) {
			// Mouseposition is given in screen coordinates, rather than world coordinates, so we can use this function to convert it relative to a camera
			// In this case we just use Main Camera
			float z = transform.localPosition.z;
			Vector3 tempPosition = Camera.main.ScreenToWorldPoint (new Vector3(Input.mousePosition.x,Input.mousePosition.y, universalHelper.cameraZDistance));
			tempPosition.z = z;
			transform.localPosition = tempPosition;
		}
	}
	
	// Use this for initialization
	void Start () {
		universalHelper = GameObject.FindObjectOfType(typeof(UniversalHelperScript)) as UniversalHelperScript; // Find appropriate universalHelper script to use
		spriteRenderer = GetComponent<SpriteRenderer>();
		if (spriteRenderer.sprite != null) {
			spriteRenderer.sprite = unactive;
		}
	}
	
	
	// We flip the wall 90 degrees
	public void Flip() {
		if (activated == true) {
			EditorManagerScript.Instance.SetTeleporter (null);
		} else {
			EditorManagerScript.Instance.SetTeleporter (this);
		}
		activated = !activated;
		activateTeleporter (activated);
	}
	
	// We resize the wall based on the resize float
	public void Resize(float resize) {
		return;
	}
	
	// We snap the wall into a grid
	public void Snap() {
		Vector3 tempVector = transform.position;
		tempVector.x = Mathf.Round(tempVector.x);
		tempVector.y = Mathf.Round(tempVector.y);
		transform.position = tempVector;
	}
	
	public void ChangeResizeDirection() {
		return;
	}
	
	public void ToggleEntity() {
		entity = !entity;
	}
	
	// We need this to stop an INFINITE telport between two points.
	public void SetEnter(bool enter) {
		alreadyEntered = enter;
	}
	// Update is called once per frame
	void FixedUpdate () {
		
	}
}


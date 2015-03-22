using UnityEngine;
using System.Collections;

public class AntiMatterScript : MonoBehaviour, IEnvironmentObject {
	private bool entity = true;
	private float pushconstant = 30f;
	private UniversalHelperScript universalHelper;
	public bool isMatter = false; // Whether or not the antimatter is antimatter or simply matter
	public Sprite antiMatter; // Sprite for field lines going into page
	public Sprite matter; // Sprite for field lines going out of the page
	private SpriteRenderer spriteRenderer; // Object to actually render the sprites
	
	void OnTriggerStay2D(Collider2D col) {
		if(col.gameObject.tag == "Player" && entity && ShouldAttack ()) {
			Vector3 targetPosition = col.gameObject.transform.localPosition - this.transform.localPosition;
			targetPosition.Normalize ();
			this.GetComponent<Rigidbody2D>().AddForce (targetPosition*pushconstant);
		}
	}
	
	void OnCollisionEnter2D(Collision2D col) {
		if(col.gameObject.tag == "Player" && entity && ShouldAttack ()) {
			Debug.Log ("Dead");
			Energy energyscript = col.gameObject.GetComponent<Energy>();
			energyscript.DecreaseEnergy (100);
		}
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
	void Awake () {
		universalHelper = GameObject.FindObjectOfType(typeof(UniversalHelperScript)) as UniversalHelperScript; // Find appropriate universalHelper script to use
		matter = SpriteKeeperScript.Instance.GetMatter ();
		antiMatter = SpriteKeeperScript.Instance.GetAntiMatter();
		Vector3 tempVector = transform.localPosition;
		tempVector.z = universalHelper.antiMatterZDistance;
		transform.localPosition = tempVector;
		spriteRenderer = GetComponent<SpriteRenderer>();
		if (spriteRenderer.sprite != null) {
			spriteRenderer.sprite = antiMatter;
		}
	}
	
	
	// We flip the wall 90 degrees
	public void Flip() {
		isMatter = !isMatter;
		if (spriteRenderer.sprite == antiMatter) {
			spriteRenderer.sprite = matter;
		} else {
			spriteRenderer.sprite = antiMatter;
		}
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
	// Update is called once per frame
	void FixedUpdate () {
		
	}

	private bool ShouldAttack() {
	  return (isMatter == !universalHelper.playerScript.GetMatter());
	}
}

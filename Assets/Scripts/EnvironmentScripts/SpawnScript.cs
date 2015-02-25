using UnityEngine;
using System.Collections;

public class SpawnScript : MonoBehaviour, IEnvironmentObject {
	
	private UniversalHelperScript universalHelper;
	
	public bool playerSpawn; //If it is a player spawn point
	
	public Sprite EnemySprite;//The enemy spawn sprite image
	public Sprite PlayerSprite; //The player spawn sprite image
	SpriteRenderer spriteRender;
	
	public GameObject spawningObject; //The gameobject the spawn point will spawn 
	
	// Use this for initialization
	void Start () {
		universalHelper = GameObject.FindObjectOfType(typeof(UniversalHelperScript)) as UniversalHelperScript; // Find appropriate universalHelper script to use
		spriteRender = GetComponent<SpriteRenderer>(); 
		SetEditor (universalHelper.editor);
		if(playerSpawn) { //If it is the player spawn point, give it the distinctive sprite appearance
			spriteRender.sprite = PlayerSprite;
		} else {
			spriteRender.sprite = EnemySprite;
		}
		
		
		Debug.Log (GameObject.FindGameObjectWithTag ("Player"));
		//If the spawn point spawns a player, and if a player object hasn't been spawned yet, instantiate the player
		//This condition will not be met when a player creates a new player spawn point in the editor
		if ( playerSpawn && (GameObject.FindGameObjectWithTag ("Player") == null) ) { 
			Debug.Log ("Hello World");
			Instantiate (spawningObject,this.transform.position, Quaternion.identity);
		} 
		
		
	}
	
	// Update is called once per frame
	void Update () {
	
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
	
	// Sets the editor
	public void SetEditor(bool edit) {
		spriteRender.enabled = edit; //Changes the visibility of the sprite. Spawn points will not be visible in play mode
	}
	
	// Sets whether or not the field should have a force
	public void ToggleEntity () {
		this.collider2D.enabled = !this.collider2D.enabled;
	}
	
	
}

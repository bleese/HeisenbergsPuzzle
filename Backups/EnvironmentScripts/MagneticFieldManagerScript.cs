using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MagneticFieldManagerScript : MonoBehaviour {
	public GameObject mfield; // Electric Field prefab;
	private GameObject tempField; 
	GameObject selectedField = null;
	MagneticFieldScript selectedScript = null;
	List<GameObject> mFields = new List<GameObject>();
	float cameraZDistance = 10f;
	float ResizeExtraPush = 10f;
	bool create;
	
	// Listens for appropriate Input
	void OnInputEvent(Vector2 rawValue, ActionType action) {
		if (action == ActionType.Create && create == true) {
			createField ();
		} else if (selectedField != null) {
			if (action == ActionType.Destroy) {
				Destroy ();
			} else if (action == ActionType.Snap) {
				Snap ();
			} else if (action == ActionType.Flip) {
				Flip ();
			} else if (action == ActionType.Resize) {
				Resize (rawValue.x*ResizeExtraPush);
			} else if (action == ActionType.ResizeDirection) {
				ResizeDirection ();
			} else if (action == ActionType.DisableEntity) {
				SetEntityField();
			}
		}
	}
	
	// We create a wall in the space where the mouse is currently at
	// We then add the wall to our list of walls
	public void createField() {
		Debug.Log ("Field is being created");
		Vector3 fieldPosition = Camera.main.ScreenToWorldPoint (new Vector3(Input.mousePosition.x,Input.mousePosition.y, cameraZDistance));
		tempField = Instantiate (mfield,fieldPosition, Quaternion.identity) as GameObject;
		mFields.Add (tempField);		
	}
	
	private void Destroy() {
		Destroy (selectedField);
		selectedScript = null;
		selectedField = null;
	}
	
	private void Flip() {
		selectedScript.Flip ();
	}
	
	private void Resize(float resize) {
		selectedScript.Resize (resize);
	}
	
	private void ResizeDirection() {
		selectedScript.ChangeResizeDirection();
	}
	
	private void Snap() {
		selectedScript.Snap();
	}
	
	private void SetEntityField() {
		selectedScript.SetForce();
	}
	
	public void SetEditor(bool edit) {
		MagneticFieldScript tempScript = null;
		for (int counter = 0; counter < mFields.Count; counter++) {
			tempScript = mFields[counter].GetComponent<MagneticFieldScript>();
			tempScript.SetEditor (edit);
		}
	}
	
	
	// Makes a given wall a selected wall
	public void select(GameObject field) {
		selectedField = field;
		selectedScript = field.GetComponent<MagneticFieldScript>();
	}
	
	// Makes the selected wall null
	public void deselect() {
		selectedField = null;
		selectedScript = null;
	}
	
	
	// Sets whether or not we should create walls when pressing space
	public void SetCreate(bool createField) {
		create = createField;
		Debug.Log (create);
	}
	
	public void EnableInput() {
		InputSystem.Instance.OnInputEditor += OnInputEvent;
	}
	
	public void DisableInput() {
		InputSystem.Instance.OnInputEditor -= OnInputEvent;
	}
}
using UnityEngine;
using System.Collections;

public class WallScript : MonoBehaviour, IEnvironmentObject {
	public bool editor = true; // A bool to determine whether we're in editor mode or not;
	public float cameraZDistance = 10f; // The distance the camera is away from the object. In my case the camera is at Z -10, which means it's 10 away.
	public bool selected = false;
	public bool horizontal; // False is vertical direction, true is horizontal direction
    public float rotationConstant = 1 / Mathf.Sqrt (2);
    public float energyConsumption = 25;


    public void SetEditor(bool edit) {
       editor = edit;
    }
    
	// Function is called when mouse is held down
	void OnMouseDrag() {
		if (editor == true) {
			// Mouseposition is given in screen coordinates, rather than world coordinates, so we can use this function to convert it relative to a camera
			// In this case we just use Main Camera
			float z = transform.localPosition.z;
			Vector3 tempPosition = Camera.main.ScreenToWorldPoint (new Vector3(Input.mousePosition.x,Input.mousePosition.y, cameraZDistance));
			tempPosition.z = z;
			transform.localPosition = tempPosition;
		}
	}
	
	// Use this for initialization
	void Start () {
	   if (Mathf.Round(transform.rotation.eulerAngles.z) == 90) {
	      horizontal = true;
	   } else {
	      horizontal = false;
	   }
	}
	
	public bool IsHorizontal() {
		return horizontal;
	}
   
   	public bool IsSelected() {
   		return selected;
   	}

	public void setSelected(bool selectedSetter) {
	   selected = selectedSetter;
	}
	
	// We flip the wall 90 degrees
	public void Flip() {
		horizontal = !horizontal;
		transform.Rotate (0,0,90);
	}
	
	// We resize the wall based on the resize float
	public void Resize(float resize) {
		Vector3 tempVector = transform.localScale;
		tempVector.y += resize;
		transform.localScale = tempVector;
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
		return;
	}
	// Update is called once per frame
	void FixedUpdate () {

	}
}


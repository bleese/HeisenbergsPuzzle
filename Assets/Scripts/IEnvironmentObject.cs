using UnityEngine;
using System.Collections;

public interface IEnvironmentObject {
	// Resizes the object
	void Resize(float resize);
	// Changes what axis needs to be resized
	void ChangeResizeDirection();
	// Snaps the object to a 1x1 grid
	void Snap();
	// Flips or rotates the object
	void Flip();
	// Toggles whether or not the object will trigger. 
	// EG: An electric field will have a force, or a teleporter will teleport.
	void ToggleEntity();
}

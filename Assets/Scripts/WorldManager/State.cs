using UnityEngine;
using System.Collections;

public class State : MonoBehaviour {
	public string name; // The name of the object
	public Transform transform; // The transform of the object
	public Vector3 velocity; // The velocity of the object
	public bool certain; // Whether the object is "Certain" or not
	public string newWorld = ""; // The world associated with the object ("" = No world associated and is default);
}

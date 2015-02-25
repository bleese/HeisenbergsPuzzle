using UnityEngine;
using System.Collections;

//Draws a blue rectangular gizmo on the game object it is attached to. Used for Debugging
public class GizmoBound : MonoBehaviour {


	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {

	
	}
	
	void OnDrawGizmos() {
		Gizmos.color = new Color(0,0.2f,1,0.2f); //Blue
		//Gizmos.DrawCube(transform.position, new Vector3(0.1f,0.15f,0.1f)); //Rectangular Gizmo
		Gizmos.DrawSphere (transform.position, 0.7f); //Spherical Gizmo
	}
	
}

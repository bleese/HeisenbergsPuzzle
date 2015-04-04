using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class WorldManagerScript : MonoBehaviour {
	// A world is given a name identifier, then is represented as a list of Gameobject states that can be used to make a gameobject;
	Dictionary<string, List<State>> worlds;
	private static WorldManagerScript instance;
	private WorldManagerScript() {}
	string currentWorld = "root"; // The top world is always called Root;
	
	// Need to create the initial worldclass
	public void Awake() {
		worlds = new Dictionary<string, List<State>>();
	}
	
	// Creating a new world
	public void createNode(string name) {
		worlds.Add (name, new List<State>()); // Generates the new world;
	}
	
	// Adding an object to a new world via the objects State
	public void createObject(string name, State objectState) {
		List<State> states;
		if (worlds.TryGetValue (name, out states)) { // If the world (given by name) exists
			states.Add (objectState); // Add the new object to the states list
		}
	}
	
	// We are tunnelling into the new world associated with an object
	public void tunnelWorld(State stateObject) {
		if (stateObject.newWorld != "") {
			// Some function to deleteWorld
			makeWorld (stateObject.newWorld);
		}
	}
	
	// We are trying to tunnel backwards to a parent world
	public void tunnelOutWorld(string currentWorld) {
		if (!currentWorld.Equals ("root")) { // If we're at the root, we finish
			// Assume the setup is WorldName142 to represent we are FOUR deep
			currentWorld.Remove (currentWorld.Length - 1); // Remove the trailing character
			Regex numberpattern = new Regex("[0-9]$"); // Check to see if the has a version at the end;
			if (!numberpattern.IsMatch (currentWorld)) { // If it does not we're at the root world
				currentWorld = "root";
			}
			// Call some function to remove the current world;
			makeWorld(currentWorld);
		}
	}
	
	// Based on the states, tell EditorManager to create the new world
	public void makeWorld(string name) {
		List<State> states;
		if (worlds.TryGetValue (name,out states)) {
			foreach (State state in states) {
				EditorManagerScript.Instance.Create (state);
			}
			currentWorld = name;
		}
	}
	
	
	public static WorldManagerScript Instance
	{
		get 
		{
			if(instance == null) {
				instance = GameObject.FindObjectOfType(typeof(WorldManagerScript)) as WorldManagerScript; //Instantiate class if instance == null
			}
			return instance;
		}
	}
}

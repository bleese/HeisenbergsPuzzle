using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WorldManagerScript : MonoBehaviour {
	// A world is given a name identifier, then is represented as a list of Gameobject states that can be used to make a gameobject;
	Dictionary<string, List<State>> worlds;
	private static WorldManagerScript instance;
	private WorldManagerScript() {}
	
	public void Awake() {
		worlds = new Dictionary<String, List<State>>();
	}
	
	public void createNode(string name) {
		worlds.Add (name, new List<State>()); // Generates the new world;
	}
	
	public void createObject(string name, State objectState) {
		List<State> states;
		if (worlds.TryGetValue (name, out states)) { // If the world (given by name) exists
			states.Add (objectState); // Add the new object to the states list
		}
	}
	
	public void tunnelWorld(State stateObject) {
		if (stateObject.newWorld != "") {
			makeWorld (stateObject.newWorld);
		}
	}
	
	public void makeWorld(string name) {
		List<State> states;
		if (worlds.TryGetValue (name,out states)) {
			foreach (State state in states) {
				EditorManagerScript.Instance.Create (state);
			}
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

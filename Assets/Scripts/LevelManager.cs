using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour {
	
	List<string> playlist; //The current playlist of maps the player is going through
	private int currentIndex; //Index of the current map the player is up to
	
	private static LevelManager instance; //Instance of LevelManager gameobject. LevelManager acts as a singleton class, being independent of other game objects 
	
	private LevelManager() {}
	
	public static LevelManager Instance
	{
		
		get 
		{
			if(instance == null) {
				instance = GameObject.FindObjectOfType(typeof(LevelManager)) as LevelManager; //Instantiate class if instance == null
				
			}
			return instance;
			
		}
	}
	
	
	// Use this for initialization
	void Start () {
		playlist = new List<string>();
		currentIndex = -1;
		RetrieveAllMazes();
	}
	
	//Retrives all the saved mazes and places it in the playlist
	void RetrieveAllMazes() {
		Object[] mazeObjs = Resources.LoadAll("Mazes/");
		foreach (Object maze in mazeObjs) {
			playlist.Add (maze.name);
		}
		
		
	}
	
	//Return name of current map the player is on
	public string RetrieveCurrentMap() {
		return playlist[currentIndex];
	}
	
	
	//Return name of the next map the player will be playing on
	public string RetriveNextMap() {
		
		string map;
		
		if( (currentIndex + 1) >= playlist.Count) {
			map = playlist[0];	
		} else {
			map = playlist[currentIndex];
		}
		
		return map;
		
	}
	
	//Increment the current map index
	//Index cycles around the playlist
	private void IncrementIndex() {
		
		
		
		if( (currentIndex + 1) >= playlist.Count) {
			currentIndex = 0;	
		} else {
			currentIndex++;
		}
	
	
		
	}
	
	//Move to the next map
	//Used to get the name of the next map and set the playlist map index at that point
	public string MoveToNextMap() {
		
		IncrementIndex();
		return RetrieveCurrentMap();
		
	}
	
	
	
	
}

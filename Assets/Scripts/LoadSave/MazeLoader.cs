using UnityEngine;
using System.Collections;
using SimpleJSON;
using System.Collections.Generic;
using UnityEditor;
using System.IO;
using UnityEngine.UI;

public class MazeLoader : MonoBehaviour {
	
	private ComboBox comboBox;
	
	
	private static MazeLoader instance; //Instance of LevelManager gameobject. LevelManager acts as a singleton class, being independent of other game objects 
	
	private MazeLoader() {}
	
	public static MazeLoader Instance
	{
		
		get 
		{
			if(instance == null) {
				instance = GameObject.FindObjectOfType(typeof(MazeLoader)) as MazeLoader; //Instantiate class if instance == null
				
			}
			return instance;
			
		}
	}
	
	void Start() {
	
		comboBox = GameObject.Find ("ComboBox").GetComponent<ComboBox>();
		
	}
	
	//Scans the scene for all environment objects in the scene and saves them to file 
	public void SaveScene () {
		InputField saveText = GameObject.Find ("NameSaveText").GetComponent<InputField>(); //Input Field with file name
		List<ObjectSet> mazeSet = ScanScene(); //List of all the environment objects
		SaveSceneToFile(saveText.text, mazeSet);	
		saveText.text = "";
		RefreshComboBox();
	}
	
	private void RefreshComboBox() {
		Object[] mazeObjs = Resources.LoadAll("Mazes/");
		foreach (Object maze in mazeObjs) {
			comboBox.AddItems (maze.name);
		}
	}
	
	//Scans and retrieves all the objects in the scene with the environment tag
	private List<ObjectSet> ScanScene() {
		
		List<GameObject> envObjects = GameObjectsInScene();
		
		List<ObjectSet> mazeSet = new List<ObjectSet>();
		
	 	foreach(GameObject env in envObjects) {
	 		//Save the objects transform properties into an ObjectSet 
			
			string envName = env.name;
			
			if(env.tag == "Spawn") {
				if(env.GetComponent<SpawnScript>().playerSpawn) {
					envName += "P";
				}
			}
			//Debug.Log (env);	 		
	 		int state = 0;
	 		// For antimatter state determines if it is antimatter or matter. 0 = antimatter, 1 = matter
			if (env.tag == "AntiMatter" && !env.GetComponent<AntiMatterScript>().isMatter) {
	 			state = 1;
	 		} else if (env.tag == "EField") {
	 		 	state = env.GetComponent<ElectricFieldScript>().flips;
	 		// For Magnetic field state refers to whether or not the magnetic field is into our out of page. 0 = into, 1 = out of
	 		} else if (env.tag == "MField" && env.GetComponent<MagneticFieldScript>().direction) {
	 			state = 1;
	 		// Whether or not the teleporter is activated, 0 = no, 1 = yes
	 		} else if (env.tag == "Teleporter" && env.GetComponent<TeleporterScript>().enabled) {
	 			state = 1;
	 		// Whether or not the Measurer is measuring momentem or position. 0 for position, 1 for momentum
	 		} else if (env.tag == "Measurer" && env.GetComponent<MeasurerScript>().pORxMeasure) {
	 			state = 1;
	 		// Whether or not the spawn is a player spawn or not
	 		} else if (env.tag == "Spawn" && env.GetComponent<SpawnScript>().playerSpawn) {
	 			state = 1;
	 		} else if (env.tag == "Walls") {
	 			//Debug.Log (env.GetComponent<WallScript>());
				// The state is twice the value of the energy in the wall. If the wall is even then the wall is verticle, otherwise it is horizontal
				state = 2*env.GetComponent<WallScript>().energyConsumption;
				//Debug.Log (env.GetComponent<WallScript>().energyConsumption);
				if (env.GetComponent<WallScript>().IsHorizontal()) {
					state++;
			    }
			    //Debug.Log (state);
	 		} else if (env.tag == "Trigger") {
	 			TriggerPoint triggerScript = env.GetComponent<TriggerPoint>();
	 			TriggerFlags stateFlag = 0;
	 			if(triggerScript.levelEnd) {
	 				stateFlag |= TriggerFlags.LevelEnd;
	 			}
	 			
	 			state = ((int)stateFlag);
	 			
	 		} else if (env.tag == "Gate" && !env.GetComponent<GateScript>().isMatter) {
	 		   state = 1;
	 		}
	 		// Determining the state of the object, in terms of rotation or what-not
	 		
	 		/*
	 		if(env.tag == dg) {
	 		
	 		
	 		} else {

	 		}
	 		*/
	 		mazeSet.Add (new ObjectSet(env.transform.position, env.transform.rotation, env.transform.localScale, envName, state));
	 		
	 	}
	 	
	 	return mazeSet;	
	} 
	
	/*Scans for all the GameObjects in the scene. Returns a list of all Game Objects that are tagged:
		-Environment
		-Walls
		-Spawn
		-EField
		-MField
	*/
	private List<GameObject> GameObjectsInScene() {
		
		string[] tags = {"Walls", "Spawn", "EField", "MField", "AntiMatter", "Trigger", "Measurer", "Teleporter", "Gate"}; //Update tag array if you want to save extra objects
		
		List<GameObject> envObjects = new List<GameObject>();
		
		foreach(string tag in tags) { //Goes through each tag
			
			GameObject[] envObj = GameObject.FindGameObjectsWithTag(tag);  //Collects all objects of the tag
			
			foreach(GameObject obj in envObj) {
				envObjects.Add(obj);	//Adds them to envObjects
			}
			
		}
		
		return envObjects;
	}
	
	/*Scans for all the GameObjects in the scene specified in the specificTags parameter
	*/
	private List<GameObject> GameObjectsInScene(string[] specificTags) {
		
		
		List<GameObject> envObjects = new List<GameObject>();
		
		foreach(string tag in specificTags) { //Goes through each tag
			
			GameObject[] envObj = GameObject.FindGameObjectsWithTag(tag);  //Collects all objects of the tag
			
			foreach(GameObject obj in envObj) {
				envObjects.Add(obj);	//Adds them to envObjects
			}
			
		}
		
		return envObjects;
	}
	
	//Save each object properties to a JSON file
	/* JSON format:
		{
			"ObjectN": {
				"Position" : {
					"x" : Double, 
					"y": Double,
					"z" : Double
				},
				"Rotation" : {
					"x" : Double, 
					"y": Double,
					"z" : Double
				},
				"Scale" : {
					"x" : Double, 
					"y": Double,
					"z" : Double
				},
				"Name" : String
			}
		}
	*/
	private void SaveSceneToFile(string mazeName, List<ObjectSet> mazeSet) {
		
		SimpleJSON.JSONNode mazeJSON = new SimpleJSON.JSONClass();
		
		int i = 0;
		
		foreach(ObjectSet mazeObj in mazeSet) {
			//Debug.Log (mazeObj.name + " " + mazeObj.postion + " " + mazeObj.rotation + " " + mazeObj.scale); 
			
			string objectName = "Object"+i;
			
			//JSON Format, creates the JSON format through the mazeJSON Node
			mazeJSON[objectName]["Position"]["x"].AsDouble = mazeObj.position.x;
			mazeJSON[objectName]["Position"]["y"].AsDouble = mazeObj.position.y;
			mazeJSON[objectName]["Position"]["z"].AsDouble = mazeObj.position.z;
			
			mazeJSON[objectName]["Rotation"]["x"].AsDouble = mazeObj.rotation.x;
			mazeJSON[objectName]["Rotation"]["y"].AsDouble = mazeObj.rotation.y;
			mazeJSON[objectName]["Rotation"]["z"].AsDouble = mazeObj.rotation.z;
			
			mazeJSON[objectName]["Scale"]["x"].AsDouble = mazeObj.scale.x;
			mazeJSON[objectName]["Scale"]["y"].AsDouble = mazeObj.scale.y;
			mazeJSON[objectName]["Scale"]["z"].AsDouble = mazeObj.scale.z;
			
			mazeJSON[objectName]["State"].AsInt = mazeObj.toggleState;
			
			mazeJSON[objectName]["Name"] = mazeObj.name;
			
			i++;
			
		}
		
		string fileDest = "Assets/Resources/Mazes/" + mazeName + ".txt";
		File.WriteAllText(fileDest, mazeJSON.ToString()); //Writes the JSON file 
		
		UnityEditor.AssetDatabase.Refresh();	//Refresh the Editor, making changes immeadiate			

	}
	
	//Loads Text Asset and returns its string 
	private string GetTextFromFile (string fileName) {
		TextAsset txt = (TextAsset)Resources.Load ("Mazes/" + fileName,typeof(TextAsset));
		string data = txt.text;
		return data;
		
		
	}
	
	//Loads the Scene specified by the combo box in the Pause screen combo box
	public void LoadScene() {
		
		string mazeName = RetrieveMaze();
		List<ObjectSet> envObjects = ParseScene(mazeName);
		BuildScene(envObjects);
		
					
	}
	
	//Loads the Scene specified by parameter 'mazeName'
	public void LoadScene(string mazeName) {
		
		List<ObjectSet> envObjects = ParseScene(mazeName);
		BuildScene(envObjects);
		
		
	}
	
	//Clears environment objects from the scene
	//Doesn't remove spawn points
	private void ClearScene() {
		
		List<GameObject> envObjects;
		
		string[] specificTags = {"Walls", "EField", "MField", "AntiMatter", "Trigger","Measurer", "Teleporter", "Gate"}; //Objects the ClearScene function uses
		
		envObjects = GameObjectsInScene(specificTags);
		
		foreach(GameObject env in envObjects) {
			Destroy(env);	
		}
	}
	
	private string RetrieveMaze() {
		
		ComboBoxItem selecItem = comboBox.Items[comboBox.SelectedIndex];
		return selecItem.Caption;
		
	}
	
	private List<ObjectSet> ParseScene(string mazeName) {
	
	
		SimpleJSON.JSONNode mazeJSON = SimpleJSON.JSONNode.Parse(GetTextFromFile(mazeName));
		List<ObjectSet> envObjects = new List<ObjectSet>();
		
		for(int i = 0; i < mazeJSON.Count; i++) {
			string tempName = mazeJSON[i]["Name"];
			Vector3 tempPosition = new Vector3(mazeJSON[i]["Position"]["x"].AsFloat,mazeJSON[i]["Position"]["y"].AsFloat, mazeJSON[i]["Position"]["z"].AsFloat);
			Quaternion tempRotation = new Quaternion(mazeJSON[i]["Rotation"]["x"].AsFloat,mazeJSON[i]["Rotation"]["y"].AsFloat, mazeJSON[i]["Rotation"]["z"].AsFloat, 0);
			Vector3 tempScale = new Vector3(mazeJSON[i]["Scale"]["x"].AsFloat,mazeJSON[i]["Scale"]["y"].AsFloat, mazeJSON[i]["Scale"]["z"].AsFloat);
			int state = mazeJSON[i]["State"].AsInt;
			ObjectSet tempObject = new ObjectSet(tempPosition,tempRotation,tempScale,tempName, state);
			envObjects.Add(tempObject);
			
		}
		
		
		return envObjects;
	
	}
	
	/*
	-Clears the current scene -> Deletes all environment objects
	-Instantiates all environment objects specified in envObjects parameter
	*/
	private void BuildScene(List<ObjectSet> envObjects) {
		
		ClearScene(); // Remove all environment objects
		
		//Find the player object and destroy it
		GameObject player = GameObject.FindGameObjectWithTag("Player");
		if(player != null) {
			player.GetComponent<Controller>().DestroyMe();
		}
		
		
		foreach(ObjectSet tempObject in envObjects) {
			EditorManagerScript.Instance.Create (tempObject);
			//Debug.Log (tempObject.name + ": " + tempObject.position.ToString("F3") + " " + tempObject.rotation.ToString("F3") + " " + tempObject.scale.ToString("F3") );
		}
		
		//Respawn the player at the new spawn point
		EditorManagerScript.Instance.GetCurrentSpawnPoint().GetComponent<SpawnScript>().Respawn();
		
		
	}
	

	
}

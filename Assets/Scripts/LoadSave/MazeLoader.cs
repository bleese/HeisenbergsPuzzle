using UnityEngine;
using System.Collections;
using SimpleJSON;
using System.Collections.Generic;
using UnityEditor;
using System.IO;
using UnityEngine.UI;

public class MazeLoader : MonoBehaviour {
	
	private ComboBox comboBox;
	
	void Start() {
	
		comboBox = GameObject.Find ("ComboBox").GetComponent<ComboBox>();
		
	}
	
	//Scans the scene for all environment objects in the scene and saves them to file 
	public void SaveScene () {
		InputField saveText = GameObject.Find ("NameSaveText").GetComponent<InputField>(); //Input Field with file name
		List<ObjectSet> mazeSet = ScanScene(); //List of all the environment objects
		SaveSceneToFile(saveText.text, mazeSet);	
		saveText.text = "";
	}
	
	//Scans and retrieves all the objects in the scene with the environment tag
	private List<ObjectSet> ScanScene() {
		
		GameObject[] envObjects = GameObjectsInScene();
		
		List<ObjectSet> mazeSet = new List<ObjectSet>();
		
	 	foreach(GameObject env in envObjects) {
	 		//Save the objects transform properties into an ObjectSet 
			
			string envName = env.name;
			
			if(env.tag == "Spawn") {
				if(env.GetComponent<SpawnScript>().playerSpawn) {
					envName += "P";
				}
			}
	 		
	 		float state = 0f;
	 		// For antimatter state determines if it is antimatter or matter. 0 = antimatter, 1 = matter
			if (env.tag == "AntiMatter" && env.GetComponent<AntiMatterScript>().isMatter) {
	 			state = 1f;
	 		// For Magnetic field state refers to whether or not the magnetic field is into our out of page. 0 = into, 1 = out of
	 		} else if (env.tag == "MField" && env.GetComponent<MagneticFieldScript>().direction) {
	 			state = 1f;
	 		// Whether or not the teleporter is activated, 0 = no, 1 = yes
	 		} else if (env.tag == "Teleporter" && env.GetComponent<TeleporterScript>().enabled) {
	 			state = 1f;
	 		// Whether or not the Measurer is measuring momentem or position. 0 for position, 1 for momentum
	 		} else if (env.tag == "Measurer" && env.GetComponent<MeasurerScript>().pORxMeasure) {
	 			state = 1f;
	 		// Whether or not the spawn is a player spawn or not
	 		} else if (env.tag == "Spawn" && env.GetComponent<SpawnScript>().playerSpawn) {
	 			state = 1f;
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
	
	/*Scans for all the GameObjects in the scene. Returns an array of all Game Objects that are tagged:
		-Environment
		-Walls
		-Spawn
		-EField
		-MField
	*/
	private GameObject[] GameObjectsInScene() {
		
		//Code is a bit of a mess at the moments, will improve
		
	 	//Get all the gameobject of the tag
		GameObject[] envObjects1 = GameObject.FindGameObjectsWithTag("Walls");
		GameObject[] envObjects2 = GameObject.FindGameObjectsWithTag("Spawn");
		GameObject[] envObjects3 = GameObject.FindGameObjectsWithTag("EField");
		GameObject[] envObjects4 = GameObject.FindGameObjectsWithTag("MField");
		GameObject[] envObjects5 = GameObject.FindGameObjectsWithTag("AntiMatter");
		
		GameObject[] envObjects = new GameObject[envObjects1.Length + envObjects2.Length + envObjects3.Length + envObjects4.Length + envObjects5.Length ];
		
		//Concatenate all the game objects into one array
		int tempIndex = 0;
		envObjects1.CopyTo(envObjects,tempIndex);
		tempIndex += envObjects1.Length;
		envObjects2.CopyTo(envObjects,tempIndex);
		tempIndex += envObjects2.Length;
		envObjects3.CopyTo(envObjects,tempIndex);
		tempIndex += envObjects3.Length;
		envObjects4.CopyTo(envObjects,tempIndex);
		tempIndex += envObjects4.Length;
		envObjects5.CopyTo(envObjects,tempIndex);
		tempIndex += envObjects5.Length;
		//envObjects6.CopyTo(envObjects, tempIndex);
		//tempIndex += envObjects6.Length;
		
		
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
			
			mazeJSON[objectName]["State"].AsFloat = mazeObj.toggleState;
			
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
	

	public void LoadScene() {
		
		//ClearScene ();
		string mazeName = RetrieveMaze();
		List<ObjectSet> envObjects = ParseScene(mazeName);
		
					
	}
	
	
	private void ClearScene() {
		
		GameObject[] envObjects;
		
		envObjects = GameObjectsInScene();
		
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
			float state = mazeJSON[i]["State"].AsFloat;
			ObjectSet tempObject = new ObjectSet(tempPosition,tempRotation,tempScale,tempName, state);
			envObjects.Add(tempObject);
			
		}
		
		
		foreach(ObjectSet tempObject in envObjects) {
			EditorManagerScript.Instance.Create (tempObject);
			//Debug.Log (tempObject.name + ": " + tempObject.position.ToString("F3") + " " + tempObject.rotation.ToString("F3") + " " + tempObject.scale.ToString("F3") );
		}
		
		
		
		return envObjects;
	
	}
	

	
}

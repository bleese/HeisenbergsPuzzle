using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PauseManager : MonoBehaviour {
	
	private Canvas canvas;
	
	private bool editorEnabled;
	public InputField saveText;
	private ComboBox comboBox;
	
	// Use this for initialization
	void Start () {
		InputSystem.Instance.OnInputPlayer += OnInputEvent;
		canvas = GetComponent<Canvas>();
		canvas.enabled = false;
		editorEnabled = false;
		
		saveText = GameObject.Find("NameSaveText").GetComponent<InputField>();
		comboBox = 	GameObject.Find("ComboBox").GetComponent<ComboBox>(); // This is ComboBox
		
		LoadSaveOptions();
	}
	
	private void LoadSaveOptions() {
		Object[] mazeObjs = Resources.LoadAll("Mazes/");
		foreach (Object maze in mazeObjs) {
			comboBox.AddItems (maze.name);
		}
	}
			
	void OnInputEvent(Vector2 rawValue, ActionType action) {
		
		if(action == ActionType.Pause) {
			Pause ();
			UpdateText();
		}
					
	}
	
	void Update() {
		//Debug.Log (EventSystem.current.currentSelectedGameObject);
		if(EventSystem.current.currentSelectedGameObject != saveText.gameObject) {
			EventSystem.current.SetSelectedGameObject(null);
		}
	}
	
	public void Pause() {
		canvas.enabled = !canvas.enabled;
		Time.timeScale = Time.timeScale == 0 ? 1 : 0;
	}
	
	private void UpdateText() {
		Text pausedText = GameObject.Find ("Paused Text").GetComponent<Text>();
		
		if(editorEnabled) {
			pausedText.text = "PAUSED: Editor Mode";
		} else {
			pausedText.text = "PAUSED: Play Mode";
		}
	}
	
	public void EditorMode() {
		
		if(!editorEnabled) {
			EditorManagerScript editorManager = GameObject.Find ("EditorManager").GetComponent<EditorManagerScript>();
			editorManager.EnableInput ();
			UniversalHelperScript.Instance.Editor = true;
			editorEnabled = true;
			Pause ();
			
			
		}
		
		
		
		
	
	}
	
	public void PlayMode() {
		
		if(editorEnabled) {
			EditorManagerScript editorManager = GameObject.Find ("EditorManager").GetComponent<EditorManagerScript>();
			editorManager.DisableInput ();
			UniversalHelperScript.Instance.Editor = false;
			editorEnabled = false;
		}
		
		Pause ();
		
		
		
	}
	
	
}

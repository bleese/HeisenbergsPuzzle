using UnityEngine;
using System.Collections;

public class EnergyMonitor : MonoBehaviour {
	
	private Energy playerEnergy;
	
	// Use this for initialization
	void Start () {
		UniversalHelperScript.Instance.OnPlayer += OnPlayerCreate;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		
		if(playerEnergy && playerEnergy.updated) {
			
			Vector3 tempScale = transform.localScale;
			
			float percentage = playerEnergy.energy / 100;
			//Debug.Log (percentage);
			
			tempScale.x *=  percentage;
			
			this.transform.localScale = tempScale;
			playerEnergy.updated = false;
			
		}
		
	}
	
	public void OnPlayerCreate(GameObject player) {
		playerEnergy = player.GetComponent<Energy>();
	}
}

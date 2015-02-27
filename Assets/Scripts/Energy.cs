using UnityEngine;
using System.Collections;

public class Energy : MonoBehaviour {
	
	public float energy;
	public bool updated;
	
	// Use this for initialization
	void Start () {
		energy = 100f;
		updated = true;
	}
	
	public void DecreaseEnergy(float decEnergy) {
		
		energy -= decEnergy;
		
		if(energy <= 0) {
			energy = 0;
			
			Destroy();
		}
		
		updated = true;
	}
	
	public void IncreaseEnergy(float incEnergy) {
		
		if(energy < 100) {
			energy += incEnergy;
			
		} 
		
		if(energy >= 100) {
			energy = 100;
		}
		
		updated = true;
		
	}
	
	public void Destroy() {
		
		Debug.Log ("Destroy");
		
	}
	
	
	
}

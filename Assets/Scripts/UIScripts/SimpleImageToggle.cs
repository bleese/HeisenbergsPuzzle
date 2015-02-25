using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SimpleImageToggle : MonoBehaviour {
	
	private Image image;
	
	// Use this for initialization
	void Start () {
		image = GetComponent<Image>();
	}
	
	public void ToggleVisible() {
		image.enabled = !image.enabled;
	}
}

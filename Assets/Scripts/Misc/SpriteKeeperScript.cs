using UnityEngine;
using System.Collections;

public class SpriteKeeperScript : MonoBehaviour {

	private static SpriteKeeperScript instance;
	private SpriteKeeperScript() {}
	public static SpriteKeeperScript Instance
	{
		
		get 
		{
			if(instance == null) {
				instance = GameObject.FindObjectOfType(typeof(SpriteKeeperScript)) as SpriteKeeperScript; //Instantiate class if instance == null
				
			}
			return instance;
			
		}
	}
	
  	public Sprite MFieldInto;
 	public Sprite MFieldOuto;
  	public SpriteRenderer spriteRender;
 	  
   	void Start() {
		spriteRender = GetComponent<SpriteRenderer>(); 
   	}
   
   	public Sprite GetMFieldInto() {
    	return MFieldInto;
   	} 
   
   	public Sprite GetMFieldOuto() {
    	return MFieldOuto;
   	}
}

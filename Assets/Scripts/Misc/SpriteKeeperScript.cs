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
 	public Sprite antiMatter;
 	public Sprite matter;
 	public Sprite blackWall;
 	public Sprite yellowWall;
 	public Sprite redWall;
 	public Sprite greenWall;
 	public Sprite blueWall;
 	public Sprite pMeasurer;
 	public Sprite xMeasurer;
 	public Sprite uTele;
 	public Sprite aTele;
 	public Sprite gate;
 	public Sprite antiGate;
  	private SpriteRenderer spriteRender;
 	  
   	void Start() {
		spriteRender = GetComponent<SpriteRenderer>(); 
   	}
   	
   	public Sprite GetGate() {
   	   return gate;
   	}
   	
	public Sprite GetAntiGate() {
		return antiGate;
	}
   	
   	public Sprite GetMFieldInto() {
   	  	return MFieldInto;
   	}
   	
   	public Sprite GetMFieldOuto() {
   	  	return MFieldOuto;
   	}
   	
	public Sprite GetAntiMatter() {
		return antiMatter;
	} 
	
	public Sprite GetMatter() {
		return matter;
	}
	
	public Sprite GetBlackWall() {
		return blackWall;
	} 
	
	public Sprite GetRedWall() {
		return redWall;
	}   	
	
	public Sprite GetYellowWall() {
		return yellowWall;
	} 
	
	public Sprite GetGreenWall() {
		return greenWall;
	}   	
	
	public Sprite GetBlueWall() {
		return blueWall;
	}
	
	public Sprite GetPMeasure() {
		return pMeasurer;
	} 
	
	public Sprite GetXMeasure() {
		return xMeasurer;
	}   	
	
	public Sprite GetUTele() {
		return uTele;
	} 
	
	public Sprite GetATele() {
		return aTele;
	}
}

using UnityEngine;
using System.Collections;

public class WallScript : MonoBehaviour, IEnvironmentObject {
	private UniversalHelperScript universalHelper;
	public bool selected = false;
	public bool horizontal; // False is vertical direction, true is horizontal direction
    public float rotationConstant = 1 / Mathf.Sqrt (2);
    public int energyConsumption = 100;
    private Vector3 offset;
	private bool antiMatter = false;
	private SpriteRenderer spriteRenderer;
	public Sprite black;
	public Sprite blue;
	public Sprite green;
	public Sprite red;
	public Sprite yellow;
	
	
    void OnMouseDown() {
		RaycastHit2D hit = Physics2D.Raycast (Camera.main.ScreenToWorldPoint (Input.mousePosition), Vector2.zero);
		offset = transform.localPosition - new Vector3(hit.point.x,hit.point.y,0);
    }
    
	// Function is called when mouse is held down
	void OnMouseDrag() {
		if (universalHelper.editor == true) {
			// Mouseposition is given in screen coordinates, rather than world coordinates, so we can use this function to convert it relative to a camera
			// In this case we just use Main Camera
			float z = transform.localPosition.z;
			Vector3 tempPosition = Camera.main.ScreenToWorldPoint (new Vector3(Input.mousePosition.x,Input.mousePosition.y, universalHelper.cameraZDistance));
			tempPosition += offset;
			tempPosition.z = z;
			transform.localPosition = tempPosition;
		}
	}
	
	// Temporary function to see snap objects to their nearest grid (0.5)
	void OnMouseUp() {
		if (universalHelper.editor && !universalHelper.shiftEnabled) {
			transform.localPosition = universalHelper.Snap (transform.localPosition);
		}
	}
	
	void OnCollisionEnter2D(Collision2D col) {
		if(col.gameObject.tag == "Player" && antiMatter) {
			Energy health = col.gameObject.GetComponent<Energy>();
			health.DecreaseEnergy (100f);
		}
	}
	
	public void SetEnergy(int energy) {
		energyConsumption = energy;
		antiMatter = false;
		if (energy > 50) {
			spriteRenderer.sprite = black;		
		} else if (energy > 25) {
			spriteRenderer.sprite = yellow;
		} else if (energy > 10) {
			spriteRenderer.sprite = green;
		} else if (energy > 0) {
			spriteRenderer.sprite = blue;
		} else {
			spriteRenderer.sprite = red;
			antiMatter = true;
		}
	}
	
	// Use this for initialization
	void Awake () {
	   universalHelper = GameObject.FindObjectOfType(typeof(UniversalHelperScript)) as UniversalHelperScript; // Find appropriate universalHelper script to use
	   Vector3 tempPosition = transform.localPosition;
	   black = SpriteKeeperScript.Instance.GetBlackWall();
	   red = SpriteKeeperScript.Instance.GetRedWall();
	   yellow = SpriteKeeperScript.Instance.GetYellowWall();
	   green = SpriteKeeperScript.Instance.GetGreenWall();
	   blue = SpriteKeeperScript.Instance.GetBlueWall();	
	   tempPosition.z = UniversalHelperScript.Instance.wallZDistance;
	   transform.localPosition = tempPosition;
	   energyConsumption = 100;
	   if (Mathf.Round(transform.rotation.eulerAngles.z) == 90) {
	      horizontal = true;
	   } else {
	      horizontal = false;
	   }
		spriteRenderer = GetComponent<SpriteRenderer>();
		if (spriteRenderer.sprite != null) {
			spriteRenderer.sprite = black;
		}
	}
	
	public bool IsHorizontal() {
		return horizontal;
	}
   
   	public bool IsSelected() {
   		return selected;
   	}

	public void setSelected(bool selectedSetter) {
	   selected = selectedSetter;
	}
	
	// We flip the wall 90 degrees
	public void Flip() {
		horizontal = !horizontal;
		transform.Rotate (0,0,90);
	}
	
	// We resize the wall based on the resize float
	public void Resize(float resize) {
		if (universalHelper.shiftEnabled) {
			resize *= 0.1f;
		}
		Vector3 tempVector = transform.localScale;
		tempVector.y += resize*0.1f;
		if (tempVector.y > 0 ) {
	    	transform.localScale = tempVector;
	    }
	}
	
	// We snap the wall into a grid
	public void Snap() {
		Vector3 tempVector = transform.position;
		tempVector.x = Mathf.Round(tempVector.x);
		tempVector.y = Mathf.Round(tempVector.y);
		transform.position = tempVector;
	}
	
	public void ChangeResizeDirection() {
		if (energyConsumption > 50) {
			SetEnergy (50);	
		} else if (energyConsumption > 25) {
			SetEnergy (25);
		} else if (energyConsumption> 10) {
			SetEnergy (10);
		} else if (energyConsumption > 0) {
			SetEnergy (-1);
		} else {
			SetEnergy (100);
		}
	}
	
	public void ToggleEntity() {
		return;
	}
	// Update is called once per frame
	void FixedUpdate () {

	}
}


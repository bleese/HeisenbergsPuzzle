﻿using UnityEngine;
using System.Collections;

public class Controller : MonoBehaviour {
	// Constants
	public float maxspeed = 5; // Maximum speed player can move
	public float minSize = 0.05f; // Minimum size the player can contract too
	public float maxSize = 0.3f; // Maximum size the player can expand too
	private float expansionConstant = 0.01f; // The constant that determines the rate of expansion per frame
	private float momentumConstant = 1f;
	private float playerZDistance = -2f;
	private bool canChangeSize = true;
	private bool isMatter = true;
	private Sprite playersprite;
	private Sprite antiPlayerSprite;
	public SpriteRenderer spriteRenderer;
	//Usable scripts
	public CameraFollow cameraScript;
	
	//public Transform wallCheck;
	public float wallRadius = 0.1f;
	public LayerMask theWall;
	public LayerMask electricField;
	//private bool isMolesting = false;

	// useful variables
	private Vector3 tempVector;

	// The spriteRenderer needs to be in Awake, not start, as the script uses the sprites given BEFORE the script is even initialized!
	void Awake () {
		playersprite = SpriteKeeperScript.Instance.GetPlayer ();
		antiPlayerSprite = SpriteKeeperScript.Instance.GetAntiPlayer ();
		spriteRenderer = GetComponent<SpriteRenderer>();
		spriteRenderer.sprite = playersprite;
	}

	// Use this for initialization
	void Start () {
		EnableInput();
		
		Vector3 tempVector = transform.localPosition;
		tempVector.z = playerZDistance;
		transform.localPosition = tempVector;
		UniversalHelperScript.Instance.InformPlayerCreation (gameObject);
		cameraScript = GameObject.Find ("Main Camera").GetComponent<CameraFollow>();

	}

	// Forces the a particular size to the player
	public void setSize(float size) {
		transform.localScale = new Vector3(size,size,transform.localScale.z);
	}

	// Disables the players ability to change size	
	public void fixSize(bool input) {
		canChangeSize = !input;
	}

	void OnInputEvent(Vector2 rawValue, ActionType action) {
    	if (action == ActionType.Move) {
        	Move(rawValue);
       	} else if (action == ActionType.Uncertainty) {
        	  ApplyUncertainty(rawValue);
       	}
    }

   void Move(Vector2 rawValue) {
      GetComponent<Rigidbody2D>().velocity = rawValue*maxspeed;
      //rigidbody2D.AddForce (rawValue*maxspeed);
   }
   
   void ApplyUncertainty(Vector2 uncertaintyVector) {
		// Prototype of instant SNAPPING
		/*
		if (canChangeSize) {
			Vector3 tempVector = transform.localScale;
			if (tempVector.x == maxSize) {
				tempVector = new Vector3(minSize,minSize,0); 
			} else {
				tempVector = new Vector3(maxSize, maxSize,0);
			}
			transform.localScale = tempVector;
		}
		*/
		// Gradual Uncertainty
   		if (canChangeSize) {
    	    float uncertainty = uncertaintyVector.x;
			uncertainty *= expansionConstant;
			Vector3 sizeUnc = transform.localScale;
			Vector3 incrementSizeUnc = new Vector3 (uncertainty, uncertainty, 0);
			if ((uncertainty < 0 && sizeUnc.x > minSize) || (uncertainty > 0 && sizeUnc.x < maxSize)) {
				maxspeed += -uncertainty*2;
				transform.localScale = sizeUnc + incrementSizeUnc;
			}
		}
   }

	// Update is called once per frame
	void FixedUpdate () {
		Vector3 sizeUnc = transform.localScale;
		float randomConstraint = momentumConstant*(maxSize - sizeUnc.x);
		float speedUnc = UnityEngine.Random.Range(-randomConstraint, randomConstraint);
		Vector2 currentVelocity = GetComponent<Rigidbody2D>().velocity;
		Vector3 currentPosition = transform.localPosition;
		if (currentVelocity.x != 0 && Mathf.Abs (currentVelocity.x) + speedUnc > 0) {
			tempVector = currentPosition;
			tempVector.x += speedUnc;
			if (!Physics2D.OverlapCircle (tempVector, wallRadius, theWall)) {
				currentPosition.x += speedUnc;
			}
		}
		if (currentVelocity.y != 0 && Mathf.Abs (currentVelocity.y) + speedUnc > 0) {
			tempVector = currentPosition;
			tempVector.y += speedUnc;
			if (!Physics2D.OverlapCircle (tempVector, wallRadius, theWall)) {
				currentPosition.y += speedUnc;
			}
		}
		transform.localPosition = currentPosition;
		
		//cameraScript.Target = transform.position;
		
		

		
		if (Input.GetAxis ("Horizontal") != 0) {
			//Debug.Log (Input.GetAxis ("Horizontal"));
		}
	}
	
	public bool GetMatter() {
		return isMatter;
	}
	
	public void SetMatter(bool newMatter) {
		isMatter = newMatter;
		
		if (isMatter = true) {
			spriteRenderer.sprite = playersprite;
		} else {
			spriteRenderer.sprite = antiPlayerSprite;
		}
	}
		
	public void EnableInput() {
	   InputSystem.Instance.OnInputPlayer += OnInputEvent;
	}
	
	public void DisableInput() {
		InputSystem.Instance.OnInputPlayer -= OnInputEvent;
	}
	
	public void DestroyMe() {
		DisableInput();
		Destroy(this.gameObject);
	}
}

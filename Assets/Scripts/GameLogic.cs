using UnityEngine;
using System.Collections;

public class GameLogic : MonoBehaviour {
	public GameObject player;
	public GameObject startUI, restartUI;
	public GameObject startPoint, playPoint, restartPoint;
	public Animator SignAnimatorStart;
	public Animator SignAnimatorEnd;

	public bool playerWon = false;

	// Use this for initialization
	void Start () {
		player.transform.position = startPoint.transform.position;
		//SignAnimator = GetComponent<Animator> ();
	}

	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown (0) && player.transform.position==playPoint.transform.position) {
			puzzleSuccess ();
		}
	}

	public void startPuzzle() { //Begin the puzzle sequence
		toggleUIStart();
		iTween.MoveTo (player, 
			iTween.Hash (
				"position", playPoint.transform.position, 
				"time", 2, 
				"easetype", "linear"
			)
		);

	}

	public void resetPuzzle() { //Reset the puzzle sequence
		player.transform.position = startPoint.transform.position;
		toggleUIEnd ();
	}


	public void puzzleSuccess() { //Do this when the player gets it right
		iTween.MoveTo (player, 
			iTween.Hash (
				"position", restartPoint.transform.position, 
				"time", 2, 
				"easetype", "linear"
			)
		);
	}
	public void toggleUIStart() {
		SignAnimatorStart.SetBool ("SignClicked", true);
		SignAnimatorEnd.SetBool ("SignClicked", false);
	}

	public void toggleUIEnd() {
		SignAnimatorStart.SetBool ("SignClicked", false);
		SignAnimatorEnd.SetBool ("SignClicked", true);

		//startUI.SetActive (!startUI.activeSelf);
		//restartUI.SetActive (!restartUI.activeSelf);
	}

}
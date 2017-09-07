using UnityEngine;
using System.Collections;

public class GameLogic : MonoBehaviour {

	// Gameplay variables
	[Header("Gameplay")]
	public GameObject player;
	public float timeToGameplay;
	public float timeToEndScreen;
	public GameObject startPoint, playPoint, restartPoint;
	public bool playerWon = false;

	[Space(10)]

	// puzzle variables
	[Header("Puzzle")]
	public GameObject eventSystem;
	public GameObject[] puzzleSpheres; //An array to hold our puzzle spheres
	public int puzzleLength = 5; //How many times we light up.  This is the difficulty factor.  The longer it is the more you have to memorize in-game.
	public float puzzleSpeed = 1f; //How many seconds between puzzle display pulses
	private int[] puzzleOrder; //For now let's have 5 orbs
	private int currentDisplayIndex = 0; //Temporary variable for storing the index when displaying the pattern
	public bool currentlyDisplayingPattern = true;
	private int currentSolveIndex = 0; //Temporary variable for storing the index that the player is solving for in the pattern.
	public Material errorMaterial;

	[Space(10)]

	// UI sign variables
	[Header("UI Signs")]
	public GameObject startSignModel;
	public GameObject startUI, restartUI;
	public Animator SignAnimatorStart;
	public Animator SignAnimatorEnd;

	[Space(10)]

	// Audio variables
	[Header("Audio")]
	public GameObject failAudioHolder;
	public GameObject puzzleStartingAudioHolder;
	public GameObject puzzleWinAudioHolder;


	// Use this for initialization
	void Start () {

		puzzleOrder = new int[puzzleLength]; //Set the size of our array to the declared puzzle length
		generatePuzzleSequence (); //Generate the puzzle sequence for this playthrough.  

		// places the player at the starting waypoint
		player.transform.position = startPoint.transform.position;
	}

	// Update is called once per frame
	void Update () {


//		if (Input.GetMouseButtonDown (0) && player.transform.position==playPoint.transform.position) {
//			puzzleSuccess ();
//		}
	}

	public void playerSelection(GameObject sphere) {
		if(playerWon != true) { //If the player hasn't won yet
			int selectedIndex=0;
			//Get the index of the selected object
			for (int i = 0; i < puzzleSpheres.Length; i++) { //Go through the puzzlespheres array
				if(puzzleSpheres[i] == sphere) { //If the object we have matches this index, we're good
					Debug.Log("Looks like we hit sphere: " + i);
					selectedIndex = i;
				}
			}
			solutionCheck (selectedIndex);//Check if it's correct
		}
	}

	public void solutionCheck(int playerSelectionIndex) { //We check whether or not the passed index matches the solution index
		if (playerSelectionIndex == puzzleOrder [currentSolveIndex]) { //Check if the index of the object the player passed is the same as the current solve index in our solution array
			currentSolveIndex++;
			Debug.Log ("Correct!  You've solved " + currentSolveIndex + " out of " + puzzleLength);
			if (currentSolveIndex >= puzzleLength) {
				puzzleSuccess ();
			}
		} else {
			puzzleFailure ();
		}

	}

	public void startPuzzle() { //Begin the puzzle sequence
		
		//Generate a random number one through five, save it in an array.  Do this n times.
		//Step through the array for displaying the puzzle, and checking puzzle failure or success.

		toggleUIStart(); 
		eventSystem.SetActive(false);
			
		iTween.MoveTo (player, 
			iTween.Hash (
				"delay", 0.9,
				"position", playPoint.transform.position, 
				"time", timeToGameplay, 
				"easetype", "linear",
				"onComplete", "playStartPuzzleAudio",
				"oncompletetarget", this.gameObject
			)
		);
			
		CancelInvoke ("displayPattern");

		InvokeRepeating("displayPattern", 9, puzzleSpeed); //Start running through the displaypattern function
		currentSolveIndex = 0; //Set our puzzle index at 0

	}

	void displayPattern() { //Invoked repeating.
		currentlyDisplayingPattern = true; //Let us know were displaying the pattern
		eventSystem.SetActive(false); //Disable gaze input events while we are displaying the pattern.

		if (currentlyDisplayingPattern == true) { //If we are not finished displaying the pattern
			if (currentDisplayIndex < puzzleOrder.Length) { //If we haven't reached the end of the puzzle
				Debug.Log (puzzleOrder[currentDisplayIndex] + " at index: " + currentDisplayIndex); 
				puzzleSpheres [puzzleOrder [currentDisplayIndex]].GetComponent<lightUp> ().patternLightUp (puzzleSpeed); //Light up the sphere at the proper index.  For now we keep it lit up the same amount of time as our interval, but could adjust this to be less.
				currentDisplayIndex++; //Move one further up.
			} else {
				Debug.Log ("End of puzzle display");
				currentlyDisplayingPattern = false; //Let us know were done displaying the pattern
				currentDisplayIndex = 0;
				CancelInvoke(); //Stop the pattern display.  May be better to use coroutines for this but oh well
				eventSystem.SetActive(true); //Enable gaze input when we aren't displaying the pattern.
			}
		}
	}

	public void generatePuzzleSequence() {

		int tempReference;
		for (int i = 0; i < puzzleLength; i++) { //Do this as many times as necessary for puzzle length
			tempReference = Random.Range(0, puzzleSpheres.Length); //Generate a random reference number for our puzzle spheres
			puzzleOrder [i] = tempReference; //Set the current index to our randomly generated reference number
		}
	}

	public void resetPuzzle() { //Reset the puzzle sequence
		//player.transform.position = startPoint.transform.position;
		iTween.MoveTo (player, 
			iTween.Hash (
				"position", startPoint.transform.position, 
				"time", 4, 
				"easetype", "linear",
				"oncomplete", "resetGame", 
				"oncompletetarget", this.gameObject
			)
		);
		toggleUIEnd ();
	}

	public void resetGame() {
		//restartUI.SetActive (false);
		//startUI.SetActive (true);
		toggleUIEnd();
		playerWon = false;
		generatePuzzleSequence (); //Generate the puzzle sequence for this playthrough.  
	}

	public void puzzleSuccess() { //Do this when the player gets it right
		puzzleWinAudioHolder.GetComponent<GvrAudioSource> ().Play ();
		iTween.MoveTo (player, 
			iTween.Hash (
				"position", restartPoint.transform.position, 
				"time", timeToEndScreen, 
				"easetype", "linear"
			)
		);
	}

	public void puzzleFailure() { //Do this when the player gets it wrong
		Debug.Log("You've Failed, Resetting puzzle");

		//makes puzzle spheres turn red
		for (int i = 0; i < puzzleSpheres.Length; i++) { //Go through the puzzlespheres array
			//puzzleSpheres[i].GetComponent<MeshRenderer>().material = errorMaterial;
			iTween.ColorTo (puzzleSpheres[i], 
				iTween.Hash (
					"color", new Color(1, 0, 0), 
					"time", 0.3,
					"oncomplete", "orbsBackToOriginalColor",
					"oncompletetarget", this.gameObject
				)
			);
		}


		failAudioHolder.GetComponent<GvrAudioSource> ().Play ();
		currentSolveIndex = 0;
		startPuzzle ();

	}

	public void finishingFlourish() { //A nice visual flourish when the player wins
		//this.GetComponent<AudioSource>().Play(); //Play the success audio
		//restartUI.SetActive (true);
		puzzleWinAudioHolder.GetComponent<GvrAudioSource> ().Play ();
		toggleUIStart();
		playerWon = true;

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

	public void playStartPuzzleAudio() {
		Debug.Log ("Puzzle will now begin");
		puzzleStartingAudioHolder.GetComponent<GvrAudioSource> ().Play ();
	}

	public void orbsBackToOriginalColor(){
		for (int i = 0; i < puzzleSpheres.Length; i++) {
			iTween.ColorTo (puzzleSpheres [i], 
				iTween.Hash (
					"color", new Color (1, 1, 1), 
					"time", 0.3
				)
			);
		}
		
	}

}
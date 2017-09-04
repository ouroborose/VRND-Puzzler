using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbFloating : MonoBehaviour {

	public AnimationCurve OrbFloatCurve;

	public float yPosOriginal;

	// Use this for initialization
	void Start () {
		yPosOriginal = transform.localPosition.y;
//		Debug.Log (yPosOriginal);
	}
	
	// Update is called once per frame
	void Update () {

		transform.position = new Vector3 (transform.position.x, yPosOriginal + OrbFloatCurve.Evaluate (Time.time % OrbFloatCurve.length), transform.position.z);

	}
}

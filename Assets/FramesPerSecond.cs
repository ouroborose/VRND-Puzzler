using UnityEngine;
using System.Collections;
using UnityEngine.UI;
 
public class FramesPerSecond : MonoBehaviour 
{
	int fps;
	float time = 1.0f;
	Text text;
	void Start()
	{
		text = GetComponent<Text>();
	}
 
	void Update()
	{
		fps++;
		time -= Time.deltaTime;
		if(time < 0.0f)
		{
			text.text = string.Format("{0} fps", fps);
			
			if(fps < 30)
				text.color = Color.yellow;
			else if(fps < 10)
				text.color = Color.red;
			else
				text.color = Color.green;

			time = 1.0f;
			fps = 0;
		}
		
			
	}
}
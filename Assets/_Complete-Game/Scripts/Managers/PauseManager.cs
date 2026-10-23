using UnityEngine;
using System.Collections;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class PauseManager : MonoBehaviour {

	Canvas canvas;

	void Start()
	{
		canvas = GetComponent<Canvas>();
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			canvas.enabled = !canvas.enabled;
			Pause();
		}
	}

	public void Pause()
	{
		if (Time.timeScale == 0)
		{
			Time.timeScale = 1;
			AudioListener.pause = false;
		}
		else
		{
			Time.timeScale = 0;
			AudioListener.pause = true;
		}
	}

	public void Quit()
	{
		#if UNITY_EDITOR 
		EditorApplication.isPlaying = false;
		#else 
		Application.Quit();
		#endif
	}
}

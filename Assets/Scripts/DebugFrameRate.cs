using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugFrameRate : MonoBehaviour
{
	public static DebugFrameRate debug;

	private Text fpsText;
	private float fps = 0;
    
    void Awake()
    {
		if (debug == null)
		{
			DontDestroyOnLoad(gameObject);
			debug = this;
		}
		else if (debug != this)
		{
			Destroy(gameObject);
		}
		fpsText = GetComponentInChildren<Text>();
    }

    void Update()
    {
		fps = 1f / Time.deltaTime;
		fpsText.text = "PRE-ALPHA | FPS  " + fps.ToString("F0");
    }
}

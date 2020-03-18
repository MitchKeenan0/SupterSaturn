using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugFrameRate : MonoBehaviour
{
	private Text fpsText;
	float fps = 0;
    
    void Awake()
    {
		fpsText = GetComponentInChildren<Text>();
		DontDestroyOnLoad(transform.gameObject);
    }

    void Update()
    {
		fps = 1f / Time.deltaTime;
		fpsText.text = "PRE-ALPHA                FPS  " + fps.ToString("F0");
    }
}

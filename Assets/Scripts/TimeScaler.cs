using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeScaler : MonoBehaviour
{
	public float warmupTimescale = 10f;
	public float warmupDuration = 1f;

	private float timescale = 1f;

    void Start()
    {
		timescale = warmupTimescale;
		Time.timeScale = warmupTimescale;
    }
	
    void Update()
    {
		if (timescale > 1f)
		{
			float ease = Mathf.Clamp((warmupDuration - Time.unscaledTime), 0.1f, 1f);
			timescale = Mathf.Lerp(timescale, 1f, Time.unscaledTime * ease);
			Time.timeScale = timescale;
		}
    }
}

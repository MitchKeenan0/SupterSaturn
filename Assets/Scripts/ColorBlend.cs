using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorBlend : MonoBehaviour
{
	public float speed = 0.1f;
	public float cycleInterval = 10f;
	public float intervalDeviation = 1f;
	public Color startColor;
	public Color[] colorArray;

	private int colorIndex = 0;
	private LineRenderer myRenderer;
	private float startTime = 0f;
	private float cycleTime = 0f;
	private float t = 0f;
	private Color targetColor;

	public Color MyColor() { return targetColor; }

    void Start()
    {
		myRenderer = GetComponent<LineRenderer>();
		startTime = Time.time;
		
		cycleInterval += Random.Range(-intervalDeviation, intervalDeviation);

		int randomIndex = Random.Range(0, colorArray.Length);
		colorIndex = randomIndex;
		Color c = colorArray[colorIndex];
		targetColor = c;
		myRenderer.startColor = targetColor;
		myRenderer.endColor = targetColor;
	}

	void Update()
	{
		if (cycleTime >= cycleInterval)
		{
			targetColor = CycleColor();
			cycleTime = 0f;
			t = 0f;
		}
		else
		{
			cycleTime += Time.deltaTime;
			t = (Time.time - startTime) * speed;
			myRenderer.startColor = Color.Lerp(startColor, targetColor, t);
			myRenderer.endColor = myRenderer.startColor;
		}
	}

	Color CycleColor()
	{
		int index = colorIndex;
		if (index >= colorArray.Length)
			colorIndex = index = 0;

		Color nextColor = colorArray[index];
		colorIndex++;
		return nextColor;
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Prediction : MonoBehaviour
{
	public float predictionOpacity = 0.5f;
	public Spacecraft spacecraft;
	public Image img;
	public Vector3 position = Vector3.zero;
	public Vector3 velocity = Vector3.zero;

	private bool bEnabled = false;
	private LineRenderer velocityLine;

	void Start()
	{
		velocityLine = GetComponent<LineRenderer>();
		img = GetComponent<Image>();
		EnablePrediction(false);
	}

	void EnablePrediction(bool value)
	{
		bEnabled = value;
		if (spacecraft != null)
			spacecraft.enabled = value;
		if (velocityLine != null)
			velocityLine.enabled = value;
		if (img != null)
			img.enabled = value;
	}

	void UpdateVelocityLine()
	{
		velocityLine.SetPosition(0, position);
		velocityLine.SetPosition(1, position + velocity);
	}

	public void SetPrediction(Spacecraft sp, Vector3 pos, Vector3 vel)
	{
		spacecraft = sp;
		position = pos;
		velocity = vel;
		bool enableValue = (position != Vector3.zero);
		if (enableValue != bEnabled)
			EnablePrediction(enableValue);
		if (enableValue)
		{
			UpdateVelocityLine();
			img.sprite = sp.craftIcon;
		}
	}
}

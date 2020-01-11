using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Prediction : MonoBehaviour
{
	public float predictionOpacity = 0.5f;
	public Spacecraft spacecraft;
	public Image img;
	public Transform targetTransform;
	public Vector3 position = Vector3.zero;
	public Vector3 velocity = Vector3.zero;

	private bool bEnabled = false;
	private LineRenderer velocityLine;

	void Start()
	{
		targetTransform.SetParent(null);
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

	public void SetPrediction(Spacecraft sp, Vector3 pos, Vector3 vel, float period)
	{
		spacecraft = sp;
		position = pos;
		velocity = vel * period;

		if ((sp != null) && (pos != Vector3.zero))
		{
			targetTransform.position = pos + (vel * period);
			targetTransform.name = sp.transform.name + " Prediction";
		}
		else
		{
			targetTransform.position = Vector3.zero;
			targetTransform.name = "TargetTransform";
		}

		bool enableValue = (pos != Vector3.zero);
		EnablePrediction(enableValue);

		if (enableValue)
		{
			UpdateVelocityLine();
			img.sprite = sp.craftIcon;
		}
	}
}

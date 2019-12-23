using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseCommandHUD : MonoBehaviour
{
	// circle rendering
	[Range(0.1f, 100f)]
	public float		radius = 1.0f;
	[Range(3, 256)]
	public int			numSegments = 128;

	private MouseSelection mouseSelection;
	private MouseContextHUD mouseContext;
	private Camera cameraMain;
	private LineRenderer lineRenderer;
	private List<Spacecraft> selectedSpacecraftList;
	private bool bEnabled = true;

	void Start()
	{
		mouseSelection = FindObjectOfType<MouseSelection>();
		mouseContext = FindObjectOfType<MouseContextHUD>();
		cameraMain = Camera.main;
		lineRenderer = GetComponentInChildren<LineRenderer>();
		lineRenderer.positionCount = numSegments + 1;
		selectedSpacecraftList = new List<Spacecraft>();
	}

	void Update()
	{
		if (Input.GetButtonDown("Fire2"))
		{
			SetEnabled(true);
		}
		if (Input.GetButton("Fire2"))
		{
			UpdateCircleRender();
		}
		if (Input.GetButtonUp("Fire2"))
		{
			if (mouseContext.GetSpacecraftInformation() != null)
				SetFollowOrder(mouseContext.GetSpacecraftInformation().transform);
			else
				SetMoveOrder(Input.mousePosition);
			SetEnabled(false);
		}
	}

	void SetMoveOrder(Vector3 position)
	{
		selectedSpacecraftList = mouseSelection.GetSelectedSpacecraft();
		Vector3 screenWorldPosition = cameraMain.ScreenToWorldPoint(position) + cameraMain.transform.forward * 100f;
		foreach(Spacecraft sp in selectedSpacecraftList)
		{
			sp.GetAgent().SetMoveOrder(screenWorldPosition, null);
		}
	}

	void SetFollowOrder(Transform value)
	{
		selectedSpacecraftList = mouseSelection.GetSelectedSpacecraft();
		foreach (Spacecraft sp in selectedSpacecraftList)
		{
			sp.GetAgent().SetMoveOrder(value.position, value);
		}
	}

	public void SetEnabled(bool value)
	{
		bEnabled = value;
		lineRenderer.enabled = value;
	}

	public void UpdateCircleRender()
	{
		

		float deltaTheta = (float)(2.0 * Mathf.PI) / numSegments;
		float theta = 0f;

		for (int i = 0; i < numSegments + 1; i++)
		{
			float x = radius * Mathf.Cos(theta);
			float z = radius * Mathf.Sin(theta);
			Vector3 pos = new Vector3(x, 0, z);
			lineRenderer.SetPosition(i, pos);
			theta += deltaTheta;
		}
	}
}

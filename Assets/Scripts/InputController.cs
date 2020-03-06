using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InputController : MonoBehaviour
{
	public Text statusText;
	public Button selectionModeButton;
	public Button navigationModeButton;
	public Button cameraModeButton;
	public Color standbyColor;
	public Color activeColor;

	private MouseSelection mouseSelection;
	private CameraTargetFinder cameraTargetFinder;
	private bool bBoxDragMode = false;
	private bool bNavigationMode = false;
	private bool bCameraMode = false;

	void Start()
	{
		mouseSelection = FindObjectOfType<MouseSelection>();
		cameraTargetFinder = FindObjectOfType<CameraTargetFinder>();
		mouseSelection.bBoxDragMode = bBoxDragMode;
		statusText.text = "";
		ActivateButton(selectionModeButton, false, true);
		ActivateButton(navigationModeButton, false, true);
		ActivateButton(cameraModeButton, false, true);
	}

	void ActivateButton(Button button, bool value, bool finished)
	{
		ColorBlock buttonColor = button.colors;
		if (value && !finished)
			buttonColor.normalColor = activeColor;
		else
			buttonColor.normalColor = standbyColor;
		button.colors = buttonColor;

		if (finished)
			EventSystem.current.SetSelectedGameObject(null);
	}

	void UpdateStatusText()
	{
		if (bBoxDragMode)
			statusText.text = "SELECTION MODE ENABLED";
		else if (bNavigationMode)
			statusText.text = "NAVIGATION MODE ENABLED";
		else if (bCameraMode)
			statusText.text = "DIRECT MODE ENABLED";
		else
			statusText.text = "";
	}

	public void SelectionMode()
	{
		if (bNavigationMode)
			NavigationMode();
		bool turningOff = bBoxDragMode;
		bBoxDragMode = !bBoxDragMode;
		mouseSelection.bBoxDragMode = bBoxDragMode;
		ActivateButton(selectionModeButton, bBoxDragMode, turningOff);
		UpdateStatusText();
	}

	public void NavigationMode()
	{
		if (bBoxDragMode)
			SelectionMode();
		bool bTurningOff = bNavigationMode;
		bNavigationMode = !bNavigationMode;
		List<Spacecraft> selectedSpacecraft = mouseSelection.GetSelectedSpacecraft();
		foreach (Spacecraft sp in selectedSpacecraft)
			sp.GetComponentInChildren<SpacecraftController>().SetActive(bNavigationMode);
		ActivateButton(navigationModeButton, bNavigationMode, bTurningOff);
		UpdateStatusText();
	}

	public void CameraMode()
	{
		bool turningOff = bCameraMode;
		bCameraMode = !bCameraMode;
		if (bCameraMode)
			cameraTargetFinder.GetFirstTarget();
		else
			cameraTargetFinder.SetActive(false);
		ActivateButton(cameraModeButton, bCameraMode, turningOff);
	}

	public void IncreaseOrbit(float value)
	{
		List<Spacecraft> selectedSpacecraft = mouseSelection.GetSelectedSpacecraft();
		foreach (Spacecraft sp in selectedSpacecraft)
		{
			sp.GetComponent<Autopilot>().IncreaseOrbit(value);
		}
	}

	public void DecreaseOrbit(float value)
	{
		List<Spacecraft> selectedSpacecraft = mouseSelection.GetSelectedSpacecraft();
		foreach (Spacecraft sp in selectedSpacecraft)
		{
			sp.GetComponent<Autopilot>().DecreaseOrbit(value);
		}
	}
}

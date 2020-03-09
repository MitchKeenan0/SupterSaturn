using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InputController : MonoBehaviour
{
	public Text statusText;
	public Button navigationModeButton;
	public Button cameraModeButton;
	public Color standbyColor;
	public Color activeColor;

	private MouseSelection mouseSelection;
	private CameraTargetFinder cameraTargetFinder;
	private TouchOrbit touchOrbit;
	private bool bNavigationMode = false;
	private bool bCameraMode = false;

	void Start()
	{
		mouseSelection = FindObjectOfType<MouseSelection>();
		cameraTargetFinder = FindObjectOfType<CameraTargetFinder>();
		touchOrbit = FindObjectOfType<TouchOrbit>();
		statusText.text = "";
		ActivateButton(navigationModeButton, false, true);
		ActivateButton(cameraModeButton, false, true);
		CameraMode(true);
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
		if (bNavigationMode)
			statusText.text = "Navigation Mode Enabled";
		else if (bCameraMode)
			statusText.text = "Camera Mode Enabled";
		else
			statusText.text = "";
	}

	void FallbackCheck()
	{
		if (!bNavigationMode && !bCameraMode)
			CameraMode(true);
	}

	public void NavigationMode(bool active)
	{
		if (bCameraMode)
			CameraMode(false);
		bool bTurningOff = bNavigationMode;
		bNavigationMode = active;
		List<Spacecraft> selectedSpacecraft = mouseSelection.GetSelectedSpacecraft();
		foreach (Spacecraft sp in selectedSpacecraft)
			sp.GetComponentInChildren<SpacecraftController>().SetActive(bNavigationMode);
		ActivateButton(navigationModeButton, bNavigationMode, bTurningOff);
		UpdateStatusText();
		FallbackCheck();
		Debug.Log("NavigationMode " + bNavigationMode);
	}

	public void CameraMode(bool active)
	{
		if (bNavigationMode)
			NavigationMode(false);
		bool bTurningOff = bCameraMode;
		bCameraMode = active;
		if (bCameraMode)
			cameraTargetFinder.GetFirstTarget();
		else
			cameraTargetFinder.SetActive(false);
		touchOrbit.SetActive(bCameraMode);
		ActivateButton(cameraModeButton, bCameraMode, bTurningOff);
		UpdateStatusText();
		Debug.Log("CameraMode " + bCameraMode);
	}
}

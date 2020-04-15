﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InputController : MonoBehaviour
{
	public Text statusText;
	public Button navigationModeButton;
	public Button cameraModeButton;
	public Button stopButton;
	public Color standbyColor;
	public Color activeColor;

	private Game game;
	private Camera cameraMain;
	private CameraTargetFinder cameraTargetFinder;
	private TouchOrbit touchOrbit;
	private OrbitController orbitController;
	private Spacecraft spacecraft = null;
	private ContextHeader contextHeader;
	private SkillPanel skillPanel;
	private float originalCameraScale = 1f;
	private bool bNavigationMode = false;
	private bool bFreeCameraMode = false;
	private bool bStopped = false;
	private int cameraMode = 1;
	private int numCameraModes = 0;
	private List<float> distanceModes;

	public bool IsStopped() { return bStopped; }

	void Awake()
	{
		distanceModes = new List<float>();
	}

	void Start()
	{
		game = FindObjectOfType<Game>();
		cameraMain = Camera.main;
		cameraTargetFinder = FindObjectOfType<CameraTargetFinder>();
		touchOrbit = FindObjectOfType<TouchOrbit>();
		orbitController = FindObjectOfType<OrbitController>();
		contextHeader = FindObjectOfType<ContextHeader>();
		skillPanel = FindObjectOfType<SkillPanel>();
		distanceModes.Add(touchOrbit.distanceMin);
		distanceModes.Add(touchOrbit.distanceMax);
		originalCameraScale = touchOrbit.GetInputScale();
		statusText.text = "";
		numCameraModes = distanceModes.Count;

		ActivateButton(navigationModeButton, false, true);
		ActivateButton(cameraModeButton, false, true);
		ActivateButton(stopButton, false, true);
	}

	public void Begin()
	{
		AllStop(false);
		CameraMode();
		ActivateButton(stopButton, false, true);
	}

	public void ActivateButton(Button button, bool value, bool finished)
	{
		if (finished)
			EventSystem.current.SetSelectedGameObject(null);

		Color color = standbyColor;
		if (value && !finished)
			color = activeColor;
		ColorBlock buttonColorBlock = button.colors;
		buttonColorBlock.normalColor = color;
		buttonColorBlock.highlightedColor = color;
		buttonColorBlock.pressedColor = Color.black;
		buttonColorBlock.selectedColor = color;
		button.colors = buttonColorBlock;

		//if (value)
			//skillPanel.CancelAll();
		if (!value)
			FallbackCheck();
	}

	public void UpdateStatusText(string value)
	{
		statusText.text = value;
	}

	void FallbackCheck()
	{
		if (!bNavigationMode && !bFreeCameraMode)
			EnableCameraControl(true);
	}

	void SetFineCameraInput(bool value)
	{
		if (value)
			touchOrbit.SetInputScale(originalCameraScale * 0.6f);
		else
			touchOrbit.SetInputScale(originalCameraScale);
	}

	public void AllStop(bool bActuallyThough)
	{
		bool bTurningOff = bStopped;
		bStopped = !bStopped;
		if (bActuallyThough)
			bStopped = true;
		bool bEngineShutdown = false;
		if (game.GetSpacecraftList() != null)
		{
			if (bStopped)
			{
				spacecraft = game.GetSpacecraftList()[0];
				Autopilot autopilot = spacecraft.GetComponent<Autopilot>();
				if (autopilot.IsEngineActive())
					bEngineShutdown = true;
				autopilot.AllStop();
			}
			else
			{
				spacecraft = game.GetSpacecraftList()[0];
				Autopilot autopilot = spacecraft.GetComponent<Autopilot>();
				autopilot.ReleaseBrakes();
			}
		}

		if (bStopped)
		{
			if (bEngineShutdown)
				UpdateStatusText("Main engine shutdown");
			else
				UpdateStatusText("All stop");
		}
		else
		{
			UpdateStatusText("Intertial stabilizers off");
			//orbitController.SetUpdating(true);
		}

		if (bEngineShutdown)
		{
			bTurningOff = false;
			bStopped = false;
			orbitController.SetUpdating(false);
		}

		if (bStopped && !bEngineShutdown)
			orbitController.KillOrbit();

		bool bActivated = bStopped || bEngineShutdown;
		ActivateButton(stopButton, bActivated, bTurningOff);
	}

	public void NavigationMode(bool value)
	{
		bool bTurningOff = bNavigationMode;
		if (value == false)
			bTurningOff = true;
		if (bTurningOff)
			value = false;
		
		bNavigationMode = value;
		if (spacecraft == null)
			spacecraft = game.GetSpacecraftList()[0];
		if (spacecraft != null)
			spacecraft.GetComponentInChildren<SpacecraftController>().SetActive(bNavigationMode);

		if (bNavigationMode)
		{
			UpdateStatusText("Navigation mode");
			contextHeader.SetContextHeader("Navigation awaiting input");
		}
		else
		{
			UpdateStatusText("");
			contextHeader.SetContextHeader("");
		}

		ActivateButton(navigationModeButton, bNavigationMode, bTurningOff);
	}

	public void CameraMode()
	{
		EnableCameraControl(true);

		if (numCameraModes > 0)
		{
			cameraTargetFinder.SetActive(true);
			float dist = distanceModes[cameraMode];
			bool bFar = (cameraMode == 0) ? false : true;
			spacecraft.SetBigGhostMode(bFar);
			touchOrbit.SetDistance(dist);
			///cameraMain.focalLength = 1f / (Mathf.Sqrt(dist) * 0.1f) * 6f;
			cameraMode++;
			if (cameraMode >= numCameraModes)
				cameraMode = 0;
			UpdateStatusText("Camera range " + dist + " km");
		}
	}

	public void CameraPlanet()
	{

	}

	public void EnableCameraControl(bool value)
	{
		bFreeCameraMode = value;
		if (!touchOrbit)
			touchOrbit = FindObjectOfType<TouchOrbit>();
		touchOrbit.SetActive(bFreeCameraMode);
		UpdateStatusText("Free camera " + (value ? ("on") : ("off")));
	}

	public void SetCameraTarget(Transform target)
	{
		cameraTargetFinder.SetTarget(target);
	}
}

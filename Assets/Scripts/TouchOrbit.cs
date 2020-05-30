﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchOrbit : MonoBehaviour
{
	public Transform orbitAnchor;
	public Transform focusTransform = null;
	public float distance = 5.0f;
	public float xSpeed = 120.0f;
	public float ySpeed = 120.0f;
	public float mousePanSpeed = 0.01f;

	public float yMinLimit = -20f;
	public float yMaxLimit = 80f;

	public float distanceMin = 0.5f;
	public float distanceMax = 15f;

	public float orbitSpeed = 10f;
	public float moveAcceleration = 10f;
	public float turnAcceleration = 10f;

	public Vector3 startRotationVector;
	public float offsetX = 0f;
	public float offsetY = 0f;

	private CameraController cameraController;
	private InputController inputController;
	private Camera cameraMain;
	private LocusHud locusHud;
	private Vector3 moveInput = Vector3.zero;
	private Vector3 orbitOffset = Vector3.zero;
	private float lx = 0f;
	private float ly = 0f;
	private float x = 0f;
	private float y = 0f;
	private float twoTouchDistance = 0;
	private float timeAtLastTargeting = 0f;
	private bool bActivated = true;
	private Transform anchorTransform = null;

	public float GetInputScale() { return orbitSpeed; }
	public void SetInputScale(float value) { orbitSpeed = value; }
	public void SetDistance(float value) { distance = value; }

	void Awake()
	{
		ResetAnchor(false);
		cameraController = FindObjectOfType<CameraController>();
		cameraMain = Camera.main;
		inputController = FindObjectOfType<InputController>();
		locusHud = FindObjectOfType<LocusHud>();
		Vector3 angles = transform.eulerAngles;
		x = angles.y;
		y = angles.x;
		distance = distanceMax;
		this.enabled = bActivated;
		transform.rotation = Quaternion.LookRotation(startRotationVector);
		if (orbitAnchor != null) 
			SetFocusTransform(orbitAnchor);
	}

	void Update()
	{
		UpdateTouchControl();
		///InputOrbit();
	}

	void UpdateTouchControl()
	{
		bool bTouching = false;
		if (Input.touchCount > 0)
		{
			// move input
			if (Input.touchCount == 1)
			{
				Touch touch = Input.GetTouch(0);
				if (touch.phase == TouchPhase.Moved)
				{
					moveInput = touch.deltaPosition;
				}
				if (moveInput.magnitude > 5f)
				{
					bTouching = true;
				}
			}

			// camera distance scoping
			else if (Input.touchCount == 2)
			{
				Vector3 deltaOne = Input.GetTouch(0).position;
				Vector3 deltaTwo = Input.GetTouch(1).position;
				float touchDistance = Vector3.Distance(deltaOne, deltaTwo);
				float targetDistance = distance;
				if (touchDistance > twoTouchDistance)
					targetDistance *= 0.6f;
				else if (touchDistance < twoTouchDistance)
					targetDistance *= 1.6f;
				twoTouchDistance = touchDistance;
				distance = Mathf.Lerp(distance, targetDistance, Time.deltaTime * orbitSpeed);
			}
		}
		else
		{
			float deltaT = Time.deltaTime * moveAcceleration;
			moveInput = Vector3.Lerp(moveInput, Vector3.zero, deltaT);
		}

		// target box switch on/off
		float targetBoxMinimumUpdateTime = 0.5f;
		if (((Time.timeSinceLevelLoad - timeAtLastTargeting) >= targetBoxMinimumUpdateTime)
			|| bTouching)
		{
			locusHud.SetTargetBoxEnabled(bTouching);
			timeAtLastTargeting = Time.timeSinceLevelLoad;
		}
	}

	void LateUpdate()
	{
		InputOrbit();
	}

	void InputOrbit()
	{
		Quaternion rotation = Quaternion.identity;
		Vector3 position = orbitAnchor.transform.position;

		if (anchorTransform != null)
		{
			if (((lx != 0f) || (ly != 0f)) && (moveInput.magnitude > 0.5f))
			{
				lx = Mathf.MoveTowards(lx, moveInput.x * 0.5f * xSpeed, Time.deltaTime * turnAcceleration * orbitSpeed);
				ly = Mathf.MoveTowards(ly, moveInput.y * 0.5f * ySpeed, Time.deltaTime * turnAcceleration * orbitSpeed);
				x += lx;
				y -= ly;
			}
			else if (moveInput.magnitude < 0.05f)
			{
				if (focusTransform != null)
					LookAtFocus();
			}

			y = Mathf.Clamp(y, -85f, 85f);
			rotation *= Quaternion.Euler(y, x, 0);
			Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);
			position = (rotation * negDistance) + anchorTransform.position;
		}

		Vector3 offset = ((transform.right * offsetX) + (transform.up * offsetY)) * Mathf.Abs(distance);
		transform.position = position + offset;

		transform.rotation = rotation;
	}

	void LookAtFocus()
	{
		Vector3 localForward = cameraMain.transform.forward;
		Vector3 localRight = cameraMain.transform.right;
		Vector3 localUp = cameraMain.transform.up;
		Vector3 toFocus = (focusTransform.position - cameraMain.transform.position);

		moveInput.x = Vector3.Dot(localRight, toFocus);
		moveInput.y = Vector3.Dot(localUp, toFocus);
		float targetX = moveInput.x * 0.05f * xSpeed;
		float targetY = moveInput.y * 0.05f * ySpeed;
		float deltaT = Time.deltaTime * turnAcceleration;
		lx = targetX;
		ly = targetY;
		x += lx;
		y -= ly;
	}

	public void SetFocusTransform(Transform value)
	{
		focusTransform = value;
	}

	public void ResetAnchor(bool overrideDistance)
	{
		if (orbitAnchor != null)
			anchorTransform = orbitAnchor;
		anchorTransform.transform.SetParent(null);
		anchorTransform.transform.position = Vector3.zero;
		if (overrideDistance)
		{
			float currentDistance = Vector3.Distance(cameraMain.transform.position, Vector3.zero);
			distance = currentDistance * 1.2f;
		}
	}

	public void SetActive(bool value)
	{
		bActivated = value;
		this.enabled = value;
	}

	public void SetAnchorTransform(Transform value)
	{
		anchorTransform = value;
		distance = distanceMax - distanceMin;
		transform.position = value.position;
		transform.rotation = Quaternion.LookRotation(startRotationVector);
	}

	public void SetScoped(bool value)
	{
		float focal = value ? 24.78f : 8.26f;
		cameraMain.focalLength = focal;
	}
}

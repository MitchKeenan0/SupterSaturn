﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchOrbit : MonoBehaviour
{
	public Transform orbitAnchor;
	public float distance = 5.0f;
	public float xSpeed = 120.0f;
	public float ySpeed = 120.0f;
	public float mousePanSpeed = 0.01f;

	public float yMinLimit = -20f;
	public float yMaxLimit = 80f;

	public float distanceMin = 0.5f;
	public float distanceMax = 15f;

	public float moveSpeed = 10f;
	public float moveAcceleration = 10f;
	public float turnAcceleration = 10f;

	private CameraController cameraController;
	private InputController inputController;
	private Camera cameraMain;
	private Vector3 moveInput = Vector3.zero;
	private Vector3 moveVector = Vector3.zero;
	private float inputX = 0f;
	private float inputZ = 0f;
	private float lx = 0f;
	private float ly = 0f;
	private float x = 0f;
	private float y = 0f;
	private float twoTouchDistance = 0;
	private bool bActivated = true;

	void Awake()
	{
		orbitAnchor.transform.SetParent(null);
		orbitAnchor.transform.position = Vector3.zero;
		cameraController = FindObjectOfType<CameraController>();
		cameraMain = Camera.main;
		inputController = FindObjectOfType<InputController>();
		Vector3 angles = transform.eulerAngles;
		x = angles.y;
		y = angles.x;
		distance = distanceMax;
		this.enabled = bActivated;
	}

	void Update()
	{
		MoveInput();
	}

	void MoveInput()
	{
		moveInput = Vector3.zero;
		if (Input.touchCount > 0)
		{
			if (Input.touchCount == 1)
			{
				Touch touch = Input.GetTouch(0);
				moveInput = touch.deltaPosition;

				if (moveInput != Vector3.zero)
				{
					inputX = moveInput.x;
					inputZ = moveInput.y;

					Vector3 rawMove = (
						(cameraMain.transform.right * inputX)
						+ (cameraMain.transform.forward * inputZ)
						).normalized * moveSpeed;

					// normalized to anchor surface
					Vector3 orbitNormal = (Vector3.zero - cameraMain.transform.position).normalized;
					rawMove = Vector3.ProjectOnPlane(rawMove, orbitNormal);

					// move interply
					moveVector = Vector3.Lerp(moveVector, rawMove, Time.deltaTime * moveAcceleration * 0.1f);
					moveVector = Vector3.ClampMagnitude(moveVector, moveSpeed);
				}
			}
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
				distance = Mathf.Lerp(distance, targetDistance, Time.deltaTime * moveSpeed);
			}
		}
		else
		{
			moveVector = Vector3.Lerp(moveVector, Vector3.zero, Time.deltaTime * moveAcceleration);
		}
	}

	void LateUpdate()
	{
		Quaternion rotation = Quaternion.identity;
		Vector3 position = Vector3.zero;

		if (orbitAnchor != null)
		{
			if (bActivated || (lx != 0f) || (ly != 0f))
			{
				lx = Mathf.Lerp(lx, moveInput.x * 0.05f * xSpeed, Time.deltaTime * turnAcceleration);
				ly = Mathf.Lerp(ly, moveInput.y * 0.05f * ySpeed, Time.deltaTime * turnAcceleration);
				x += lx;
				y -= ly;
			}

			rotation *= Quaternion.Euler(y, x, 0);
			Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);
			position = (rotation * negDistance) + orbitAnchor.position;
		}

		transform.rotation = rotation;
		transform.position = position + moveVector;
	}

	public void SetActive(bool value)
	{
		bActivated = value;
		this.enabled = value;
	}
}
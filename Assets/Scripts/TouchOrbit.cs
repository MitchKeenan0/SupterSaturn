using System.Collections;
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
	private Transform anchorTransform = null;

	public float GetInputScale() { return moveSpeed; }
	public void SetInputScale(float value) { moveSpeed = ySpeed = value; }
	public void SetDistance(float value) { distance = value; }

	void Awake()
	{
		ResetAnchor(false);
		cameraController = FindObjectOfType<CameraController>();
		cameraMain = Camera.main;
		inputController = FindObjectOfType<InputController>();
		Vector3 angles = transform.eulerAngles;
		x = angles.y;
		y = angles.x;
		distance = distanceMax;
		this.enabled = bActivated;
		//moveInput = new Vector3(-150f, -150f, 0f);
	}

	void Update()
	{
		UpdateTouchControl();
	}

	void UpdateTouchControl()
	{
		if (Input.touchCount > 0)
		{
			// move input
			if (Input.touchCount == 1)
			{
				Touch touch = Input.GetTouch(0);
				moveInput = touch.deltaPosition;
				if (moveInput != Vector3.zero)
				{
					inputX = moveInput.x;
					inputZ = moveInput.y;
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
				distance = Mathf.Lerp(distance, targetDistance, Time.deltaTime * moveSpeed);
			}
		}
		else
		{
			moveVector = Vector3.Lerp(moveVector, Vector3.zero, Time.deltaTime * moveAcceleration);
			moveInput = Vector3.Lerp(moveInput, Vector3.zero, Time.deltaTime * moveAcceleration);
		}
	}

	void LateUpdate()
	{
		Quaternion rotation = Quaternion.identity;
		Vector3 position = orbitAnchor.transform.position;

		if (anchorTransform != null)
		{
			if (bActivated || (lx != 0f) || (ly != 0f))
			{
				lx = Mathf.Lerp(lx, moveInput.x * 0.05f * xSpeed, Time.deltaTime * turnAcceleration);
				ly = Mathf.Lerp(ly, moveInput.y * 0.05f * ySpeed, Time.deltaTime * turnAcceleration);
				x += lx;
				y -= ly;
			}

			y = Mathf.Clamp(y, -85f, 85f);
			rotation *= Quaternion.Euler(y, x, 0);
			Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);
			position = (rotation * negDistance) + anchorTransform.position;
		}

		transform.rotation = rotation;
		transform.position = position; ///Vector3.MoveTowards(transform.position, position + moveVector, Time.deltaTime * moveAcceleration * moveSpeed);
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
	}

	public void SetScoped(bool value)
	{
		float focal = value ? 24.78f : 8.26f;
		cameraMain.focalLength = focal;
	}

	public void SetTarget(Transform target)
	{

	}
}

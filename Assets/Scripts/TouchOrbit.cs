using System.Collections;
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
	private Vector3 moveInput = Vector3.zero;
	private Vector3 moveVector = Vector3.zero;
	private Vector3 orbitOffset = Vector3.zero;
	private float inputX = 0f;
	private float inputZ = 0f;
	private float lx = 0f;
	private float ly = 0f;
	private float x = 0f;
	private float y = 0f;
	private float twoTouchDistance = 0;
	private bool bActivated = true;
	private bool bMoveMode = true;
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
		Vector3 angles = transform.eulerAngles;
		x = angles.y;
		y = angles.x;
		distance = distanceMax;
		this.enabled = bActivated;
		transform.rotation = Quaternion.LookRotation(startRotationVector);
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
				if (touch.phase == TouchPhase.Began)
				{
					moveInput = Vector3.zero;
					if (moveInput != Vector3.zero)
					{
						inputX = moveInput.x;
						inputZ = moveInput.y;
					}
				}
				else if (touch.phase == TouchPhase.Moved)
				{
					Vector3 deltaTouch = touch.deltaPosition;
					moveInput += deltaTouch;
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
			moveVector = Vector3.Lerp(moveVector, Vector3.zero, Time.deltaTime * moveAcceleration);
			moveInput = Vector3.Lerp(moveInput, Vector3.zero, Time.deltaTime * moveAcceleration);
		}
	}

	void LateUpdate()
	{
		if (bMoveMode)
			InputOrbit();
		else
			IdleFollow();
	}

	void IdleFollow()
	{
		Vector3 followPosition = anchorTransform.position + (cameraMain.transform.forward * -distance);
		Vector3 offset = (cameraMain.transform.right * inputX) + (cameraMain.transform.forward * inputZ);
		offset.y = 0f;
		transform.position = followPosition + offset;
	}

	void InputOrbit()
	{
		Quaternion rotation = Quaternion.identity;
		Vector3 position = orbitAnchor.transform.position;

		if (anchorTransform != null)
		{
			if ((lx != 0f) || (ly != 0f))
			{
				lx = Mathf.Lerp(lx, moveInput.x * 0.05f * xSpeed, Time.deltaTime * turnAcceleration * orbitSpeed);
				ly = Mathf.Lerp(ly, moveInput.y * 0.05f * ySpeed, Time.deltaTime * turnAcceleration * orbitSpeed);
				x += lx;
				y -= ly;
			}
			else if (focusTransform != null)
			{
				LookAtFocus();
			}

			y = Mathf.Clamp(y, -85f, 85f);
			rotation *= Quaternion.Euler(y, x, 0);
			Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);
			position = (rotation * negDistance) + anchorTransform.position;
		}

		transform.rotation = rotation;

		Vector3 offset = ((transform.right * offsetX) + (transform.up * offsetY)) * Mathf.Abs(distance);
		transform.position = position + offset; ///Vector3.MoveTowards(transform.position, position + moveVector, Time.deltaTime * moveAcceleration * moveSpeed);
	}

	void LookAtFocus()
	{
		Vector3 localForward = cameraMain.transform.forward;
		Vector3 localRight = cameraMain.transform.right;
		Vector3 localUp = cameraMain.transform.up;
		Vector3 toFocus = (focusTransform.position - cameraMain.transform.position);

		moveInput.x = Vector3.Dot(localRight, toFocus);
		moveInput.y = Vector3.Dot(localUp, toFocus);
		lx = Mathf.Lerp(lx, moveInput.x * 0.05f * xSpeed, Time.deltaTime * turnAcceleration);
		ly = Mathf.Lerp(ly, moveInput.y * 0.05f * ySpeed, Time.deltaTime * turnAcceleration);
		x += lx;
		y -= ly;

		Debug.Log("looking at focus");
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

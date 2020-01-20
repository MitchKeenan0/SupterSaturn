using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Camera-Control/Mouse Orbit with zoom")]
public class MouseOrbitImproved : MonoBehaviour
{
	public Transform target;
	public Transform orbitAnchor;
	public float distance = 5.0f;
	public float xSpeed = 120.0f;
	public float ySpeed = 120.0f;
	public float mousePanSpeed = 0.01f;

	public float yMinLimit = -20f;
	public float yMaxLimit = 80f;

	public float distanceMin = .5f;
	public float distanceMax = 15f;

	public float moveSpeed = 10f;
	public float moveAcceleration = 10f;
	public float turnAcceleration = 10f;

	float lx = 0;
	float ly = 0;
	float x = 0.0f;
	float y = 0.0f;

	private float inputX = 0f;
	private float inputZ = 0f;
	private float inputElevation = 0f;
	bool bActivated = false;
	bool bInputting = false;
	bool bEdgeMousing = false;
	Vector3 moveInput = Vector3.zero;
	Vector3 lastMovePosition = Vector3.zero;
	private Vector3 moveVector = Vector3.zero;
	private Vector3 lastWidePosition = Vector3.zero;
	private Quaternion lastWideRotation = Quaternion.identity;
	CameraController cameraController;
	Camera cameraMain;

	// Use this for initialization
	void Start()
	{
		cameraController = FindObjectOfType<CameraController>();
		cameraMain = Camera.main;
		Vector3 angles = transform.eulerAngles;
		x = angles.y;
		y = angles.x;
		target = orbitAnchor;
		distance = distanceMax = orbitAnchor.localPosition.magnitude;
		lastMovePosition = transform.position;
	}

	void Update()
	{
		MoveInput();
	}

	void LateUpdate()
	{
		MouseScreenEdgeInput();
		bActivated = bInputting || Input.GetButton("Fire2");
		if (target != null)
		{
			if (bEdgeMousing || bActivated || ((target != orbitAnchor) && (lx != 0f || ly != 0f)))
			{
				lx = Mathf.Lerp(lx, Input.GetAxis("Mouse X") * 0.05f * xSpeed, Time.deltaTime * turnAcceleration);
				ly = Mathf.Lerp(ly, Input.GetAxis("Mouse Y") * 0.05f * ySpeed, Time.deltaTime * turnAcceleration);
				x += lx;
				y -= ly;

				y = ClampAngle(y, yMinLimit, yMaxLimit);
			}

			Quaternion rotation = Quaternion.Euler(y, x, 0);

			distance = Mathf.Clamp(distance - Input.GetAxis("Mouse ScrollWheel") * 5, distanceMin, distanceMax + 1f);
			if (distance > distanceMax)
			{
				SetOrbitTarget(null);
			}

			Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);
			Vector3 position = (rotation * negDistance) + target.position;

			float lerpSpeed = moveAcceleration;
			if (moveInput == Vector3.zero)
				lerpSpeed *= 10f;

			transform.rotation = rotation;
			transform.position = position;

			transform.position += moveVector;
		}
		else
		{
			SetOrbitTarget(null);
		}
	}

	void MoveInput()
	{
		inputX = Input.GetAxis("Horizontal");
		inputZ = Input.GetAxis("Vertical");
		inputElevation = Input.GetAxis("Elevation");

		bInputting = !((inputX == 0f) && (inputZ == 0f) && (inputElevation == 0f));
		if (bInputting)
		{
			Vector3 rawMove = (
				(cameraMain.transform.right * inputX)
				+ (cameraMain.transform.forward * inputZ)
				).normalized * moveSpeed;
			rawMove = Vector3.ProjectOnPlane(rawMove, Vector3.up);
			rawMove += (cameraMain.transform.up * inputElevation) * moveSpeed;

			moveVector = Vector3.Lerp(moveVector, rawMove, Time.deltaTime * moveAcceleration * 0.1f);
			moveVector = Vector3.ClampMagnitude(moveVector, moveSpeed);
		}
		else
		{
			moveVector = Vector3.Lerp(moveVector, Vector3.zero, Time.deltaTime * moveAcceleration);
		}
	}

	void MouseScreenEdgeInput()
	{
		Vector3 mousePosition = Input.mousePosition;
		bEdgeMousing = false;
		int margin = 3;

		if (mousePosition.x <= 0f + margin)
		{
			//lx += -mousePanSpeed * Time.deltaTime;
			bEdgeMousing = true;
		}
		if (mousePosition.x >= Screen.width - margin)
		{
			//lx += mousePanSpeed * Time.deltaTime;
			bEdgeMousing = true;
		}
		if (mousePosition.y <= 0f + margin)
		{
			//ly += -mousePanSpeed * Time.deltaTime;
			bEdgeMousing = true;
		}
		if (mousePosition.y >= Screen.height - margin)
		{
			//ly += mousePanSpeed * Time.deltaTime;
			bEdgeMousing = true;
		}
	}

	public static float ClampAngle(float angle, float min, float max)
	{
		if (angle < -360F)
			angle += 360F;
		if (angle > 360F)
			angle -= 360F;
		return Mathf.Clamp(angle, min, max);
	}

	public void SetOrbitTarget(Transform t)
	{
		moveVector = Vector3.zero;
		if (t != null)
		{
			target = t;
			distance = distanceMin * 1.6f;
		}
		else
		{
			target = orbitAnchor;
			distance = distanceMax = orbitAnchor.localPosition.magnitude;
		}
	}
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseCommandHUD : MonoBehaviour
{
	// circle rendering
	[Range(0.1f, 100f)]
	public float		circleRadius = 1.0f;
	[Range(-10f, 10f)]
	public float		lineElevation = 0f;
	[Range(3, 256)]
	public int			numCircleSegments = 128;

	public LineRenderer circleLineRenderer;
	public LineRenderer elevationLineRenderer;

	private MouseSelection mouseSelection;
	private MouseContextHUD mouseContext;
	private Camera cameraMain;
	private List<Spacecraft> selectedSpacecraftList;
	private Vector3 moveCommandPosition = Vector3.zero;
	private Vector3 lastMousePosition = Vector3.zero;
	private Vector3 screenCenter = Vector3.zero;
	private bool bEnabled = true;
	private float originalRadius = 0f;
	private float mouseX = 0;
	private float mouseY = 0;

	void Awake()
	{
		cameraMain = Camera.main;
	}

	void Start()
	{
		moveCommandPosition = transform.position;
		mouseSelection = FindObjectOfType<MouseSelection>();
		mouseContext = FindObjectOfType<MouseContextHUD>();
		
		circleLineRenderer.positionCount = numCircleSegments + 1;
		selectedSpacecraftList = new List<Spacecraft>();

		lastMousePosition = Input.mousePosition;
		originalRadius = circleRadius;
	}

	void Update()
	{
		UpdateMouseInput();
	}

	void UpdateMouseInput()
	{
		if (Input.GetButtonDown("Fire2"))
		{
			SetEnabled(true);
			//SetCircleLocation();
		}

		if (Input.GetButton("Fire2"))
		{
			SetCircleLocation();
			UpdateMouseMovement();
			UpdateCircleRender();
			VisualizeMoveOrder(moveCommandPosition);
		}

		if (Input.GetButtonUp("Fire2"))
		{
			if (mouseContext.GetSpacecraftInformation() != null)
				SetFollowOrder(mouseContext.GetSpacecraftInformation().transform);
			else
				SetMoveOrder(moveCommandPosition);
			SetEnabled(false);
		}
	}

	void UpdateMouseMovement()
	{
		Vector3 deltaMouse = (Input.mousePosition - lastMousePosition);
		mouseX = deltaMouse.x;
		mouseY = deltaMouse.y;
		lastMousePosition = Input.mousePosition;

		if (deltaMouse != Vector3.zero)
		{
			screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, deltaMouse.z);

			// command circle width control
			Vector3 flatMousePosition = Input.mousePosition;
			flatMousePosition.y = screenCenter.y;
			float distToCenter = Vector3.Distance(flatMousePosition, screenCenter);

			circleRadius = (distToCenter * originalRadius) / 10f;
		}

		// command line elevation
		Vector3 verticalMousePosition = Input.mousePosition;
		verticalMousePosition.x = screenCenter.x;
		float distToMiddle = Vector3.Distance(verticalMousePosition, screenCenter);

		lineElevation = distToMiddle * (Input.mousePosition - screenCenter).normalized.y / 10f;

		Vector3 elevationOrigin = ClosestLineToMouse();
		Vector3 elevationLine = elevationOrigin + (Vector3.up * lineElevation);

		elevationLineRenderer.SetPosition(0, elevationOrigin);
		elevationLineRenderer.SetPosition(1, elevationLine);

		moveCommandPosition = elevationLine;
	}

	void UpdateCircleRender()
	{
		float deltaTheta = (float)(2.0 * Mathf.PI) / numCircleSegments;
		float theta = 0f;

		for (int i = 0; i < numCircleSegments + 1; i++)
		{
			float x = circleRadius * Mathf.Cos(theta);
			float z = circleRadius * Mathf.Sin(theta);
			Vector3 pos = new Vector3(x, 0, z);
			circleLineRenderer.SetPosition(i, pos);
			theta += deltaTheta;
		}
	}

	void SetCircleLocation()
	{
		selectedSpacecraftList = mouseSelection.GetSelectedSpacecraft();
		Vector3 combinedPositions = Vector3.zero;
		int numSpacecrafts = 0;
		if (selectedSpacecraftList.Count > 0)
		{
			foreach(Spacecraft sp in selectedSpacecraftList)
			{
				combinedPositions += sp.transform.position;
				numSpacecrafts++;
			}
		}
		Vector3 centerPosition = combinedPositions / Mathf.Round(numSpacecrafts);
		if (centerPosition != Vector3.zero && (centerPosition.normalized != Vector3.zero))
			circleLineRenderer.transform.position = centerPosition;
	}

	Vector3 ClosestLineToMouse()
	{
		Vector3 mousePos = Vector3.zero;
		Vector3 closestLinePosition = Vector3.positiveInfinity;
		Vector3[] linePositions = new Vector3[circleLineRenderer.positionCount];
		circleLineRenderer.GetPositions(linePositions);

		float closestDistance = Mathf.Infinity;
		int numPos = linePositions.Length;
		for (int i = 0; i < numPos; i++){
			float dotToLine = Vector3.Dot(cameraMain.transform.forward, (linePositions[i] - cameraMain.transform.position).normalized);
			if (dotToLine > 0.5f)
			{
				Vector3 lineToCamera = cameraMain.transform.position - linePositions[i];
				var mouseRay = cameraMain.ScreenPointToRay(Input.mousePosition);
				mousePos = mouseRay.GetPoint(Mathf.Abs(lineToCamera.magnitude));
				Vector3 mouseToLine = (linePositions[i] - mousePos).normalized;
				mouseToLine.z = 0f;
				mouseToLine.y = 0f;
				float lateralDistance = mouseToLine.x;
				if (Mathf.Abs(lateralDistance) < Mathf.Abs(closestDistance))
				{
					closestLinePosition = linePositions[i];
					closestDistance = lateralDistance;
				}
			}
		}

		return closestLinePosition;
	}

	void VisualizeMoveOrder(Vector3 position)
	{
		selectedSpacecraftList = mouseSelection.GetSelectedSpacecraft();
		foreach (Spacecraft sp in selectedSpacecraftList)
		{
			if (sp.GetAgent().teamID == 0)
			{
				sp.GetAgent().SetMoveOrder(position, null);
			}
		}
	}
	
	void SetMoveOrder(Vector3 position)
	{
		selectedSpacecraftList = mouseSelection.GetSelectedSpacecraft();
		foreach(Spacecraft sp in selectedSpacecraftList)
		{
			if (sp.GetAgent().teamID == 0)
			{
				sp.GetAgent().EnableMoveCommand(true);
			}
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
		circleLineRenderer.enabled = value;
		elevationLineRenderer.enabled = value;
	}
}

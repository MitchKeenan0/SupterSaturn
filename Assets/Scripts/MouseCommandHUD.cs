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
	private SelectionSquad selectedSquad;
	private Camera cameraMain;
	private List<Spacecraft> selectedSpacecraftList;
	private Vector3 moveCommandPosition = Vector3.zero;
	private Vector3 lastMousePosition = Vector3.zero;
	private Vector3 screenCenter = Vector3.zero;
	private bool bEnabled = true;
	private bool bActionCommand = false;
	private float originalRadius = 0f;
	private float mouseX = 0f;
	private float mouseY = 0f;
	private float scroll = 0f;
	private float lineScrollScale = 0f;

	void Awake()
	{
		cameraMain = Camera.main;
	}

	void Start()
	{
		moveCommandPosition = transform.position;
		mouseSelection = FindObjectOfType<MouseSelection>();
		mouseContext = FindObjectOfType<MouseContextHUD>();
		selectedSquad = FindObjectOfType<SelectionSquad>();

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
			if (mouseContext.GetSpacecraftInformation() != null)
			{
				SpacecraftInformation si = mouseContext.GetSpacecraftInformation();
				if ((si != null) && (si.GetTeamID() != 0))
				{
					SetActionOrder(si.gameObject.transform);
					bActionCommand = true;
				}
			}

			if (!bActionCommand)
			{
				SetEnabled(true);
				SetCircleLocation();
				UpdateCircleRender();
				moveCommandPosition = circleLineRenderer.transform.position;
			}
		}

		if (Input.GetButton("Fire2"))
		{
			if (!bActionCommand)
			{
				SetCircleLocation();
				UpdateMouseMovement();
				scroll += Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime * 1000f;
				Debug.Log("scroll: " + scroll);
				UpdateCircleRender();
				VisualizeMoveOrder(moveCommandPosition);
			}
		}

		if (Input.GetButtonUp("Fire2"))
		{
			if (!bActionCommand)
				SetMoveOrder(moveCommandPosition);
			SetEnabled(false);
			bActionCommand = false;
			scroll = 0f;
		}
	}

	void UpdateMouseMovement()
	{
		Vector3 deltaMouse = (Input.mousePosition - lastMousePosition);
		mouseX = deltaMouse.x;
		mouseY = deltaMouse.y;
		lastMousePosition = Input.mousePosition;
		
		// command circle width control
		Vector3 mousePosition = Input.mousePosition;
		Vector3 squadScreenPosition = cameraMain.WorldToScreenPoint(selectedSquad.GetCenterPoint());
		float distToCenter = Mathf.Abs((mousePosition - squadScreenPosition).magnitude / 10);
		circleRadius = Mathf.Lerp(circleRadius, distToCenter, Time.deltaTime);
		UpdateCircleRender();

		// command line elevation
		Vector3 lineOrigin = ClosestLineToMouse();
		if (lineOrigin != Vector3.zero)
		{
			Vector3 originScreenPosition = cameraMain.WorldToScreenPoint(lineOrigin);
			Vector3 verticalMousePosition = Input.mousePosition;
			float distToMouse = (mousePosition - originScreenPosition).y / 15;
			lineElevation = distToMouse + (scroll * 1.6f);

			Vector3 elevationLine = lineOrigin + (Vector3.up * lineElevation);
			elevationLineRenderer.SetPosition(0, lineOrigin);
			elevationLineRenderer.SetPosition(1, elevationLine);
			moveCommandPosition = Vector3.Lerp(moveCommandPosition, elevationLine, Time.deltaTime * 10f);

			lineScrollScale = Vector3.Distance(lineOrigin, elevationLine);
		}
	}

	void UpdateCircleRender()
	{
		float deltaTheta = (float)(2.0 * Mathf.PI) / numCircleSegments;
		float theta = 0f;

		for (int i = 0; i < numCircleSegments + 1; i++)
		{
			float x = (circleRadius + (scroll * 5f)) * Mathf.Cos(theta);
			float z = (circleRadius + (scroll * 5f)) * Mathf.Sin(theta);
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
		Vector3 closestLinePosition = Vector3.zero;

		Vector3[] linePositions = new Vector3[circleLineRenderer.positionCount];
		int numPos = circleLineRenderer.GetPositions(linePositions);
		float closestDistance = Mathf.Infinity;
		for (int i = 0; i < numPos; i++)
		{
			Vector3 linePosition = linePositions[i];
			Vector3 lineToCamera = cameraMain.transform.position - linePosition;
			var mouseRay = cameraMain.ScreenPointToRay(Input.mousePosition);
			mousePos = mouseRay.GetPoint(Mathf.Abs(lineToCamera.magnitude));

			Vector3 circlePosition = circleLineRenderer.transform.position;
			Vector3 screenLinePosition = cameraMain.WorldToScreenPoint(circlePosition + linePosition);
			Vector3 compVector = screenLinePosition - Input.mousePosition;

			float distanceToLine = compVector.magnitude;
			if (distanceToLine < closestDistance)
			{
				Vector3 position = (circleLineRenderer.transform.position + linePosition);
				if (IsOnScreen(position))
				{
					closestLinePosition = position;
					closestDistance = distanceToLine;
				}
			}
		}

		///Debug.DrawLine(mousePos, closestLinePosition, Color.blue);

		return closestLinePosition;
	}

	bool IsOnScreen(Vector3 position)
	{
		bool result = false;
		Vector3 positionScreenPosition = cameraMain.WorldToScreenPoint(position);
		if ((positionScreenPosition.x < Screen.width) && (positionScreenPosition.x > 0)
			&& (positionScreenPosition.y < Screen.height) && (positionScreenPosition.y > 0)
			&& (Vector3.Dot(cameraMain.transform.forward, (position - cameraMain.transform.position).normalized) > 0.3f))
			result = true;
		return result;
	}

	void VisualizeMoveOrder(Vector3 position)
	{
		selectedSpacecraftList = mouseSelection.GetSelectedSpacecraft();
		foreach (Spacecraft sp in selectedSpacecraftList)
		{
			if (sp.GetAgent().teamID == 0)
			{
				sp.GetAgent().SetMoveOrder(position, true, null);
			}
		}
	}
	
	void SetMoveOrder(Vector3 position)
	{
		if (selectedSquad != null)
			selectedSquad.BeginCommandMove();
	}

	void SetActionOrder(Transform actionTarget)
	{
		selectedSpacecraftList = mouseSelection.GetSelectedSpacecraft();
		foreach (Spacecraft sp in selectedSpacecraftList)
		{
			if (sp.GetAgent().teamID == 0)
				sp.GetAgent().SuggestTarget(actionTarget);
		}
	}

	void SetFollowOrder(Transform value)
	{
		//selectedSpacecraftList = mouseSelection.GetSelectedSpacecraft();
		//foreach (Spacecraft sp in selectedSpacecraftList)
		//{
		//	sp.GetAgent().SetMoveOrder(value.position, value);
		//}
	}

	public void SetEnabled(bool value)
	{
		bEnabled = value;
		circleLineRenderer.enabled = value;
		elevationLineRenderer.enabled = value;
	}
}

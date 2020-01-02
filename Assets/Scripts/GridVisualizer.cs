using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridVisualizer : MonoBehaviour
{
	public GameObject linePrefab;
	public bool bUpdating = false;
	public float updateInterval = 0.1f;
	public int gridLayers = 3;
	public float lineSpacing = 1f;
	public float layerSpacing = 1f;
	public int rowPopulation = 10;
	public float lineLength = 50f;

	private Transform cameraTransform;
	private Transform updateTransform;
	private List<LineRenderer> northSouthList;
	private List<LineRenderer> upDownList;
	private List<LineRenderer> leftRightList;
	private int flipper = 1;

	private IEnumerator updateCoroutine;

    void Start()
    {
		cameraTransform = Camera.main.transform;
		InitGrid();
		if (bUpdating)
		{
			updateTransform = transform.parent;
			Debug.Log("Set updateParent " + updateTransform.name);
			transform.SetParent(null);
			updateCoroutine = TimedUpdate(updateInterval);
			StartCoroutine(updateCoroutine);
		}
	}

	void InitGrid()
	{
		northSouthList = new List<LineRenderer>();
		for (int i = 0; i < gridLayers; i++)
		{
			CreateLineRow(Vector3.forward, 1, i, northSouthList);
			CreateLineRow(Vector3.forward, 1, -i, northSouthList);
			CreateLineRow(Vector3.forward, -1, i, northSouthList);
			CreateLineRow(Vector3.forward, -1, -i, northSouthList);
		}

		upDownList = new List<LineRenderer>();
		for (int i = 0; i < gridLayers; i++)
		{
			CreateLineRow(Vector3.up, 1, i, upDownList);
			CreateLineRow(Vector3.up, 1, -i, upDownList);
			CreateLineRow(Vector3.up, -1, i, upDownList);
			CreateLineRow(Vector3.up, -1, -i, upDownList);
		}

		leftRightList = new List<LineRenderer>();
		for (int i = 0; i < gridLayers; i++)
		{
			CreateLineRow(Vector3.right, 1, i, leftRightList);
			CreateLineRow(Vector3.right, 1, -i, leftRightList);
			CreateLineRow(Vector3.right, -1, i, leftRightList);
			CreateLineRow(Vector3.right, -1, -i, leftRightList);
		}
	}

	void CreateLineRow(Vector3 axis, int normal, int layer, List<LineRenderer> list)
	{
		List<LineRenderer> row = new List<LineRenderer>();
		for (int i = 0; i < rowPopulation; i++)
		{
			GameObject line = Instantiate(linePrefab, Vector3.zero, Quaternion.identity);
			line.transform.SetParent(transform);
			transform.rotation = Quaternion.identity;
			LineRenderer lr = line.GetComponent<LineRenderer>();

			Vector3 positionOffset = Vector3.zero;

			// row offset
			Vector3 rowOffset = Vector3.zero;
			if (axis == Vector3.forward)
				rowOffset = Vector3.up;
			else if (axis == Vector3.up)
				rowOffset = Vector3.right;
			else if (axis == Vector3.right)
				rowOffset = Vector3.forward;
			float rowIndexScale = i * lineSpacing * normal;
			positionOffset += rowOffset * rowIndexScale;

			// layer offset (for the entire "floor")
			Vector3 layerOffset = Vector3.zero;
			if (axis == Vector3.forward)
				layerOffset = Vector3.right;
			else if (axis == Vector3.up)
				layerOffset = Vector3.forward;
			else if (axis == Vector3.right)
				layerOffset = Vector3.up;
			float layerIndexScale = layer * layerSpacing;
			positionOffset += layerOffset * layerIndexScale;

			Vector3 linePointA = transform.position + positionOffset + (axis * lineLength);
			Vector3 linePointB = transform.position + positionOffset + (-axis * lineLength);
			lr.SetPosition(0, linePointA);
			lr.SetPosition(1, linePointB);
			list.Add(lr);
			flipper *= -1;
		}
	}

	private IEnumerator TimedUpdate(float intervalTime)
	{
		while (true)
		{
			yield return new WaitForSeconds(intervalTime);
			if (updateTransform != null)
			{
				transform.position = updateTransform.position;
				transform.rotation = Quaternion.identity;
			}
		}
	}
}

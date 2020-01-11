using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CampaignLocation : MonoBehaviour
{
	public float reach = 10f;
	public int connections = 2;
	private LineRenderer lineRenderer;
	private List<CampaignLocation> connectedLocations;

    void Start()
    {
		InitLines();
    }

	void InitLines()
	{
		connectedLocations = new List<CampaignLocation>();
		lineRenderer = GetComponent<LineRenderer>();
		lineRenderer.positionCount = 3;
		lineRenderer.SetPosition(0, transform.position);
		lineRenderer.SetPosition(1, transform.position);
		lineRenderer.SetPosition(2, transform.position);

		Collider[] cols = Physics.OverlapSphere(transform.position, reach);
		for(int i = 0; i < connections; i++)
		{
			if (i < cols.Length)
			{
				GameObject colGameObject = cols[i].gameObject;
				if (Vector3.Distance(colGameObject.transform.position, transform.position) <= reach)
				{
					if (colGameObject.GetComponent<CampaignLocation>())
					{
						connectedLocations.Add(colGameObject.GetComponent<CampaignLocation>());
					}
				}
			}
		}

		if (connectedLocations.Count >= 1)
			lineRenderer.SetPosition(0, connectedLocations[0].transform.position);

		lineRenderer.SetPosition(1, transform.position);

		if (connectedLocations.Count > 1)
			lineRenderer.SetPosition(2, connectedLocations[1].transform.position);
	}
}

﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionSquad : MonoBehaviour
{
	private List<Spacecraft> spacecraftList;
	private Vector3 destinationVector = Vector3.zero;
	private Vector3 centerPoint = Vector3.zero;

    void Start()
    {
		spacecraftList = new List<Spacecraft>();
    }

	void Update()
	{
		if (spacecraftList.Count > 0)
			centerPoint = GetCenterPoint();
		else
			centerPoint = Vector3.zero;
	}

	Vector3 GetCenterPoint()
	{
		Vector3 centerPosition = Vector3.zero;
		if (spacecraftList.Count > 0){
			Vector3 combinedPositions = Vector3.zero;
			int countedSpacecrafts = 0;
			int listCount = spacecraftList.Count;
			for (int i = 0; i < listCount; i++){
				if (spacecraftList[i] != null)
				{
					combinedPositions += spacecraftList[i].transform.position;
					countedSpacecrafts++;
				}
			}
			centerPosition = combinedPositions / Mathf.Round(countedSpacecrafts);
		}
		return centerPosition;
	}

	public void SetSquad(List<Spacecraft> spacecrafts)
	{
		if (spacecrafts != null && spacecrafts.Count > 0)
			spacecraftList = spacecrafts;
		else
			spacecraftList.Clear();
		centerPoint = GetCenterPoint();
	}

	public void SetDestination(Vector3 destination)
	{
		destinationVector = destination;
		int numSelected = spacecraftList.Count;
		if (numSelected > 0)
		{
			for (int i = 0; i < numSelected; i++)
			{
				if (spacecraftList[i] != null)
				{
					Vector3 spPosition = spacecraftList[i].transform.position;
					Vector3 centerOffset = spPosition - centerPoint;
					if (spacecraftList[i].GetAgent().teamID == 0)
						spacecraftList[i].GetAgent().EnableMoveCommand(true);
				}
			}
		}
	}

	public Vector3 GetOffsetOf(Spacecraft sp)
	{
		Vector3 offset = Vector3.zero;
		if (spacecraftList.Contains(sp))
		{
			Vector3 position = sp.transform.position;
			offset = position - centerPoint;
		}
		return offset;
	}
}

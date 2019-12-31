using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetPredictionHUD : MonoBehaviour
{
	public GameObject predictionPrefab;

	private TeamFleetHUD teamFleetHud;
	private Camera cameraMain;
	private List<Prediction> predictionList;
	private List<Spacecraft> enemyList;

    void Start()
    {
		teamFleetHud = FindObjectOfType<TeamFleetHUD>();
		enemyList = teamFleetHud.GetEnemyList();
		cameraMain = Camera.main;
		predictionList = new List<Prediction>();
    }

	void Update()
	{
		foreach(Prediction pre in predictionList)
		{
			if (pre.position != Vector3.zero)
			{
				Vector3 screenPosition = cameraMain.WorldToScreenPoint(pre.position);
				pre.img.transform.position = screenPosition;
			}
		}
	}

	void InitPredictionPool()
	{
		enemyList = teamFleetHud.GetEnemyList();
		int numTargets = enemyList.Count;
		for (int i = 0; i < numTargets; i++)
		{
			Prediction pre = CreateNewPrediction();
			pre.SetPrediction(enemyList[i], Vector3.zero, Vector3.zero);
		}
	}

	Prediction CreateNewPrediction()
	{
		GameObject preGO = Instantiate(predictionPrefab, Vector3.zero, Quaternion.identity);
		preGO.transform.SetParent(transform);
		Prediction pre = preGO.GetComponent<Prediction>();
		predictionList.Add(pre);
		return pre;
	}

	public void SetPrediction(Spacecraft sp, Vector3 position, Vector3 velocity)
	{
		if (predictionList.Count == 0)
			InitPredictionPool();

		Prediction predictionToSet = null;
		foreach(Prediction pre in predictionList){
			if (((pre.spacecraft != null) && (pre.spacecraft == sp))
				|| (pre.position == Vector3.zero))
			{
				predictionToSet = pre;
			}
		}

		if (predictionToSet != null)
			predictionToSet.SetPrediction(sp, position, velocity);
	}
}

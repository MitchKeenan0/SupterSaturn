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

	public List<Prediction> GetPredictions() { return predictionList; }

    void Start()
    {
		predictionList = new List<Prediction>();
		teamFleetHud = FindObjectOfType<TeamFleetHUD>();
		enemyList = teamFleetHud.GetEnemyList();
		cameraMain = Camera.main;
    }

	void Update()
	{
		if (predictionList != null && predictionList.Count == 0)
		{
			InitPredictionPool();
		}
		else
		{
			foreach (Prediction pre in predictionList)
			{
				if (pre.position != Vector3.zero)
				{
					Vector3 screenPosition = cameraMain.WorldToScreenPoint(pre.position);
					pre.img.transform.position = screenPosition;
				}
			}
		}
	}

	void InitPredictionPool()
	{
		predictionList = new List<Prediction>();
		if ((teamFleetHud != null) && (teamFleetHud.GetEnemyList().Count > 0))
		{
			enemyList = teamFleetHud.GetEnemyList();
			int numTargets = enemyList.Count;
			for (int i = 0; i < numTargets; i++)
			{
				Prediction pre = CreateNewPrediction();
				pre.SetPrediction(enemyList[i], Vector3.zero, Vector3.zero, 0f);
			}
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

	public void SetPrediction(Spacecraft sp, Vector3 position, Vector3 velocity, float period)
	{
		Prediction predictionToSet = null;

		foreach (Prediction pre in predictionList)
		{
			if (pre.spacecraft == sp)
				predictionToSet = pre;
		}

		if (!predictionToSet)
			predictionToSet = CreateNewPrediction();

		if (predictionToSet != null)
			predictionToSet.SetPrediction(sp, position, velocity, period);
	}
}

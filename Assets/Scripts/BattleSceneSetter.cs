using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleSceneSetter : MonoBehaviour
{
	public GameObject solarSystemPrefab;
	public GameObject nebulaPrefab;
	public GameObject templePrefab;

	private Game game;
	private IEnumerator loadWaitCoroutine;

    void Start()
    {
		game = FindObjectOfType<Game>();
		loadWaitCoroutine = LoadWait(0.3f);
		StartCoroutine(loadWaitCoroutine);
    }

	private IEnumerator LoadWait(float waitTime)
	{
		yield return new WaitForSeconds(waitTime);
		LoadSceneSetting();
	}

	void LoadSceneSetting()
	{
		GameObject objectToLoad = null;
		int sceneType = game.GetSceneSetting();
		if (sceneType == 0)
			objectToLoad = solarSystemPrefab;
		else if (sceneType == 1)
			objectToLoad = nebulaPrefab;
		else if (sceneType == 2)
			objectToLoad = templePrefab;

		if (objectToLoad != null)
		{
			GameObject spawnObj = Instantiate(objectToLoad, transform);
			BattleScene bs = spawnObj.GetComponent<BattleScene>();
			if (bs != null)
				bs.InitScene();
		}
	}
}

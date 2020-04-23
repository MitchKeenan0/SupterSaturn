using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class BattleSceneSetter : MonoBehaviour
{
	public GameObject solarSystemPrefab;
	public GameObject nebulaPrefab;
	public GameObject templePrefab;

	private Game game;
	private GravitySystem gravitySystem;
	private CelestialHUD celestialHud;
	private IEnumerator loadWaitCoroutine;

    void Start()
    {
		BattleScene[] existingScenes = GetComponentsInChildren<BattleScene>();
		int numScenes = existingScenes.Length;
		for (int i = 0; i < numScenes; i++)
			DestroyImmediate(existingScenes[i].gameObject);
		game = FindObjectOfType<Game>();
		gravitySystem = FindObjectOfType<GravitySystem>();
		celestialHud = FindObjectOfType<CelestialHUD>();
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
			celestialHud.InitCelestialHud();
		}

		//if (gravitySystem != null)
		//	gravitySystem.InitGravitySystem();
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerFailsafe : MonoBehaviour
{
	public GameObject playerPrefab;

    void OnEnable()
	{
		SceneManager.sceneLoaded += OnSceneLoaded;
	}

	void OnDisable()
	{
		SceneManager.sceneLoaded -= OnSceneLoaded;
	}

	private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		Player player = FindObjectOfType<Player>();
		if (!player)
		{
			GameObject failsafePlayer = Instantiate(playerPrefab, null);
		}
	}

	void Awake()
	{
		Player player = FindObjectOfType<Player>();
		if (!player)
		{
			GameObject failsafePlayer = Instantiate(playerPrefab, null);
		}
	}
}

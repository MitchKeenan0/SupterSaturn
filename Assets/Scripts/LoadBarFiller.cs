using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadBarFiller : MonoBehaviour
{
	public GameObject loadPanel;
	public RectTransform loadBar;
	public float loadDuration = 0.5f;
	public float totalSize = 300f;

	private float deltaLoadSize = 0;

	void Awake()
	{
		loadPanel.gameObject.SetActive(true);
	}

	void Start()
    {
		loadBar.sizeDelta = new Vector2(0f, loadBar.sizeDelta.y);
    }

    void Update()
    {
		UpdateBarValue();
	}

	void UpdateBarValue()
	{
		deltaLoadSize = Mathf.Lerp(deltaLoadSize, totalSize * 1.1f, Time.deltaTime * (1f / loadDuration));
		if (deltaLoadSize >= totalSize)
			deltaLoadSize = totalSize;
		if (deltaLoadSize == totalSize)
			gameObject.SetActive(false);
		else
			loadBar.sizeDelta = new Vector2(deltaLoadSize, loadBar.sizeDelta.y);
	}
}

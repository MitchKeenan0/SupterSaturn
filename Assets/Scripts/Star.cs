using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Star : MonoBehaviour
{
	public float minSize = 0.1f;
	public float maxSize = 1f;

	private MeshRenderer meshRenderer;

    void Awake()
    {
		meshRenderer = GetComponentInChildren<MeshRenderer>();

		Color newColor = new Color(
			Random.Range(0.5f, 1.5f),
			Random.Range(0.5f, 1.5f),
			Random.Range(0.5f, 1.5f));
		meshRenderer.material.color = newColor;
		meshRenderer.material.SetColor("_EmissionColor", newColor);

		float randomSize = Random.Range(minSize, maxSize);
		meshRenderer.transform.localScale = Vector3.one * randomSize;
	}
}

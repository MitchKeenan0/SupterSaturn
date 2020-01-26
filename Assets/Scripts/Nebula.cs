using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Nebula : MonoBehaviour
{
	public float minSize = 0.1f;
	public float maxSize = 1f;

	private ParticleSystem particles;

    void Awake()
    {
		particles = GetComponentInChildren<ParticleSystem>();
		var em = particles.main;

		em.startColor = new Color(
			Random.Range(0f, 1f),
			Random.Range(0f, 1f),
			Random.Range(0f, 1f));

		float randomSize = Random.Range(minSize, maxSize);
		particles.gameObject.transform.localScale = Vector3.one * randomSize;
	}
}

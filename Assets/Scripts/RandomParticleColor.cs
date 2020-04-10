using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomParticleColor : MonoBehaviour
{
	public float topLight = 35f;
	private ParticleSystem myParticles;

    void Start()
    {
		myParticles = GetComponent<ParticleSystem>();
		var mem = myParticles.main;
		float r1 = Random.Range(0f, topLight);
		float r2 = Random.Range(0f, topLight);
		float r3 = Random.Range(0f, topLight);
		mem.startColor = new Color(r1, r2, r3);
	}
}

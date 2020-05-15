using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScanCollider : MonoBehaviour
{
	private VertexVisualizer visualizer;
	private ParticleSystem hitParticles;
	private ScoreHUD scoreHud;
	private ColorBlend colorBlend;
	private LineRenderer line;
	private bool bHit = false;

    void Awake()
    {
		transform.localScale = Vector3.one;
		visualizer = transform.parent.GetComponent<VertexVisualizer>();
		colorBlend = GetComponent<ColorBlend>();
		hitParticles = GetComponentInChildren<ParticleSystem>();
		scoreHud = FindObjectOfType<ScoreHUD>();
		line = GetComponent<LineRenderer>();
		
	}

	public void Hit()
	{
		if (!bHit)
		{
			Vector3 vertexPosition = transform.TransformPoint(line.GetPosition(0));
			line.enabled = false;

			visualizer.DisableLine(vertexPosition);
			if (!scoreHud)
				scoreHud = FindObjectOfType<ScoreHUD>();
			if (scoreHud != null)
				scoreHud.PopupScore(vertexPosition, 1);

			hitParticles.transform.position = vertexPosition;
			Vector3 lineDirection = (transform.TransformPoint(line.GetPosition(1)) - vertexPosition).normalized;
			hitParticles.transform.rotation = Quaternion.LookRotation(lineDirection);
			Color c = colorBlend.MyColor();
			var ma = hitParticles.main;
			ma.startColor = c;
			hitParticles.Play();
			//var em = hitParticles.emission;
			//em.enabled = true;

			Destroy(gameObject, 1.9f);
			bHit = true;
		}
	}
}

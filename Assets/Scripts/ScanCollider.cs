using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScanCollider : MonoBehaviour
{
	private VertexVisualizer visualizer;
	private ParticleSystem hitParticles;
	private ScoreHUD scoreHud;
	private ColorBlend colorBlend;
	private bool bHit = false;

    void Awake()
    {
		visualizer = transform.parent.GetComponent<VertexVisualizer>();
		colorBlend = GetComponent<ColorBlend>();
		hitParticles = GetComponentInChildren<ParticleSystem>();
		scoreHud = FindObjectOfType<ScoreHUD>();
		//var em = hitParticles.emission;
		//em.enabled = false;
    }

	public void Hit()
	{
		if (!bHit)
		{
			LineRenderer line = GetComponent<LineRenderer>();
			Vector3 vertexPosition = transform.TransformPoint(line.GetPosition(0));
			visualizer.DisableLine(vertexPosition);
			int maxScore = visualizer.GetNumVerticies();
			if (!scoreHud)
				scoreHud = FindObjectOfType<ScoreHUD>();
			if (scoreHud != null)
				scoreHud.PopupScore(vertexPosition, 1, maxScore);

			hitParticles.transform.position = vertexPosition;
			Vector3 lineDirection = (vertexPosition - Vector3.zero).normalized;
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

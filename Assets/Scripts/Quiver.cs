using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quiver : MonoBehaviour
{
	private SpriteRenderer sprite;
	private IEnumerator updateCoroutine;
	Vector3 originalScale;

    void Start()
    {
		sprite = GetComponent<SpriteRenderer>();
        if (sprite != null)
		{
			originalScale = sprite.transform.localScale;
			updateCoroutine = UpdateSprite(0.1f);
			StartCoroutine(updateCoroutine);
		}
    }

	private IEnumerator UpdateSprite(float waitTime)
	{
		while (true)
		{
			yield return new WaitForSeconds(waitTime);
			Vector3 randomScale = originalScale * Random.Range(0.8f, 1.2f);
			sprite.transform.localScale = randomScale;
		}
	}
}

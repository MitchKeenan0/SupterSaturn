using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RendererOverUI : MonoBehaviour
{
	private Renderer myRenderer;

    void Start()
    {
		myRenderer = GetComponent<Renderer>();
		if (myRenderer != null)
			myRenderer.sortingLayerID = 0;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectiveDetailPanel : MonoBehaviour
{
	public Text objText;
	public Text disText;
	public float offset = 280f;

	private RectTransform rectTransform;

    void Start()
    {
		rectTransform = GetComponent<RectTransform>();
		rectTransform.localPosition = new Vector3(offset, 0f, 0f);
	}

	public void SetAlignment(int normal)
	{
		rectTransform.localPosition = new Vector3(offset * normal, 0f, 0f);
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sun : MonoBehaviour
{
	public static Sun sun;

	private void Awake()
	{
		if (sun == null)
			sun = this;
		else if (sun != this)
			Destroy(gameObject);
	}

	void Start()
    {
		transform.position = Vector3.zero;
    }
}

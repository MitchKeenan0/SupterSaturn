using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchLine : MonoBehaviour
{
	public Vector2 GetLine() { return direction; }
	public bool IsTouching() { return bTouching; }

	private Vector2 startPosition = Vector3.zero;
	private Vector2 direction = Vector3.zero;
	private bool bTouching = false;

    void Start()
    {
        
    }

	void Update()
	{
		if (Input.touchCount > 0)
		{
			Touch touch = Input.GetTouch(0);
			switch (touch.phase)
			{
				case TouchPhase.Began:
					bTouching = true;
					startPosition = touch.position;
					break;

				case TouchPhase.Moved:
					bTouching = true;
					direction = touch.position - startPosition;
					break;

				case TouchPhase.Ended:
					bTouching = false;
					break;
			}
		}
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchLine : MonoBehaviour
{
	public Vector2 GetLine() { return direction; }
	public Vector2 GetPosition() { return position; }
	public bool IsTouching() { return bTouching; }

	private Vector2 startPosition = Vector2.zero;
	private Vector2 direction = Vector2.zero;
	private Vector2 position = Vector2.zero;
	private bool bTouching = false;

	void Update()
	{
		if (Input.touchCount > 0)
		{
			if (Input.touchCount == 1)
			{
				Touch touch = Input.GetTouch(0);
				switch (touch.phase)
				{
					case TouchPhase.Began:
						bTouching = true;
						startPosition = touch.position;
						position = startPosition;
						break;

					case TouchPhase.Moved:
						bTouching = true;
						direction = touch.position - startPosition;
						position = touch.position;
						break;

					case TouchPhase.Ended:
						bTouching = false;
						position = touch.position;
						break;
				}
			}
		}
	}
}

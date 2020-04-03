using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ContextHeader : MonoBehaviour
{
	public Text contextText;

    void Start()
    {
		contextText.text = "";
	}

	public void SetContextHeader(string value)
	{
		contextText.text = value;
	}
}

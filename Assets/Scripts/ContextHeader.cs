using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ContextHeader : MonoBehaviour
{
	public Text contextText;
	public float textWriterInterval = 0.01f;

	private string context;
	private int textCharCount = 0;
	private IEnumerator typeCoroutine;

    void Start()
    {
		contextText.text = "";
	}

	public void SetContextHeader(string value)
	{
		context = value;
		textCharCount = value.Length;
		typeCoroutine = TextWriter(textWriterInterval);
		StartCoroutine(typeCoroutine);
	}

	private IEnumerator TextWriter(float interval)
	{
		contextText.text = "";
		int i = 0;
		string write = "";
		while (i < textCharCount)
		{
			write += context[i];
			contextText.text = write;
			yield return new WaitForSeconds(interval);
			i++;
		}
	}
}

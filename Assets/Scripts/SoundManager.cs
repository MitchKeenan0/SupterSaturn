using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
	public static SoundManager sound;

	public AudioClip buttonAffirmative;
	public AudioClip buttonNegative;

	private AudioSource audioSource;

	void Awake()
	{
		if (sound == null)
		{
			DontDestroyOnLoad(gameObject);
			sound = this;
		}
		else if (sound != this)
		{
			Destroy(gameObject);
		}
		audioSource = GetComponent<AudioSource>();
	}

    public void AffirmativeButton()
	{
		if (audioSource != null)
			audioSource.PlayOneShot(buttonAffirmative);
	}

	public void NegativeButton()
	{
		if (audioSource != null)
			audioSource.PlayOneShot(buttonNegative);
	}
}

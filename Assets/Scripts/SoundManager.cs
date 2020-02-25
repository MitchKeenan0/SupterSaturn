using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
	public AudioClip buttonAffirmative;
	public AudioClip buttonNegative;

	private AudioSource audioSource;

	void Awake()
	{
		DontDestroyOnLoad(gameObject);
		audioSource = GetComponent<AudioSource>();
	}

    public void AffirmativeButton()
	{
		audioSource.PlayOneShot(buttonAffirmative);
	}

	public void NegativeButton()
	{
		audioSource.PlayOneShot(buttonNegative);
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Salient : MonoBehaviour
{
	public int teamID = 1;
	public Scan scanPrefab;
	public GameObject attacPrefab;
	public float senseInterval = 2f;
	public float moveSpeed = 1f;
	public float attacInterval = 6f;
	public bool bPlayerLOS = false;
	public Spacecraft playerObject;

	private Game game;
	private Rigidbody rb;
	private IEnumerator loadCoroutine;
	private IEnumerator senseCoroutine;
	private IEnumerator attacCoroutine;

    public virtual void Start()
    {
		game = FindObjectOfType<Game>();
		rb = GetComponent<Rigidbody>();
		loadCoroutine = LoadDelay(0.5f);
		StartCoroutine(loadCoroutine);
    }

	public virtual void InitSalience()
	{
		playerObject = game.GetSpacecraftList()[0];
		senseCoroutine = SenseUpdate(senseInterval);
		StartCoroutine(senseCoroutine);
	}

	public virtual void Sense()
	{
		Vector3 toPlayer = playerObject.transform.position - transform.position;
		RaycastHit hit;
		bool bHit = false;
		if (Physics.Raycast(transform.position, toPlayer, out hit))
		{
			if (hit.transform == playerObject.transform)
			{
				bHit = true;
				Debug.Log("los hit");
			}
		}
		bPlayerLOS = bHit;
	}

	public virtual void Move(Vector3 moveDirection, ForceMode forceMode)
	{
		rb.AddForce(moveDirection, forceMode);
	}

	public virtual void Attac()
	{

	}

	private IEnumerator LoadDelay(float waitTime)
	{
		yield return new WaitForSeconds(waitTime);
		InitSalience();
	}

	private IEnumerator SenseUpdate(float interval)
	{
		while (true)
		{
			yield return new WaitForSeconds(interval);
			Sense();
		}
	}
}

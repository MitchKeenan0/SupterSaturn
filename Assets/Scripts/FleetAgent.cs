using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FleetAgent : MonoBehaviour
{
	public int maxCapacity = 100;

	private Game game;
	private Campaign campaign;
	private ObjectManager objectManager;
	private Fleet fleet;
	private FleetController playerFleetController;
	private LocationManager locationManager;
	private NameLibrary nameLibrary;
	private List<Spacecraft> spacecraftList;

	public List<Spacecraft> GetAgentSpacecraftList() { return spacecraftList; }

	private IEnumerator loadWaitCoroutine;

    void Awake()
    {
		spacecraftList = new List<Spacecraft>();
		game = FindObjectOfType<Game>();
		campaign = FindObjectOfType<Campaign>();
		objectManager = FindObjectOfType<ObjectManager>();
		fleet = GetComponentInParent<Fleet>();
		locationManager = FindObjectOfType<LocationManager>();
		playerFleetController = GetComponent<FleetController>();
		nameLibrary = FindObjectOfType<NameLibrary>();

		loadWaitCoroutine = LoadWait(0.1f);
		StartCoroutine(loadWaitCoroutine);
    }

	private IEnumerator LoadWait(float waitTime)
	{
		yield return new WaitForSeconds(waitTime);
		InitAgent();
	}

	void InitAgent()
	{
		fleet.fleetName = nameLibrary.GetFleetName();

		int fleetSpacecraftCount = Random.Range(1, maxCapacity);
		for (int i = 0; i < fleetSpacecraftCount; i++)
		{
			int numCards = game.cardLibrary.Length;
			int rando = Random.Range(0, numCards);
			int index = Mathf.FloorToInt(Mathf.Sqrt(rando));
			Debug.Log("FleetAgent index " + index);
			if (index < numCards)
			{
				GameObject spacecraftObj = Instantiate(game.cardLibrary[index].cardObjectPrefab, transform);
				spacecraftObj.SetActive(false);
				Spacecraft sp = spacecraftObj.GetComponent<Spacecraft>();
				spacecraftList.Add(sp);
				game.SetEnemyCard(i, game.cardLibrary[index]);
				game.AddEnemy(sp);
			}
		}
	}
}

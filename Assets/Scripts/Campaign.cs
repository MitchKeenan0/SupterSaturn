using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System;
using UnityEngine.SceneManagement;

public class Campaign : MonoBehaviour
{
	private Game game;
	private Camera cameraMain;
	private FleetHUD fleetHud;
	private SpacecraftEditor spacecraftEditor;
	private CampaignBattle campaignBattle;
	private LocationManager locationManager;
	private NameLibrary nameLibrary;
	private List<CampaignLocation> locationList;
	private List<Fleet> fleetList;
	private List<CampaignLocation> originLocations = new List<CampaignLocation>();
	private bool bLocationsInit = false;

	void OnEnable()
	{
		SceneManager.sceneLoaded += OnLevelFinishedLoading;
	}
	void OnDisable()
	{
		SceneManager.sceneLoaded -= OnLevelFinishedLoading;
	}
	void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
	{
		nameLibrary = FindObjectOfType<NameLibrary>();
		nameLibrary.ResetUsedLists();

		LoadCampaign();
		if (game != null)
			game.LoadGame();
	}

	void Awake()
    {
		Time.timeScale = 1;
		fleetList = new List<Fleet>();
		locationList = new List<CampaignLocation>();
		originLocations = new List<CampaignLocation>();
		fleetHud = FindObjectOfType<FleetHUD>();
		spacecraftEditor = FindObjectOfType<SpacecraftEditor>();
		campaignBattle = FindObjectOfType<CampaignBattle>();
		locationManager = FindObjectOfType<LocationManager>();
		cameraMain = Camera.main;
		game = FindObjectOfType<Game>();
	}

	public void InitFleetLocations()
	{
		if (!bLocationsInit)
		{
			locationList = locationManager.GetAllLocations();
			fleetList = fleetHud.GetFleetList();
			int numFleets = fleetList.Count;

			///Debug.Log("init fleet locations with " + numFleets + " fleets    " + locationList.Count + " locations");
			for (int i = 0; i < numFleets; i++)
			{
				CampaignLocation furthestLocation = null;
				float furthestDistance = 0f;
				foreach (CampaignLocation cl in locationList)
				{
					///Debug.Log("neighbors: " + cl.GetNeighbors().Count + "    contained in origins: " + originLocations.Contains(cl));
					if ((cl.GetNeighbors().Count > 0) && (!originLocations.Contains(cl)))
					{
						float distanceToLocation = Vector3.Distance(cl.transform.position, Vector3.zero);
						if (distanceToLocation > furthestDistance)
						{
							furthestLocation = cl;
							furthestDistance = distanceToLocation;
						}
					}
				}

				if (furthestLocation != null)
				{
					///Debug.Log("campaign setting fleet location");
					fleetList[i].SetLocation(furthestLocation, true);
					originLocations.Add(furthestLocation);

					if (fleetList[i].teamID == 0)
					{
						MouseOrbitImproved moi = FindObjectOfType<MouseOrbitImproved>();
						if (moi != null)
						{
							Vector3 locationPosition = furthestLocation.transform.position;
							moi.SetCameraPosition(locationPosition + (locationPosition.normalized * 10));

							Quaternion toCenter = Quaternion.LookRotation(locationPosition - moi.transform.position, Vector3.up);
							moi.SetCameraRotation(toCenter);
						}
					}
				}
				//else
				//{
				//	Debug.Log("furthest location was null");
				//}

				bLocationsInit = true;
			}

			SaveCampaign();
		}
	}

	public void SaveCampaign()
	{
		SaveCampaign save = CreateSaveCampaignObject();
		BinaryFormatter bf = new BinaryFormatter();
		FileStream file = File.Create(Application.persistentDataPath + "/campaignsave.save");
		bf.Serialize(file, save);
		file.Close();
	}

	private SaveCampaign CreateSaveCampaignObject()
	{
		SaveCampaign save = new SaveCampaign();
		if (locationList == null)
			locationList = new List<CampaignLocation>();
		int numLocations = locationList.Count;
		if (numLocations > 0)
		{
			Debug.Log("campaign saving " + numLocations + " locations");
			for (int i = 0; i < numLocations; i++)
			{
				CampaignLocation cl = locationList[i];
				if (cl != null)
				{
					save.locationList.Add(cl.locationID);
					save.locationPositionsX.Add(cl.transform.position.x);
					save.locationPositionsY.Add(cl.transform.position.y);
					save.locationPositionsZ.Add(cl.transform.position.z);
					save.locationNameList.Add(cl.locationName);
				}
			}
		}

		Identity[] ids = FindObjectsOfType<Identity>();
		List<Identity> idList = new List<Identity>();
		int numIDs = ids.Length;
		if (numIDs > 0)
		{
			Debug.Log("campaign saving " + numIDs + " identities");
			for (int i = 0; i < numIDs; i++)
			{
				if ((i < ids.Length) && (ids[i] != null))
				{
					Identity id = ids[i];
					save.identityNames.Add(id.identityName);
					save.identityColorsR.Add(id.identityColor.r);
					save.identityColorsG.Add(id.identityColor.g);
					save.identityColorsB.Add(id.identityColor.b);

					if (id.GetLocation() != null)
					{
						CampaignLocation idLocation = id.GetLocation();
						if (locationList.Contains(idLocation))
							save.identityLocationList.Add(locationList.IndexOf(idLocation));
					}
				}
			}

			///Debug.Log("Campaign saved " + save.identityNames.Count + " identity names    " + save.identityLocationList.Count + " id locations");
		}

		return save;
	}

	public void LoadCampaign()
	{
		if (File.Exists(Application.persistentDataPath + "/campaignsave.save"))
		{
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open(Application.persistentDataPath + "/campaignsave.save", FileMode.Open);
			SaveCampaign save = (SaveCampaign)bf.Deserialize(file);
			file.Close();

			if (save.locationList.Count > 0)
			{
				int numSavedLocations = save.locationList.Count;
				if (numSavedLocations > 0)
				{
					List<Vector3> positionList = new List<Vector3>();
					for (int i = 0; i < numSavedLocations; i++)
					{
						Vector3 locationPosition = new Vector3(save.locationPositionsX[i], save.locationPositionsY[i], save.locationPositionsZ[i]);
						positionList.Add(locationPosition);
					}

					locationManager.LoadLocations(save.locationList, positionList, save.locationNameList);
				}
			}
			else
			{
				locationManager.InitNewLocations();
			}

			if ((save.identityNames != null) && (save.identityNames.Count > 0))
			{
				int numSavedIdentities = save.identityNames.Count;
				Identity[] allIDs = FindObjectsOfType<Identity>();
				if (allIDs.Length > 0)
				{
					for (int i = 0; i < numSavedIdentities; i++)
					{
						if ((i < allIDs.Length) && (allIDs[i] != null))
						{
							Identity id = allIDs[i];
							Color idColor = new Color(save.identityColorsR[i], save.identityColorsG[i], save.identityColorsB[i]);
							id.LoadIdentity(save.identityNames[i], idColor);
						}
					}
				}
			}
		}
		else
		{
			locationManager.InitNewLocations();
		}

		///Debug.Log("Campaign loaded");
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
	private Camera cameraMain;
	private MouseOrbitImproved mouseOrbit;
	private MouseContextHUD mouseContext;
	private TeamFleetHUD teamFleetHUD;
	private Transform lastTarget = null;

    void Start()
    {
		cameraMain = Camera.main;
		teamFleetHUD = FindObjectOfType<TeamFleetHUD>();
		mouseContext = FindObjectOfType<MouseContextHUD>();
		mouseOrbit = GetComponent<MouseOrbitImproved>();
    }

	public void SetOrbitTarget(Transform target)
	{
		mouseOrbit.SetOrbitTarget(target);
		if (target != null)
		{
			lastTarget = target;
		}
	}

	public void Highlight(bool value, Spacecraft sp)
	{
		if (value)
			teamFleetHUD.Highlight(sp);
		else
			teamFleetHUD.Unhighlight(sp);
	}

	public void SetMouseContext(bool value, Spacecraft sp)
	{
		SpacecraftInformation si = null;
		if (sp != null)
			si = sp.GetSpacecraftInformation();
		mouseContext.SetSpacecraftInformation(value, si);
	}
}

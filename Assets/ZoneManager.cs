using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneManager : MonoBehaviour {

    public DamageZone parent;

    [HideInInspector]
    public string frontZone = "";
    [HideInInspector]
    public string backZone = "";
    [HideInInspector]
    public string leftZone = "";
    [HideInInspector]
    public string rightZone = "";

    private ArrayList zoneList;

    // Use this for initialization
    void Start () {
        zoneList = parent.GetCurrentZonesStatus();
	}
	
	// Update is called once per frame
	void Update () {

        //Update each frame the current zone status
        zoneList = parent.GetCurrentZonesStatus();
        frontZone = zoneList[0].ToString();
        backZone = zoneList[1].ToString();
        leftZone = zoneList[2].ToString();
        rightZone = zoneList[3].ToString();
    }

    public string DetermineCurrentEnemyZone(string zoneName)
    {
        string currentZoneStatus = "";

        switch(zoneName)
        {
            case "FrontZone":
                currentZoneStatus = frontZone;
                break;
            case "BackZone":
                currentZoneStatus = backZone;
                break;
            case "LeftZone":
                currentZoneStatus = leftZone;
                break;
            case "RightZone":
                currentZoneStatus = rightZone;
                break;
            default:
                currentZoneStatus = frontZone;
                break;
        }

        return currentZoneStatus;
    }

}

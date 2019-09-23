using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

    public SteamVR_Render steamVr;
    private string vrEquipment;

    public GameObject UIParent;

	// Use this for initialization
	void Start () {

        /*
        vrEquipment = steamVr.hmdName;

        switch (vrEquipment)
        {
            case "lighthouse":
                SetViveUI(); break;
            case "oculus":
                SetOculusUI(); break;
            default:
                SetViveUI(); break;
        } */
	}
	
	// Update is called once per frame
	void Update () {
	}

    private void SetOculusUI()
    {
        Debug.Log("Setting Oculus UI!");

        //Find target UI and get children rect transform
        GameObject oculusTarget = GameObject.FindGameObjectWithTag("OculusUI");
        RectTransform[] oculusLocations = oculusTarget.GetComponentsInChildren<RectTransform>();
        RectTransform[] playerUI = UIParent.GetComponentsInChildren<RectTransform>();

        for (int i = 0; i < playerUI.Length; i++)
        {
            CopyRectValues(playerUI[i], oculusLocations[i]);
        }
    }

    private void SetViveUI()
    {
        Debug.Log("Setting HTCVive UI!");

        //Find target UI and get children rect transform
        GameObject viveTarget = GameObject.FindGameObjectWithTag("ViveUI");
        RectTransform[] viveLocations = viveTarget.GetComponentsInChildren<RectTransform>();
        RectTransform[] playerUI = UIParent.GetComponentsInChildren<RectTransform>();
        
        for(int i = 0; i < playerUI.Length; i++)
        {
            CopyRectValues(playerUI[i], viveLocations[i]);
        }
    }

    private void CopyRectValues(RectTransform playerUI, RectTransform targetUI)
    {
        playerUI.position = targetUI.position;
        playerUI.pivot = targetUI.pivot;
        playerUI.rotation = targetUI.rotation;
        playerUI.localScale = targetUI.localScale;
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpectatorModeManager : MonoBehaviour {

    public Camera[] spectatorCameras;
    private int currentCamera = 0;

	// Use this for initialization
	void Start () {

        if(spectatorCameras.Length > 1)
        StartCoroutine(ChangeCamera());
	}

    IEnumerator ChangeCamera()
    {
        while (true)
        {
            // Wait before changing camera
            yield return new WaitForSeconds(6.5f);

            // Deactivate current camera
            spectatorCameras[currentCamera].enabled = false;

            // Check which is the next camera that needs to be activated
            if (currentCamera < (spectatorCameras.Length-1) ) { currentCamera++; }
              else { currentCamera = 0; }

            // Activate next camera
            spectatorCameras[currentCamera].enabled = true;
        }
    }

    public void StopCameraChange()
    {
        StopCoroutine("ChangeCamera");
    }

}

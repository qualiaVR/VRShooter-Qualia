using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shoot : MonoBehaviour
{
    public bool isShooting;
    public bool aButtonPressed;

    // Vive controller
    /*
    private Valve.VR.EVRButtonId trigger = Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger;
    private Valve.VR.EVRButtonId aButton = Valve.VR.EVRButtonId.k_EButton_A;
    private Valve.VR.EVRButtonId touchPad = Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad;

    private SteamVR_Controller.Device controller { get { return SteamVR_Controller.Input((int)trackedObj.index); } }
    private SteamVR_TrackedObject trackedObj;
    private SteamVR_TrackedController viveController; */

    // Oculus controller

    public OVRInput.Controller questController;

    public float controllerYAxis = 0.0f;

    void Start()
    {
        /* try
        {
            trackedObj = GetComponent<SteamVR_TrackedObject>();
            viveController = this.gameObject.GetComponent<SteamVR_TrackedController>();
        } catch (Exception e)
        {
            Debug.Log("HTC Vive Controllers not found: " + e);
        } */
        
    }

    void Update()
    {
        if (OVRInput.Get(OVRInput.RawAxis1D.RIndexTrigger) > .1f)
        {
            isShooting = true;
        } else
        {
            isShooting = false;
        }

        if (OVRInput.Get(OVRInput.RawButton.B))
        {
            aButtonPressed = true;
        }
        else
        {
            aButtonPressed = false;
        }

        Debug.Log(aButtonPressed);

        controllerYAxis = OVRInput.Get(OVRInput.RawAxis2D.RThumbstick).y;

        /*
        if (controller == null)
        {

        } else
        {
            if (controller.GetPress(trigger))
            {
                isShooting = true;
            }
            else
            {
                isShooting = false;
            }

            controllerYAxis = controller.GetAxis().y;

            if (controller.GetPressDown(aButton) || viveController.padPressed)
            {
                aButtonPressed = true;
            }

            if (controller.GetPressUp(aButton) || !viveController.padPressed)
                aButtonPressed = false;
        } */

    }

}

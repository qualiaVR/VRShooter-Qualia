using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class VRTouchpadMove : MonoBehaviour
{

    [SerializeField]
    private Transform rig;
    [SerializeField]
    private Transform head;
    [SerializeField]
    private float walkingSpeed;
    [SerializeField]
    private float sprintSpeed;

    private Valve.VR.EVRButtonId touchpad = Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad;
    private Valve.VR.EVRButtonId grip = Valve.VR.EVRButtonId.k_EButton_Grip;

    private SteamVR_Controller.Device controller { get { return SteamVR_Controller.Input((int)trackedObj.index); } }
    private SteamVR_TrackedObject trackedObj;

    private Vector2 axis = Vector2.zero;
    private float speed;

    void Start()
    {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
    }

    void Update()
    {
        if (controller == null)
        {
            Debug.Log("Controller not initialized");
            return;
        }

        var device = SteamVR_Controller.Input((int)trackedObj.index);

        if (controller.GetTouch(touchpad))
        {
            if (controller.GetPress(grip))
            {
                speed = sprintSpeed;
            }
            else
            {
                speed = walkingSpeed;
            }

            axis = device.GetAxis(Valve.VR.EVRButtonId.k_EButton_Axis0);

            if (rig != null)
            {
                rig.position += (head.right * axis.x + head.forward * axis.y) * speed * Time.deltaTime;
                rig.position = new Vector3(rig.position.x, 0, rig.position.z);
            }
        }

    }
}

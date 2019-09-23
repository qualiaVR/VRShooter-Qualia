using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageZone : MonoBehaviour {

    //public Transform cameraTransform;
    //public float distanceFromCamera = 1.0f;
    //public string zone = "";
    //float euler_y;

    Ray shootRay = new Ray();
    RaycastHit raycastHit;
    int raycastMask;
    public float range = 100f;

    //Zone Status
    public string frontZone = "forward";
    public string backZone = "back";
    public string leftZone = "left";
    public string rightZone = "right";

	// Use this for initialization
	void Start () {

        raycastMask = LayerMask.GetMask("DamageZone");

    }
	
	// Update is called once per frame
	void Update () {
        //euler_y = cameraTransform.eulerAngles.y;
        //this.transform.eulerAngles = new Vector3(transform.eulerAngles.x, euler_y, transform.eulerAngles.z);

        //Vector3 resultingPosition = transform.position + cameraTransform.position * distanceFromCamera;
        //transform.position = resultingPosition;
        //transform.RotateAround(Vector3.zero, cameraTransform.right, 120 * Time.deltaTime);

        shootRay.origin = transform.position;
        shootRay.direction = transform.forward;

        if (Physics.Raycast(shootRay, out raycastHit, range, raycastMask))
        {
            ChooseForward(raycastHit.collider.name);
        }
    }

    private void ChooseForward(string collidedObject)
    {
        switch(collidedObject)
        {
            case "FrontZone":
                frontZone = "forward";
                backZone = "back";
                leftZone = "left";
                rightZone = "right";
                break;
            case "BackZone":
                frontZone = "back";
                backZone = "forward";
                leftZone = "right";
                rightZone = "left";
                break;
            case "RightZone":
                frontZone = "left";
                backZone = "right";
                leftZone = "back";
                rightZone = "forward";
                break;
            case "LeftZone":
                frontZone = "right";
                backZone = "left";
                leftZone = "forward";
                rightZone = "back";
                break;
        }
    }

    public ArrayList GetCurrentZonesStatus()
    {
        ArrayList array = new ArrayList
        {
            frontZone,
            backZone,
            leftZone,
            rightZone
        };

        return array;
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightManager : MonoBehaviour {

    public Light alarmLight;
    private new Light light;
    public float duration = 1.0F;

    // Use this for initialization
    void Start () {
        light = alarmLight.GetComponent<Light>();
	}
	
	// Update is called once per frame
	void Update () {
        float phi = Time.time / duration * 2 * Mathf.PI;
        float amplitude = Mathf.Cos(phi) * 0.5F + 2.5F;
        light.intensity = amplitude;
    }


}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReloadController : MonoBehaviour
{
    public bool canReload = false;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Gun")
            canReload = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Gun")
            canReload = false;
    }

}

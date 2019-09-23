using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeapons : MonoBehaviour {

    // Available player weapons
    public GameObject laserRifle;
    public GameObject sniperRifle;
    private AudioSource cannotSwitchSound;

    // Switching weapons timer
    public float timeBetweenSwitchingWeapons = 2.5f;
    private float switchingWeaponsTimer = 5.0f;

    // Not available weapon timer
    public float timeBetweenNotAvailableSound = 1.25f;
    private float notAvailableTimer;

    // Laser rifle - 0  || Sniper rifle - 1
    private int currentWeaponIndex = 0;

    // Shoot object
    public GameObject shootManager;
    private Shoot shoot;
    private bool aButtonIsPressed;

    private void Start()
    {
        shoot = shootManager.GetComponent<Shoot>();
        aButtonIsPressed = shoot.aButtonPressed;
        cannotSwitchSound = GetComponents<AudioSource>()[1];
    }
	
	// Update is called once per frame
	void Update () {

        switchingWeaponsTimer += Time.deltaTime;
        notAvailableTimer += Time.deltaTime;

        //Check every frame if button A was pressed
        aButtonIsPressed = shoot.aButtonPressed;

        if (aButtonIsPressed) {
            if (Time.timeScale != 0 && switchingWeaponsTimer >= timeBetweenSwitchingWeapons)
                ChangeWeapon(switchingWeaponsTimer);
            else
                PlayNotAvailableSound();
        } 
	}

    void ChangeWeapon(float timer)
    {
        switchingWeaponsTimer = 0f;
        Debug.Log("Changing weapons!");

            switch (currentWeaponIndex)
            {
                // Laser rifle active - switch to sniper
                case 0:
                    laserRifle.SetActive(false);
                    sniperRifle.SetActive(true);
                    currentWeaponIndex = 1;
                    break;

                // Sniper rifle active - switch to laser rifle
                case 1:
                    laserRifle.SetActive(true);
                    sniperRifle.SetActive(false);
                    currentWeaponIndex = 0;
                    break;
            }
    }

    public void DisableWeapons()
    {
        if (laserRifle.activeSelf) {
            laserRifle.SetActive(false);
        }

        if(sniperRifle.activeSelf) {
            sniperRifle.SetActive(false);
        }
    }

    void PlayNotAvailableSound()
    {
        if (Time.timeScale != 0 && switchingWeaponsTimer >= timeBetweenSwitchingWeapons)
        {
            switchingWeaponsTimer = 0.0f;
            cannotSwitchSound.Play();
            Debug.Log("playing sound!");
        }
        
    }

}

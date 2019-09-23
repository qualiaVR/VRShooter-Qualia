using CompleteProject;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnitySampleAssets.CrossPlatformInput;

public class PlayerShootingSniper : MonoBehaviour
{
    public GameObject RightControl;
    public int damagePerShot = 20;                  // The damage inflicted by each bullet.
    public float timeBetweenBullets = 0.15f;        // The time between each shot.
    public float range = 100f;                      // The distance the gun can fire.

    //Shooting variables and particles
    float timer;                                    // A timer to determine when to fire.
    Ray shootRay = new Ray();                       // A ray from the gun end forwards.
    RaycastHit shootHit;                            // A raycast hit to get information about what was hit.
    int shootableMask;                              // A layer mask so the raycast only hits things on the shootable layer.
    ParticleSystem gunParticles;                    // Reference to the particle system.
    LineRenderer gunLine;                           // Reference to the line renderer.
    Light gunLight;                                 // Reference to the light component.
    public Light faceLight;                             // Duh
    float effectsDisplayTime = 0.05f;                // The proportion of the timeBetweenBullets that the effects will display for.

    //Sniper variables
    public Camera sniperCamera;

    //Reload Controller Script
    private ReloadController reloadZoneScript;

    private int startingBullets = 20;
    private int currentBullets;

    //Sounds
    private AudioSource bulletSound;
    private AudioSource noBulletsLeft;
    private AudioSource reloadingSound;
    private AudioSource headshotSound;

    private float noBulletsSoundTimer;
    private float reloadingSoundTimer;
    private float timeBetweenNoBulletsSound;
    private float timeBetweenReloadingSound;

    public AudioSource[] sounds;

    //Energy Gun bar
    public Image energyBar;
    public Image spectatorEnergyBar;
    public Text energyText;
    private bool isBlinking = false;

    private Color startingTextColor = new Color(0.0f / 255.0f, 184.0f / 255.0f, 16.0f / 255.0f, 255.0f / 255.0f);
    private Color blinkingColor = new Color(233.0f / 255.0f, 217.0f / 255.0f, 10.0f / 255.0f, 255.0f / 255.0f);

    //EnemyHealth Components and Headshots
    EnemyHealth enemyHealth;
    EnemyHealthMelee enemyHealthMelee;
    EnemyHealthWorker enemyHealthWorker;
    EnemyHealthAssault enemyHealthAssault;
    EnemyHealthHeavy enemyHealthHeavy;


    private bool headShotHit = false;
    string enemyTag = "";
    string correctHealthComponentTag = "";

    //CCTV
    private CCTV_Screen cctvManager;

    void Awake()
    {
        currentBullets = startingBullets;

        timeBetweenNoBulletsSound = .75f;
        timeBetweenReloadingSound = .5f;

        // Create a layer mask for the Shootable layer.
        shootableMask = LayerMask.GetMask("Shootable");

        // Set up the references.
        gunParticles = GetComponent<ParticleSystem>();
        gunLine = GetComponent<LineRenderer>();
        gunLight = GetComponent<Light>();
        //faceLight = GetComponentInChildren<Light> ();

        //Get reload zone script
        reloadZoneScript = GameObject.FindWithTag("ReloadZone").GetComponent<ReloadController>();

        //Initialize gun sounds - audio source components in gun
        sounds = GetComponents<AudioSource>();
        bulletSound = sounds[0];
        noBulletsLeft = sounds[1];
        reloadingSound = sounds[2];
        headshotSound = sounds[3];

        cctvManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<CCTV_Screen>();
    }


    void Update()
    {
        ManageZoom();

        bool isShooting = RightControl.GetComponent<Shoot>().isShooting;
        bool canReload = reloadZoneScript.canReload;

        // Add the time since Update was last called to the timer.
        timer += Time.deltaTime;
        noBulletsSoundTimer += Time.deltaTime;
        reloadingSoundTimer += Time.deltaTime;

        DisableEffects();

        // If the Fire1 button is being press and it's time to fire...
        if (isShooting && timer >= timeBetweenBullets && Time.timeScale != 0 && currentBullets >= 0 && !canReload)
        {
            // ... shoot the gun.
            Shoot();
            currentBullets--;
        }
        else if (isShooting && currentBullets < 0 && noBulletsSoundTimer >= timeBetweenNoBulletsSound && Time.timeScale != 0 && !canReload)
        {
            noBulletsLeft.Play();
            noBulletsSoundTimer = 0.0f;
        }

        if (canReload && isShooting)
        {
            Reload();
        }
    }


    public void DisableEffects()
    {
        // Disable the line renderer and the light.
        gunLine.enabled = false;
        faceLight.enabled = false;
        gunLight.enabled = false;
    }

    void ManageZoom()
    {
        float yAxis = RightControl.GetComponent<Shoot>().controllerYAxis;

        //
        if (yAxis != 0.0f)
        {
            if(yAxis > 0)
            {
                //Zoom in camera depending on control stick force
                float strength = 10 * yAxis;

                if ((sniperCamera.fieldOfView - strength) > 10)
                    sniperCamera.fieldOfView -= strength;
                else //Cannot zoom in further than 10 units 
                    if ((sniperCamera.fieldOfView - strength) <= 10)
                    sniperCamera.fieldOfView = 10;
            } else
                {
                float strength = 10 * Mathf.Abs(yAxis);

                if ((sniperCamera.fieldOfView + strength) < 30)
                    sniperCamera.fieldOfView += strength;
                else  //Cannot zoom out further than 30 units
                    if ((sniperCamera.fieldOfView + strength) >= 30)
                    sniperCamera.fieldOfView = 30;
            }
        }

    }

    void Shoot()
    {
        gunLine.enabled = true;

        // Reset the timer.
        timer = 0f;

        // Enable the lights.
        gunLight.enabled = true;
        faceLight.enabled = true;

        // Stop the particles from playing if they were, then start the particles.
        gunParticles.Stop();
        gunParticles.Play();

        // Enable the line renderer and set it's first position to be the end of the gun.
        gunLine.enabled = true;
        gunLine.SetPosition(0, transform.position);

        // Set the shootRay so that it starts at the end of the gun and points forward from the barrel.
        shootRay.origin = transform.position;
        shootRay.direction = transform.forward;

        // Perform the raycast against gameobjects on the shootable layer and if it hits something...
        if (Physics.Raycast(shootRay, out shootHit, range, shootableMask))
        {
            enemyTag = shootHit.collider.tag;
            Debug.Log(enemyTag);

            switch (enemyTag)
            {
                case "CCTV":
                    Debug.Log("CCTV HIT!");
                    int CCTV_ID = shootHit.collider.GetComponent<CCTV_ID>().getCCTVID();
                    cctvManager.ChangeColor(CCTV_ID);
                    break;

                case "Melee":
                    Debug.Log("Melee HIT!");
                    enemyHealthMelee = shootHit.collider.GetComponent<EnemyHealthMelee>();
                    enemyHealthMelee.TakeDamage(damagePerShot, shootHit.point);
                    break;

                case "Worker":
                    Debug.Log("Worker HIT!");
                    enemyHealthWorker = shootHit.collider.GetComponent<EnemyHealthWorker>();
                    enemyHealthWorker.TakeDamage(damagePerShot, shootHit.point);
                    break;

                case "Assault":
                    Debug.Log("Assault HIT!");
                    enemyHealthAssault = shootHit.collider.GetComponent<EnemyHealthAssault>();
                    enemyHealthAssault.TakeDamage(damagePerShot, shootHit.point);
                    break;

                case "Heavy":
                    Debug.Log("Heavy HIT!");
                    enemyHealthHeavy = shootHit.collider.GetComponent<EnemyHealthHeavy>();
                    enemyHealthHeavy.TakeDamage(damagePerShot, shootHit.point);
                    break;

                case "Critical":
                    string parentTag = shootHit.collider.gameObject.GetComponent<HeadshotParent>().getHeadshotTag();
                    ApplyHeadShotDamage(shootHit.collider.gameObject, parentTag);
                    headShotHit = true;
                    headshotSound.Play();
                    break;

            }

            //// Try and find an EnemyHealth script on the gameobject hit.
            //if (shootHit.collider != null)
            //{
            //    Debug.Log("no null!        " + shootHit.collider.gameObject.name);
            //}


            // Set the second position of the line renderer to the point the raycast hit.
            gunLine.SetPosition(1, shootHit.point);

        }
        // If the raycast didn't hit anything on the shootable layer...
        else
        {
            // ... set the second position of the line renderer to the fullest extent of the gun's range.
            gunLine.SetPosition(1, shootRay.origin + shootRay.direction * range);
        }

        //Update Energy Bar
        energyBar.fillAmount = (float)currentBullets / startingBullets;
        spectatorEnergyBar.fillAmount = (float)currentBullets / startingBullets;

        if (((float)currentBullets / startingBullets) < .2f && !isBlinking)
        {
            StartBlink();
            isBlinking = true;
            energyText.color = blinkingColor;
        }

        if (!headShotHit)
            bulletSound.Play();

        //Reset headShotHit value
        headShotHit = false;

    }

    void Reload()
    {
        currentBullets = 20;
        energyBar.fillAmount = currentBullets / startingBullets;
        spectatorEnergyBar.fillAmount = currentBullets / startingBullets;

        CancelInvoke("ToggleText");
        energyText.enabled = true;
        isBlinking = false;
        energyText.color = startingTextColor;


        if (reloadingSoundTimer >= timeBetweenReloadingSound && Time.timeScale != 0)
        {
            reloadingSoundTimer = 0.0f;
            reloadingSound.Play();
        }

    }

    private IEnumerator ReloadWaitTime()
    {
        yield return new WaitForSeconds(2.0f);
    }

    private void StartBlink()
    {
        InvokeRepeating("ToggleText", .5f, .5f);
    }

    private void ToggleText()
    {
        energyText.enabled = !energyText.enabled;
    }

    private void ApplyHeadShotDamage(GameObject enemy, string correctHealthComp)
    {

        GameObject parent = null;

        switch (correctHealthComp)
        {
            case "Melee":
                parent = FindParentWithTag(enemy, correctHealthComp);
                enemyHealthMelee = parent.GetComponent<EnemyHealthMelee>();
                enemyHealthMelee.TakeDamage(damagePerShot * 2, shootHit.point);
                break;

            case "Worker":
                parent = FindParentWithTag(enemy, correctHealthComp);
                enemyHealthWorker = parent.GetComponent<EnemyHealthWorker>();
                enemyHealthWorker.TakeDamage(damagePerShot * 2, shootHit.point);
                break;

            case "Assault":
                parent = FindParentWithTag(enemy, correctHealthComp);
                enemyHealthAssault = parent.GetComponent<EnemyHealthAssault>();
                enemyHealthAssault.TakeDamage(damagePerShot * 2, shootHit.point);
                break;

            case "Heavy":
                parent = FindParentWithTag(enemy, correctHealthComp);
                enemyHealthHeavy = parent.GetComponent<EnemyHealthHeavy>();
                enemyHealthHeavy.TakeDamage(damagePerShot * 2, shootHit.point);
                break;
        }
    }

    private GameObject FindParentWithTag(GameObject child, string tag)
    {
        Transform t = child.transform;
        while (t.parent != null)
        {
            if (t.parent.tag == tag)
            {
                return t.parent.gameObject;
            }

            t = t.parent.transform;
        }
        return null;
    }

}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CompleteProject
{
    public class EnemyAttackAssault : MonoBehaviour
    {

        UnityEngine.AI.NavMeshAgent nav;

        //Shooting variables
        public int damagePerShot = 20;                  // The damage inflicted by each bullet.
        public float timeBetweenBullets = 0.10f;        // The time between each shot.
        public float range = 100f;

        //Raycast variables
        float timer;                                    // A timer to determine when to fire.
        Ray shootRay = new Ray();                       // A ray from the gun end forwards.
        RaycastHit shootHit;                            // A raycast hit to get information about what was hit.
        int shootableMask;                              // A layer mask so the raycast only hits things on the shootable layer.
        ParticleSystem gunParticles;                    // Reference to the particle system.
        LineRenderer gunLine;                           // Reference to the line renderer.
        Light gunLight;                                 // Reference to the light component.
        public Light faceLight;                             // Duh
        float effectsDisplayTime = 0.2f;

        public bool playerInRange;
        private AudioSource shootSound;

        //Other components
        PlayerHealth playerHealth;
        GameObject player;
        public EnemyMovementAssault enemyMovement;

        //Motor and zone variables
        private BoxCollider boxCollider;
        private string zoneName = "";
        private string currentZoneStatus = "";
        private ZoneManager zoneManager;

        //Set ray variables
        void Awake()
        {
            gunParticles = GetComponent<ParticleSystem>();
            gunLine = GetComponent<LineRenderer>();
            gunLight = GetComponent<Light>();
            shootSound = GetComponent<AudioSource>();
            shootableMask = LayerMask.GetMask("Player");

            player = GameObject.Find("Player(Clone)");
            playerHealth = player.GetComponent<PlayerHealth>();

            boxCollider = GetComponent<BoxCollider>();
            zoneManager = GameObject.FindGameObjectWithTag("ZoneManager").GetComponent<ZoneManager>();

        }

        // Update is called once per frame
        void Update()
        {

            timer += Time.deltaTime;

            if (timer >= timeBetweenBullets && Time.timeScale != 0 && playerInRange && playerHealth.currentHealth > 0)
            {
                Shoot();
            }

            if (timer >= timeBetweenBullets * effectsDisplayTime)
            {
                DisableEffects();
            }

        }

        public void DisableEffects()
        {
            gunLine.enabled = false;
            faceLight.enabled = false;
            gunLight.enabled = false;
        }

        void Shoot()
        {
            timer = 0f;

            gunLight.enabled = true;
            faceLight.enabled = true;

            gunParticles.Stop();
            gunParticles.Play();

            gunLine.enabled = true;
            gunLine.SetPosition(0, transform.position);

            shootRay.origin = transform.position;
            shootRay.direction = transform.forward;

            if (Physics.Raycast(shootRay, out shootHit, range, shootableMask))
            {
                currentZoneStatus = zoneManager.DetermineCurrentEnemyZone(zoneName);

                gunLine.SetPosition(1, shootHit.point);
                enemyMovement.TriggerAttackAnim();
                Debug.Log("Player hit!");
                if (playerHealth.currentHealth > 0)
                {
                    playerHealth.TakeDamage(damagePerShot, currentZoneStatus, timeBetweenBullets);
                }
            }
            else
                gunLine.SetPosition(1, shootRay.origin + shootRay.direction * range);

            shootSound.Play();
        }

        public void ResetTimer()
        {
            //50% chance to reset timer - stun enemy when attacking
            timer = 0.0f;
        }

        private void OnTriggerEnter(Collider other)
        {
            //14 = damage zone layer int
            if (other.gameObject.layer == 14)
                zoneName = other.gameObject.name;
        }
    }
}



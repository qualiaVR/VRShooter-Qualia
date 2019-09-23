using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CompleteProject
{
    public class EnemyAttackMelee : MonoBehaviour
    {
        public float timeBetweenAttacks = 0.5f;     // The time in seconds between each attack.
        public int attackDamage = 10;               // The amount of health taken away per attack.

        UnityEngine.AI.NavMeshAgent nav;               // Reference to the nav mesh agent.

        public Transform trans;

        Animator anim;                              // Reference to the animator component.
        GameObject player;                          // Reference to the player GameObject.
        PlayerHealth playerHealth;                  // Reference to the player's health.
        EnemyHealthMelee enemyHealthMelee;                    // Reference to this enemy's health.
        public bool playerInRange;                         // Whether player is within the trigger collider and can be attacked.
        float timer;                                // Timer for counting up to the next attack.

        bool toggleOnce = false;

        AudioSource[] sounds;                     // Reference to the audio source.
        AudioSource attackClip;

        private bool stunned = false;
        private SphereCollider sphereCollider;

        private string zoneName = "";
        private string currentZoneStatus = "";
        private ZoneManager zoneManager;

        void Awake()
        {
            // Setting up the references.
            player = GameObject.Find("Player(Clone)");
            playerHealth = player.GetComponent<PlayerHealth>();
            enemyHealthMelee = GetComponent<EnemyHealthMelee>();
            anim = GetComponent<Animator>();

            sounds = GetComponents<AudioSource>();

            attackClip = sounds[1];
            nav = GetComponent<UnityEngine.AI.NavMeshAgent>();

            sphereCollider = GetComponent<SphereCollider>();
            zoneManager = GameObject.FindGameObjectWithTag("ZoneManager").GetComponent<ZoneManager>();
        }


        void Update()
        {

            // Add the time since Update was last called to the timer.
            timer += Time.deltaTime;

            // If the timer exceeds the time between attacks, the player is in range and this enemy is alive...
            if (timer >= timeBetweenAttacks && playerInRange && enemyHealthMelee.currentHealth > 0 && playerHealth.currentHealth > 0 && !stunned)
            {
                Attack();
                attackClip.Play();
            }

                if (playerHealth.currentHealth <= 0 && !toggleOnce)
                {
                toggleOnce = true;
                anim.SetTrigger("PlayerDead");
                anim.SetBool("PlayedOnce", true);
                }

        }

        void Attack()
        {
            currentZoneStatus = zoneManager.DetermineCurrentEnemyZone(zoneName);

            int randomAnim = Random.Range(1, 3);
            anim.SetTrigger("Attack"+randomAnim.ToString());

            // Reset the timer.
            timer = 0f;

            // If the player has health to lose...
            if (playerHealth.currentHealth > 0)
            {
                playerHealth.TakeDamage(attackDamage, currentZoneStatus, timeBetweenAttacks);
            }
        }

        public void ResetTimer()
        {
            //50% chance to reset timer - stun enemy when attacking
                timer = 0.0f;
        }

        public void StopAttacking()
        {
            ResetTimer();
            StartCoroutine(StunMechanic());
        }

        IEnumerator StunMechanic()
        {
            //Stun enemy, wait 1.5 seconds then remove stun
            stunned = true;
            yield return new WaitForSeconds(1.5f);
            RemoveStun();
        }

        private void RemoveStun()
        {
            stunned = false;
        }

        private void OnTriggerEnter(Collider other)
        {
            //14 = damage zone layer int
            if (other.gameObject.layer == 14)
                zoneName = other.gameObject.name;
        }

    }

}

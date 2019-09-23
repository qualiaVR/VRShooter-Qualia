using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CompleteProject
{
    public class EnemyAttackWorker : MonoBehaviour
    {
        public float timeBetweenAttacks = 0.5f;     // The time in seconds between each attack.
        public int attackDamage = 10;               // The amount of health taken away per attack.

        UnityEngine.AI.NavMeshAgent nav;               // Reference to the nav mesh agent.

        public Transform trans;

        Animator anim;                              // Reference to the animator component.
        GameObject player;                          // Reference to the player GameObject.
        PlayerHealth playerHealth;                  // Reference to the player's health.
        EnemyHealthWorker enemyHealthWorker;                    // Reference to this enemy's health.
        public bool playerInRange;                         // Whether player is within the trigger collider and can be attacked.
        float timer;                                // Timer for counting up to the next attack.

        AudioSource[] sounds;                     // Reference to the audio source.
        AudioSource attackClip;

        private SphereCollider sphereCollider;

        private string zoneName = "";
        private string currentZoneStatus = "";
        private ZoneManager zoneManager;

        private bool animOnce = false;

        void Awake()
        {
            // Setting up the references.
            player = GameObject.Find("Player(Clone)");
            playerHealth = player.GetComponent<PlayerHealth>();
            enemyHealthWorker = GetComponent<EnemyHealthWorker>();
            anim = GetComponent<Animator>();

            sounds = GetComponents<AudioSource>();

            attackClip = sounds[1];
            nav = GetComponent<UnityEngine.AI.NavMeshAgent>();
            zoneManager = GameObject.FindGameObjectWithTag("ZoneManager").GetComponent<ZoneManager>();
        }


        public void OnTriggerEnter(Collider other)
        {
            //14 = damage zone layer int
            if (other.gameObject.layer == 14)
                zoneName = other.gameObject.name;

            GameObject collisionObject = FindParentWithTag(other.gameObject, "Player");

            // If the entering collider is the player...
            if (collisionObject)
            {
                playerInRange = true;
                this.nav.enabled = false;
                gameObject.GetComponent<SphereCollider>().enabled = false;
            }
        }


        void OnTriggerExit(Collider other)
        {
            // If the exiting collider is the player...
            if (other.gameObject == player)
            {
                // ... the player is no longer in range.
                //playerInRange = false;
            }
        }


        void Update()
        {
            // Add the time since Update was last called to the timer.
            timer += Time.deltaTime;

            // If the timer exceeds the time between attacks, the player is in range and this enemy is alive...
            if (timer >= timeBetweenAttacks && playerInRange && enemyHealthWorker.currentHealth > 0 && playerHealth.currentHealth > 0)
            {
                // ... attack.
                Attack();
                attackClip.Play();
            }

            // If the player has zero or less health...
            if (playerHealth.currentHealth <= 0 && !animOnce)
            {
                // ... tell the animator the player is dead.
                animOnce = true;
                anim.SetTrigger("PlayerDead");
            }
        }


        void Attack()
        {
            currentZoneStatus = zoneManager.DetermineCurrentEnemyZone(zoneName);

            int randomAnim = Random.Range(1, 3);
            anim.SetTrigger("Attack" + randomAnim.ToString());

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
}

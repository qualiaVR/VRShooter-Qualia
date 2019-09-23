using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CompleteProject
{
    public class EnemyAttackSoldier : MonoBehaviour
    {

        public float timeBetweenAttacks = 0.5f;     // The time in seconds between each attack.
        public int attackDamage = 10;               // The amount of health taken away per attack.

        UnityEngine.AI.NavMeshAgent nav;               // Reference to the nav mesh agent.

        public Transform trans;

        Animator anim;                              // Reference to the animator component.
        GameObject player;                          // Reference to the player GameObject.
        public PlayerHealth playerHealth;                  // Reference to the player's health.
        EnemyHealth enemyHealth;                    // Reference to this enemy's health.
        public bool playerInRange;                         // Whether player is within the trigger collider and can be attacked.
        float timer;                                // Timer for counting up to the next attack.

        AudioSource[] sounds;                     // Reference to the audio source.
        AudioSource attackClip;

        PlayerHealth playerHealthSoldierRef;

        void Awake()
        {
            // Setting up the references.
            player = GameObject.FindGameObjectWithTag("Player");
            playerHealthSoldierRef = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>();

            enemyHealth = GetComponent<EnemyHealth>();
            anim = GetComponent<Animator>();

            sounds = GetComponents<AudioSource>();

            attackClip = sounds[1];
            nav = GetComponent<UnityEngine.AI.NavMeshAgent>();
        }


        public void OnTriggerEnter(Collider other)
        {
            if (other.gameObject == player)
            {
                Debug.Log("Player is in range!");
                playerInRange = true;
                this.nav.enabled = false;
            }
        }


        void Update()
        {
            // Add the time since Update was last called to the timer.
            timer += Time.deltaTime;

            // If the timer exceeds the time between attacks, the player is in range and this enemy is alive
            if (timer >= timeBetweenAttacks && playerInRange && enemyHealth.currentHealth > 0 && playerHealthSoldierRef.currentHealth > 0)
            {
                Idle();
                attackClip.Play();
            }

            // If the player has zero or less health...
            if (playerHealthSoldierRef.currentHealth <= 0)
            {
                anim.SetTrigger("PlayerDead");
            }
        }

        //Scifi Soldier animation cannot go from running to attacking instantly - it must go through another animation
        //We'll use the idle animation to immediately stop the running animation and then trigger the attack animation
        void Idle()
        {
            anim.SetTrigger("Idle");
            Attack();
        }

        void Attack()
        {
            anim.SetTrigger("Attack");
            // Reset the timer.
            timer = 0f;

            // If the player has health to lose...
            if (playerHealthSoldierRef.currentHealth > 0)
            {
                // ... damage the player.
                //playerHealthSoldierRef.TakeDamage(attackDamage);
            }
        }
    }

}



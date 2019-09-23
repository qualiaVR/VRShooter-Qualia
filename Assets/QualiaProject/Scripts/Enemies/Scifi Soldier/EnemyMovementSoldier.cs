using UnityEngine;
using System.Collections;

namespace CompleteProject
{
    public class EnemyMovementSoldier : MonoBehaviour
    {
        Transform player;               // Reference to the player's position.
        PlayerHealth playerHealth;      // Reference to the player's health.
        EnemyHealth enemyHealth;        // Reference to this enemy's health.
        UnityEngine.AI.NavMeshAgent nav;               // Reference to the nav mesh agent.
        EnemyAttackSoldier enemyAttackSoldier;
        Rigidbody rigidbody;


        void Awake()
        {
            // Set up the references.
            rigidbody = gameObject.GetComponent<Rigidbody>();
            player = GameObject.FindGameObjectWithTag("Player").transform;
            playerHealth = player.GetComponent<PlayerHealth>();
            enemyHealth = GetComponent<EnemyHealth>();
            enemyAttackSoldier = GetComponent<EnemyAttackSoldier>();
            nav = GetComponent<UnityEngine.AI.NavMeshAgent>();
        }


        void Update()
        {
            // Walk towards the player if both zombie and player are alive AND the zombie is not in range
            if ((enemyHealth.currentHealth > 0) && (playerHealth.currentHealth > 0) && !enemyAttackSoldier.playerInRange)
                nav.SetDestination(player.position);
            // Otherwise...
            else
            //Disable navigation agent if zombie is within player range
            {
                nav.enabled = false;
                rigidbody.detectCollisions = false;
            }
                

        }
    }
}
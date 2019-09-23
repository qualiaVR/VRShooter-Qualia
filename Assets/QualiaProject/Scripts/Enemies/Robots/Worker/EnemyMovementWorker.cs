using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CompleteProject
{
    public class EnemyMovementWorker : MonoBehaviour
    {
        public GameObject player;
        Transform playerTransform;                               // Reference to the player's position.
        PlayerHealth playerHealth;                      // Reference to the player's health.
        EnemyHealthWorker enemyHealthWorker;                        // Reference to this enemy's health.
        UnityEngine.AI.NavMeshAgent nav;               // Reference to the nav mesh agent.
        EnemyAttackWorker enemyAttackWorker;


        void Awake()
        {
            // Set up the references.
            player = GameObject.Find("Player(Clone)");
            playerHealth = player.GetComponent<PlayerHealth>();
            playerTransform = player.transform;
            enemyHealthWorker = GetComponent<EnemyHealthWorker>();
            enemyAttackWorker = GetComponent<EnemyAttackWorker>();
            nav = GetComponent<UnityEngine.AI.NavMeshAgent>();
        }


        void Update()
        {
            // Walk towards the player if both zombie and player are alive AND the zombie is not in range
            if ((enemyHealthWorker.currentHealth > 0) && (playerHealth.currentHealth > 0) && !enemyAttackWorker.playerInRange)
                nav.SetDestination(playerTransform.position);
            // Otherwise...
            else
                //    //Disable navigation agent if zombie is within player range
                nav.enabled = false;

        }


        public void PauseMesh()
        {
            StopAllCoroutines();
            if (nav.enabled)
            {
                nav.isStopped = true;
                StartCoroutine(WaitFor1Second());
            }
        }

        public void ResumeMesh()
        {
            if (nav.enabled)
                nav.isStopped = false;
        }

        IEnumerator WaitFor1Second()
        {
            yield return new WaitForSeconds(1f);
            ResumeMesh();
        }

    }
}

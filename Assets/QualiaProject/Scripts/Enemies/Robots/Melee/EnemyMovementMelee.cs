using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CompleteProject
{
    public class EnemyMovementMelee : MonoBehaviour
    {
        Transform player;                               // Reference to the player's position.
        PlayerHealth playerHealth;                      // Reference to the player's health.
        EnemyHealthMelee enemyHealthMelee;                        // Reference to this enemy's health.
        UnityEngine.AI.NavMeshAgent nav;               // Reference to the nav mesh agent.
        EnemyAttackMelee enemyAttackMelee;

        Animator anim;

        void Start()
        {
            // Set up the references.
            player = GameObject.Find("Player(Clone)").transform;
            playerHealth = player.GetComponent<PlayerHealth>();
            enemyHealthMelee = GetComponent<EnemyHealthMelee>();
            enemyAttackMelee = GetComponent<EnemyAttackMelee>();
            nav = GetComponent<UnityEngine.AI.NavMeshAgent>();
            anim = GetComponent<Animator>();
        }


        void Update()
        {
            // Walk towards the player if both zombie and player are alive AND the zombie is not in range
            if ((enemyHealthMelee.currentHealth > 0) && (playerHealth.currentHealth > 0) && !enemyAttackMelee.playerInRange)
                nav.SetDestination(player.position);
            // Otherwise...
            else
                //    //Disable navigation agent if zombie is within player range
                nav.enabled = false;

        }


        public void PauseMesh()
        { 
            //Disable nav mesh so enemy stops moving
            if (nav.enabled)
            {
                Debug.Log("Mesh paused, waiting for 1.5 seconds");
                nav.isStopped = true;
                StartCoroutine(WaitFor1Second());
            }
        }

        public void ResumeMesh()
        {
            if (!nav.enabled || !enemyAttackMelee.playerInRange) {
                nav.isStopped = false;
                anim.SetTrigger("Walk");
                Debug.Log("Mesh resumed");
            }
                
        }

        IEnumerator WaitFor1Second()
        {
            yield return new WaitForSeconds(1.5f);
            ResumeMesh();
        }

        public void OnTriggerEnter(Collider other)
        {
            GameObject collisionObject = FindParentWithTag(other.gameObject, "Player");

            // If the entering collider is the player...
            if (collisionObject)
            {
                enemyAttackMelee.playerInRange = true;
                this.nav.enabled = false;
                gameObject.GetComponent<SphereCollider>().enabled = false;
                anim.SetTrigger("Attack");
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

        public void TriggerStunAnimation()
        {
            anim.SetTrigger("Stun");
            Debug.Log("Melee stunned!");

            if (!enemyAttackMelee.playerInRange) {
                Debug.Log("Pausing mesh - not in player range");
                PauseMesh();
            }
            else if (enemyAttackMelee.playerInRange)
            {
                Debug.Log("Not attacking - stunned and in player range.");
                enemyAttackMelee.StopAttacking();
            }

        }

    }
}

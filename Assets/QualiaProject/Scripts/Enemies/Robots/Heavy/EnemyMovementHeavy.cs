using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CompleteProject
{
    public class EnemyMovementHeavy : MonoBehaviour
    {
        Transform player;                               // Reference to the player's position.
        PlayerHealth playerHealth;                      // Reference to the player's health.
        EnemyHealthHeavy enemyHealthHeavy;                        // Reference to this enemy's health.
        UnityEngine.AI.NavMeshAgent nav;               // Reference to the nav mesh agent.
        public EnemyAttackHeavy enemyAttackHeavy;

        Animator anim;

        void Awake()
        {
            // Set up the references
            player = GameObject.Find("Player(Clone)").transform;
            playerHealth = player.GetComponent<PlayerHealth>();
            enemyHealthHeavy = GetComponent<EnemyHealthHeavy>();
            nav = GetComponent<UnityEngine.AI.NavMeshAgent>();
            anim = GetComponent<Animator>();
        }

        void Update()
        {
            if ((enemyHealthHeavy.currentHealth > 0) && (playerHealth.currentHealth > 0) && !enemyAttackHeavy.playerInRange)
                nav.SetDestination(player.position);
            else
                //Disable navigation agent if zombie is within player range
                nav.enabled = false;
        }


        public void PauseMesh()
        {
            StopAllCoroutines();

            //Disable nav mesh so enemy stops moving
            if (nav.enabled)
            {
                nav.isStopped = true;
                StartCoroutine(WaitFor1Second());
            }
        }

        public void ResumeMesh()
        {
            if (!nav.enabled || !enemyAttackHeavy.playerInRange) {
                nav.isStopped = false;
                anim.SetTrigger("Walk");
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
                enemyAttackHeavy.playerInRange = true;
                this.nav.enabled = false;
                gameObject.GetComponent<BoxCollider>().enabled = false;
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

        public void TriggerStunAnimation() {
            anim.SetTrigger("Stun");
            

            if (!enemyAttackHeavy.playerInRange)
            {
                PauseMesh();
            } else if (enemyAttackHeavy.playerInRange)
            {
                enemyAttackHeavy.StopAttacking();
            }

        }


    }
}

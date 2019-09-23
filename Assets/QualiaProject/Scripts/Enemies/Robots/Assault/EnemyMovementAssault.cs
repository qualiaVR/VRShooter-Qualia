using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CompleteProject
{
    public class EnemyMovementAssault : MonoBehaviour
    {
        Transform player;                               // Reference to the player's position.
        PlayerHealth playerHealth;                      // Reference to the player's health.
        EnemyHealthAssault enemyHealthAssault;                        // Reference to this enemy's health.
        UnityEngine.AI.NavMeshAgent nav;               // Reference to the nav mesh agent.
        public EnemyAttackAssault enemyAttackAssault;

        Animator anim;


        void Start()
        {
            // Set up the references.
            //player = GameObject.FindGameObjectWithTag("Player").transform;
            player = GameObject.Find("Player(Clone)").transform;
            playerHealth = player.GetComponent<PlayerHealth>();
            enemyHealthAssault = GetComponent<EnemyHealthAssault>();
            nav = GetComponent<UnityEngine.AI.NavMeshAgent>();
            anim = GetComponent<Animator>();
        }


        void Update()
        {
            if ((enemyHealthAssault.currentHealth > 0) && (playerHealth.currentHealth > 0) && !enemyAttackAssault.playerInRange)
                nav.SetDestination(player.position);
            else
                //Disable navigation agent if zombie is within player range
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

        public void OnTriggerEnter(Collider other)
        {
            GameObject collisionObject = FindParentWithTag(other.gameObject, "Player");

            // If the entering collider is the player...
            if (collisionObject)
            {
                Debug.Log("Collided with player!");
                enemyAttackAssault.playerInRange = true;
                this.nav.enabled = false;
                gameObject.GetComponent<BoxCollider>().enabled = false;
                
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

        public void TriggerAttackAnim()
        {
            anim.SetTrigger("Attack");
        }

    }
}

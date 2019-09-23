using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CompleteProject
{
    public class EnemyHealthMelee : MonoBehaviour
    {
        public int startingHealth = 100;            // The amount of health the enemy starts the game with.
        public int currentHealth;                   // The current health the enemy has.
        public float sinkSpeed = 1f;              // The speed at which the enemy sinks through the floor when dead.
        public int scoreValue = 10;                 // The amount added to the player's score when the enemy dies.
        public AudioClip deathClip;                 // The sound to play when the enemy dies.

        private WaveManager waveManager;
        private UIScoreManager scoreManager;

        Animator anim;                              // Reference to the animator.
        AudioSource enemyAudio;                     // Reference to the audio source.
        //ParticleSystem hitParticles;                // Reference to the particle system that plays when the enemy is damaged.
        CapsuleCollider capsuleCollider;            // Reference to the capsule collider.
        bool isDead;                                // Whether the enemy is dead.
        bool isSinking;                             // Whether the enemy has started sinking through the floor.

        private float hurtCryTimer;
        private float timeBetweenHurtCry;

        public Image enemyHealthBar;

        float timer;

        EnemyMovementMelee enemyMovementMelee;
        EnemyAttackMelee enemyAttackMelee;


        void Awake()
        {
            timeBetweenHurtCry = 2f;

            enemyMovementMelee = GetComponent<EnemyMovementMelee>();
            enemyAttackMelee = GetComponent<EnemyAttackMelee>();


            // Setting up the references.
            anim = GetComponent<Animator>();
            enemyAudio = GetComponent<AudioSource>();
            capsuleCollider = GetComponent<CapsuleCollider>();
            waveManager = GameObject.FindGameObjectWithTag("WaveManager").GetComponent<WaveManager>();
            scoreManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<UIScoreManager>();

            // Setting the current health when the enemy first spawns.
            currentHealth = startingHealth;
        }


        void Update()
        {

            timer += Time.deltaTime;
            hurtCryTimer += Time.deltaTime;

            // If the enemy should be sinking...
            if (isSinking)
            {
                // ... move the enemy down by the sinkSpeed per second.
                transform.Translate(-Vector3.up * sinkSpeed * Time.deltaTime);
            }
        }


        public void TakeDamage(int amount, Vector3 hitPoint)
        {
            // If the enemy is dead...
            if (isDead)
                // ... no need to take damage so exit the function.
                return;

            // Play the hurt sound effect
            if (hurtCryTimer >= timeBetweenHurtCry && Time.timeScale != 0)
            {
                enemyAudio.Play();
                hurtCryTimer = 0.0f;
            }

            // Reduce the current health by the amount of damage sustained.
            currentHealth -= amount;
            enemyHealthBar.fillAmount = ((float)currentHealth / startingHealth);

            // If the current health is less than or equal to zero...
            if (currentHealth <= 0)
            {
                
                // ... the enemy is dead.
                Death();
                waveManager.SubstractCurrentEnemy();
            }
            else //10% chance to stun enemy while running
                if (Random.Range(1, 11) == 1)
            {
                enemyMovementMelee.TriggerStunAnimation();
            }

        }

        void Death()
        {
            // The enemy is dead.
            isDead = true;

            // Turn the collider into a trigger so shots can pass through it.
            capsuleCollider.isTrigger = true;

            int randomAnim = Random.Range(1, 4);

            // Tell the animator that the enemy is dead.
            anim.SetTrigger("Dead"+randomAnim.ToString());
            
            //anim.Play("Dead" + randomAnim.ToString(), 0, 0);
            anim.SetBool("isDead", isDead);

            // Change the audio clip of the audio source to the death clip and play it (this will stop the hurt clip playing).
            enemyAudio.clip = deathClip;
            enemyAudio.Play();

            StartSinking();
        }


        public void StartSinking()
        {
            //Debug.Log("I'M SINKING");

            // Find and disable the Nav Mesh Agent.
            GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = false;

            // Find the rigidbody component and make it kinematic (since we use Translate to sink the enemy).
            GetComponent<Rigidbody>().isKinematic = true;

            // The enemy should no sink.
            isSinking = true;



            // Increase the score by the enemy's score value.
            //ScoreManager.score += scoreValue;
            //Debug.Log("Adding " + scoreValue + " to score.");
            scoreManager.waveScore += scoreValue;
            scoreManager.zombiesKilled++;

            // After 2 seconds destory the enemy.
            Destroy(gameObject, 2f);
        }
    }
}

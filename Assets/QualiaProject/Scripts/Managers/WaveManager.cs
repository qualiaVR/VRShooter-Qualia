using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CompleteProject
{
    public class WaveManager : MonoBehaviour
    {
        //Debug variables
        public bool debugMode = false;
        public string comPort = "COM3";

        //Game control variables
        public bool playWithSerial = false;
        private Wave[] wave;
        private bool gameWon = false;
        private bool waveStarted = false;
        private bool waitingForNextWave = false;
        private int currentWave = 0;
        private int totalWaves;

        //Enemy variables
        private GameObject easyEnemy;
        private GameObject midEnemy;
        private GameObject hardEnemy;

        private GameObject[] enemySpawnLocations;

        //Total number of enemies per difficulty per wave
        private int totalEasy;
        private int totalMid;
        private int totalHard;

        //Wave variables
        private int totalWaveEnemies;
        private int enemiesToSpawn;
        private int currentEnemies;
        private float timeBetweenWaves = 11.0f;

        private AudioSource waveAdvanceJingle;
        private UIScoreManager scoreManager;

        private PlayerHealth playerHealth;
        private PlayerWeapons playerWeapons;
        public GameObject playerPrefab;
        public GameObject playerPosition;
        private GameObject player;

        private bool playOnce = false;

        // Get waves and enemy locations and start game
        void Awake()
        {
            PlayerSetUp playerSetUp = gameObject.GetComponent<PlayerSetUp>();
            waveAdvanceJingle = GameObject.FindGameObjectWithTag("GameManager").GetComponent<AudioSource>();
            scoreManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<UIScoreManager>();

            player = (GameObject) Instantiate(playerPrefab, playerPosition.transform, true);
            playerHealth = player.GetComponent<PlayerHealth>();
            playerWeapons = player.GetComponent<PlayerWeapons>();

            playerHealth.RightController = playerSetUp.rightController;
            playerHealth.healthBar = playerSetUp.healthBar;
            playerHealth.spectatorHealthBar = playerSetUp.spectatorHealthBar;
            playerHealth.backgroundBar = playerSetUp.backgroundHealthBar;
            playerHealth.healthText = playerSetUp.healthText;
            playerHealth.enemiesRemainingText = playerSetUp.enemiesRemainingText;

            playerWeapons.laserRifle = playerSetUp.laserRifle;
            playerWeapons.sniperRifle = playerSetUp.sniperRifle;
            playerWeapons.shootManager = playerSetUp.shootManager;

            wave = gameObject.GetComponents<Wave>();
            totalWaves = wave.Length;
            enemySpawnLocations = GameObject.FindGameObjectsWithTag("EnemySpawn");

            if (!debugMode) { StartWave(); }
                
        }

        // Update is called once per frame
        void Update()
        {
            if (debugMode)
                return;
            else
            {
                CheckIfGameIsWon();

                //Check every frame if game is won, if not, advance to next wave
                if (waveStarted == true && currentEnemies == 0 && !waitingForNextWave && !gameWon)
                {
                    currentWave++;

                    //If the game isn't over yet, start next waiting time for next wave
                    if (!gameWon)
                    {
                        waveAdvanceJingle.Play();
                        waitingForNextWave = true;
                        WaitForNextWave();
                    }
                }

                if (gameWon && !playOnce)
                {
                    waveAdvanceJingle.Play();
                    Debug.Log("VICTORY ACHIEVED!");
                    scoreManager.StartFadeIn();
                    playOnce = true;
                }
            }

        }

        private void StartWave()
        {
            Debug.Log("Starting wave #" + currentWave);

            waitingForNextWave = false;

            //Get the enemies of the wave and their quantities
            easyEnemy = wave[currentWave].easyEnemy;
            totalEasy = wave[currentWave].numberOfEasy;

            midEnemy = wave[currentWave].midEnemy;
            totalMid = wave[currentWave].numberOfMid;

            hardEnemy = wave[currentWave].hardEnemy;
            totalHard = wave[currentWave].numberOfHard;

            //Get the total amount of enemies
            totalWaveEnemies = totalEasy + totalMid + totalHard;
            currentEnemies = enemiesToSpawn = totalWaveEnemies;

            waveStarted = true;

            UpdateEnemiesRemainingText();

            if (currentWave != 0) {
                if ((playerHealth.currentHealth + 50) > 250) {
                    playerHealth.HealPlayerComplete();
                }
                else
                    playerHealth.HealPlayer(50);
            }
                

            EnemySpawning();

        }

        private void EnemySpawning()
        {
            //Pick randomly an enemy spawn
            //Random.Range with integers is exclusive, which means it will never return the max value
            StartCoroutine(FixedWait());
        }


        GameObject PickEnemyDifficulty()
        {
            GameObject enemy = null;
            bool enemyChosen = false;
            int chosenEnemy;

            //Choose enemy
            while (!enemyChosen)
            {
                //Pick a random number between 1 and 3 (1 = easy, 2 = medium, 3 = hard)
                chosenEnemy = Random.Range(1, 4);

                //Check if there are still available enemies from that difficulty to spawn
                switch (chosenEnemy)
                {

                    //Easy case
                    case 1:
                        //If we still have easy enemies to spawn, spawn it and decrease easy enemy counter
                        if (totalEasy > 0)
                        {
                            totalEasy--;
                            enemyChosen = true;
                            enemy = easyEnemy;
                            break;
                        }
                        else
                            break;

                    //Mid case
                    case 2:
                        if (totalMid > 0)
                        {
                            totalMid--;
                            enemyChosen = true;
                            enemy = midEnemy;
                            break;
                        }
                        else
                            break;

                    case 3:
                        //Hard case
                        if (totalHard > 0)
                        {
                            totalHard--;
                            enemyChosen = true;
                            enemy = hardEnemy;
                            break;
                        }
                        else
                            break;

                }

            }

            return enemy;
        }

        private void CheckIfGameIsWon()
        {
            if (currentWave == totalWaves)
            {
                gameWon = true;
            }

        }

        private void WaitForNextWave()
        {
            StartCoroutine(WaitForWave(timeBetweenWaves));
            waveStarted = false;
        }



        //COROUTINES

        IEnumerator WaitForWave(float timeToWait)
        {
            Debug.Log("Waiting for wave #" + (currentWave));
            scoreManager.StartFadeIn();
            yield return new WaitForSeconds(timeToWait);
            scoreManager.currentWave++;

            if (wave[currentWave] != null)
                StartWave();
        }

        IEnumerator FixedWait()
        {
            yield return new WaitForSeconds(3.0f);
            Debug.Log("Waited 3 seconds");
            StartCoroutine(SpawningCouroutine());
        }

        IEnumerator SpawningCouroutine()
        {

            GameObject enemyToSpawn = null;
            int spawnIndex = 0;

            //Spawn enemies until there are no more
            while (enemiesToSpawn > 0)
            {
                enemyToSpawn = PickEnemyDifficulty();

                //Pick random spawn to spawn enemy
                spawnIndex = Random.Range(0, enemySpawnLocations.Length);

                //Spawn the enemy and subtract from total enemy count
                Instantiate(enemyToSpawn, enemySpawnLocations[spawnIndex].gameObject.transform.position, enemySpawnLocations[spawnIndex].gameObject.transform.rotation);
                //Debug.Log("Chose " + enemySpawnLocations[spawnIndex].name);
                enemiesToSpawn--;

                //Spawn enemy then wait between 2 and 7 seconds before next spawn
                yield return new WaitForSeconds(Random.Range(2, 7));
            }

        }





        //METHODS USED BY OTHER SCRIPTS


        //Method used by enemy health to subtract enemy counter
        public void SubstractCurrentEnemy()
        {
            currentEnemies--;
            UpdateEnemiesRemainingText();
            Debug.Log(currentEnemies + " enemies remaining in wave " + (currentWave + 1));
        }

        public void UpdateEnemiesRemainingText()
        {
            playerHealth.UpdateEnemyCounter(currentEnemies);
        }

    }
}


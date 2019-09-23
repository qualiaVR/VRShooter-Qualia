using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.IO.Ports;

namespace CompleteProject
{
    public class PlayerHealth : MonoBehaviour
    {
        // VR variables
        public GameObject RightController;

        // Health variables
        public int startingHealth = 100;                            // The amount of health the player starts the game with.
        public int currentHealth;                                   // The current health the player has.
        public AudioClip deathClip;                                 // The audio clip to play when the player dies.


        Animator anim;                                              // Reference to the Animator component.
        AudioSource playerAudio;                                    // Reference to the AudioSource component.
        PlayerMovement playerMovement;                              // Reference to the player's movement.
        PlayerShooting playerShooting;                              // Reference to the PlayerShooting script.
        PlayerShootingSniper playerShootingSniper;                  // Reference to the PlayerShooting script.
        bool isDead;                                                // Whether the player is dead.

        //Health Bar
        [Header("Health Bar UI")]
        public Image healthBar;
        public Image spectatorHealthBar;
        public Image backgroundBar;
        public Text healthText;
        public Text enemiesRemainingText;

        private Color backgroundWarningColor = new Color(255.0f / 255.0f, 236.0f / 255.0f, 79f / 255.0f, 255.0f / 255.0f);
        private Color backgroundDangerColor = new Color(255.0f / 255.0f, 90f / 255.0f, 93f / 255.0f, 255.0f / 255.0f);

        private Color warningColor = new Color(255.0f / 255.0f, 195f / 255.0f, 0f / 255.0f, 255.0f / 255.0f);
        private Color dangerColor = new Color(128.0f / 255.0f, 29f / 255.0f, 29f / 255.0f, 255.0f / 255.0f);

        private bool isWarning = false;
        private bool isDanger = false;

        //Player Area - Camera rig object
        /* private SteamVR_PlayArea playArea;
         * public MeshRenderer healthArea;
         * public Material[] healthTextures; */

        private Color blueColor = new Color(0f / 255.0f, 0f / 255.0f, 0f / 255.0f, 255.0f / 255.0f);
        private Color damageColor = new Color(255.0f / 255.0f, 0f / 255.0f, 0f / 255.0f, 255.0f / 255.0f);

        //Damage motors
        public SerialPort serial;
        
        [Header("Motors State")]
        public bool activatingFrontMotor = false;
        public bool activatingBackMotor = false;
        public bool activatingLeftMotor = false;
        public bool activatingRightMotor = false;

        private PlayerWeapons playerWeapons;
        private string comPort = "COM3";
        private bool playWithSerial = false;

        void Start ()
        {
            // Setting up the references.
            anim = GetComponent <Animator> ();
            playerAudio = GetComponent <AudioSource> ();
            playerMovement = GetComponent <PlayerMovement> ();
            playerShooting = RightController.GetComponentInChildren<PlayerShooting>();
            playerShootingSniper = RightController.GetComponentInChildren<PlayerShootingSniper>();
            playerWeapons = this.gameObject.GetComponent<PlayerWeapons>();

            WaveManager waveManager = GameObject.FindGameObjectWithTag("WaveManager").GetComponent<WaveManager>();

            if (waveManager) {
                playWithSerial = waveManager.playWithSerial;
                comPort = waveManager.comPort;
            }
                

            if(playWithSerial)
            {
                serial = new SerialPort(comPort, 9600);

                if (!serial.IsOpen)
                {
                    try { serial.Open(); Debug.Log("Connected to serial!"); }
                    catch (System.Exception e)
                    {
                        Debug.Log(e);
                    }
                }
            }

            // Set the initial health of the player.
            currentHealth = startingHealth;

            // playArea = GetComponentInChildren<SteamVR_PlayArea>();
        }


        void Update()
        {
                
        }


        public void TakeDamage(int amount, string currentZone, float timeBetweenMotorActivations)
        {
            //StopAllCoroutines();
            //StopCoroutine(ChangePlayAreaColor());
            //StartCoroutine(ChangePlayAreaColor());

            if (playWithSerial)
                ActivateMotors(currentZone, timeBetweenMotorActivations);

            // Reduce the current health by the damage amount.
            currentHealth -= amount;

            healthBar.fillAmount = ((float)currentHealth / startingHealth);
            spectatorHealthBar.fillAmount = ((float)currentHealth / startingHealth);
            ChangeHealthBarColor();

            // Play the hurt sound effect.
            playerAudio.Play ();

            // If the player has lost all it's health and the death flag hasn't been set yet...
            if(currentHealth <= 0 && !isDead)
            {
                // ... it should die.
                Death ();
            }
        }

        public void HealPlayer(int amount)
        {
            // Add the current health by the heal amount.
            currentHealth += amount;

            healthBar.fillAmount = ((float)currentHealth / startingHealth);
            spectatorHealthBar.fillAmount = ((float)currentHealth / startingHealth);
            ChangeHealthBarColor();

        }

        public void HealPlayerComplete()
        {
            // Add the current health by the heal amount.
            currentHealth = 250;

            healthBar.fillAmount = ((float)currentHealth / startingHealth);
            spectatorHealthBar.fillAmount = ((float)currentHealth / startingHealth);
            ChangeHealthBarColor();
        }

        private void ChangeHealthBarColor()
        {
            if (currentHealth < 50 && currentHealth > 25)
            {
                healthBar.color = warningColor;
                backgroundBar.color = backgroundWarningColor;

                if(!isWarning)
                {
                    StartBlink(.5f, .7f);
                    isWarning = true;
                    healthText.color = warningColor;
                }
                
            }
                

            if ((float)currentHealth < 25)
                {
                healthBar.color = dangerColor;
                backgroundBar.color = backgroundDangerColor;

                if(!isDanger)
                {
                    CancelInvoke("ToggleText");
                    StartBlink(.3f, .3f);
                    isDanger = true;
                    healthText.color = dangerColor;
                    healthText.text = "LOW\nHEALTH";
                    healthText.fontSize = 13;
                }

                
            }
                
        }

        private void StartBlink(float playTime, float repeatTime)
        {
            InvokeRepeating("ToggleText", playTime, repeatTime);
        }

        private void ToggleText()
        {
            healthText.enabled = !healthText.enabled;
        }

        void Death ()
        {
            // Set the death flag so this function won't be called again.
            isDead = true;

            // Turn off any remaining shooting effects.
            playerWeapons.DisableWeapons();

            // Set the audiosource to play the death clip and play it (this will stop the hurt sound from playing).
            playerAudio.clip = deathClip;
            playerAudio.Play ();

            // Turn off the movement and shooting scripts.
            //playerMovement.enabled = false;
            playerShooting.enabled = false;
        }

        public void UpdateEnemyCounter(int enemiesRemaining)
        {
            enemiesRemainingText.text = enemiesRemaining.ToString();
        }

        public void RestartLevel ()
        {
            // Reload the level that is currently loaded.
            //SceneManager.LoadScene (0);
        }

        private void OnApplicationQuit()
        {
            if(playWithSerial)
            {
                serial.Write("0");
                serial.Close();
                Debug.Log("serial closed");
            }
        }

        private void ActivateMotors(string currentZone, float timeBetweenMotors)
        {
            Debug.Log("Activating motors method!");

            //Determine which motor must be activated
            switch(currentZone)
            {
                case "forward":
                    if(!activatingFrontMotor)
                        StartCoroutine(TimeBetweenMotorsFront(timeBetweenMotors));
                    break;
                case "back":
                    if(!activatingBackMotor)
                        StartCoroutine(TimeBetweenMotorsBack(timeBetweenMotors));
                    break;
                case "left":
                    if (!activatingLeftMotor)
                        StartCoroutine(TimeBetweenMotorsLeft(timeBetweenMotors));
                    break;
                case "right":
                    if (!activatingRightMotor)
                        StartCoroutine(TimeBetweenMotorsRight(timeBetweenMotors));
                    break;
            }


          
        }

        IEnumerator TimeBetweenMotorsFront(float waitTime)
        {
            activatingFrontMotor = true;
            serial.Write("4");
            yield return new WaitForSeconds(waitTime);
            activatingFrontMotor = false;
        }

        IEnumerator TimeBetweenMotorsBack(float waitTime)
        {
            activatingBackMotor = true;
            serial.Write("5");
            yield return new WaitForSeconds(waitTime);
            activatingBackMotor = false;
        }

        IEnumerator TimeBetweenMotorsLeft(float waitTime)
        {
            activatingLeftMotor = true;

            //Do nothing if both front and back motors are active
            //Activate left back motor if back motors are not being activated
            if (!activatingBackMotor)
                serial.Write("6");
            //Activate left front motor if front motors are not being activated
            //else if (!activatingFrontMotor)
            //{
            //    Debug.Log("Activating only left front motor!");
            //    serial.Write("8");
            //}
                
            yield return new WaitForSeconds(waitTime);
            activatingLeftMotor = false;
        }

        IEnumerator TimeBetweenMotorsRight(float waitTime)
        {
            activatingRightMotor = true;

            //Do nothing if both front and back motors are active
            //Activate right back motor if back motors are not being activated
            if (!activatingBackMotor)
                serial.Write("7");
            //Activate right front motor if front motors are not being activated
            //else if (!activatingFrontMotor)
            //    serial.Write("9");

            yield return new WaitForSeconds(waitTime);
            activatingRightMotor = false;
        }


        /* IEnumerator ChangePlayAreaColor()
        {
            playArea.color = damageColor;
            healthArea.material = healthTextures[1];
            yield return new WaitForSeconds(1f);
            playArea.color = blueColor;
            healthArea.material = healthTextures[0];
            yield return new WaitForSeconds(.5f);
        }  */

    }
}
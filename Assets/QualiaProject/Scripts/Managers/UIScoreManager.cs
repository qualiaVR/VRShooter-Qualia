using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIScoreManager : MonoBehaviour {

    //Score image and it's components
    public Image scoreRoot;
    public Text scoreTitles;

    public Text zKilled;
    public Text wScore;
    public Text tScore;
    public Text waveText;

    //Scores
    private int totalScore = 0;
    public int waveScore = 0;
    public int zombiesKilled = 0;

    public int currentWave = 1;

    private float timeBetweenFades = 0.6f;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void StartFadeIn()
    {
        UpdateScoreTexts();
        StartCoroutine(FadeInScores());
    }

    public void StartFadeOut()
    {
        StartCoroutine(FadeOutScores());
    }

    IEnumerator FadeInScores()
    {
        //Root image
        yield return new WaitForSeconds(timeBetweenFades * 2);
        scoreRoot.gameObject.SetActive(true);

        //Text
        yield return new WaitForSeconds(timeBetweenFades*2);
        scoreTitles.gameObject.SetActive(true);
        yield return new WaitForSeconds(timeBetweenFades);
        zKilled.gameObject.SetActive(true);
        yield return new WaitForSeconds(timeBetweenFades);
        wScore.gameObject.SetActive(true);
        yield return new WaitForSeconds(timeBetweenFades);
        tScore.gameObject.SetActive(true);
        yield return new WaitForSeconds(timeBetweenFades);

        yield return new WaitForSeconds(3.0f);

        StartFadeOut();
    }

    IEnumerator FadeOutScores()
    {
        yield return new WaitForSeconds(timeBetweenFades * 3);
        scoreRoot.gameObject.SetActive(false);
        scoreTitles.gameObject.SetActive(false);
        zKilled.gameObject.SetActive(false);
        wScore.gameObject.SetActive(false);
        tScore.gameObject.SetActive(false);

        ResetWaveAndZombies();
    }

    public void UpdateScoreTexts()
    {
        waveText.text = "Wave " + currentWave.ToString();

        totalScore = totalScore + waveScore;

        zKilled.text = zombiesKilled.ToString();
        wScore.text = waveScore.ToString();
        tScore.text = totalScore.ToString();
    }

    public void ResetWaveAndZombies()
    {
        waveScore = 0;
        zombiesKilled = 0;
    }

}

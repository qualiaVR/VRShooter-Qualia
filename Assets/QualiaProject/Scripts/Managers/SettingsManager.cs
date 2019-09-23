using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor.Events;
#endif
using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour {

    // GAME DIFFICULTY
    //  VARIABLES
    public Text gameDifficultyText;
    enum DifficultySettings { Easy, Normal, Hard, Nightmare };
    DifficultySettings Difficulty = DifficultySettings.Normal;

    // ENABLE COM
    //  VARIABLES
    private bool COM_enabled = false;

    // COM PORT
    //  VARIABLES
    public Text comPortText;
    public GameObject comPortSection;
    enum COMPortSettings { COM1, COM2, COM3, COM4, COM5, COM6, COM7, COM8 };
    COMPortSettings COMPort = COMPortSettings.COM4;

    // GAME DIFFICULTY
    // METHODS

#if UNITY_EDITOR
    public void IncreaseDifficultyEnum() {
        if (Difficulty == DifficultySettings.Nightmare) { Difficulty = 0; }
            else { Difficulty++; }
        gameDifficultyText.text = Difficulty.ToString();
    }
#endif

#if UNITY_EDITOR
    public void DecreaseDifficultyEnum() {
        if (Difficulty == DifficultySettings.Easy) { Difficulty = (DifficultySettings) 3; }
            else { Difficulty--; }
        gameDifficultyText.text = Difficulty.ToString();
    }
#endif

#if UNITY_EDITOR
    public void IncreaseCOMEnum() {
        if (COMPort == COMPortSettings.COM8) { COMPort = 0; }
            else { COMPort++; }
        comPortText.text = COMPort.ToString();
    }
#endif

#if UNITY_EDITOR
    public void DecreaseCOMEnum()
    {
        if (COMPort == COMPortSettings.COM1) { COMPort = (COMPortSettings) 8; }
        else { COMPort++; }
        comPortText.text = COMPort.ToString();
    }
#endif

#if UNITY_EDITOR
    public void EnableCOM()
    {
        COM_enabled = true;
        comPortSection.SetActive(true);
    }
#endif

#if UNITY_EDITOR
    public void DisableCOM()
    {
        COM_enabled = false;
        comPortSection.SetActive(false);
    }
#endif

}

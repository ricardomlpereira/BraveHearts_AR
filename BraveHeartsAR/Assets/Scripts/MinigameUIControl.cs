using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.ARFoundation;
using TMPro;

public class MinigameUIControl : MonoBehaviour
{
    [SerializeField] private GameObject getEggBtn;
    [SerializeField] private GameObject procedeBtn;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI mainText;
    public static bool isMinigameCompleted;
    private AudioManager audioManager;
    private TW_MultiStrings_All typewriter;

    void Start()
    {
        audioManager = FindObjectOfType<AudioManager>();
        typewriter = mainText.gameObject.AddComponent<TW_MultiStrings_All>();
        typewriter.timeOut = 1; // Set timeout for the typewriter effect
        typewriter.LaunchOnStart = false;

        scoreText.text = MainControl.score + "/3";
        isMinigameCompleted = false;
    }

    void Update()
    {
        /* There's probably a better way to do this */
        if(isMinigameCompleted) {
            getEggBtn.SetActive(true);
        }
    }

    public void CompleteMinigame() {
        isMinigameCompleted = true;
        audioManager.PlayAudio("congrats");
    }

    public void NextAction() {
        audioManager.PlayAudio("btn");
        DisplayMessage("VAMOS ENCONTRAR O MEU OVO!");
        StartCoroutine(HatchEggSceneCoroutine());
    }

    IEnumerator HatchEggSceneCoroutine() {
        /* Wait for 3 seconds */
        yield return new WaitForSeconds(3f);

        /* Load next level */
        SceneManager.LoadScene("SearchEgg");
        LoaderUtility.Deinitialize();
        LoaderUtility.Initialize();
    }

    /*public void GetEgg() {
        isEggCollected = true;
        DisplayMessage("PARABÉNS! AJUDASTE O ALFREDO A ENCONTRAR UM DOS SEUS OVOS!");
        // TODO - Only temporary; Score should only increment after the player taps the egg (after finishing the minigame)
        if(!isScoreUpdated) {
            MainControl.score++;
            isScoreUpdated = true;
        }
    
        MainControl.foundFirstMatch = false;
        MainControl.foundSecondMatch = false;
        MainControl.foundThirdMatch = false;

        scoreText.text = MainControl.score + "/3";
        getEggBtn.SetActive(false);
        procedeBtn.SetActive(true);

    } */

    /*public void NextAction() {
        MinigameControl.minigameLevel++;
        if(MainControl.score < 3) {
            StartCoroutine(NextLevelCoroutine());
        } else {
            StartCoroutine(EndGameCoroutine());
        }
    } */

    public void DisplayMessage(string msg) {
        typewriter.ORIGINAL_TEXT = msg;
        typewriter.StartTypewriter();
    }

    /*IEnumerator NextLevelCoroutine() {
        DisplayMessage("A PROSSEGUIR PARA O PRÓXIMO NÍVEL...");
        // Wait for 3 seconds
        yield return new WaitForSeconds(3f);

        /* Load next level */
        //SceneManager.LoadScene("Main");
        //LoaderUtility.Deinitialize();
        //LoaderUtility.Initialize();
    //}

    /*IEnumerator EndGameCoroutine() {
        DisplayMessage("A PROSSEGUIR PARA O FINAL...");
        
        /* Wait for 3 seconds */
        //yield return new WaitForSeconds(3f);

        /* Load next level */
        //SceneManager.LoadScene("End");
        //LoaderUtility.Deinitialize();
        //LoaderUtility.Initialize();
    //}
}

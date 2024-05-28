using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.ARFoundation;
using TMPro;
using JetBrains.Annotations;

public class MinigameUIControl : MonoBehaviour
{
    [SerializeField] private GameObject getEggBtn;
    [SerializeField] private GameObject procedeBtn;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI mainText;
    private bool isScoreUpdated = false;
    public static bool isMinigameCompleted;
    private bool isEggCollected = false; //TEMPORARY

    void Start()
    {
        scoreText.text = MainControl.score + "/3";
        isMinigameCompleted = false;
    }

    void Update()
    {
        /* There's probably a better way to do this */
        if(isMinigameCompleted) {
            EnableEggCatching();
            if(!isEggCollected) {
                DisplayMessage("PARABÉNS! COMPLETASTE O MINIJOGO!");
            }
        }
    }

    public void EnableEggCatching() {
        getEggBtn.SetActive(true);
    }

    public void GetEgg() {
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

        isMinigameCompleted = true;

        scoreText.text = MainControl.score + "/3";
        getEggBtn.SetActive(false);
        procedeBtn.SetActive(true);

    }

    public void NextAction() {
        if(MainControl.score < 3) {
            StartCoroutine(NextLevelCoroutine());
        } else {
            StartCoroutine(EndGameCoroutine());
        }
    }

    public void DisplayMessage(string msg) {
        mainText.text = msg;
    }

    IEnumerator NextLevelCoroutine() {
        DisplayMessage("A PROSSEGUIR PARA O PRÓXIMO NÍVEL...");
        // Wait for 3 seconds
        yield return new WaitForSeconds(3f);
        
        /* Load next level */
        SceneManager.LoadScene("Main");
        LoaderUtility.Deinitialize();
        LoaderUtility.Initialize();
    }

    IEnumerator EndGameCoroutine() {
        DisplayMessage("A PROSSEGUIR PARA O FINAL...");
        
        /* Wait for 3 seconds */
        yield return new WaitForSeconds(3f);

        /* Load next level */
        SceneManager.LoadScene("End");
        LoaderUtility.Deinitialize();
        LoaderUtility.Initialize();
    }
}
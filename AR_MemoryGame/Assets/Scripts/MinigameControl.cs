using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.ARFoundation;
using TMPro;
using JetBrains.Annotations;

public class MinigameControl : MonoBehaviour
{
    [SerializeField] private GameObject getEggBtn;
    [SerializeField] private GameObject procedeBtn;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI mainText;
    private bool isScoreUpdated = false;
    public static bool isMinigameCompleted;
    private bool isEggCollected = false; //TEMPORARY

    // Start is called before the first frame update
    void Start()
    {
        scoreText.text = MultiTargetsManager.score + "/3";
        isMinigameCompleted = false;
    }

    // Update is called once per frame
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
            MultiTargetsManager.score++;
            isScoreUpdated = true;
        }
    
        MultiTargetsManager.foundFirstMatch = false;
        MultiTargetsManager.foundSecondMatch = false;
        MultiTargetsManager.foundThirdMatch = false;

        isMinigameCompleted = true;

        scoreText.text = MultiTargetsManager.score + "/3";
        getEggBtn.SetActive(false);
        procedeBtn.SetActive(true);

    }

    public void NextAction() {
        if(MultiTargetsManager.score < 3) {
            StartCoroutine(NextLevelCoroutine());
        } else {
            StartCoroutine(EndGameCoroutine());
        }
    }


    public void DisplayMessage(string msg) {
        mainText.text = msg;
    }

    IEnumerator NextLevelCoroutine() {
        DisplayMessage("A PROSEGUIR PARA O PRÓXIMO NÍVEL...");
        // Wait for 3 seconds
        yield return new WaitForSeconds(3f);
        
        // Load next level
        SceneManager.LoadScene("Main");
        LoaderUtility.Deinitialize();
        LoaderUtility.Initialize();
    }

    IEnumerator EndGameCoroutine() {
        DisplayMessage("A PROSEGUIR PARA O FINAL...");
        // Wait for 3 seconds
        yield return new WaitForSeconds(3f);

        // Load next level
        SceneManager.LoadScene("End");
        LoaderUtility.Deinitialize();
        LoaderUtility.Initialize();
    }
}

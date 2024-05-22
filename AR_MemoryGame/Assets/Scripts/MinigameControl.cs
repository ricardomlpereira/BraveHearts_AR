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

    // Start is called before the first frame update
    void Start()
    {
        scoreText.text = "OVOS: " + MultiTargetsManager.score + "/3";
        isMinigameCompleted = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(isMinigameCompleted) {
            EnableEggCatching();
        }
    }

    public void EnableEggCatching() {
        getEggBtn.SetActive(true);
    }

    public void GetEgg() {
        // TODO - Only temporary; Score should only increment after the player taps the egg (after finishing the minigame)
        if(!isScoreUpdated) {
            MultiTargetsManager.score++;
            isScoreUpdated = true;
        }
    
        MultiTargetsManager.foundFirstMatch = false;
        MultiTargetsManager.foundSecondMatch = false;
        MultiTargetsManager.foundThirdMatch = false;

        isMinigameCompleted = true;

        scoreText.text = "OVOS: " + MultiTargetsManager.score + "/3";
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

    IEnumerator NextLevelCoroutine() {
        mainText.text = "A PROSEGUIR PARA O PRÓXIMO NÍVEL...";
        // Wait for 3 seconds
        yield return new WaitForSeconds(3f);
        
        // Load next level
        SceneManager.LoadScene("Main");
        LoaderUtility.Deinitialize();
        LoaderUtility.Initialize();
    }

    IEnumerator EndGameCoroutine() {
        mainText.text = "A PROSEGUIR PARA O FINAL...";
        // Wait for 3 seconds
        yield return new WaitForSeconds(3f);

        // Load next level
        SceneManager.LoadScene("End");
        LoaderUtility.Deinitialize();
        LoaderUtility.Initialize();
    }
}

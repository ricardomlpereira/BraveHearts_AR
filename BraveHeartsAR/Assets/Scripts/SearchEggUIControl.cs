using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.XR.ARFoundation;

public class SearchEggUIControl : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI mainText;
    private bool isScoreUpdated = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GetEgg() {
        MinigameControl.minigameLevel++;
        if(!isScoreUpdated) {
            MainControl.score++;
            isScoreUpdated = true;
        }

        MainControl.foundFirstMatch = false;
        MainControl.foundSecondMatch = false;
        MainControl.foundThirdMatch = false;

        scoreText.text = MainControl.score + "/3";
        MinigameControl.placedObjects = 0;

        if(MainControl.score == 3) {
            StartCoroutine(EndGameCoroutine());
        } else {
            StartCoroutine(NextLevelCoroutine());
        }
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

    public void DisplayMessage(string msg) {
        mainText.text = msg;
    }
}

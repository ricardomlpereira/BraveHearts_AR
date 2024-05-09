using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.ARFoundation;
using TMPro;

public class MinigameControl : MonoBehaviour
{
    [SerializeField] private GameObject nextLevelBtn;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI mainText;
    private int score;
    private bool isScoreUpdated = false;

    // Start is called before the first frame update
    void Start()
    {
        int score = MultiTargetsManager.score;
        scoreText.text = "OVOS: " + score + "/3";
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void NextLevel() {
        // TODO - Only temporary; Score should only increment after the player taps the egg (after finishing the minigame)
        if(!isScoreUpdated) {
            MultiTargetsManager.score++;
            isScoreUpdated = true;
        }
    
        MultiTargetsManager.foundFirstMatch = false;
        MultiTargetsManager.foundSecondMatch = false;
        MultiTargetsManager.foundThirdMatch = false;

        scoreText.text = "OVOS: " + MultiTargetsManager.score + "/3";

        StartCoroutine(NextLevelCoroutine());
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
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.ARFoundation;
using TMPro;

public class MinigameUIControl : MonoBehaviour
{
    [SerializeField] private GameObject getEggBtn;
    [SerializeField] private GameObject nextBtn;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI mainText;
    [SerializeField] private TextMeshProUGUI mainText_1;
    [SerializeField] private TextMeshProUGUI mainText_2;
    //private MinigameControl minigameControl;
    public static bool isMinigameCompleted;
    private AudioManager audioManager;
    private TW_MultiStrings_All typewriter;
    /*
    private TW_MultiStrings_All typewriter_1;
    private TW_MultiStrings_All typewriter_2;
    */

    void Start()
    {
        audioManager = FindObjectOfType<AudioManager>();
        typewriter = mainText.gameObject.AddComponent<TW_MultiStrings_All>();
        typewriter.timeOut = 1; // Set timeout for the typewriter effect
        typewriter.LaunchOnStart = false;

        /*

        typewriter_1 = mainText_1.gameObject.AddComponent<TW_MultiStrings_All>();
        typewriter_1.timeOut = 1; // Set timeout for the typewriter effect
        typewriter_1.LaunchOnStart = false;

        typewriter_2 = mainText_2.gameObject.AddComponent<TW_MultiStrings_All>();
        typewriter_2.timeOut = 1; // Set timeout for the typewriter effect
        typewriter_2.LaunchOnStart = false;
        */

        //minigameControl = FindObjectOfType<MinigameControl>();
        

        scoreText.text = MainControl.score + "/3";
        isMinigameCompleted = false;
    }

    void Update()
    {
        /* There's probably a better way to do this */
        if(isMinigameCompleted) {
            getEggBtn.SetActive(true);
        }

        switch(MinigameControl.minigameLevel) {
            case 0:
            case 2:
                if(MinigameControl.placedObjects > 0) {
                    nextBtn.SetActive(false);
                }
                break;
            case 1:
                if(MinigameControl.placedObjects > 1) {
                    nextBtn.SetActive(false);
                }
                break;
            default:
            break;
        }
        
    }

    public void CompleteMinigame() {
        isMinigameCompleted = true;
        audioManager.PlayAudio("congrats");
    }

    public void NextAction() {
        audioManager.PlayAudio("btn");
        //DisplayMessage("VAMOS ENCONTRAR O MEU OVO!");
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

    public void DisplayMessage(string msg) {
        typewriter.ORIGINAL_TEXT = msg;
        typewriter.StartTypewriter();
    }
}

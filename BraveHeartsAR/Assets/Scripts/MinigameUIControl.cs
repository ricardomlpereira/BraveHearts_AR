using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.ARFoundation;
using TMPro;

public class MinigameUIControl : MonoBehaviour
{
    [SerializeField] private List<GameObject> nextBtns;
    [SerializeField] private List<GameObject> getEggBtns;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private List<TextMeshProUGUI> mainTexts;
    //private MinigameControl minigameControl;
    public static bool isMinigameCompleted;
    private AudioManager audioManager;
    private GameObject nextBtn;
    private GameObject getEggBtn;
    private TextMeshProUGUI mainText;
    private TW_MultiStrings_All typewriter;

    void Start()
    {
        audioManager = FindObjectOfType<AudioManager>();

        HandleTexts();

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

        switch(MinigameControl.minigameLevel) {
            case 0:
                if(MinigameControl.placedObjects > 0) {
                    nextBtn.SetActive(false);
                }
                break;
            case 1:
                if(MinigameControl.placedObjects > 1) {
                    nextBtn.SetActive(false);
                }
                break;
            case 2:
                if(MinigameControl.placedObjects > 0) {
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

    private void HandleTexts() {
        switch(MinigameControl.minigameLevel) {
            case 0:
                foreach(var text in mainTexts) {
                    if(text.name != "mainText") {
                        Destroy(text.gameObject);
                    } else {
                        mainText = text;
                        getEggBtn = getEggBtns[0];
                        nextBtn = nextBtns[0];
                    }
                }
                break;
            case 1:
                foreach(var text in mainTexts) {
                    if(text.name != "mainText_1") {
                        Destroy(text.gameObject);
                    } else {
                        mainText = text;
                        getEggBtn = getEggBtns[1];
                        nextBtn = nextBtns[1];
                    }
                }
                break;
            case 2:
                foreach(var text in mainTexts) {
                    if(text.name != "mainText_2") {
                        Destroy(text.gameObject);
                    } else {
                        mainText = text;
                        getEggBtn = getEggBtns[2];
                        nextBtn = nextBtns[2];
                    }
                }
                break;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.XR.ARFoundation;

public class MainUIControl : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI mainText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI foundMatchesText;
    [SerializeField] private TextMeshProUGUI infoText;
    [SerializeField] private GameObject miniGameBtn;
    [SerializeField] private RawImage butterflyImage;
    [SerializeField] private RawImage koalaImage;
    [SerializeField] private RawImage beeImage;
    public int foundMatches;
    private TW_MultiStrings_All typewriter;
    private bool isDisplayingMessage = false;
    private AudioManager audioManager;

    void Start()
    {
        audioManager = FindObjectOfType<AudioManager>();

        typewriter = mainText.gameObject.AddComponent<TW_MultiStrings_All>();
        typewriter.timeOut = 1; // Set timeout for the typewriter effect
        typewriter.LaunchOnStart = false;

        infoText.enabled = false;
        miniGameBtn.SetActive(false);
        scoreText.text = MainControl.score + "/3";
        switch(MainControl.score){
            case 0:
                butterflyImage.gameObject.SetActive(true);
                break;
            case 1:
                butterflyImage.gameObject.SetActive(false);
                koalaImage.gameObject.SetActive(true);
                break;
            case 2:
                koalaImage.gameObject.SetActive(false);
                //butterflyImage.gameObject.SetActive(false);
                beeImage.gameObject.SetActive(true);
                break;
            default:
                break;
        }
        
    }

    private void Update()
    {
        foundMatchesText.text = foundMatches + "/3";
    }

    public void DisplayMessage(string msg) {
        if(!isDisplayingMessage){
            StartCoroutine(DisplayMessageCoroutine(msg));
        }
        typewriter.ORIGINAL_TEXT = msg;
        typewriter.StartTypewriter();
        //mainText.text = msg;
    }

    private IEnumerator DisplayMessageCoroutine(string msg)
    {
        isDisplayingMessage = true;
        typewriter.ORIGINAL_TEXT = msg;
        typewriter.StartTypewriter();

        // Wait until the typewriter finishes
        yield return new WaitUntil(() => typewriter.IsFinished());

        isDisplayingMessage = false;
    }

    public bool IsDisplayingMessage()
    {
        return isDisplayingMessage;
    }

    public void EnableMinigame() {
        audioManager.PlayAudio("congrats");
        mainText.alignment = TextAlignmentOptions.Top;

        switch (MainControl.score){
            case 0:
                DisplayMessage("Encontraste a minha amiga, a Borboleta Aurora!\n E ela quer brincar contigo!");
                break;
            case 1:
                DisplayMessage("Encontraste o meu amigo, o Coala Kiko!\n E ele quer brincar contigo!");
                break;
            case 2:
                DisplayMessage("Encontraste a minha amiga, a Abelha Mel!\n E ela quer brincar contigo!");
                break;
            default:
                DisplayMessage("Boa! Encontraste todos os pares!");
                break;
        }

        miniGameBtn.SetActive(true);
    }

    public void StartMinigame() {
        audioManager.PlayAudio("btn");
        StartCoroutine(StartMinigameCoroutine());
    }

    IEnumerator StartMinigameCoroutine()
    {
        infoText.enabled = true;
        infoText.text = "A ABRIR O MINIJOGO...";

        /* Wait for 3 seconds */
        yield return new WaitForSeconds(3f);

        /* Switch to minigame scene */
        SceneManager.LoadScene("Minigame");
        LoaderUtility.Deinitialize();
        LoaderUtility.Initialize();

        /* Disable infoText after switching to minigame scene - probably useless */
        infoText.enabled = false;
    }

    public void ReturnToStart()
    {
        audioManager.PlayAudio("btn");
        StartCoroutine(ReturnCoroutine());
    }

    IEnumerator ReturnCoroutine()
    {
        infoText.enabled = true;
        infoText.text = "VOLTANDO AO INICIO...";

        /* Wait for 3 seconds */
        yield return new WaitForSeconds(3f);

        /* Switch to start scene */
        StartControl.hasReturned = true;

        SceneManager.LoadScene("Start");
        LoaderUtility.Deinitialize();
        LoaderUtility.Initialize();

        /* Disable infoText after switching to start scene */
        infoText.enabled = false;
    }

    // TEMP: Only for debugging purposes (minigames)
    public void SkipGame() {
        MainControl.foundFirstMatch = true;
        MainControl.foundSecondMatch = true;
        MainControl.foundThirdMatch = true;
        foundMatches = 3;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.XR.ARFoundation;

public class MainUIControl : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI mainText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI infoText;
    [SerializeField] private GameObject miniGameBtn;

    void Start()
    {
        infoText.enabled = false;
        miniGameBtn.SetActive(false);
        scoreText.text = MainControl.score + "/3";
    }

    private void Update()
    {

    }

    public void DisplayMessage(string msg) {
        mainText.text = msg;
    }

    public void EnableMinigame() {
        mainText.alignment = TextAlignmentOptions.Top;

        switch (MainControl.score){
            case 0:
                mainText.text = "A BORBOLETA AURORA QUER BRINCAR CONTIGO!";
                break;
            case 1:
                mainText.text = "O COALA KIKO QUER BRINCAR CONTIGO!";
                break;
            case 2:
                mainText.text = "A ABELHA MEL QUER BRINCAR CONTIGO!";
                break;
            default:
                mainText.text = "BOA! ENCONTRASTE TODOS OS PARES";
                break;
        }

        miniGameBtn.SetActive(true);
    }

    public void StartMinigame() {
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
        StartCoroutine(ReturnCoroutine());
    }

    IEnumerator ReturnCoroutine()
    {
        infoText.enabled = true;
        infoText.text = "VOLTANDO AO INICIO...";

        /* Wait for 3 seconds */
        yield return new WaitForSeconds(3f);

        /* Switch to start scene */
        SceneManager.LoadScene("Start");
        LoaderUtility.Deinitialize();
        LoaderUtility.Initialize();

        /* Disable infoText after switching to start scene */
        infoText.enabled = false;
    }
}

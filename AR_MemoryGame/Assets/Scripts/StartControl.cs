using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.ARFoundation;
using TMPro;

public class StartControl : MonoBehaviour
{
    public TextMeshProUGUI startInfo;

    public TextMeshProUGUI speechBubbleText;
    public GameObject speechBubbleBtnNext;
    public GameObject speechBubbleBtnPlay;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SwitchCanvas()
    {
        speechBubbleText.text = "Vamos jogar!";
        speechBubbleBtnNext.SetActive(false);
        speechBubbleBtnPlay.SetActive(true);
    }

    public void StartGame()
    {
        StartCoroutine(StartGameCoroutine());
    }

    IEnumerator StartGameCoroutine()
    {
        startInfo.text = "A COMEÇAR O JOGO...";
        // Wait for 3 seconds
        yield return new WaitForSeconds(3f);

        // Start Game

        SceneManager.LoadScene("MemoryGame");
        LoaderUtility.Deinitialize();
        LoaderUtility.Initialize();

        // Disable infoText after restart
        startInfo.enabled = false;
    }

    public void QuitGame()
    {
        StartCoroutine(QuitGameCoroutine());
    }

    IEnumerator QuitGameCoroutine()
    {
        startInfo.text = "A SAIR DO JOGO...";

        yield return new WaitForSeconds(3f);

        // Quit game
        Application.Quit();
    }
}

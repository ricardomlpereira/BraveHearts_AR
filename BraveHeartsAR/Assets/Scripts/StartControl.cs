using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class StartControl : MonoBehaviour
{
    // UI elements
    [SerializeField] private TextMeshProUGUI startInfo;
    [SerializeField] private TextMeshProUGUI speechBubbleText;
    [SerializeField] private GameObject speechBubbleBtnNext;
    [SerializeField] private GameObject speechBubbleBtnPlay;

    private AudioManager audioManager;
    public static bool hasReturned;

    void Start()
    {
        audioManager = FindObjectOfType<AudioManager>();
    }

    void Update()
    {
        
    }

    public void SwitchCanvas()
    {
        if (speechBubbleText.text == "Placeholder") // Change to the last string before "Let's play?"
        {
            speechBubbleBtnNext.SetActive(false);
            audioManager.PlayAudio("btn");
            speechBubbleBtnPlay.SetActive(true);
        }
    }

    public void StartGame()
    {
        // FIXME: It shouldn't be possible to keep pressing the button while the coroutine is processing
        audioManager.PlayAudio("btn");
        StartCoroutine(StartGameCoroutine());
    }

    IEnumerator StartGameCoroutine()
    {
        startInfo.text = "A COMEÇAR O JOGO...";
        yield return new WaitForSeconds(3f);

        if (hasReturned)
        {
            hasReturned = false;
            MainControl.resetProgress = true;
        }

        SceneManager.LoadScene("Main");

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

        Application.Quit();
    }

}

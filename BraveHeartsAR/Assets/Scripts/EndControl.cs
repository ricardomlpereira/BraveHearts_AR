using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.ARFoundation;
using TMPro;
using Unity.VisualScripting;

public class EndControl : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI infoText;
    [SerializeField] private TextMeshProUGUI sbText;
    [SerializeField] private GameObject sbBtnNext;
    [SerializeField] private GameObject sbBtnRestart;
    [SerializeField] private GameObject quitBtn;
    [SerializeField] private GameObject buddy;
    private AudioManager audioManager;

    // Start is called before the first frame update
    void Start()
    {
        audioManager = FindObjectOfType<AudioManager>();

        Animator anim = buddy.GetComponent<Animator>();
        anim.SetBool("hasFoundAllEggs", true);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ChangeSBBtn()
    {
        if (sbText.text == "Placeholder") // Change to the last string before "Let's play?"
        {
            sbBtnNext.SetActive(false);
            audioManager.PlayAudio("btn");
            sbBtnRestart.SetActive(true);
        }
    }

    public void RestartGame() {
        audioManager.PlayAudio("btn");
        StartCoroutine(RestartGameCoroutine());
    }

    IEnumerator RestartGameCoroutine()
    {
        infoText.text = "A RECOMEÇAR O JOGO...";

        // Wait for 3 seconds
        yield return new WaitForSeconds(3f);

        // Restart Game
        SceneManager.LoadScene("Start");
        LoaderUtility.Deinitialize();
        LoaderUtility.Initialize();

        // Reset score after restart
        MainControl.score = 0;
        MinigameControl.minigameLevel = 0;

        // Reset matches status
        MainControl.foundFirstMatch = false;
        MainControl.foundSecondMatch = false;
        MainControl.foundThirdMatch = false;
    }

        public void QuitGame()
    {
        StartCoroutine(QuitGameCoroutine());
    }

    IEnumerator QuitGameCoroutine()
    {
        infoText.text = "OBRIGADO POR JOGARES!";

        yield return new WaitForSeconds(3f);

        // Quit game
        Application.Quit();
    }
}

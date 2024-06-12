using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.ARFoundation;
using TMPro;

public class StartControl : MonoBehaviour
{
    private Camera cam;
    [SerializeField] private TextMeshProUGUI startInfo;
    [SerializeField] private TextMeshProUGUI speechBubbleText;

    [SerializeField] private GameObject speechBubbleBtnNext;
    [SerializeField] private GameObject speechBubbleBtnPlay;
    [SerializeField] private GameObject objectToShow;
    [SerializeField] private GameObject speechToShow;
    private AudioManager audioManager;
    public static bool hasReturned;

    //private float distanceFromCamera = 1.0f;
    //private Vector3 speechOffset = new Vector3(0.2f, 0.2f, 0);

    void Start()
    {
        audioManager = FindObjectOfType<AudioManager>();

        /* Get the camera */
        //Camera cam = Camera.main;
    }

    void Update()
    {
        /* Check if the object to show exists - if so centers it */
        /*if (objectToShow != null)
        {
            /* Calculate the position of the object in world space */
            //Vector3 screenCenter = new Vector3(Screen.width / 2 - 380.0f, Screen.height / 2 - 400.0f, cam.nearClipPlane + distanceFromCamera);
            //Vector3 objectPosition = cam.ScreenToWorldPoint(screenCenter);

            /* Set the position of the object to the calculated position */
            //objectToShow.transform.position = objectPosition;
            //speechToShow.transform.position = objectPosition + speechOffset;
            
    
            /* Make the speechToShow always face the camera - originally the speech is reversed*/
            //speechToShow.transform.LookAt(cam.transform);
            //speechToShow.transform.Rotate(0, 180, 0);

        //} 
    }

    public void SwitchCanvas()
    {
        if(speechBubbleText.text == "Placeholder"){ //Mudar para a ultima string antes de "Vamos jogar?"
        speechBubbleBtnNext.SetActive(false);
        audioManager.PlayAudio("btn");
        speechBubbleBtnPlay.SetActive(true);
        }
    }

    public void StartGame()
    {
        // FIXME: não devia de ser possível estar constantemente a carregar no btn enquanto a coroutine processa
        audioManager.PlayAudio("btn");
        StartCoroutine(StartGameCoroutine());
    }

    IEnumerator StartGameCoroutine()
    {
        startInfo.text = "A COMEÇAR O JOGO...";
        // Wait for 3 seconds
        yield return new WaitForSeconds(3f);

        // Start Game
        if(hasReturned) {
            hasReturned = false;
            MainControl.resetProgress = true;
        }

        SceneManager.LoadScene("Main");
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

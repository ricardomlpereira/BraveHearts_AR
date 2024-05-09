using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.XR.ARFoundation;

public class GameControl : MonoBehaviour
{
    public TextMeshProUGUI infoText;
    public GameObject objectToShow;

    // Offset distance from the camera
    private float distanceFromCamera = 1.0f;

    // Offset for speechToShow
    private Vector3 speechOffset = new Vector3(0.2f, 0.2f, 0);

    // Start is called before the first frame update
    void Start()
    {
        infoText.enabled = false;
    }

    private void Update()
    {
        if (objectToShow != null)
        {
            // Get the camera
            Camera cam = Camera.main;

            // Calculate the position of the object in world space
            Vector3 screenCenter = new Vector3(Screen.width / 2 - 380.0f, Screen.height / 2 - 400.0f, cam.nearClipPlane + distanceFromCamera);
            Vector3 objectPosition = cam.ScreenToWorldPoint(screenCenter);

            // Set the position of the object to the calculated position
            objectToShow.transform.position = objectPosition;

            objectToShow.transform.LookAt(cam.transform);
            objectToShow.transform.Rotate(0, 45, 0);
        }

    }

    public void StartMinigame() {
        StartCoroutine(StartMinigameCoroutine());
    }

    IEnumerator StartMinigameCoroutine()
    {
        infoText.enabled = true;
        infoText.text = "A ABRIR O MINIJOGO...";

        // Wait for 3 seconds
        yield return new WaitForSeconds(3f);

        // Restart Game

        SceneManager.LoadScene("Minigame");
        LoaderUtility.Deinitialize();
        LoaderUtility.Initialize();

        // Disable infoText after restart
        infoText.enabled = false;
    }

    public void RestartGame() {
        StartCoroutine(RestartGameCoroutine());
    }

    IEnumerator RestartGameCoroutine()
    {
        infoText.enabled = true;
        infoText.text = "A RECOMEÇAR O JOGO...";

        // Wait for 3 seconds
        yield return new WaitForSeconds(3f);

        // Restart Game
        SceneManager.LoadScene("Main");
        LoaderUtility.Deinitialize();
        LoaderUtility.Initialize();

        // Disable infoText after restart
        infoText.enabled = false;

        // Reset score after restart
        MultiTargetsManager.score = 0;

        // Reset matches status
        MultiTargetsManager.foundFirstMatch = false;
        MultiTargetsManager.foundSecondMatch = false;
        MultiTargetsManager.foundThirdMatch = false;
    }

    public void ReturnToStart()
    {
        StartCoroutine(ReturnCoroutine());
    }

    IEnumerator ReturnCoroutine()
    {
        infoText.enabled = true;
        infoText.text = "VOLTANDO AO INICIO...";

        // Wait for 3 seconds
        yield return new WaitForSeconds(3f);

        SceneManager.LoadScene("Start");
        LoaderUtility.Deinitialize();
        LoaderUtility.Initialize();

        infoText.enabled = false;
    }
}

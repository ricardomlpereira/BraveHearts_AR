using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BuddyControl : MonoBehaviour
{
    private Camera cam;

    [SerializeField] private GameObject buddy;
    [SerializeField] private GameObject objectToShow;
    [SerializeField] private GameObject objectToShow_1;
    [SerializeField] private GameObject objectToShow_2;
    private float distanceFromCam = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
        Scene currentScene = SceneManager.GetActiveScene();
        if(currentScene.name == "Minigame"){
        switch(MinigameControl.minigameLevel){
            case 0:
            objectToShow.SetActive(true);
            objectToShow_1.SetActive(false);
            objectToShow_2.SetActive(false);
            break;
            case 1:
            objectToShow.SetActive(false);
            objectToShow_1.SetActive(true);
            objectToShow_2.SetActive(false);
            break;
            case 2:
            objectToShow.SetActive(false);
            objectToShow_1.SetActive(false);
            objectToShow_2.SetActive(true);
            break;
            default:
            break;
        }
        }
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 screenCenter = new Vector3(Screen.width / 2 - 1000.0f, 
                                            Screen.height / 2 + 300.0f, 
                                            cam.nearClipPlane + distanceFromCam);

        Vector3 objectPosition = cam.ScreenToWorldPoint(screenCenter);

        buddy.transform.position = objectPosition;

        /* Set the position of the object to the calculated position */
        Scene currentScene = SceneManager.GetActiveScene();
        if(currentScene.name == "Minigame"){
        switch(MinigameControl.minigameLevel){
            case 0:
            objectToShow.transform.position = objectPosition;
            break;
            case 1:
            objectToShow_1.transform.position = objectPosition;
            break;
            case 2:
            objectToShow_2.transform.position = objectPosition;
            break;
            default:
            break;
        }
        }
    }
}

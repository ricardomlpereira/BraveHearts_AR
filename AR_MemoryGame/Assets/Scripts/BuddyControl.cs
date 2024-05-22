using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuddyControl : MonoBehaviour
{
    [SerializeField] private GameObject objectToShow;
    private float distanceFromCam = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Camera cam = Camera.main;

        /*Vector3 screenCenter = new Vector3(
            Screen.width / 2 - 380.0f, 
            Screen.height / 2 - 400.0f, 
            cam.nearClipPlane + distanceFromCam);*/
        Vector3 screenCenter = new Vector3(
            Screen.width / 2 - 1000.0f, 
            Screen.height / 2 + 325.0f, 
            cam.nearClipPlane + distanceFromCam);
        Vector3 objectPosition = cam.ScreenToWorldPoint(screenCenter);

        // Set the position of the object to the calculated position
        objectToShow.transform.position = objectPosition;
        objectToShow.transform.LookAt(cam.transform);
        objectToShow.transform.Rotate(0, -55, 0);

    }
}

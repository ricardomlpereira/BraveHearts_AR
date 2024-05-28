using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuddyControl : MonoBehaviour
{
    private Camera cam;
    [SerializeField] private GameObject objectToShow;
    private float distanceFromCam = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 screenCenter = new Vector3(Screen.width / 2 - 1000.0f, 
                                            Screen.height / 2 + 300.0f, 
                                            cam.nearClipPlane + distanceFromCam);

        Vector3 objectPosition = cam.ScreenToWorldPoint(screenCenter);

        /* Set the position of the object to the calculated position */
        objectToShow.transform.position = objectPosition;
    }
}

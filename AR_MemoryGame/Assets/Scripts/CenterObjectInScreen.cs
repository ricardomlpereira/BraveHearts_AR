using UnityEngine;

public class CenterObjectInScreen : MonoBehaviour
{
    
    public GameObject objectToShow;

    public GameObject speechToShow;

    // Offset distance from the camera
    private float distanceFromCamera = 1.0f;

    // Offset for speechToShow
    private Vector3 speechOffset = new Vector3(0.2f, 0.2f, 0);


    void Update()
    {
        // Check if the object to show exists
        if (objectToShow != null)
        {
            // Get the camera
            Camera cam = Camera.main;

            // Calculate the position of the object in world space
            Vector3 screenCenter = new Vector3(Screen.width / 2 - 380.0f, Screen.height / 2 - 400.0f, cam.nearClipPlane + distanceFromCamera);
            Vector3 objectPosition = cam.ScreenToWorldPoint(screenCenter);

            // Set the position of the object to the calculated position
            objectToShow.transform.position = objectPosition;
            speechToShow.transform.position = objectPosition + speechOffset;
            
    
            // Make the speechToShow always face the camera
            // A parte da frente por default é a parte de trás
            speechToShow.transform.LookAt(cam.transform);
            speechToShow.transform.Rotate(0, 180, 0);

        }
    }
}

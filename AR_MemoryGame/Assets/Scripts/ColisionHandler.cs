using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class ColisionHandler : MonoBehaviour
{
    [SerializeField] private Camera ARCamera;
    [SerializeField] private GameObject arm;
    [SerializeField] private GameObject pensoDetector;
    [SerializeField] private GameObject garroteDdetector;
    [SerializeField] private GameObject cateterdetector;
    [SerializeField] private GameObject[] objectCollection; 
    private Dictionary<int, GameObject> objectsToPlace = new Dictionary<int, GameObject>();
    private GameObject currentObj;
    public static int minigameLevel; // TODO - Tal como o score deve haver uma maneira melhor de fazer isto
    private int idx = 0;
    private bool isObjectSelected;
    private UnityEngine.Vector2 initialTouchPos;
    private string tagObjects = "Object";
    private float screenFactor = 0.0001f;
    private float speedMovement = 4.0f;

    // Start is called before the first frame update
    void Start()
    {
        arm.SetActive(true);
        int aux = idx;

        foreach(GameObject obj in objectCollection) {
            GameObject newObj = Instantiate(obj, UnityEngine.Vector3.zero, UnityEngine.Quaternion.identity);
            newObj.name = newObj.name.Replace("(Clone)", "").Trim();
            if(minigameLevel == 0 && (newObj.name == "pomada" || newObj.name == "pensoFechado" || newObj.name == "pensoAberto")) {
                objectsToPlace.Add(aux, newObj);
                aux++;
            }
        }

        /* Set first object to place to active */
        objectsToPlace[idx].SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        /* Check if player has completed the minigame */
        if(MinigameControl.isMinigameCompleted) {
            return;
        }

        /* Get current object to place */
        GameObject obj = objectsToPlace[idx].gameObject;
        obj.SetActive(true);

        if(Input.touchCount > 0) {
            Touch touch1 = Input.GetTouch(0);
            initialTouchPos = touch1.position;
            isObjectSelected = CheckTouchOnObject(initialTouchPos);

            if(touch1.phase == TouchPhase.Began)
                {
                    initialTouchPos = touch1.position;
                    isObjectSelected = CheckTouchOnObject(initialTouchPos);
                }

            if(touch1.phase == TouchPhase.Moved && isObjectSelected)
                {
                    UnityEngine.Vector2 diffPos = (touch1.position - initialTouchPos) * screenFactor;

                    currentObj.transform.position = currentObj.transform.position + new UnityEngine.Vector3(diffPos.x * speedMovement, diffPos.y * speedMovement, 0);

                    initialTouchPos = touch1.position;
                }
        }

        /* Check if object to place is in the correct position */
        if(checkCollision(obj)) {
            // Place object
            placeObject(obj);

            /* Check if current object is the last object to be placed */
            if(objectsToPlace.Count == idx) {
                MinigameControl.isMinigameCompleted = true;
                return;
            }

            /* Object has been placed in the correct position - increment idx for next object */
            objectsToPlace[idx].SetActive(false);
            idx++;
            objectsToPlace[idx].SetActive(true);
        }
    }

    private bool CheckTouchOnObject(UnityEngine.Vector2 touchPos)
    {
        Ray ray = ARCamera.ScreenPointToRay(touchPos);

        if(Physics.Raycast(ray, out RaycastHit hitObject))
        {
            if(hitObject.collider.CompareTag(tagObjects))
            {
                currentObj = hitObject.transform.gameObject;
                return true;
            }
        }

        return false;
    }

    private bool checkCollision(GameObject curObj) {
        if(minigameLevel == 0) {
            // Penso
            Collider curObjCollider = curObj.GetComponent<Collider>();
            Collider pensoDetectorCollider = pensoDetector.GetComponent<Collider>();

            return curObjCollider.bounds.Intersects(pensoDetectorCollider.bounds);
        } else {
            return false;
        }
    }

    private void placeObject(GameObject obj) {
        if(minigameLevel == 0) {
            // Penso
            obj.transform.position = pensoDetector.transform.position;
        }
    }
}

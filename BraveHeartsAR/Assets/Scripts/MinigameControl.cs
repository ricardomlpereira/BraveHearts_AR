using System.Collections.Generic;
using System.Numerics;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class MinigameControl : MonoBehaviour
{
    [SerializeField] private Camera ARCamera;
    [SerializeField] private GameObject pensoPos;
    [SerializeField] private GameObject garrotePos;
    [SerializeField] private GameObject cateterPos;
    [SerializeField] private GameObject[] objectCollection; 
    private GameObject currentObj;
    public static int minigameLevel; // TODO - Tal como o score deve haver uma maneira melhor de fazer isto
    private int idx = 0;
    private bool isObjectSelected;
    private UnityEngine.Vector2 initialTouchPos;
    private string tagObjects = "Object";
    private float screenFactor = 0.0001f;
    private float speedMovement = 4.0f;
    private int placedObjects = 0;
    
    private MinigameUIControl minigameUIControl;

    // Start is called before the first frame update
    void Start()
    {
        minigameUIControl = FindObjectOfType<MinigameUIControl>();
    }

    // Update is called once per frame
    void Update()
    {
        switch (placedObjects){
            case 0:
                minigameUIControl.DisplayMessage("VAMOS APLICAR A POMADA");
                break;
            case 1:
                minigameUIControl.DisplayMessage("VAMOS ABRIR O PENSO");
                break;
            case 2:
                minigameUIControl.DisplayMessage("VAMOS APLICAR O PENSO");
                break;
            default:
                break;
        }

        /* Check if player has completed the minigame */
        if(MinigameUIControl.isMinigameCompleted) {
            return;
        }

        /* Get current object */
        currentObj = objectCollection[idx].gameObject;
        currentObj.SetActive(true);

        /* Check input */
        if(Input.touchCount > 0) {
            Touch touch1 = Input.GetTouch(0);
            initialTouchPos = touch1.position;
            isObjectSelected = CheckTouchOnObject(initialTouchPos);

            /* Transition from the closed bandaid to the open bandaid (ready to be applied) */ 
            if(touch1.phase == TouchPhase.Began && currentObj.name == "pensoFechado" && isObjectSelected) {
                objectCollection[idx].SetActive(false);
                idx++;
                placedObjects++;
                return;
            }

            /* Check if the player has touched the screen - MIGHT BE UNNECESSEARY */
            if(touch1.phase == TouchPhase.Began) {
                initialTouchPos = touch1.position;
                isObjectSelected = CheckTouchOnObject(initialTouchPos);
            }

            /* Moves the object selected by the player in 2D (x and y dimensions) */
            if(touch1.phase == TouchPhase.Moved && isObjectSelected)
            {
                UnityEngine.Vector2 diffPos = (touch1.position - initialTouchPos) * screenFactor;
                UnityEngine.Vector3 worldDiffPos = new UnityEngine.Vector3(diffPos.x, diffPos.y, 0);

                // Convert screen position to world position
                UnityEngine.Vector3 screenToWorldPoint = ARCamera.ScreenToWorldPoint(new UnityEngine.Vector3(touch1.position.x, touch1.position.y, ARCamera.WorldToScreenPoint(currentObj.transform.position).z));
                UnityEngine.Vector3 worldPosition = screenToWorldPoint + worldDiffPos * speedMovement;

                currentObj.transform.position = new UnityEngine.Vector3(worldPosition.x, worldPosition.y, worldPosition.z);

                initialTouchPos = touch1.position;
            }
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

    public void placeObject() {
        if (minigameLevel == 0) {
            /* Penso */
            if(currentObj.name == "pomada") {
                objectCollection[idx].SetActive(false);
                idx++;
                placedObjects++;

                /* Colocar o creme - automatico */
                objectCollection[idx].SetActive(true);
                idx++;
                placedObjects++;

                return;
            }

            currentObj.transform.position = pensoPos.transform.position;
            idx++;
            placedObjects++;
        }

        /* Check if last object of the minigame was placed - TODO: Needs ajustments when implemeting more minigames */
        isLastObject();
    }

    private void isLastObject() {
        if(placedObjects == objectCollection.Length) {
            MinigameUIControl.isMinigameCompleted = true;
            return;
        }
    }
}

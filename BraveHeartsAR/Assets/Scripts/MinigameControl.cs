using System.Collections.Generic;
using System.Numerics;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class MinigameControl : MonoBehaviour
{
    [SerializeField] private Camera ARCamera;
    [SerializeField] private GameObject posPenso;
    [SerializeField] private GameObject posGarrote;
    [SerializeField] private GameObject posCateter;
    //[SerializeField] private GameObject[] objectCollection;
    [SerializeField] private List<GameObject> objectCollection;
    //[SerializeField] private GameObject[] colliderCollection;
    [SerializeField] private List<GameObject> colliderCollection;
    private GameObject currentObj;
    public static int minigameLevel; // TODO - Tal como o score deve haver uma maneira melhor de fazer isto
    private int idx;
    private bool isObjectSelected;
    private UnityEngine.Vector2 initialTouchPos;
    private string tagObjects = "Object";
    private float screenFactor = 0.0001f;
    private float speedMovement = 4.0f;
    private int placedObjects;
    private string backgroundObj = "BackgroundObject";
    private MinigameUIControl minigameUIControl;

    // Start is called before the first frame update
    void Start()
    {
        minigameUIControl = FindObjectOfType<MinigameUIControl>();
        HandleCollections();
    }

    // Update is called once per frame
    void Update()
    {   
        // TODO: Only here until third minigame is implemented
        if(minigameLevel == 2) {
            minigameUIControl.NextAction();
        }

        // Check if player has completed the minigame - if so, return
        if(isLastObject()) {
            return;
        }

        HandleMessage();
        HandleInput();
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

    private void HandleCollections() {
        /* Enable and disable GameObjects based on the minigame level */    
        List<GameObject> objectsToRemove = new List<GameObject>();
        List<GameObject> collidersToRemove = new List<GameObject>();
        idx = 0;
        placedObjects = 0;

        if(minigameLevel == 0) {
            /* Minigame - Penso */
            foreach(GameObject obj in objectCollection) {
                if(obj.name != "pomada" && obj.name != "creme" && obj.name != "pensoFechado" && obj.name != "pensoAberto") {
                    objectsToRemove.Add(obj);
                }
            }

            foreach(GameObject obj in colliderCollection) {
                if(obj.name != "pensoCollider") {
                    collidersToRemove.Add(obj);
                }
            }
        } else if(minigameLevel == 1) {
            /* Minigame - Garrote */
            foreach(GameObject obj in objectCollection) {
                if(obj.name == "creme") {
                    obj.SetActive(true);
                    obj.tag = backgroundObj;
                    idx++; // Porque a coleção vai ter o penso aberto e o creme
                    placedObjects++; // Porque a coleção vai ter o penso aberto e o creme
                    continue;
                }

                if(obj.name == "pensoAberto") {
                    obj.SetActive(true);
                    obj.transform.position = posPenso.transform.position;
                    obj.tag = backgroundObj;
                    idx++; // Porque a coleção vai ter o penso aberto e o creme
                    placedObjects++; // Porque a coleção vai ter o penso aberto e o creme
                    continue;
                }

                if(obj.name != "garrote" && obj.name != "garrote_arm") {
                    objectsToRemove.Add(obj);
                }
            }

            foreach(GameObject obj in colliderCollection) {
                if(obj.name != "garroteCollider") {
                    collidersToRemove.Add(obj);
                }
            }
        } else if(minigameLevel == 2) {
            // TODO
            return;
        }

        /* Remove unnecessary objects and colliders */
        foreach(GameObject obj in objectsToRemove) {
            objectCollection.Remove(obj);
            Destroy(obj);
        }

        foreach(GameObject obj in collidersToRemove) {
            colliderCollection.Remove(obj);
            Destroy(obj);
        }
        
        /* Enable objects and colliders to be used in the minigame */
        currentObj = objectCollection[idx].gameObject;
        currentObj.SetActive(true);
        colliderCollection[0].SetActive(true); // Se houver algum minijogo com mais do que um collider -> criar um idx para os colliders
    }

    private void HandleInput() {
        /* Check input */
        if(Input.touchCount > 0) {
            Touch touch1 = Input.GetTouch(0);
            initialTouchPos = touch1.position;
            isObjectSelected = CheckTouchOnObject(initialTouchPos);

            /* Transition from the closed bandaid to the open bandaid (ready to be applied) */ 
            if(touch1.phase == TouchPhase.Began && currentObj.name == "pensoFechado" && isObjectSelected) {
                objectCollection[idx].SetActive(false); // Desativar o penso fechado
                PlaceObject();
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

    public void HandleObject() {
        if(currentObj.name == "pomada") {
            objectCollection[idx].SetActive(false); // Desativar a pomada
            placedObjects++;
            idx++;
            
            objectCollection[idx].SetActive(true); // Ativar o creme
        } else if(currentObj.name == "pensoFechado") {
            // Isto é necessário e eu não faço ideia porque
            return;
        } else if(currentObj.name == "pensoAberto") {
            currentObj.transform.position = posPenso.transform.position;
            placedObjects++;
            return;
        } else if(currentObj.name == "garrote") {
            objectCollection[idx].SetActive(false); // Desativar o garrote
            placedObjects++;
        }

        /* Place object - if is last object the complete the minigame */
        PlaceObject();
    }

    private void PlaceObject() {
        if(idx < objectCollection.Count - 1) {
            idx++;
            placedObjects++;
            currentObj = objectCollection[idx].gameObject;
            currentObj.SetActive(true);
        }

        isLastObject();
    }

    private bool isLastObject() {
        if(placedObjects >= objectCollection.Count) {
            MinigameUIControl.isMinigameCompleted = true;
            minigameUIControl.DisplayMessage("BOA! COMPLETASTE O MINIJOGO!");
            return true;
        }

        return false;
    }

    private void HandleMessage() {
        // TODO - Refactor
        if(minigameLevel == 0) {
            switch (placedObjects){
                case 0:
                    minigameUIControl.DisplayMessage("VAMOS APLICAR A POMADA");
                    break;
                case 2:
                    minigameUIControl.DisplayMessage("VAMOS ABRIR O PENSO");
                    break;
                case 3:
                    minigameUIControl.DisplayMessage("VAMOS APLICAR O PENSO");
                    break;
                default:
                    break;
            }
        } else if(minigameLevel == 1) {
            switch(placedObjects) {
                case 2:
                    minigameUIControl.DisplayMessage("VAMOS APLICAR O GARROTE");
                    break;
                default:
                    break;
            }
        }
    }
}

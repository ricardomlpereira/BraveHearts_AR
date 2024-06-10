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
    private AudioManager audioManager;
    private bool isCompleted = false;
    private int previousPlacedObjects = 0;
    private bool isFirstInteration = true;

    // Start is called before the first frame update
    void Start()
    {
        audioManager = FindObjectOfType<AudioManager>();
        minigameUIControl = FindObjectOfType<MinigameUIControl>();
        HandleCollections();
        HandleMessage();
    }

    // Update is called once per frame
    void Update()
    {   
        // TODO: Only here until third minigame is implemented
        if(minigameLevel == 2) {
            minigameUIControl.NextAction();
        }

        // FIXME: meter isto como deve ser depois
        if(isFirstInteration) {
            HandleMessage();
            isFirstInteration = false;
        }

        // FIXME: meter isto como deve ser depois; n faz sentido ter 2 bools relativos ao facto de o minijogo estar completo ou n em 2 scripts
        if(isCompleted) {
            return;
        }

        // Check if player has completed the minigame - if so, return
        if(isLastObject()) {
            return;
        }

        if(previousPlacedObjects != placedObjects) {
            // Mudar a msg quando é colocado um objeto)
            HandleMessage();
            previousPlacedObjects = placedObjects;
        }

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
                    //obj.tag = backgroundObj;
                    //idx++; // Porque a coleção vai ter o penso aberto e o creme
                    //placedObjects++; // Porque a coleção vai ter o penso aberto e o creme
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

            // Remover o penso da mao no minijogo do garrote -> TODO: Em vez de ser um toque no penso devia de ser arrastar o penso
            if(touch1.phase == TouchPhase.Began && currentObj.name == "pensoAberto" && isObjectSelected && minigameLevel == 1) {
                objectCollection[idx].SetActive(false); // Desativar o penso aberto
                objectCollection[idx - 1].SetActive(false);
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
            audioManager.PlayAudio("progress");
            return;
        } else if(currentObj.name == "garrote") {
            objectCollection[idx].SetActive(false); // Desativar o garrote
            audioManager.PlayAudio("progress");
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

        audioManager.PlayAudio("progress");
        isLastObject();
    }

    private bool isLastObject() {
        if(placedObjects >= objectCollection.Count) {
            minigameUIControl.DisplayMessage("BOA! COMPLETASTE O MINIJOGO!");
            minigameUIControl.CompleteMinigame();
            isCompleted = true;
            return true;
        }

        return false;
    }

    private void HandleMessage() {
        // TODO - Refactor
        if(minigameLevel == 0) {
            switch (placedObjects){
                case 0:
                    minigameUIControl.DisplayMessage("VAMOS COMEÇAR POR APLICAR A POMADA. ARRASTA ESTA PARA O LOCAL CORRETO.");
                    break;
                case 2:
                    minigameUIControl.DisplayMessage("BOA! AGORA, VAMOS ABRIR O PENSO. CARREGA NELE!");
                    break;
                case 3:
                    minigameUIControl.DisplayMessage("MUITO BEM! PARA APLICAR O PENSO ARRASTA-O PARA CIMA DA POMADA!");
                    break;
                default:
                    minigameUIControl.DisplayMessage("ERRO");
                    break;
            }
        } else if(minigameLevel == 1) {
            switch(placedObjects) {
                case 1:
                    minigameUIControl.DisplayMessage("VAMOS COMEÇAR POR RETIRAR O PENSO. CARREGA NELE!");
                    break;
                case 2:
                    minigameUIControl.DisplayMessage("BOA! VAMOS AGORA APLICAR O GARROTE. ARRASTA-O PARA O LOCAL CORRETO.");
                    break;
                default:
                    minigameUIControl.DisplayMessage("ERRO");
                    break;
            }
        }
    }
}

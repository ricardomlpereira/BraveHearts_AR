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
    [SerializeField] private GameObject posObturador;
    [SerializeField] private List<GameObject> objectCollection;
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
    private int garroteArmIdx = -1;

    // Start is called before the first frame update
    void Start()
    {
        minigameLevel = 2;
        audioManager = FindObjectOfType<AudioManager>();
        minigameUIControl = FindObjectOfType<MinigameUIControl>();
        HandleCollections();
        HandleMessage();

        
        /*int aux = 0;
        foreach(GameObject obj in objectCollection) {
            Debug.Log("idx: " + aux + "/ obj name: " + obj.name);
            aux++;
        } */
    }

    // Update is called once per frame
    void Update()
    {   
        Debug.Log("placed Object upd: " + placedObjects);

        if(idx == garroteArmIdx) {
            idx++;
            placedObjects++;
            currentObj = objectCollection[idx].gameObject;
            currentObj.SetActive(true);
            return;
        }

        if(currentObj.name == "obturador") {
            /* Desativar todos os colliders menos o do catater */
            foreach(GameObject obj in colliderCollection) {
                if(obj.name != "cateterCollider") {
                    obj.SetActive(false);
                }
            }
        }

        // TODO: Temporary, ignorar o desinfetante pq n da para detectar clicks no environment
        if(currentObj.name == "desinfetante") {
            objectCollection[idx].SetActive(false); // Desativar o desinfetante
            placedObjects++;
            idx++;
            objectCollection[idx].SetActive(true); // Ativar as gotas
            idx++;
            placedObjects++;
            objectCollection[idx].SetActive(true); // Ativar o pano
            currentObj = objectCollection[idx].gameObject;
            return;
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

        switch(minigameLevel) {
            case 0:
                /* Minigame - Penso */
                foreach(GameObject obj in objectCollection) {
                    if(obj.name != "pomada" && obj.name != "creme" && obj.name != "pensoFechado" && obj.name != "pensoAberto") {
                        objectsToRemove.Add(obj);
                    }
                }

                foreach(GameObject obj in colliderCollection) {
                    if(obj.name != "handCollider") {
                        collidersToRemove.Add(obj);
                    } else {
                        obj.SetActive(true);
                    }
                }
                break;
            case 1:
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
                        continue;
                    }

                    if(obj.name != "garrote" && obj.name != "garrote_arm" && obj.name != "desinfetante" && obj.name != "pano" && obj.name != "gotas") {
                        objectsToRemove.Add(obj);
                    }
                }

                foreach(GameObject obj in colliderCollection) {
                    if(obj.name != "garroteCollider" && obj.name != "handCollider") { // collider do penso passa a ser necessário neste minijogo por causa do pano
                        collidersToRemove.Add(obj);
                    }
                }
                break;
            case 2:
                // Minigame - Cateter
                foreach(GameObject obj in objectCollection) {
                    if(obj.name != "desinfetante" && obj.name != "gotas" && obj.name != "pano" && obj.name != "cateter" && obj.name != "cateter_arm" && obj.name != "obturador" && obj.name != "adesivo" && obj.name != "tala" && obj.name != "tala_arm" && obj.name != "garrote_arm") {
                        objectsToRemove.Add(obj);
                    }

                    if(obj.name == "garrote_arm") {
                        obj.SetActive(true);
                    }
                }

                foreach(GameObject obj in colliderCollection) {
                    if(obj.name != "handCollider" && obj.name != "talaCollider" && obj.name != "cateterCollider") {
                        collidersToRemove.Add(obj);
                    }

                    // Começamos com o collider da mao ativo e o da tala inativo
                    if(obj.name == "handCollider") {
                        obj.SetActive(true);
                    }
                }
                break;
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
        
        /*foreach(GameObject obj in objectCollection) {
            obj.AddComponent<CollisionDetector>();
        }*/

        /* Enable first object to be used in the minigame */
        currentObj = objectCollection[idx].gameObject;
        currentObj.SetActive(true);

        // Hack fix - get garrote arm idx para o minijogo 3
        if(minigameLevel == 2) {
            foreach(GameObject obj in objectCollection) {
                if(obj.name == "garrote_arm") {
                    garroteArmIdx = objectCollection.IndexOf(obj);
                }
            }
        }
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
                objectCollection[idx - 1].SetActive(false); // Desativar a pomada
                PlaceObject();
                return;
            }

            if(touch1.phase == TouchPhase.Began && currentObj.name == "desinfetante" && isObjectSelected) {
                objectCollection[idx].SetActive(false); // Desativar o desinfetante
                placedObjects++;
                idx++;
                objectCollection[idx].SetActive(true); // Ativar as gotas
                // Ativar o collider do penso (por causa do pano)
                colliderCollection[0].SetActive(true);
                objectCollection[idx + 1].SetActive(true); // Ativar o pano
                //PlaceObject();
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
        if(minigameLevel == 0) {
            switch(currentObj.name) {
                case "pomada":
                    objectCollection[idx].SetActive(false); // Desativar a pomada
                    placedObjects++;
                    idx++;
                    objectCollection[idx].SetActive(true); // Ativar o creme
                    break;
                case "pensoFechado":
                    return;
                case "pensoAberto":
                    currentObj.transform.position = posPenso.transform.position;
                    placedObjects++;
                    audioManager.PlayAudio("progress");
                    return;
                default:
                    break;
            }
        } else if (minigameLevel == 1) {
            switch(currentObj.name) {
                case "pensoAberto":
                    return;
                case "desinfetante":
                    return;
                case "pano":
                    colliderCollection[0].SetActive(false);
                    colliderCollection[1].SetActive(true);

                    objectCollection[idx - 1].SetActive(false); // Desativar as gotas
                    objectCollection[idx].SetActive(false); // Desativar o pano

                    idx++;
                    placedObjects++;
                    currentObj = objectCollection[idx].gameObject;
                    currentObj.SetActive(true);
                    return;
                case "garrote":
                    objectCollection[idx].SetActive(false); // Desativar o garrote
                    audioManager.PlayAudio("progress");
                    placedObjects++;
                    break;
                default:
                    break;
            }
        } else if(minigameLevel == 2) {
            switch(currentObj.name) {
                case "desinfetante":
                    return;
                case "pano":
                    objectCollection[idx - 1].SetActive(false); // Desativar as gotas
                    objectCollection[idx].SetActive(false); // Desativar o pano

                    idx++;
                    placedObjects++;
                    currentObj = objectCollection[idx].gameObject;
                    currentObj.SetActive(true);
                    return;
                case "cateter":
                    objectCollection[idx].SetActive(false); // Desativar o cateter
                    idx++;
                    placedObjects++;
                    objectCollection[idx].gameObject.SetActive(true); // Ativar o cateter_arm
                    idx++;
                    placedObjects++;
                    currentObj = objectCollection[idx].gameObject;
                    currentObj.SetActive(true);

                    objectCollection[garroteArmIdx].SetActive(false); //desativar o garrote

                    return;
                case "obturador":
                    // TODO: BUGADO; ESTA MERDA VAI LOGO PARA O SITIO QUE É SUPOSTO PQP FODA-SE
                    /* Ativar o collider da mao */
                    foreach(GameObject obj in colliderCollection) {
                        if(obj.name == "handCollider") {
                            obj.SetActive(true);
                        } else {
                            obj.SetActive(false);
                        
                        }
                    }

                    currentObj.transform.position = posObturador.transform.position;
                    placedObjects++;
                    idx++;
                    currentObj = objectCollection[idx].gameObject;
                    currentObj.SetActive(true);
                    return;
                case "adesivo":
                    currentObj.transform.position = posPenso.transform.position;
                    // hack fix - só para n dar para mexer mais este gajo dps de ele estar no sitio certo
                    currentObj.tag = backgroundObj;
                    placedObjects++;
                    idx++;
                    audioManager.PlayAudio("progress");

                    currentObj = objectCollection[idx].gameObject;
                    currentObj.SetActive(true); // ativar a tala

                    // Ativar o collider da tala
                    foreach(GameObject obj in colliderCollection) {
                        if(obj.name == "talaCollider") {
                            obj.SetActive(true);
                        } else {
                            obj.SetActive(false);
                        }
                    }

                    return;
                case "tala":
                    currentObj.SetActive(false);
                    idx++;
                    placedObjects++;
                    objectCollection[idx].SetActive(true); // Ativar a tala_arm
                    placedObjects++;

                    Debug.Log("placed objects: " + placedObjects);
                    Debug.Log("object collection count: " + objectCollection.Count);

                    isLastObject();
                    return;
                default:
                    return;
            }
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
                    minigameUIControl.DisplayMessage("BOA! VAMOS DESINFETAR O LOCAL. CARREGA NO DESINFETANTE!");
                    break;
                case 3:
                case 4:
                    minigameUIControl.DisplayMessage("MUITO BEM! AGORA VAMOS LIMPAR O LOCAL. ARRASTA O PANO PARA LÁ!");
                    break;
                case 5:
                    minigameUIControl.DisplayMessage("BOA! VAMOS AGORA APLICAR O GARROTE. ARRASTA-O PARA O INICIO DO ANTEBRAÇO!");
                    break;
                default:
                    minigameUIControl.DisplayMessage("ERRO");
                    break;
            }
        } else if(minigameLevel == 2) {
            switch(placedObjects) {
                case 0:
                    minigameUIControl.DisplayMessage("VAMOS COMEÇAR POR DESINFETAR O LOCAL. CARREGA NO DESINFETANTE!");
                    break;
                case 1:
                case 2:
                    minigameUIControl.DisplayMessage("MUITO BEM! AGORA VAMOS LIMPAR O LOCAL. ARRASTA O PANO PARA LÁ!");
                    break;
                case 3:
                case 4:
                    minigameUIControl.DisplayMessage("BOA! AGORA VAMOS APLICAR O CATETER. ARRASTA-O PARA O LOCAL CORRETO!");
                    break;
                case 5:
                case 6:
                    minigameUIControl.DisplayMessage("MUITO BEM! AGORA VAMOS APLICAR O OBTURADOR. ARRASTA-O PARA O LOCAL CORRETO!");
                    break;
                case 7:
                    minigameUIControl.DisplayMessage("BOA! VAMOS AGORA APLICAR O ADESIVO. ARRASTA-O PARA O LOCAL CORRETO!");
                    break;
                case 8:
                case 9:
                    minigameUIControl.DisplayMessage("MUITO BEM! VAMOS AGORA APLICAR A TALA. ARRASTA-A PARA O LOCAL CORRETO!");
                    break;
                default:
                    minigameUIControl.DisplayMessage("ERRO");
                    break;
            }

        }
    }
}

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
        audioManager = FindObjectOfType<AudioManager>();
        minigameUIControl = FindObjectOfType<MinigameUIControl>();
        HandleCollections();
        HandleMessage();
    }

    // Update is called once per frame
    void Update()
    {   
        Debug.Log("teste: " + placedObjects);
        if(idx == garroteArmIdx) {
            idx++;
            placedObjects++;
            currentObj = objectCollection[idx].gameObject;
            currentObj.SetActive(true);
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
                audioManager.PlayAudio("progress");
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
                    break;
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
                    break;
                case "cateter":
                    objectCollection[idx].SetActive(false); // Desativar o cateter
                    idx++;
                    placedObjects++;
                    objectCollection[idx].gameObject.SetActive(true); // Ativar o cateter_arm
                    idx++;
                    placedObjects++;

                    objectCollection[garroteArmIdx].SetActive(false); //desativar o garrote_arm

                    /* Desativar todos os colliders menos o do catater */
                    foreach(GameObject obj in colliderCollection) {
                        if(obj.name != "cateterCollider") {
                            obj.SetActive(false);
                        }
                    }

                    currentObj = objectCollection[idx].gameObject;
                    currentObj.SetActive(true);

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
                    currentObj.tag = backgroundObj;
                    break;
                case "adesivo":
                    currentObj.transform.position = posPenso.transform.position;
                    currentObj.tag = backgroundObj;
                    PlaceObject();

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
                    minigameUIControl.DisplayMessage("Vamos começar por aplicar a pomada. Arrasta esta para o local correto.");
                    break;
                case 2:
                    minigameUIControl.DisplayMessage("Boa! Agora, vamos abrir o penso. Carrega nele!");
                    break;
                case 3:
                    minigameUIControl.DisplayMessage("Muito bem! Para aplicar o penso, arrasta-o para cima da pomada!");
                    break;
                default:
                    minigameUIControl.DisplayMessage("Erro");
                    break;
            }
        } else if(minigameLevel == 1) {
            switch(placedObjects) {
                case 1:
                    minigameUIControl.DisplayMessage("Vamos começar por retirar o penso. Carrega nele!");
                    break;
                case 2:
                    minigameUIControl.DisplayMessage("Boa! Vamos desinfetar o local. Carrega no desinfetante!");
                    break;
                case 3:
                case 4:
                    minigameUIControl.DisplayMessage("Muito bem! Agora vamos desinfetar o local. Arrasta o pano para lá!");
                    break;
                case 5:
                    minigameUIControl.DisplayMessage("Boa! Vamos agora aplicar o garrote. Arrasta-o para o inicio do antebraço!");
                    break;
                default:
                    minigameUIControl.DisplayMessage("Erro");
                    break;
            }
        } else if(minigameLevel == 2) {
            switch(placedObjects) {
                case 0:
                    minigameUIControl.DisplayMessage("Vamos começar por desinfetar o local. Carrega no desinfetante!");
                    break;
                case 1:
                case 2:
                    minigameUIControl.DisplayMessage("Muito bem! Agora vamos limpar o local. Arrasta o pano para lá!");
                    break;
                case 3:
                case 4:
                    minigameUIControl.DisplayMessage("Boa! Agora vamos aplicar o cateter. Arrasta-o para o local correto!");
                    break;
                case 5:
                case 6:
                    minigameUIControl.DisplayMessage("Muito bem! Agora vamos aplicar o obturador. Arrasta-o para o local correto!");
                    break;
                case 7:
                    minigameUIControl.DisplayMessage("Boa! Vamos agora aplicar o adesivo. Arrasta-o para o cateter!");
                    break;
                case 8:
                case 9:
                    minigameUIControl.DisplayMessage("Muito bem! Vamos agora colocar a tala. Arrasta-a para o braço!");
                    break;
                default:
                    minigameUIControl.DisplayMessage("Erro");
                    break;
            }

        }
    }
}

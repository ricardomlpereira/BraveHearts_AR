using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using TMPro;
using System.Text.RegularExpressions;
using UnityEngine.SceneManagement;
using System.Net.Sockets;
using System.Linq;

public class MultiTargetsManager : MonoBehaviour
{
    [SerializeField] private ARTrackedImageManager arTrackedImageManager;
    [SerializeField] private GameObject[] arCollection;

    private Dictionary<string, GameObject> arModels = new Dictionary<string, GameObject>(); // Key: string (nome do gameObject). Vai retornar o gameObject cujo o nome corresponde aquele passado pela chave.
    private Dictionary<string, bool> modelState = new Dictionary<string, bool>(); // Key: string (nome do gameObject). Vai retornar o estado de um gameObject, isto é, se esta ativado ou não (presente)

    [SerializeField] private TextMeshProUGUI mainText;
    [SerializeField] private TextMeshProUGUI scoreText;

    [SerializeField] private GameObject minigameBtn;
    [SerializeField] private GameObject restartBtn; // TODO: APAGAR

    // TODO: probably a bad idea to have score as a public variable. 
    public static int score; // Score of the player; Also used as an indicator for the current level of the game; Static so that the variable keeps the value independent of the scene;
    public static bool foundFirstMatch = false;
    public static bool foundSecondMatch = false;
    public static bool foundThirdMatch = false;

    // Start is called before the first frame update
    void Start()
    {
        int idx; // Only playable for 3 levels
        int j = 1;
        int t = 4;

        if (score == 0) {
            /* Level 1 - Buttefly */
            idx = 0;
        } else if (score == 1) {
            /* Level 2 - Koala */
            idx = 3;
        } else {
            /* Level 3 - Bee */
            idx = 6;
        }

        for (int i = idx; i < idx + 3; i++) {
            GameObject newARModel1 = Instantiate(arCollection[i], Vector3.zero, Quaternion.identity);
            GameObject newARModel2 = Instantiate(arCollection[i], Vector3.zero, Quaternion.identity);

            // TODO - pares serão sempre os mesmos (1-4; 2-5; 3-6)
            newARModel1.name = "marker" + j;
            newARModel2.name = "marker" + t;

            arModels.Add(newARModel1.name, newARModel1);
            arModels.Add(newARModel2.name, newARModel2);

            newARModel1.SetActive(false);
            newARModel2.SetActive(false);

            modelState.Add(newARModel1.name, false);
            modelState.Add(newARModel2.name, false);

            /* Destroy the original AR Model - Use the models in the dictionary from now on */

            /* Increment i and j for next iteration */
            j++;
            t++;
        }

        /* Destroy the origin AR Models so that they don't appear randomly in the scene */
        foreach(var model in arCollection) {
            Destroy(model);
        }

        scoreText.text = score + "/3";
        DisplayMessage("ENCONTRA UM PAR!", false);
    }

    private void Update()
    {
        /* TODO - ONLY TEMPORARY TO FACILITATE DEBUGING */
        minigameBtn.SetActive(true);

        /* Check if all matches have been found */
        if(foundFirstMatch && foundSecondMatch && foundThirdMatch)
        {
            // TODO: when all matches are found the player can render more than 2 models simultaneously
            if(score == 0) {
                DisplayMessage("A BORBOLETA AURORA QUER BRINCAR CONTIGO!", true);
            } else if(score == 1) {
                DisplayMessage("O COALA KIKO QUER BRINCAR CONTIGO!", true);
            } else if(score == 2) {
                DisplayMessage("A ABELHA MEL QUER BRINCAR CONTIGO!", true);
            } else {
                // Score >= 3
                DisplayMessage("BOA! ENCONTRASTE TODOS OS PARES!", false);
                restartBtn.SetActive(true);
            }

            return;
        }

        (GameObject[] activeModels, int numActiveModels) = GetActiveModels();


        /* Check if enough models are being tracked */
        if(numActiveModels < 2)
        {
            DisplayMessage("ENCONTRA UM PAR!", false);
            return;
        }

        /* Check if too many models are being tracked - if so disable all active models */
        if(numActiveModels > 2)
        {
            DisplayMessage("TENHA APENAS 2 CARTAS PARA CIMA!", false);
            DisableActiveModels();
            return;
        }

        /* Check for a possible match - has two and only two active models */

        /* Get identification of each marker */
        int id1 = int.Parse(Regex.Match(activeModels[0].name, @"\d+").Value);
        int id2 = int.Parse(Regex.Match(activeModels[1].name, @"\d+").Value);

        int incMarker1 = id1 + 3;
        int decMarker2 = id2 - 3;

        string animal = "";
        if(score == 0) {
            animal = "BORBOLETA";
        } else if(score == 1) {
            animal = "COALA";
        } else if(score == 2) {
            animal = "ABELHA";
        }

        /* Increment and decrement are need in case the first model to be set active is "the second" model */
        if (incMarker1 == id2 || decMarker2 == id1)
        {
            /* Found a match */
            if(incMarker1 == 4 || decMarker2 == 1)
            {
                /* Found doctor */
                DisplayMessage("ENCONTRASTE UM PAR - " + animal, false);

                if (!foundFirstMatch)
                {
                    foundFirstMatch = true;
                }
            } else if (incMarker1 == 5 || decMarker2 == 2)
            {
                /* Found nurse */
                DisplayMessage("ENCONTRASTE UM PAR - " + animal, false);
                if (!foundSecondMatch)
                {
                    foundSecondMatch = true;
                }
            } else if(incMarker1 == 6 || decMarker2 == 3) 
            {
                /* Found patient */
                DisplayMessage("ENCONTRASTE UM PAR - " + animal, false);
                if(!foundThirdMatch)
                {
                    foundThirdMatch = true;
                }
            }
        } else
        {
            DisplayMessage("NÃO É UM PAR - TENTA OUTRA VEZ!", false);
        }
    }

    private void OnEnable()
    {
        arTrackedImageManager.trackedImagesChanged += OnTrackedImagesChanged;
    }

    private void OnDisable()
    {
        arTrackedImageManager.trackedImagesChanged -= OnTrackedImagesChanged;
    }

    private void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs args)
    {
        foreach(var trackedImage in args.added)
        {
            ShowARModel(trackedImage);
        }

        foreach(var trackedImage in args.updated)
        {
            if(trackedImage.trackingState == TrackingState.Tracking)
            {
                ShowARModel(trackedImage);
            } else if(trackedImage.trackingState == TrackingState.Limited)
            {
                HideARModel(trackedImage);
            }
        }
    }

    private void ShowARModel(ARTrackedImage trackedImage)
    {
        bool isActive = modelState[trackedImage.referenceImage.name];
        if(!isActive)
        {
            GameObject arModel = arModels[trackedImage.referenceImage.name];
            arModel.transform.position = trackedImage.transform.position;
            arModel.SetActive(true);
            
            arModel.transform.rotation = Quaternion.Euler(-90f, 0f, -130f);

            modelState[trackedImage.referenceImage.name] = true;
        } else
        {
            GameObject arModel = arModels[trackedImage.referenceImage.name];
            arModel.transform.position = trackedImage.transform.position;
        }
    }

    private void HideARModel(ARTrackedImage trackedImage)
    {
        bool isActive = modelState[trackedImage.referenceImage.name];
        if (isActive)
        {
            GameObject arModel = arModels[trackedImage.referenceImage.name];
            arModel.SetActive(false);
            modelState[trackedImage.referenceImage.name] = false;
        }
    }

    public void DisplayMessage(string text, bool align)
    {
        if(align) {
            mainText.alignment = TextAlignmentOptions.Top;
            minigameBtn.SetActive(true);
        }

        mainText.text = text;
    }

    private (GameObject[], int) GetActiveModels()
    {
        GameObject[] activeModels = new GameObject[modelState.Count];
        int numActiveModels = 0;
        for (int i = 1; i <= modelState.Count; i++)
        {
            if (modelState["marker" + i])
            {
                activeModels[numActiveModels] = arModels["marker" + i];
                numActiveModels++;
            }
        }

        return (activeModels, numActiveModels);
    }

    private void DisableActiveModels()
    {
        foreach(var model in arModels.Values)
        {
            model.SetActive(false);
            modelState[model.name] = false;
        }
    }
}

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

public class MultiTargetsManager : MonoBehaviour
{
    [SerializeField] private ARTrackedImageManager arTrackedImageManager;
    [SerializeField] private GameObject[] arCollection;

    private Dictionary<string, GameObject> arModels = new Dictionary<string, GameObject>(); // Key: string (nome do gameObject). Vai retornar o gameObject cujo o nome corresponde aquele passado pela chave.
    private Dictionary<string, bool> modelState = new Dictionary<string, bool>(); // Key: string (nome do gameObject). Vai retornar o estado de um gameObject, isto é, se esta ativado ou não (presente)

    [SerializeField] private TextMeshProUGUI mainText;
    [SerializeField] private TextMeshProUGUI scoreText;

    [SerializeField] private GameObject minigameBtn;
    [SerializeField] private GameObject restartBtn;

    public static int score; // Score of the player; Also used as an indicator for the current level of the game; Static so that the variable keeps the value independent of the scene;
    // TODO: probably a bad idea to have score as a public variable. 
    public static bool foundFirstMatch = false;
    public static bool foundSecondMatch = false;
    public static bool foundThirdMatch = false;
    //private bool isScoreUpdated = false;

    // Start is called before the first frame update
    void Start()
    {
        int i = 1;
        int j = 4;

        /* Save every model with the name "marker1-6" to facilitate the processs later on */

        foreach(var arModel in arCollection)
        {
            GameObject newARModel1 = Instantiate(arModel, Vector3.zero, Quaternion.identity);
            GameObject newARModel2 = Instantiate(arModel, Vector3.zero, Quaternion.identity);

            if(arModel.name.Equals("Doctor"))
            {
                newARModel1.name = "marker" + i;
                newARModel2.name = "marker" + j;
            } else if (arModel.name.Equals("Nurse"))
            {
                newARModel1.name = "marker" + (i + 1);
                newARModel2.name = "marker" + (j + 1);
            } else if (arModel.name.Equals("Patient"))
            {
                newARModel1.name = "marker" + (i + 2);
                newARModel2.name = "marker" + (j + 2);
            } 

            arModels.Add(newARModel1.name, newARModel1);
            arModels.Add(newARModel2.name, newARModel2);

            newARModel1.SetActive(false);
            newARModel2.SetActive(false);

            modelState.Add(newARModel1.name, false);
            modelState.Add(newARModel2.name, false);

            /* Destroy the original AR Model - Use the models in the dictionary from now on */
            Destroy(arModel);
        }

        scoreText.text = score + "/3";
        DisplayMessage("ENCONTRA UM PAR!", false);
    }

    private void Update()
    {
        /* Check if all matches have been found */
        if(foundFirstMatch && foundSecondMatch && foundThirdMatch)
        {

            // TODO: when all matches are found the player can render more than 2 models simultaneously
            /*if(!isScoreUpdated) {
                UpdateScore();
            }*/

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

        if (incMarker1 == id2 || decMarker2 == id1)
        {
            /* Found a match */
            if(incMarker1 == 4 || decMarker2 == 1)
            {
                /* Found doctor */
                DisplayMessage("ENCONTRASTE UM PAR - DOUTOR", false);

                if (!foundFirstMatch)
                {
                    foundFirstMatch = true;
                }
            } else if (incMarker1 == 5 || decMarker2 == 2)
            {
                /* Found nurse */
                DisplayMessage("ENCONTRASTE UM PAR - ENFERMEIRA", false);
                if (!foundSecondMatch)
                {
                    foundSecondMatch = true;
                }
            } else if(incMarker1 == 6 || decMarker2 == 3) 
            {
                /* Found patient */
                DisplayMessage("ENCONTRASTE UM PAR - PACIENTE", false);
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

    /*private void UpdateScore()
    {
        score += 1;
        scoreText.text = score +"/3";
        isScoreUpdated = true;

        return;
    } */

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

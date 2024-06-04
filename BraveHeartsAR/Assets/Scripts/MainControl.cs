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

public class MainControl : MonoBehaviour
{
    [SerializeField] private ParticleSystem matchParticleSystem;
    [SerializeField] private ARTrackedImageManager arTrackedImageManager;
    [SerializeField] private GameObject[] arCollection;

    private Dictionary<string, GameObject> arModels = new Dictionary<string, GameObject>(); // Key: string (nome do gameObject). Vai retornar o gameObject cujo o nome corresponde aquele passado pela chave.
    private Dictionary<string, bool> modelState = new Dictionary<string, bool>(); // Key: string (nome do gameObject). Vai retornar o estado de um gameObject, isto é, se esta ativado ou não (presente)

    // TODO: probably a bad idea to have score as a public variable. 
    public static int score; // Score of the player; Also used as an indicator for the current level of the game; Static so that the variable keeps the value independent of the scene;
    public static bool foundFirstMatch = false;
    public static bool foundSecondMatch = false;
    public static bool foundThirdMatch = false;
    private MainUIControl MainUIControl;
    private List<int> markerIds;
    private List<Tuple<int,int>> matches; // TODO: make this static and only change the matches whenever the level changes
    public static bool resetProgress;

    // Start is called before the first frame update
    void Start()
    {
        MainUIControl = FindObjectOfType<MainUIControl>();

        // Initialize the list 
        markerIds = new List<int> {1,2,3,4,5,6};
        matches = new List<Tuple<int, int>>();

        // Shuffle the ID list
        System.Random rng = new System.Random();
        int n = markerIds.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            int value = markerIds[k];
            markerIds[k] = markerIds[n];
            markerIds[n] = value;
        }

        int idx; // Only playable for 3 levels

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

            newARModel1.name = "marker" + markerIds[0];
            newARModel2.name = "marker" + markerIds[1];

            arModels.Add(newARModel1.name, newARModel1);
            arModels.Add(newARModel2.name, newARModel2);

            newARModel1.SetActive(false);
            newARModel2.SetActive(false);

            modelState.Add(newARModel1.name, false);
            modelState.Add(newARModel2.name, false);

            matches.Add(new Tuple<int, int>(markerIds[0], markerIds[1]));
            markerIds.RemoveAt(1);
            markerIds.RemoveAt(0);
        }

        /* Destroy the origin AR Models so that they don't appear randomly in the scene */
        foreach(var model in arCollection) {
            Destroy(model);
        }

        if(resetProgress) {
            resetProgress = false;
            foundFirstMatch = false;
            foundSecondMatch = false;
            foundThirdMatch = false;

            MainUIControl.foundMatches = 0;
        }

        MainUIControl.DisplayMessage("ENCONTRA UM PAR!");
    }

    private void Update()
    {
        /* Check if all matches have been found - if so enables the minigame */
        if(foundFirstMatch && foundSecondMatch && foundThirdMatch)
        {
            MainUIControl.EnableMinigame();
            return;
        }

        /* Get active models */
        (GameObject[] activeModels, int numActiveModels) = GetActiveModels();


        /* Check if enough models are being tracked */
        if(numActiveModels < 2)
        {
            MainUIControl.DisplayMessage("ENCONTRA UM PAR!");
            return;
        }

        /* Check if too many models are being tracked - if so disable all active models */
        if(numActiveModels > 2)
        {
            MainUIControl.DisplayMessage("TENHA APENAS 2 CARTAS PARA CIMA!");
            DisableActiveModels();
            return;
        }

        /* If it gets here then all conditions for a posible match are met - lets check if the player has found a match */
        /* Get identification of each marker */
        int id1 = int.Parse(Regex.Match(activeModels[0].name, @"\d+").Value);
        int id2 = int.Parse(Regex.Match(activeModels[1].name, @"\d+").Value);

        string animal = "";
        if(score == 0) {
            animal = "BORBOLETA";
        } else if(score == 1) {
            animal = "COALA";
        } else if(score == 2) {
            animal = "ABELHA";
        }

        int matchIdx = GetMatchIdx(id1, id2);
        if(matchIdx == -1)
        {
            MainUIControl.DisplayMessage("NÃO É UM PAR - TENTA OUTRA VEZ!");
            return;
        } else if(matchIdx == 0) {
            MainUIControl.DisplayMessage("ENCONTRASTE UM PAR - " + animal);
            if(!foundFirstMatch) {
                MainUIControl.foundMatches++;
                foundFirstMatch = true;
                matchParticleSystem.Play();
            }
        } else if(matchIdx == 1) {
            MainUIControl.DisplayMessage("ENCONTRASTE UM PAR - " + animal);
            if(!foundSecondMatch) {
                MainUIControl.foundMatches++;
                foundSecondMatch = true;
                matchParticleSystem.Play();
            }
        } else if(matchIdx == 2) {
            MainUIControl.DisplayMessage("ENCONTRASTE UM PAR - " + animal);
            if(!foundThirdMatch) {
                MainUIControl.foundMatches++;
                foundThirdMatch = true;
                matchParticleSystem.Play();
            }
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

    private int GetMatchIdx(int m1Id, int m2Id)
    {
        foreach(var match in matches) {
            if((match.Item1 == m1Id && match.Item2 == m2Id) || match.Item1 == m2Id && match.Item2 == m1Id) {
                return matches.IndexOf(match);
            } 
        }

        // If pair is not a match then return -1
        return -1;
    }
}

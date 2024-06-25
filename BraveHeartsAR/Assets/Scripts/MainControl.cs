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
using System.Numerics;

public class MainControl : MonoBehaviour
{
    private struct GameState
    {
        public int numActiveModels;
        public int id1;
        public int id2;
    }

    [SerializeField] private GameObject nextButton;
    [SerializeField] private ParticleSystem matchParticleSystem;
    [SerializeField] private ARTrackedImageManager arTrackedImageManager;
    [SerializeField] private GameObject[] arCollection;

    private Dictionary<string, GameObject> arModels = new Dictionary<string, GameObject>(); // Key: string (nome do gameObject). Vai retornar o gameObject cujo o nome corresponde aquele passado pela chave.
    private Dictionary<string, bool> modelState = new Dictionary<string, bool>(); // Key: string (nome do gameObject). Vai retornar o estado de um gameObject, isto é, se esta ativado ou não (presente)
    public static int score; // Score of the player; Also used as an indicator for the current level of the game; Static so that the variable keeps the value independent of the scene;
    public static bool foundFirstMatch = false;
    public static bool foundSecondMatch = false;
    public static bool foundThirdMatch = false;
    private MainUIControl MainUIControl;
    private List<int> markerIds;
    private List<Tuple<int, int>> matches;
    private bool minigameEnabled = false;
    private GameState previousState;
    public static bool resetProgress;
    private AudioManager audioManager;
    private bool playedFailAudio = false;
    private bool playedErrorAudio = false;
    
    // Start is called before the first frame update
    void Start()
    {
        MainUIControl = FindObjectOfType<MainUIControl>();
        audioManager = FindObjectOfType<AudioManager>();
        
        // Initialize the list 
        markerIds = new List<int> {1,2,3,4,5,6};
        matches = new List<Tuple<int, int>>();

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

        int idx;

        if (score == 0)
        {
            idx = 0;
        }
        else if (score == 1)
        {
            idx = 3;
        }
        else
        {
            idx = 6;
        }

        for (int i = idx; i < idx + 3; i++)
        {
            GameObject newARModel1 = Instantiate(arCollection[i], UnityEngine.Vector3.zero, UnityEngine.Quaternion.identity);
            GameObject newARModel2 = Instantiate(arCollection[i], UnityEngine.Vector3.zero, UnityEngine.Quaternion.identity);

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

        foreach (var model in arCollection)
        {
            Destroy(model);
        }

        previousState = new GameState();
        if(resetProgress) {
            resetProgress = false;
            foundFirstMatch = false;
            foundSecondMatch = false;
            foundThirdMatch = false;

            MainUIControl.foundMatches = 0;
        }
    }

    private void Update()
    {        
        if (MainUIControl.IsDisplayingMessage() || minigameEnabled)
        {
            return;
        }

        if (foundFirstMatch && foundSecondMatch && foundThirdMatch)
        {
            MainUIControl.EnableMinigame();
            minigameEnabled = true;
            return;
        }

        (GameObject[] activeModels, int numActiveModels) = GetActiveModels();

        int id1 = -1;
        int id2 = -1;
        
        if (numActiveModels == 2)
        {
            id1 = int.Parse(Regex.Match(activeModels[0].name, @"\d+").Value);
            id2 = int.Parse(Regex.Match(activeModels[1].name, @"\d+").Value);
        }

        // Changes Start: Update the currentState variable
        GameState currentState = new GameState
        {
            numActiveModels = numActiveModels,
            id1 = id1,
            id2 = id2
        };

        if(nextButton.activeSelf == true && numActiveModels == 1){ //Encontrou o primeiro model de todos - Desativa o tutorial
                MainUIControl.DisplayMessage("Encontra os pares!");
                nextButton.SetActive(false);
        }

        if ((previousState.numActiveModels == 0 && currentState.numActiveModels == 1) || (previousState.numActiveModels == 1 && currentState.numActiveModels == 0) || (previousState.Equals(currentState)))
        {
            return;
        }

        previousState = currentState;
        // Changes End
        Debug.Log("Num active models: " + numActiveModels);

        if (numActiveModels <= 1)
        {
            MainUIControl.DisplayMessage("Encontra os pares!");
            playedFailAudio = false;
            return;
        }

        if (numActiveModels > 2)
        {
            //DisableActiveModels();
            foreach (var model in arModels.Values)
            {
                model.SetActive(false);
                //modelState[model.name] = false;
            }
            MainUIControl.DisplayMessage("Mantém apenas duas cartas viradas para cima!");
            
            playedFailAudio = false;

            if (!playedErrorAudio)
            {
                audioManager.PlayAudio("error");
                playedErrorAudio = true;
            }
            
            return;
        }

        playedErrorAudio = false;

        string animal = "";
        if (score == 0)
        {
            animal = "Borboleta";
        }
        else if (score == 1)
        {
            animal = "Coala";
        }
        else if (score == 2)
        {
            animal = "Abelha";
        }

        int matchIdx = GetMatchIdx(id1, id2);
        if (matchIdx == -1)
        {
            MainUIControl.DisplayMessage("Não é um par!\nTenta outra vez");
            if (!playedFailAudio)
            {
                playedFailAudio = true;
                audioManager.PlayAudio("fail");    
            }
            
            return;
        }
        else if (matchIdx == 0)
        {
            MainUIControl.DisplayMessage("Encontraste um par - " + animal);
            playedFailAudio = false;
            
            if(!foundFirstMatch) {
                MainUIControl.foundMatches++;
                foundFirstMatch = true;
                matchParticleSystem.Play();
                audioManager.PlayAudio("progress");
            }
        }
        else if (matchIdx == 1)
        {
            MainUIControl.DisplayMessage("Encontraste um par - " + animal);
            playedFailAudio = false;
            
            if(!foundSecondMatch) {
                MainUIControl.foundMatches++;
                foundSecondMatch = true;
                matchParticleSystem.Play();
                audioManager.PlayAudio("progress");
            }
        }
        else if (matchIdx == 2)
        {
            MainUIControl.DisplayMessage("Encontraste um par - " + animal);
            playedFailAudio = false;
            
            if(!foundThirdMatch) {
                MainUIControl.foundMatches++;
                foundThirdMatch = true;
                matchParticleSystem.Play();
                audioManager.PlayAudio("progress");
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
        foreach (var trackedImage in args.added)
        {
            ShowARModel(trackedImage);
        }

        foreach (var trackedImage in args.updated)
        {
            if (trackedImage.trackingState == TrackingState.Tracking)
            {
                ShowARModel(trackedImage);
            }
            else if (trackedImage.trackingState == TrackingState.Limited)
            {
                HideARModel(trackedImage);
            }
        }
    }

    private void ShowARModel(ARTrackedImage trackedImage)
    {
        bool isActive = modelState[trackedImage.referenceImage.name];
        if (!isActive)
        {
            GameObject arModel = arModels[trackedImage.referenceImage.name];
            arModel.transform.position = trackedImage.transform.position;
            arModel.SetActive(true);
/*
            if(score == 0) {
                // Level 1 - Butterfly
                arModel.transform.rotation = UnityEngine.Quaternion.Euler(-180f, -140f, -90f);
            } else if(score == 1) {
                // Level 2 - Koala
                arModel.transform.rotation = UnityEngine.Quaternion.Euler(0f, -140f, -90f);
            } else if(score == 2) {
                // Level 3 - Bee
            }

            UnityEngine.Vector3 camPos = Camera.main.transform.position;
            arModel.transform.LookAt(camPos);

*/
            modelState[trackedImage.referenceImage.name] = true;
        }
        else
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
        foreach (var model in arModels.Values)
        {
            model.SetActive(false);
            modelState[model.name] = false;
        }
    }

    private int GetMatchIdx(int m1Id, int m2Id)
    {
        foreach (var match in matches)
        {
            if ((match.Item1 == m1Id && match.Item2 == m2Id) || match.Item1 == m2Id && match.Item2 == m1Id)
            {
                return matches.IndexOf(match);
            }
        }

        // If pair is not a match then return -1
        return -1;
    }
}
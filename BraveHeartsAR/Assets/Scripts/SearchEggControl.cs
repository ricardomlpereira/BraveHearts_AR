using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.ARFoundation;

public class SearchEggControl : MonoBehaviour
{
    [SerializeField] private GameObject nextButton;
    private SpriteRenderer goalBushSprite;
    public SpriteRenderer bushSprite_1; 
    public SpriteRenderer bushSprite_2;
    public SpriteRenderer bushSprite_3;
    private ParticleSystem goalParticleSystem;
    public ParticleSystem particleSystem_1;
    public ParticleSystem particleSystem_2;
    public ParticleSystem particleSystem_3;
    private SpriteRenderer goalEggSprite; 
    public SpriteRenderer eggSprite_1; 
    public SpriteRenderer eggSprite_2; 
    public SpriteRenderer eggSprite_3;
    public SpriteRenderer eggFinalSprite;
    public TextMeshProUGUI buddyText;
    public ParticleSystem confettiParticleSystem;
    private bool eggFound = false;
    public float shakeDuration = 0.5f; // Duration of the shake effect
    public float shakeMagnitude = 0.1f; // Magnitude of the shake effect
    [SerializeField] private TextMeshProUGUI scoreText;
    private bool hasCollectedEgg;
    private TW_MultiStrings_All typewriter;
    private AudioManager audioManager;

    // Start is called before the first frame update
    void Start()
    {
        audioManager = FindObjectOfType<AudioManager>();

        typewriter = buddyText.gameObject.AddComponent<TW_MultiStrings_All>();
        typewriter.timeOut = 1; // Set timeout for the typewriter effect
        typewriter.LaunchOnStart = false;

        scoreText.text = MainControl.score + "/3";

        hasCollectedEgg = false;

        DefineGoal();

        if (goalBushSprite == null)
        {
            goalBushSprite = GetComponent<SpriteRenderer>();
        }

        if (goalParticleSystem == null)
        {
            goalParticleSystem = GetComponentInChildren<ParticleSystem>();
        }
        
        // Start the shaking coroutine every 5 seconds
        InvokeRepeating("StartShaking", 0, 10f);
    }

    // Update is called once per frame
    void Update()
    {
        // Check if the user has touched the screen
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            // Create a ray from the touch position
            Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
            RaycastHit hit;

            // Check if the ray has hit anything
            if (Physics.Raycast(ray, out hit))
            {
                if(hit.transform == bushSprite_1.transform || hit.transform == bushSprite_2.transform || hit.transform == bushSprite_3.transform){
                    nextButton.SetActive(false);
                    if(hit.transform != goalBushSprite.transform){
                        //Arbustos desaparecem
                        hit.transform.gameObject.SetActive(false);
                        //buddyText.text = "PARECE QUE NÃO ESTÁ NESSE ARBUSTRO!\nTENTA OUTRO";
                        audioManager.PlayAudio("fail");
                        if(eggFound == false){
                        typewriter.ORIGINAL_TEXT = "Parece que não está nesse arbustro!\nTenta outro";
                        typewriter.StartTypewriter();
                        }     
                    }else if(hit.transform == goalBushSprite.transform && eggFound){ //JA ENCONTROU O OVO
                        confettiParticleSystem.gameObject.SetActive(true);
                        confettiParticleSystem.Play();

                        audioManager.PlayAudio("progress");

                        bushSprite_1.gameObject.SetActive(false);
                        bushSprite_2.gameObject.SetActive(false);
                        bushSprite_3.gameObject.SetActive(false);
                        goalEggSprite.gameObject.SetActive(false);
                        eggFinalSprite.gameObject.SetActive(true);
                    }else{ //AINDA NAO ENCONTROU O OVO
                        //Encontrou o ovo
                        audioManager.PlayAudio("progress");
                        goalEggSprite.gameObject.SetActive(true);
                        //goalBushSprite.gameObject.SetActive(false);
                        eggFound = true;
                        //buddyText.text = "BOA ENCONTRASTE O OVO!\nOBRIGADO!";
                        typewriter.ORIGINAL_TEXT = "Boa encontraste o ovo!\nObrigado!";
                        typewriter.StartTypewriter();
                    }
                }

                if(hit.transform == eggFinalSprite.transform && !hasCollectedEgg) {
                    hasCollectedEgg = true;
                    MinigameControl.minigameLevel++;
                    MainControl.score++;

                    MainControl.foundFirstMatch = false;
                    MainControl.foundSecondMatch = false;
                    MainControl.foundThirdMatch = false;

                    scoreText.text = MainControl.score + "/3";

                    audioManager.PlayAudio("congrats");

                    StartCoroutine(NextAction());
                }
            }
        }
    }

    IEnumerator NextAction() {
        /* Wait for 3 seconds */
        yield return new WaitForSeconds(3f);

        /* Load next level or end scene */
        if(MinigameControl.minigameLevel > 2) {
            SceneManager.LoadScene("End");
        } else {
            SceneManager.LoadScene("Main");
        }

        LoaderUtility.Deinitialize();
        LoaderUtility.Initialize();
    }   

    void DefineGoal(){
        int random = UnityEngine.Random.Range(0, 3);
        switch(random){
            case 0:
                goalBushSprite = bushSprite_1;
                goalParticleSystem = particleSystem_1;
                goalEggSprite = eggSprite_1;
                break;
            case 1:
                goalBushSprite = bushSprite_2;
                goalParticleSystem = particleSystem_2;
                goalEggSprite = eggSprite_2;
                break;
            case 2:
                goalBushSprite = bushSprite_3;
                goalParticleSystem = particleSystem_3;
                goalEggSprite = eggSprite_3;
                break;
            default:
                break;
        }
    }

    void StartShaking()
    {
        if (!eggFound && goalParticleSystem != null)
        {
            goalParticleSystem.Play();
        }if (!eggFound) // Check if eggFound is false
        {
            StartCoroutine(WaitAndShake());
        }
    }

    IEnumerator WaitAndShake()
    {
        // Wait for 1 second
        yield return new WaitForSeconds(0.5f);

        // Start the shaking coroutine
        StartCoroutine(Shake());
    }

    IEnumerator Shake()
    {
        Vector3 originalPosition = goalBushSprite.transform.localPosition;

        float elapsed = 0.0f;

        while (elapsed < shakeDuration)
        {
            float x = Random.Range(-1f, 1f) * shakeMagnitude;
            float y = Random.Range(-1f, 1f) * shakeMagnitude;

            goalBushSprite.transform.localPosition = new Vector3(originalPosition.x + x, originalPosition.y + y, originalPosition.z);

            elapsed += Time.deltaTime;

            yield return null;
        }

        goalBushSprite.transform.localPosition = originalPosition;
    }

    private bool CheckTouchOnObject(UnityEngine.Vector2 touchPos)
    {
        Ray ray = Camera.main.ScreenPointToRay(touchPos);

        if(Physics.Raycast(ray, out RaycastHit hitObject))
        {
            if(hitObject.collider.CompareTag("Object"))
            {
                return true;
            }
        }

        return false;
    }
}

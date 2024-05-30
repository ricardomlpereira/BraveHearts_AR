using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SearchEggControl : MonoBehaviour
{
    private SpriteRenderer goalBushSprite;
    public SpriteRenderer bushSprite_1; 
    public SpriteRenderer bushSprite_2;
    public SpriteRenderer bushSprite_3;
    private ParticleSystem goalParticleSystem;
    public ParticleSystem particleSystem_1;
    public ParticleSystem particleSystem_2;
    public ParticleSystem particleSystem_3;
    public SpriteRenderer goalEggSprite; 
    public SpriteRenderer eggSprite_1; 
    public SpriteRenderer eggSprite_2; 
    public SpriteRenderer eggSprite_3; 
    private bool eggFound = false;
    public float shakeDuration = 0.5f; // Duration of the shake effect
    public float shakeMagnitude = 0.1f; // Magnitude of the shake effect
    private MainControl mainControl;

    // Start is called before the first frame update
    void Start()
    {
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
                if(hit.transform != goalBushSprite.transform){
                    //Arbustos desaparecem
                     hit.transform.gameObject.SetActive(false);
                }else{
                    //Encontrou o ovo
                    goalEggSprite.gameObject.SetActive(true);
                    eggFound = true;
                }
            }
        }
    }
        
    }
}

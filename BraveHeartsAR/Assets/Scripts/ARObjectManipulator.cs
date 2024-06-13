using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ARObjectManipulator : MonoBehaviour
{
    [SerializeField]
    private Camera ARCamera;
    [SerializeField]
    private TextMeshProUGUI detailsText;

    private GameObject ARObject;
    private GameObject ARObjectForDetails;
    private bool isARObjectSelected;
    private string tagARObjects = "ARObject";
    private Vector2 initialTouchPos;
    
    private float speedRotation = 5.0f;
    private float scaleFactor = 0.1f;

    private float touchDis;
    private Vector2 touchPosDiff;
    private float rotationTolerance = 1.5f;
    private float scaleTolerance = 25f;

    // Start is called before the first frame update
    void Start()
    {
        detailsText.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.touchCount > 0)
        {
            Touch touch1 = Input.GetTouch(0);
            initialTouchPos = touch1.position;
            isARObjectSelected = CheckTouchOnARObject(initialTouchPos);

            if (Input.touchCount == 1 && touch1.phase == TouchPhase.Stationary && isARObjectSelected)
            {
                /* Verificar se é necessário verificar se ARObjectForDetails é != null pois:
                 * Se entra aqui então isARObjectSelect tem de ser true, ou seja, ele o ARObjectForDetails vai sempre devolver um objecto
                 * e não null */

                GetARObjectForDetails(initialTouchPos);
                if(ARObjectForDetails != null)
                {
                    if(ARObjectForDetails.name == "marker1" || ARObjectForDetails.name == "marker4")
                    {
                        StartCoroutine(DetailsCoroutine("Eu sou um médico!"));
                    } else if(ARObjectForDetails.name == "marker2" || ARObjectForDetails.name == "marker5")
                    {
                        StartCoroutine(DetailsCoroutine("Eu sou uma enfermeira!"));
                    } else
                    {
                        StartCoroutine(DetailsCoroutine("Eu sou um paciente!"));
                    }
                }
            } else
            {
                detailsText.enabled = false;
            } 

            if(Input.touchCount == 2)
            {
                Touch touch2 = Input.GetTouch(1);

                if(touch1.phase == TouchPhase.Began || touch2.phase == TouchPhase.Began)
                {
                    touchPosDiff = touch2.position - touch1.position;
                    touchDis = Vector2.Distance(touch2.position, touch1.position);
                }

                if(touch1.phase == TouchPhase.Moved || touch2.phase == TouchPhase.Moved)
                {
                    Vector2 currentTouchPosDiff = touch2.position - touch1.position;
                    float currentTouchDis = Vector2.Distance(touch2.position, touch1.position);

                    float difDistance = currentTouchDis - touchDis;

                    if(Mathf.Abs(difDistance) > scaleTolerance)
                    {
                        Vector3 newScale = ARObject.transform.localScale + Mathf.Sign(difDistance) * Vector3.one * scaleFactor;
                        ARObject.transform.localScale = Vector3.Lerp(ARObject.transform.localScale, newScale, 0.05f);
                    }

                    float angle = Vector2.SignedAngle(touchPosDiff, currentTouchPosDiff);

                    /*if(Mathf.Abs(angle) > rotationTolerance)
                    {
                        ARObject.transform.rotation = Quaternion.Euler(0, ARObject.transform.rotation.eulerAngles.y - Mathf.Sign(angle) * speedRotation, 0);
                    }*/

                    touchDis = currentTouchDis;
                    touchPosDiff = currentTouchPosDiff;
                }
            }
        }
    }

    private bool CheckTouchOnARObject(Vector2 touchPos)
    {
        Ray ray = ARCamera.ScreenPointToRay(touchPos);

        if(Physics.Raycast(ray, out RaycastHit hitARObject))
        {
            if(hitARObject.collider.CompareTag(tagARObjects))
            {
                ARObject = hitARObject.transform.gameObject;
                return true;
            }
        }

        return false;
    }

    private void GetARObjectForDetails(Vector2 touchPos)
    {
        Ray ray = ARCamera.ScreenPointToRay(touchPos);

        if(Physics.Raycast(ray, out RaycastHit hitARObject))
        {
            if (hitARObject.collider.CompareTag(tagARObjects))
            {
                ARObjectForDetails =  hitARObject.transform.gameObject;
            }
        }
    }

    IEnumerator DetailsCoroutine(string details)
    {
        detailsText.enabled = true;
        detailsText.text = details;

        // Wait for 3 seconds
        yield return new WaitForSeconds(3f);

        detailsText.enabled = false;
    }
}

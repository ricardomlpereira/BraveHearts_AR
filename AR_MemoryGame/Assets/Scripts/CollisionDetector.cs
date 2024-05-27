using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CollisionDetector : MonoBehaviour
{
    // TODO - Por enquanto e necessario colocar este script em todos os objetos que queremos que colidam 

    void Start() {

    }

    private void OnTriggerEnter(Collider other) {
        CollisionHandler handler = FindObjectOfType<CollisionHandler>();
        handler.placeObject();
    }
}

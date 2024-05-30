using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CollisionDetector : MonoBehaviour
{
    // TODO - Por enquanto e necessario colocar este script em todos os objetos que queremos que colidam
    private MinigameControl control;

    void Start() {
        /* Get control script */
        control = FindObjectOfType<MinigameControl>();
    }

    private void OnTriggerEnter(Collider other) {
        control.HandleObject();
    }
}

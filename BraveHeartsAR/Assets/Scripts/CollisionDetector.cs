using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CollisionDetector : MonoBehaviour
{
    private MinigameControl control;

    void Start() {
        /* Get control script */
        control = FindObjectOfType<MinigameControl>();
    }

    private void OnTriggerEnter(Collider other) {
        control.HandleObject();
    }
}

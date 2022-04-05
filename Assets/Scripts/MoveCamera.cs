using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    private GameManager gameManager;

    [SerializeField] private Transform cameraPosition;

    private void Awake() {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }
    void Update()
    {
        if (gameManager.isGameActive) {
            transform.position = cameraPosition.position;
        }
    }
}

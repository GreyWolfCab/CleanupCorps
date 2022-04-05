using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenDoor : MonoBehaviour
{
    [SerializeField] private int cost;
    [SerializeField] private GameObject connectedDoor;
    private GameManager gameManager;
    private PlayerStats playerStats;

    private void Awake() {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        playerStats = GameObject.Find("Player").GetComponent<PlayerStats>();
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {//if player is nearby
            gameManager.SetDoorText(cost);//show cost to open door to player
            gameManager.ShowDoorText();//show door open ui
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.CompareTag("Player")) {//if player is no longer nearby
            gameManager.HideDoorText();//hide door open ui
        }
    }

    public void RemoveDoor() {
        //check if player funds are high enough to cover door cost
        gameManager.HideDoorText();//hide door open ui
        playerStats.AddToBank(-cost);//remove funds from player bank
        Destroy(connectedDoor);//remove door connected at the other end of the hallway
        Destroy(gameObject);//remove the door
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    private GameManager gameManager;
    public int totalBank = 0;

    [Header("Health")]
    public int totalHealth = 100;
    public int maxHealth = 100;
    private float regenDelay = 3.0f;
    private float regenTimeRate = 0.5f;
    private int regenRate = 5;

    private void Awake() {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    public void AddToBank(int money) {
        totalBank += money;//update player bank total
        gameManager.SetBankText(totalBank);//update player gui bank amount
    }

    public void DamagePlayer(int damage) {
        if (!gameManager.isGameActive) {
            return;
        }
        totalHealth -= damage;
        if (totalHealth <= 0) {
            totalHealth = 0;
            gameManager.GameOver();
        }
        CancelInvoke("RegenerateHealth");
        InvokeRepeating("RegenerateHealth", regenDelay, regenTimeRate);
    }

    void RegenerateHealth() {
        if (!gameManager.isGameActive) {
            return;
        }
        totalHealth += regenRate;
        if (totalHealth >= 100) {
            totalHealth = 100;
            CancelInvoke("RegenerateHealth");
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour
{
    private Image healthbar;
    private PlayerStats playerStats;
    // Start is called before the first frame update
    void Start()
    {
        healthbar = GetComponent<Image>();
        playerStats = GameObject.Find("Player").GetComponent<PlayerStats>();
    }

    // Update is called once per frame
    void Update()
    {
        healthbar.fillAmount = playerStats.totalHealth * 0.01f;
    }
}

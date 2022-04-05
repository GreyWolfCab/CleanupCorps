using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMeleeAttack : MonoBehaviour
{
    private GameManager gameManager;
    private LayerMask playerMask;
    [SerializeField] private int damage;
    private float hitsphereOffset = 1.25f;
    private float hitsphereRadius = 0.75f;
    private float nextHit;//time till next hit
    private float hitRate = 2.0f;//time added till next hit

    private void Awake() {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        playerMask = LayerMask.GetMask("Player");//setup player layer
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position + transform.forward * hitsphereOffset, hitsphereRadius, playerMask);//grab any player colliders in the sphere
        if (gameManager.isGameActive && Time.time > nextHit && hitColliders.Length > 0) {//attack within a certain cooldown and if a player was found
            nextHit = Time.time + hitRate;//increase time till next attack
            PlayerStats playerReference = hitColliders[0].GetComponent<PlayerStats>();//store player health reference
            if (playerReference != null) {//double check reference not empty
                playerReference.DamagePlayer(damage);//damage the player
            }
        }
    }

    /*DEBUG CODE*/
    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + transform.forward * hitsphereOffset, hitsphereRadius);
    }
}
